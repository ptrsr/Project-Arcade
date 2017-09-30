using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : GlobeObject
{
    protected enum RotateType
    {
        towards,
        slerp
    }

    [SerializeField]
    protected RotateType _rotateType = RotateType.towards;

    [SerializeField] [Range(0, 1)]
    private float _acceleration = 0.9f;

    [SerializeField]
    protected float _rotateSpeed;

    [SerializeField]
    private Vector3
        _movementSpeed = new Vector3();

    private Vector3
        _lastMove;

    protected Quaternion
        _lastRotation;

    private Rigidbody   _rigidBody;

    protected void Move(Vector3 move)
    {
        if (move == new Vector3())
            return;

        float moveScalar = GlobeRadius + GlobePosition.y; // so the object speed doesn't change with altitude
        _lastMove = new Vector3((move.x * _movementSpeed.x) / moveScalar, move.y * _movementSpeed.y, (move.z * _movementSpeed.z) / moveScalar) * Time.deltaTime;

        _lastRotation = transform.rotation;
        GlobePosition += _lastMove;

        RotateTo(_lastMove);
    }

    protected virtual void RotateTo(Vector3 move, float testAngle = 0)
    {
        transform.up = GlobeUp;
        Quaternion desired = transform.rotation * Quaternion.LookRotation(move.normalized);

        if (_rotateType == RotateType.towards)
            transform.rotation = Quaternion.RotateTowards(_lastRotation, desired, _rotateSpeed);
        else
            transform.rotation = Quaternion.Slerp(_lastRotation, desired, _rotateSpeed);
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

    public Vector3 LastMove
    {
        get { return _lastMove; }
    }

    public Vector2 MovementSpeed
    {
        get { return _movementSpeed;  }
        set { _movementSpeed = value; }
    }
}
