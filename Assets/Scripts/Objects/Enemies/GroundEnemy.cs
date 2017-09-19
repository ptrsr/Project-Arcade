using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemy : MovingObject
{
    [SerializeField]
    private float
        _range,
        _rotationSpeed,
        _minimumDistance,
        _resetVelocity,
        _resetHeight,
        _rayForward,
        _rayDistance;

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
        if (!Active)
        {
            if (!Beamed)
                CheckIfUpright();

            return;
        }

        Quaternion desiredRotation = GetDesiredRotation(_target);

        Vector2 nextMove = new Vector2();

        _targetInRange = CheckInRange(_target);

        if (Mathf.Abs(_target.GlobePosition.x - GlobePosition.x) > _minimumDistance && 
            _targetInRange && Quaternion.Angle(_rotation, desiredRotation) < 2)
            nextMove = Movement(_target);

        Rotate(desiredRotation);

        nextMove = CheckDistance(nextMove);

        Move(nextMove, true);




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

    public void CheckIfUpright()
    {
        Vector3 currentGlobePos = Globe.SceneToGlobePosition(transform.position, true);

        if (Body.velocity.magnitude > _resetVelocity || currentGlobePos.y > _resetHeight)
            return;

        if (Vector3.Angle(transform.position.normalized, transform.up) < 10)
        {
            Active = true;
            MoveTarget.GlobePosition = new Vector3(currentGlobePos.x, MoveTarget.GlobePosition.y, MoveTarget.GlobePosition.z);
            return;
        }

        if (Vector3.Angle(transform.position.normalized, -transform.up) < 90)
        {
            DestroyableObject destroyableObject = GetComponent<DestroyableObject>();

            if (destroyableObject != null)
                destroyableObject.Explode();
        }
    }

    private Vector2 CheckDistance(Vector2 nextMove)
    {
        Ray ray = new Ray(transform.position + transform.forward * _rayForward, transform.forward);
        Debug.DrawRay(transform.position + _rotation * (Vector3.forward * _rayForward), transform.forward);

        if (Physics.Raycast(ray, _rayDistance))
            return new Vector2();

        return nextMove;
    }

    public bool TargetInRange
    {
        get { return _targetInRange; }
    }
}
