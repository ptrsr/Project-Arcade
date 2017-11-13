using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part : GravityObject
{
    [SerializeField]
    private float
        _maxVelocityDespawn = 1,
        _maxAltitudeDespawn = 2,
        _despawnTime = 2,
        _sinkSpeed = 1,
        _despawnDepth = -1;

    [SerializeField]
    private int blinkSpeed = 5;

    private float _despawnTimer;
    bool _despawn = false;

    Vector3 _explodeForce = new Vector3();
    Material _mat;

    protected override void Start()
    {
        ServiceLocator.Locate<ObjectSafe>().AddTemporaryObject(gameObject);

        gameObject.layer = 8;
        name = "Part";

        _mat = GetComponent<MeshRenderer>().material;
        _mat.EnableKeyword("_EMISSION");

        Rigidbody rb;

        rb = GetComponent<Rigidbody>();

        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        if (GetComponent<Collider>() == null)
            gameObject.AddComponent<BoxCollider>();

        base.Start();

        Gravity = true;
        Kinematic = false;
        rb.drag = 0.5f;

        _despawnTimer = _despawnTime;

        transform.DetachChildren();
        ApplyForce(_explodeForce);
    }

    private void Emit()
    {
        Color emitColor = Mathf.PingPong(_despawnTimer * blinkSpeed + _despawnTime, _despawnTime) * Color.yellow;
        _mat.SetColor("_EmissionColor", emitColor);
    }

    protected override void Update()
    {
        base.Update();

        Emit();

        if (_despawn)
            return;

        if (Body.velocity.magnitude < _maxVelocityDespawn &&
            Globe.SceneToGlobePosition(transform.position).y < _maxAltitudeDespawn)
        {
            _despawnTimer = Mathf.Clamp(_despawnTimer - Time.deltaTime, 0, _despawnTime);

            if (_despawnTimer == 0)
                OnDespawn();
        }
        else
            _despawnTimer = _despawnTime;
    }

    private void OnDespawn()
    {
        _despawn = true;
        Beamable = false;
        _mat.DisableKeyword("_EMISSION");

        Destroy(Body);
        Destroy(GetComponent<Collider>());
    }

    private void FixedUpdate()
    {
        if (!_despawn)
            return;

        transform.position -= transform.position.normalized * _sinkSpeed * Time.deltaTime;

        if (Globe.SceneToGlobePosition(transform.position).y < _despawnDepth)
            Destroy(gameObject);
    }

    public Vector3 ExplodeForce
    {
        get { return _explodeForce;  }
        set { _explodeForce = value; }
    }
}
