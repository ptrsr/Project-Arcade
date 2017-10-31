using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RotateType
{
    towards,
    slerp
}

public class MovingObject : GlobeObject
{
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
        _lastMove,
        _terrainNormal;

    private Rigidbody   _rigidBody;

    protected void Move(Vector3 move)
    {
        if (move == new Vector3())
            return;

        float moveScalar = GlobeRadius + GlobePosition.y; // so the object speed doesn't change with altitude
        _lastMove = new Vector3((move.x * _movementSpeed.x) / moveScalar, move.y * _movementSpeed.y, (move.z * _movementSpeed.z) / moveScalar) * Time.deltaTime;


        GlobePosition += _lastMove;
    }

    protected virtual bool RotateTo(Vector3 move, bool onTerrain = true, float testAngle = 0)
    {
        if (move.x == 0 && move.z == 0)
            return false;

        Quaternion lastRotation = transform.rotation;

        transform.up = onTerrain ? _terrainNormal : GlobeUp;

        Vector3 localDirection = transform.InverseTransformDirection(move);
        localDirection.y = 0;
        Vector3 direction = transform.TransformDirection(localDirection);

        Quaternion desired = Quaternion.LookRotation(direction, transform.up);

        Vector3 desiredVector = desired * Vector3.forward;
        Vector3 currentVector = lastRotation * Vector3.forward;
        desiredVector.y = 0; currentVector.y = 0;

        bool test = Vector3.Angle(desiredVector.normalized, currentVector.normalized) <= testAngle;

        if (_rotateType == RotateType.towards)
            transform.rotation = Quaternion.RotateTowards(lastRotation, desired, _rotateSpeed * Time.deltaTime);
        else
            transform.rotation = Quaternion.Slerp(lastRotation, desired, _rotateSpeed);

        return test;
    }

    public override Vector3 GlobePosition
    {
        get
        {
            return base.GlobePosition;
        }

        set
        {
            _globePosition = value;

            transform.position = Globe.GlobeToScenePosition(value, out _terrainNormal);
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

    public Vector3 LastMove
    {
        get { return _lastMove; }
    }

    public Vector3 MovementSpeed
    {
        get { return _movementSpeed;  }
        set { _movementSpeed = value; }
    }
}
