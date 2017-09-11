using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityObject : GlobeObject
{
    private Rigidbody _rigidBody;

    private bool _gravity;

    protected virtual void Start ()
    {
        _gravity = true;
        _rigidBody = GetComponent<Rigidbody>();
	}
	
	protected virtual void Update ()
    {
        if (_gravity)
            ApplyGravity();
    }

    public void ApplyForce(Vector3 force)
    {
        _rigidBody.velocity = force;
    }

    private void ApplyGravity()
    {
        _rigidBody.AddForce(transform.position.normalized * -Globe.Gravity);
    }

    public bool Gravity
    {
        get { return _gravity;  }
        set { _gravity = value; }
    }
}
