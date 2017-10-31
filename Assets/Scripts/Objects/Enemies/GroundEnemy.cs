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
        _explodeNormal,
        _resetDistance;

    [SerializeField]
    private Vector2 _moveDist;

    private GlobeObject _target;

    private GravityObject _gravityObject;

    private Vector3
        _borderEnd,
        _borderStart,
        _bordercenter;

    private float _maxDist;

    protected virtual void Start()
    {
        _target = ServiceLocator.Locate<SpaceShip>();
        CalculateBoundaries();
    }

    protected virtual void Update()
    {
        if (!GravityObject.Kinematic)
            CheckIfUpright();
    }

    protected override void OnValidate()
    {
        base.OnValidate();
    }

    protected bool CheckInRange(Vector3 move)
    {
        float moveScalar = GlobeRadius + GlobePosition.y; // so the object speed doesn't change with altitude
        Vector3 newMove = new Vector3((move.x * MovementSpeed.x) / moveScalar, move.y * MovementSpeed.y, (move.z * MovementSpeed.z) / moveScalar) * Time.deltaTime;

        Vector3 delta = GlobePosition + newMove - _bordercenter;
        delta.y = 0;

        return delta.magnitude < _maxDist && CheckInRange();
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

    private void Explode()
    {
        DestroyableObject destroyableObject = GetComponent<DestroyableObject>();

        if (destroyableObject != null)
            destroyableObject.Explode();
    }

    private void CalculateBoundaries()
    {
        _borderStart = Globe.SceneToGlobePosition(ScenePosition + transform.forward * _moveDist.x / 10);
        _borderEnd   = Globe.SceneToGlobePosition(ScenePosition - transform.forward * _moveDist.y / 10);

        _borderStart.y = 0; _borderEnd.y = 0;

        _bordercenter = (_borderStart + _borderEnd) / 2;
        _maxDist = Vector3.Distance(_borderStart, _bordercenter);

        _borderStart = Globe.GlobeToScenePosition(_borderStart);
        _borderEnd   = Globe.GlobeToScenePosition(_borderEnd);
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying && transform.hasChanged)
            CalculateBoundaries();

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_borderStart, 1);
        Gizmos.DrawSphere(_borderEnd, 1);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(_borderStart, ScenePosition);
        Gizmos.DrawLine(_borderEnd, ScenePosition);
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
