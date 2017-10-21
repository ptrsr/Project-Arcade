using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : GroundEnemy
{
    private Vector3
        _planeNormal,
        _moveDirection;

    protected override void Start()
    {
        base.Start();

        SetPlane(transform.forward);
    }

    protected override void Update()
    {
        base.Update();

        if (!GravityObject.Kinematic)
            return;

        Vector3 move = Movement(Target);

        bool go = false;

        if (RotateTo(move, true, 10))
            go = true;

        if (!CheckInRange() || !go)
            return;

        Move(move);
    }

    private void SetPlane(Vector3 forward)
    {
        _planeNormal = Vector3.Cross(GlobeUp, forward);
        _moveDirection = Vector3.Cross(new Vector3(0, 1, 0), _planeNormal);
    }

    private Vector3 Movement(GlobeObject target)
    {
        Vector3 move = _moveDirection;
        move *= Vector3.Dot(_moveDirection, target.ScenePosition - ScenePosition) > 0 ? 1 : -1;
        return move;
    }

    protected override float DistanceToTarget(Vector3 objectPos, Vector3 targetPos)
    {
        float distance;
        Vector3 translationVector;

        //First calculate the distance from the point to the plane:
        distance = Vector3.Dot(_planeNormal, (targetPos - objectPos));

        //Reverse the sign of the distance
        distance *= -1;

        //Get a translation vector
        translationVector = _planeNormal.normalized * distance;

        //Translate the point to form a projection
        Vector3 localPoint = transform.InverseTransformPoint(targetPos + translationVector);
        localPoint.y = 0;
        Vector3 scenePoint = transform.TransformPoint(localPoint);

        return Vector3.Distance(ScenePosition, scenePoint);
    }
}
