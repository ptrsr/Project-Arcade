using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovingObject : GlobeObject
{
    [SerializeField] [Range(0, 1)]
    private float _acceleration = 0.9f;

    [SerializeField]
    private Vector2
        _movementSpeed = new Vector2();

    private Rigidbody   _rigidBody;
    private GlobeObject _moveTarget;

    protected virtual void Start ()
    {
        _moveTarget = new GlobeObject();
        _moveTarget.Active = false;
        _moveTarget.GlobePosition = GlobePosition;
    }
	
    protected void Move(Vector2 move, bool saveOrientation = false)
    {
        if (!Active)
            return;

        float moveScalar = GlobeRadius + GlobePosition.y; // so the object speed doesn't change with altitude
        _moveTarget.GlobePosition += new Vector3((move.x * _movementSpeed.x) / moveScalar, move.y * _movementSpeed.y, 0) * Time.deltaTime;

        Quaternion rotation = transform.rotation;
        GlobePosition = Vector3.Slerp(GlobePosition, _moveTarget.GlobePosition, _acceleration);

        if (saveOrientation)
            transform.rotation = rotation;
    }

    public GlobeObject MoveTarget
    {
        get { return _moveTarget; }
        protected set { _moveTarget = value; }
    }

    public override bool Active
    {
        get { return _active;  }
        set
        {
            _active = value;

            if (_active)
                _moveTarget.GlobePosition = GlobePosition;
        }
    }

    protected Rigidbody Body
    {
        get
        {
            if (_rigidBody == null)
                _rigidBody = GetComponent<Rigidbody>();

            return _rigidBody;
        }
        set { _rigidBody = value; }
    }

    public Vector2 MovementSpeed
    {
        get { return _movementSpeed;  }
        set { _movementSpeed = value; }
    }
}
