using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GravityObject))]
public class GroundEnemy : MovingObject
{
    [SerializeField]
    private float
        _range,
        _minimumDistance,
        _resetVelocity,
        _resetHeight,
        _rayForward,
        _rayDistance,
        _explodeNormal,
        _resetDistance;

    private GlobeObject _target;

    private GravityObject _gravityObject;

    protected virtual void Start()
    {
        _target = ServiceLocator.Locate<SpaceShip>();
    }

    protected virtual void Update()
    {
        if (!GravityObject.Kinematic)
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
        if (GravityObject.Beamed || Body.velocity.magnitude > _resetVelocity)
            return;

        RaycastHit hit;
        Vector3 rayPos = transform.position.normalized * (Globe.MaxHeight + 1);

        if (Physics.Raycast(rayPos, -GlobeUp, out hit, Globe.MaxHeight - Globe.WaterLevel + 1, 1 << 10))
        {
            if (Vector3.Distance(hit.point, transform.position) > _resetDistance)
                return;

            float angle = Vector3.Angle(transform.up, hit.normal);

            if (angle < _explodeNormal)
            {
                float tempY = GlobePosition.y;
                Vector3 position = Globe.SceneToGlobePosition(ScenePosition);
                GravityObject.Reset(new Vector3(position.x, tempY, position.z));
                return;
            }
            else
                Explode();
        }
    }

    private bool CheckDistance()
    {
        Ray ray = new Ray(transform.position + transform.forward * _rayForward, transform.forward);

        if (Physics.Raycast(ray, _rayDistance))
            return false;

        return true;
    }

    private void Explode()
    {
        DestroyableObject destroyableObject = GetComponent<DestroyableObject>();

        if (destroyableObject != null)
            destroyableObject.Explode();
    }

    protected GlobeObject Target
    {
        get { return _target; }
    }

    protected GravityObject GravityObject
    {
        get
        {
            if (_gravityObject == null)
                _gravityObject = GetComponent<GravityObject>();

            return _gravityObject;
        }
    }
}
