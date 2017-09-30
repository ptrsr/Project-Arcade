using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemy : MovingObject
{
    [SerializeField]
    private float
        _range,
        _minimumDistance,
        _resetVelocity,
        _resetHeight,
        _rayForward,
        _rayDistance;

    [SerializeField]
    private TurretHead _turret;
    private GlobeObject _target;

    private Renderer rend;

    private bool _targetInRange = false;


    void Start()
    {
        _target = ServiceLocator.Locate<SpaceShip>();
        rend = GetComponent<Renderer>();

        _turret.Parent = this;
    }


    void Update()
    {
        if (!Active)
        {
            if (Body != null && !Beamed)
                CheckIfUpright();

            return;
        }

        _targetInRange = CheckInRange(_target);


        print(Vector3.Distance(WorldPosition, _target.WorldPosition));
        if (_targetInRange && _turret != null)
        {
            _turret.Aim(_target);
        }
    }


    private bool CheckInRange(GlobeObject target)
    {
        return
            Vector3.Distance(WorldPosition, target.WorldPosition) < _range &&
            target.GlobePosition.y - GlobePosition.y < _range;
    }


    public void CheckIfUpright()
    {
        Vector3 currentGlobePos = Globe.SceneToGlobePosition(transform.position, true);

        if (Body.velocity.magnitude > _resetVelocity || currentGlobePos.y > _resetHeight)
            return;

        if (Vector3.Angle(transform.position.normalized, transform.up) < 10)
        {
            Active = true;
            GlobePosition = new Vector3(currentGlobePos.x, GlobePosition.y, GlobePosition.z);
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

        if (Physics.Raycast(ray, _rayDistance))
            return new Vector2();

        return nextMove;
    }


    public bool TargetInRange
    {
        get { return _targetInRange; }
    }
}
