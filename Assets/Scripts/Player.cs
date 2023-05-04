    using System;
    using UnityEngine;
    using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Chainable))]
public class Player : MonoBehaviour
{
    private float _inputAcceleration;
    private int _lastAccelDirection;
    private float _inputTurn;
    private float _turnAmount;
    private bool _isDrifting;
    private bool _isRecovering;
    private Vector2 forward => gameObject.transform.up;

    private float _scaledAccel;
    private float _scaledTopSpeed;
    private float _scaledHandlingDriftPrevention;
    private float _scaledHandlingTurnSnappiness;
    private int _maxTrailLength;

    [SerializeField] public float drag = 0.25f;
    [SerializeField] public float angularAcceleration = 135;
    public TrailRenderer _trail;

    private void Start()
    {
        var gm = GameManager.Instance;
        _scaledAccel = 2 * gm.GetUpgradeValue(Utils.UpgradeKey.Acceleration);
        _scaledTopSpeed = 5 + 0.75f * gm.GetUpgradeValue(Utils.UpgradeKey.TopSpeed);
        _scaledHandlingDriftPrevention = 0.5f + 0.1f * gm.GetUpgradeValue(Utils.UpgradeKey.Handling);
        _scaledHandlingTurnSnappiness = 0.2f * gm.GetUpgradeValue(Utils.UpgradeKey.Handling);
        _maxTrailLength = 1 * gm.GetUpgradeValue(Utils.UpgradeKey.ChainLength);
    }

    private void OnPause()
    {
        RoundManager.Instance.OnPause();
    }

    public void OnTurn(InputValue value)
    {
        _inputTurn = value.Get<float>();
    }

    public void OnAccelerate(InputValue value)
    {
        _inputAcceleration = value.Get<float>();
    }

    public void OnDrift(InputValue value)
    {
        _isDrifting = value.isPressed;
        if (_isDrifting)
        {
            _isRecovering = true;
        }
    }

    private void FixedUpdate()
    {
        var rigidBody = GetComponent<Rigidbody2D>();
        var currVel = rigidBody.velocity;

        // Standard Acceleration
        var push = forward * _inputAcceleration * _scaledAccel * Time.fixedDeltaTime;
        currVel += push;

        var dot = Vector2.Dot(currVel.normalized, forward);
        if (dot > 0 && _inputAcceleration > 0)
        {
            if (_lastAccelDirection == -1)
                _turnAmount = 0;
            _lastAccelDirection = 1;
        } else if (dot < 0 && _inputAcceleration < 0)
        {
            if (_lastAccelDirection == 1)
                _turnAmount = 0;
            _lastAccelDirection = -1;
        }

        /*
         * Turning
         */

        // Damp turn
        _turnAmount = Math.Max(Math.Abs(_turnAmount) - 3 * Time.fixedDeltaTime, 0) * Math.Sign(_turnAmount);
        // Then apply input
        _turnAmount = Math.Clamp(_turnAmount + _inputTurn * (4 + _scaledHandlingTurnSnappiness) * Time.fixedDeltaTime, -1, 1);
        var newRot = transform.rotation;
        var turnMultiplier = _isDrifting ? 2 : 1.5f;
        turnMultiplier *= _lastAccelDirection;
        newRot *= Quaternion.AngleAxis(_turnAmount * turnMultiplier * Time.fixedDeltaTime * angularAcceleration, Vector3.back);
        transform.rotation = newRot;





        // Corrective adjustment to keep car straight
        var maxCorrectiveForce = 0.7f;
        if (_isDrifting)
        {
            maxCorrectiveForce = 0.1f;
        } else if (_isRecovering)
        {
            maxCorrectiveForce = 0.3f;
        }
        maxCorrectiveForce *= _scaledHandlingDriftPrevention;


        var fullCorrection = forward * currVel.magnitude * (dot >= 0 ? 1 : -1) - currVel;
        var appliedCorrection = fullCorrection.normalized * Math.Min(maxCorrectiveForce, fullCorrection.magnitude);
        currVel += appliedCorrection;

        // Show trail if drifting
        var fwdPct = Vector2.Dot(currVel.normalized, forward);
        _isRecovering = Math.Abs(fwdPct) < 0.99;
        _trail.emitting = _isDrifting || _isRecovering;

        // Drag
        var dragMag = currVel.magnitude * drag;
        dragMag = Math.Max(dragMag, 0.5f) * (_inputAcceleration == 0 ? 4 : 1) * Time.fixedDeltaTime;
        dragMag = Math.Min(dragMag, currVel.magnitude);
        currVel += currVel.normalized * -1 * dragMag;

        // Limit max speed
        if (currVel.magnitude > _scaledTopSpeed)
        {
            currVel = currVel.normalized * _scaledTopSpeed;
        }

        rigidBody.velocity = currVel;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var chainable = GetComponent<Chainable>();
        var trailLength = GetTrailLength();

        if (other.CompareTag("Pickup"))
        {
            if (trailLength < _maxTrailLength)
            {
                other.GetComponent<Chainable>().OnAttach(chainable);
            }
        } else if (other.CompareTag("DropZone"))
        {
            if (chainable.AttachedObj != null)
            {
                RoundManager.Instance.AddScore(trailLength-1);
                chainable.AttachedObj.OnScore();
                chainable.AttachedObj = null;
            }
        }
    }

    public int GetTrailLength()
    {
        var chainable = GetComponent<Chainable>();
        var count = 0;
        while (chainable.AttachedObj != null)
        {
            count++;
            chainable = chainable.AttachedObj;
        }

        return count;
    }

}
