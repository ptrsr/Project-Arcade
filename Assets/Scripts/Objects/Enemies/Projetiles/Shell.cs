﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : Projectile
{
    [SerializeField]
    private float
        _velocity,
        _timeOut;

    private float _timeSinceSpawned = 0;

    protected override void Start()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * _velocity, ForceMode.Impulse);
    }

    private void Update()
    {
        _timeSinceSpawned += Time.deltaTime;

        if (_timeSinceSpawned > _timeOut)
            Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}