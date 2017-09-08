using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityObject : MonoBehaviour
{
    [SerializeField] [Range(0, 10)]
    private float _acceleration = 10.0f;

    private Rigidbody _rigidBody;

    [SerializeField]
    private bool _gravity;

    // Use this for initialization
    protected virtual void Start ()
    {
        _gravity = true;
        _rigidBody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
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
        _rigidBody.AddForce(transform.position.normalized * -_acceleration);
    }

    public bool Gravity
    {
        get { return _gravity;  }
        set { _gravity = value; }
    }
}
