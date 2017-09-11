using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MovingObject
{
    [SerializeField] [Range(0, 1)]
    private float 
        _headRotationSpeed;

    [SerializeField]
    private float
        _range,
        _bodyRotationSpeed;

    private Quaternion
        _bodyRotation,
        _headRotation;

    private GlobeObject _spaceShip;


    [SerializeField]
    private GameObject _head;

    protected override void Start()
    {
        base.Start();
        _bodyRotation = new Quaternion();
        _spaceShip = ServiceLocator.Locate<SpaceShip>();
	}
	
	void Update ()
    {
        Vector2 nextMove = Movement();
        Move(nextMove);
        Rotate(nextMove);
	}

    private void Rotate(Vector2 move)
    {
        int side = MoveTarget.GlobePosition.x < _spaceShip.GlobePosition.x ? 1 : -1;

        Quaternion desiredRotation = new Quaternion();
        desiredRotation.SetLookRotation(Vector3.Cross(GlobeUp, Vector3.forward) * side, GlobeUp);

        _bodyRotation = Quaternion.RotateTowards(_bodyRotation, desiredRotation, _bodyRotationSpeed);
        transform.rotation = _bodyRotation;

        _headRotation = Quaternion.Slerp(_headRotation, desiredRotation, _headRotationSpeed);
        _head.transform.rotation = _headRotation;
    }

    private Vector2 Movement()
    {
        Vector2 move = new Vector2();

        if (
            Vector3.Distance(MoveTarget.WorldPosition, _spaceShip.WorldPosition) < _range || 
            _spaceShip.GlobePosition.y - GlobePosition.y > _range
            )
            return move;

        move.x = MoveTarget.GlobePosition.x < _spaceShip.GlobePosition.x ? 1 : -1;

        return move;
    }
}
