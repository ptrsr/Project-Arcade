using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityObject : MonoBehaviour
{
    [SerializeField]
    private bool
        _gravity = false,
        _startKinematic = true,
        _beamable = true,
        _sinkable = true;

    private bool _beamed = false;

    private GlobeObject _globeObject;

    private Globe _globe;
    private Rigidbody _rigidBody;
    private Collider _col;

    protected virtual void Start ()
    {
        Body.useGravity = false;
        Body.isKinematic = _startKinematic;
        _globeObject = GetComponent<GlobeObject>();
        _col = GetComponent<Collider>();
    }
	
	protected virtual void Update ()
    {
        if (_gravity)
            ApplyGravity();

        if (_sinkable && IsUnderWater())
            Sink();
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

    private bool IsUnderWater()
    {
        if (Kinematic || Col == null)
            return false;

        Vector3 highestPoint = Col.ClosestPoint(Col.bounds.center + transform.position.normalized * Col.bounds.extents.magnitude);
        Vector3 globePos = Globe.SceneToGlobePosition(highestPoint);

        return globePos.y < -20;
    }

    private void Sink()
    {
        Instantiate(ServiceLocator.Locate<Effects>().Splash, transform.position, transform.rotation);
        Destroy(gameObject);
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

    protected Collider Col
    {
        get
        {
            if (_col == null)
                _col = GetComponent<Collider>();

            return _col;
        }
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
        get
        {
            if (Body == null)
                return true;

            return Body.isKinematic;
        }
        protected set { Body.isKinematic = value; }
    }

    protected bool Sinkable
    {
        get { return _sinkable;  }
        set { _sinkable = value; }
    }
}
