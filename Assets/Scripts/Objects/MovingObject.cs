using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovingObject : GlobeObject
{
    [SerializeField] [Range(0, 1)]
    private float _acceleration = 0.9f;

    [SerializeField]
    private float _rotateSpeed;

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
	
    protected void Move(Vector2 move)
    {
        if (!Active)
            return;

        float moveScalar = GlobeRadius + GlobePosition.y; // so the object speed doesn't change with altitude

        Vector3 direction = Globe.GlobeToScenePosition(_moveTarget.GlobePosition);
        _moveTarget.GlobePosition += new Vector3((move.x * _movementSpeed.x) / moveScalar, 0, (move.y * _movementSpeed.y) / moveScalar) * Time.deltaTime;
        direction = (Globe.GlobeToScenePosition(_moveTarget.GlobePosition) - direction).normalized;

        Quaternion lastRotation = transform.rotation;
        GlobePosition = Vector3.Slerp(GlobePosition, _moveTarget.GlobePosition, _acceleration);

        if (move == new Vector2())
        {
            transform.rotation = lastRotation;
            return;
        }
        Quaternion desiredRotation = Quaternion.LookRotation(direction, GlobeUp);
        transform.rotation = Quaternion.RotateTowards(lastRotation, desiredRotation, _rotateSpeed);
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
