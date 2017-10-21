using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityObject : MonoBehaviour
{
    [SerializeField]
    private bool
        _gravity = false,
        _startKinematic = true,
        _beamable = true;

    private bool _beamed = false;

    private GlobeObject _globeObject;

    private Globe _globe;
    private Rigidbody _rigidBody;


    protected virtual void Start ()
    {
        Body.useGravity = false;
        Body.isKinematic = _startKinematic;
        _globeObject = GetComponent<GlobeObject>();
    }
	
	protected virtual void Update ()
    {
        if (_gravity)
            ApplyGravity();
    }

    public void ApplyForce(Vector3 force, Vector3 torq = new Vector3())
    {
        if (Body != null)
        {
            Body.velocity = force;

            if (torq != new Vector3())
                Body.angularVelocity = torq;
        }
    }

    private void ApplyGravity()
    {
        if (Body != null)
            Body.AddForce(transform.position.normalized * -Globe.Gravity);
    }

    public void Reset(Vector3 globePosition)
    {
        GlobeObject.GlobePosition = globePosition;
        Kinematic = true;
        Gravity = false;
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

    private GlobeObject GlobeObject
    {
        get
        {
            if (_globeObject == null)
                _globeObject = GetComponent<GlobeObject>();

            return _globeObject;
        }
    }

    public bool Beamed
    {
        get { return _beamed;  }
        set
        {
            _beamed = value;

            Gravity = !value;

            Kinematic = false;
        }
    }

    public bool Beamable
    {
        get { return _beamable; }
        protected set
        {
            _beamable = value;
            if (!value)
                Beamed = false;
        }
    }

    public bool Gravity
    {
        get { return _gravity; }
        protected set { _gravity = value; }
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

    public bool Kinematic
    {
        get { return Body.isKinematic; }
        protected set { Body.isKinematic = value; }
    }
}
