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

    private GlobeObject _target;

    protected virtual void Start()
    {
        _target = ServiceLocator.Locate<SpaceShip>();
    }


    protected virtual void Update()
    {
        if (Beamed)
            CheckIfUpright();
    }


    protected bool CheckInRange()
    {
        float distance = DistanceToTarget(ScenePosition, Target.ScenePosition);
        return distance > _minimumDistance && distance < _range;
    }

    protected virtual float DistanceToTarget(Vector3 objectPos, Vector3 targetPos)
    {
        return Vector3.Distance(ScenePosition, Target.ScenePosition);
    }


    private void CheckIfUpright()
    {
        Vector3 currentGlobePos = Globe.SceneToGlobePosition(transform.position, true);

        if (Body.velocity.magnitude > _resetVelocity || currentGlobePos.y > _resetHeight)
            return;

        if (Vector3.Angle(transform.position.normalized, transform.up) < 10)
        {
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


    private bool CheckDistance()
    {
        Ray ray = new Ray(transform.position + transform.forward * _rayForward, transform.forward);

        if (Physics.Raycast(ray, _rayDistance))
            return false;

        return true;
    }

    protected GlobeObject Target
    {
        get { return _target; }
    }
}
