using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

public struct Options
{
    public float mass;
    public float drag;
    public Color color;
    public float scaleFactor;
    public int score;
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Chainable : MonoBehaviour
{
    [SerializeField] public Transform anchorFront;
    [SerializeField] public Transform anchorRear;
    public ParticleSystem explosionEffect;
    public SpriteRenderer spriteRenderer;

    private LineRenderer _lineRenderer;
    [CanBeNull] private SpringJoint2D _springJoint;
    private int _score;

    [SerializeField]
    private Chainable _attachedObj;

    [CanBeNull]
    public Chainable AttachedObj
    {
        get => _attachedObj;
        set
        {
            if (value)
            {
                _lineRenderer = gameObject.AddComponent<LineRenderer>();
                _lineRenderer.startColor = Color.yellow;
                _lineRenderer.endColor = Color.yellow;
                _lineRenderer.widthMultiplier = 0.1f;
                _lineRenderer.positionCount = 2;
            }
            else
            {
                Destroy(_lineRenderer);
            }
            _attachedObj = value;

        }
    }

    private void Start()
    {
        if (!TryGetComponent(out _springJoint))
            return;
        var opt = Utils.GetOptions(transform.position.magnitude);
        spriteRenderer.material.color = opt.color;

        var rb = GetComponent<Rigidbody2D>();
        rb.mass = opt.mass;
        rb.drag = opt.drag;
        transform.localScale *= opt.scaleFactor;
        _score = opt.score;
    }

    // Update is called once per frame
    private void Update()
    {
        if (AttachedObj != null)
        {
            _lineRenderer.SetPositions(new Vector3[] {anchorRear.position, AttachedObj.anchorFront.position});
        }

        if (_springJoint != null)
        {
            // Color based on force applied
            spriteRenderer.color = Color.Lerp(Color.white, Color.red,  _springJoint.reactionForce.magnitude / _springJoint.breakForce);
        }
    }

    public void OnScore()
    {
        RoundManager.Instance.AddScore(_score);
        RoundManager.Instance.OnBoxSaved();
        AttachedObj?.SendMessage("OnScore");
        Destroy(gameObject);
    }

    public void OnAttach(GameObject source)
    {
        var attachTo = source.GetComponent<Chainable>();
        while (attachTo.AttachedObj != null)
        {
            attachTo = attachTo.AttachedObj;
        }

        var destRot = attachTo.transform.rotation;
        transform.rotation = destRot;
        transform.position = attachTo.transform.position + destRot * attachTo.anchorRear.localPosition +
                             (transform.position - anchorFront.transform.position);
        GetComponent<Rigidbody2D>().velocity = attachTo.GetComponent<Rigidbody2D>().velocity;

        var myCollider = GetComponent<BoxCollider2D>();
        myCollider.isTrigger = false;
        attachTo.AttachedObj = this;

        if (_springJoint != null)
        {
            _springJoint.enabled = true;
            _springJoint.connectedBody = attachTo.GetComponent<Rigidbody2D>();
            _springJoint.connectedAnchor = attachTo.anchorRear.localPosition;
            _springJoint.anchor = anchorFront.localPosition;

            attachTo.TryGetComponent<SpringJoint2D>(out var parentSpringJoint);
            float bf;
            if (parentSpringJoint == null)
            {
                bf = 2 + 3 * GameManager.Instance.GetUpgradeValue(Utils.UpgradeKey.RopeStrength);
            }
            else
            {
                bf = 0.8f * parentSpringJoint.breakForce;
            }
            _springJoint.breakForce = bf;
        }
    }

    private async void OnJointBreak2D(Joint2D brokenJoint)
    {
        // Inform parent about breakage
        if (_springJoint != null)
        {
            var parentChainable = _springJoint.connectedBody.gameObject.GetComponent<Chainable>();
            parentChainable.AttachedObj = null;
        }
        RoundManager.Instance.OnBoxBroken();

        await Task.Delay(100);
        Instantiate(explosionEffect, transform.position, transform.rotation);

        if (AttachedObj != null)
        {
            AttachedObj.SendMessage("OnJointBreak2D", brokenJoint);
        }
        Destroy(gameObject);
    }
    
    private void OnDestroy()
    {
        // Destroy any children
        if (AttachedObj != null)
        {
            Destroy(AttachedObj.gameObject);
        }
    }
}
