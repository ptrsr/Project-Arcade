using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemy : MonoBehaviour {


    [SerializeField]
    [Range(0, 1)]
    private float _acceleration = 0.9f;

    [SerializeField]
    private Vector2
        _movementSpeed = new Vector2(),
        _position2D = new Vector2();

    private float _worldRadius;

    private Vector3 _moveTarget;

    private Rigidbody _rigidBody;

    void Start ()
    {
        _worldRadius = ServiceLocator.Locate<Globe>().Size / 2;
        _rigidBody = GetComponent<Rigidbody>();
	}
	
	void Update ()
    {
        UseGravity();
    }

    private void UseGravity()
    {
        _rigidBody.AddForce(transform.position.normalized * -_acceleration);
    }
}
