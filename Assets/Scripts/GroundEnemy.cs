using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemy : GravityObject
{
    [SerializeField]
    private Vector2
        _movementSpeed = new Vector2(),
        _position2D = new Vector2();

    private float _worldRadius;

    private Vector3 _moveTarget;

    protected override void Start ()
    {
        base.Start();
        _worldRadius = ServiceLocator.Locate<Globe>().Radius;
	}

    protected override void Update()
    {
        base.Update();
    }
}
