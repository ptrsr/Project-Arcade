using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : GlobeObject
{
    [SerializeField] [Range(0, 1)]
    private float _acceleration = 0.9f;

    [SerializeField]
    private Vector2
        _movementSpeed = new Vector2();

    private GlobeObject _moveTarget;

    protected virtual void Start ()
    {
        _moveTarget = new GlobeObject();
        _moveTarget.GlobePosition = GlobePosition;
    }
	
    protected void Move(Vector2 move)
    {
        float moveScalar = GlobeRadius + GlobePosition.y; // so the object speed doesn't change with altitude
        _moveTarget.GlobePosition += new Vector3((move.x * _movementSpeed.x) / moveScalar, move.y * _movementSpeed.y, 0) * Time.deltaTime;

        GlobePosition = Vector3.Slerp(GlobePosition, _moveTarget.GlobePosition, _acceleration);
    }

    public GlobeObject MoveTarget
    {
        get { return _moveTarget; }
    }
}
