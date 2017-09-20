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

    private float _despawnTimer;
    bool _despawn = false;

    Vector3 _explodeForce = new Vector3();

    protected override void Start()
    {
        gameObject.layer = 8;
        name = "Part";

        if (GetComponent<Rigidbody>() == null)
            gameObject.AddComponent<Rigidbody>();

        if (GetComponent<Collider>() == null)
            gameObject.AddComponent<BoxCollider>();

        base.Start();
        Gravity = true;

        _despawnTimer = _despawnTime;

        transform.DetachChildren();
        ApplyForce(_explodeForce);
    }

    protected override void Update()
    {
        base.Update();

        if (_despawn)
            return;

        if (Body.velocity.magnitude < _maxVelocityDespawn &&
            Globe.SceneToGlobePosition(transform.position, true).y < _maxAltitudeDespawn)
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
        Gravity = false;

        Destroy(Body);
        Destroy(GetComponent<Collider>());
    }

    private void FixedUpdate()
    {
        if (!_despawn)
            return;

        transform.position -= transform.position.normalized * _sinkSpeed * Time.deltaTime;

        if (Globe.SceneToGlobePosition(transform.position, true).y < _despawnDepth)
            Destroy(gameObject);
    }

    public Vector3 ExplodeForce
    {
        get { return _explodeForce;  }
        set { _explodeForce = value; }
    }
}
