using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.TextCore.Text;

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
[RequireComponent(typeof(LineRenderer))]
public class Chainable : MonoBehaviour
{
    [SerializeField] public Transform anchorFront;
    [SerializeField] public Transform anchorRear;
    public ParticleSystem explosionEffect;
    public SpriteRenderer spriteRenderer;

    private LineRenderer _lineRenderer;
    [CanBeNull] private SpringJoint2D _springJoint;
    private int _score;

    [CanBeNull] private Chainable _parentObj;

    [CanBeNull] public Chainable AttachedObj;

    private void Start()
    {
        _lineRenderer = gameObject.GetComponent<LineRenderer>();

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
        if (_parentObj == null)
            return;


        if (_springJoint != null)
        {
            _lineRenderer.SetPositions(new Vector3[] {_parentObj.anchorRear.position, anchorFront.position});
            var dmgColor = Color.Lerp(Color.white, Color.red,
                _springJoint.reactionForce.magnitude / _springJoint.breakForce);
            _lineRenderer.startColor = dmgColor;
            _lineRenderer.endColor = dmgColor;
            spriteRenderer.color = dmgColor;
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
        _parentObj = attachTo;
        _lineRenderer.positionCount = 2;

        _parentObj.AttachedObj = this;

        var destRot = _parentObj.transform.rotation;
        transform.rotation = destRot;
        transform.position = _parentObj.transform.position + destRot * _parentObj.anchorRear.localPosition +
                             (transform.position - anchorFront.transform.position);
        GetComponent<Rigidbody2D>().velocity = _parentObj.GetComponent<Rigidbody2D>().velocity;

        var myCollider = GetComponent<BoxCollider2D>();
        myCollider.isTrigger = false;

        if (_springJoint != null)
        {
            _springJoint.enabled = true;
            _springJoint.connectedBody = _parentObj.GetComponent<Rigidbody2D>();
            _springJoint.connectedAnchor = _parentObj.anchorRear.localPosition;
            _springJoint.anchor = anchorFront.localPosition;

            _parentObj.TryGetComponent<SpringJoint2D>(out var parentSpringJoint);
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
        if (_parentObj != null)
        {
            _parentObj.AttachedObj = null;
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
