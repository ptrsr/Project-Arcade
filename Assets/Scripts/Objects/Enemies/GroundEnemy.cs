using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemy : MovingObject
{
    [SerializeField]
    private float
        _range,
        _rotationSpeed,
        _minimumDistance;

    private Quaternion
        _rotation;

    private GlobeObject _target;

    [SerializeField]
    private TurretHead _turret;

    private bool _targetInRange = false;

    protected override void Start()
    {
        base.Start();
        _rotation = new Quaternion();
        _target = ServiceLocator.Locate<SpaceShip>();

        _turret.Parent = this;
	}
	
	void Update ()
    {
        Quaternion desiredRotation = GetDesiredRotation(_target);

        Vector2 nextMove = new Vector2();

        _targetInRange = CheckInRange(_target);

        if (_targetInRange && Quaternion.Angle(_rotation, desiredRotation) < 2)
            nextMove = Movement(_target);

        if (Mathf.Abs(_target.GlobePosition.x - GlobePosition.x) > _minimumDistance)
            Move(nextMove);

        Rotate(desiredRotation);

        if (_targetInRange && _turret != null)
            _turret.Aim(_target);
    }

    private bool CheckInRange(GlobeObject target)
    {
        return
            Vector3.Distance(MoveTarget.WorldPosition, target.WorldPosition) < _range &&
            target.GlobePosition.y - GlobePosition.y < _range;
    }

    private Quaternion GetDesiredRotation(GlobeObject target)
    {
        int side = MoveTarget.GlobePosition.x < target.GlobePosition.x ? 1 : -1;

        Quaternion desiredRotation = new Quaternion();
        desiredRotation.SetLookRotation(Vector3.Cross(GlobeUp, Vector3.forward) * side, GlobeUp);

        return desiredRotation;
    }

    private void Rotate(Quaternion desiredRotation)
    {
        _rotation = Quaternion.RotateTowards(_rotation, desiredRotation, _rotationSpeed);
        transform.rotation = _rotation;
    }

    private Vector2 Movement(GlobeObject target)
    {
        Vector2 move = new Vector2();
        move.x = MoveTarget.GlobePosition.x < target.GlobePosition.x ? 1 : -1;
        return move;
    }

    public bool TargetInRange
    {
        get { return _targetInRange; }
    }
}
