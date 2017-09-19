using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityObject : MonoBehaviour
{
    [SerializeField]
    private bool _gravity;

    private GlobeObject _globeObject;

    private Globe _globe;
    private Rigidbody _rigidBody;


    protected virtual void Start ()
    {
        Body.useGravity = false;
        _globeObject = GetComponent<GlobeObject>();
    }
	
	protected virtual void Update ()
    {
        if (_gravity)
            ApplyGravity();
    }

    public void ApplyForce(Vector3 force)
    {
        if (Body != null)
            Body.velocity = force;
    }

    private void ApplyGravity()
    {
        if (Body != null)
            Body.AddForce(transform.position.normalized * -Globe.Gravity);
    }

    public Globe Globe
    {
        get
        {
            if (_globe == null)
                _globe = ServiceLocator.Locate<Globe>();

            return _globe;
        }
    }

    public bool Gravity
    {
        get { return _gravity;  }
        set { _gravity = value; }
    }

    protected Rigidbody Body
    {
        get
        {
            if (_rigidBody == null)
                _rigidBody = GetComponent<Rigidbody>();

            return _rigidBody;
        }
        set { _rigidBody = value; }
    }
}
