﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Beam : Weapon
{
    [SerializeField]
    private float _beamSpeed = 0.01f;

    private List<GravityObject> _beamableObjects;
    private MeshRenderer _mr;
    private Transform _attachmentPoint;

    private Vector2 _move;

    private void Start()
    {
        _mr = GetComponent<MeshRenderer>();
        _mr.enabled = false;

        _attachmentPoint = transform.parent;

        _beamableObjects = new List<GravityObject>();
    }

    public override void Aim(Vector2 movement)
    {
        _move = -movement * 50;
    }

    protected override void OnFire()
    {
        foreach (GravityObject gObject in _beamableObjects)
        {
            gObject.transform.parent = transform;
            gObject.ApplyForce(new Vector3());
            gObject.transform.position += (transform.parent.position - gObject.transform.position).normalized * _beamSpeed;
            gObject.Gravity = false;

        }
    }

    protected override void OnFireEnabled()
    {
        _mr.enabled = true;
    }

    protected override void OnFireDisabled()
    {
        _mr.enabled = false;

        foreach (GravityObject gObject in _beamableObjects)
        {
            gObject.transform.parent = null;
            gObject.Gravity = true;

            gObject.ApplyForce(new Vector3(_move.x, _move.y, 0));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GravityObject gObject = other.GetComponent<GravityObject>();

        if (gObject == null || _beamableObjects.Contains(gObject))
            return;

        _beamableObjects.Add(gObject);
    }

    private void OnTriggerExit(Collider other)
    {
        GravityObject gObject = other.GetComponent<GravityObject>();

        if (gObject == null)
            return;

        if (_beamableObjects.Contains(gObject))
            _beamableObjects.Remove(gObject);

        gObject.Gravity = true;
    }
}