using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Beam : Weapon
{
    [SerializeField]
    private float
        _beamSpeed = 0.01f,
        _rotateSpeed = 5;

    private List<GravityObject> _beamableObjects;
    private MeshRenderer _mr;
    private Transform _attachmentPoint;

    private Vector2 _move;
    private Vector3 _lastPosition;

    private void Start()
    {
        _mr = GetComponent<MeshRenderer>();
        _mr.enabled = false;

        _attachmentPoint = transform.parent;

        _beamableObjects = new List<GravityObject>();

        transform.localEulerAngles = new Vector3(90, 0, 0);
    }

    public override void Aim(Vector2 movement)
    {
        _move = movement;
    }

    protected override void OnFire()
    {
        Vector3 deltaPosition = transform.position - _lastPosition;

        foreach (GravityObject gObject in _beamableObjects)
        {
            if (!gObject.Beamable)
                continue;

            gObject.transform.position += (transform.parent.position - gObject.transform.position).normalized * _beamSpeed + deltaPosition;
        }
        _lastPosition = transform.position;
    }

    protected override void OnFireEnabled()
    {
        _mr.enabled = true;
        _lastPosition = transform.position;

        for (int i = _beamableObjects.Count - 1; i >= 0; i--)
        {
            if (_beamableObjects[i] == null)
                _beamableObjects.RemoveAt(i);
        }

        foreach (GravityObject gObject in _beamableObjects)
            OnBeam(gObject);
    }

    protected override void OnFireDisabled()
    {
        _mr.enabled = false;

        foreach (GravityObject gObject in _beamableObjects)
        {
            if (!gObject.Beamable)
                continue;

            gObject.ApplyForce(new Vector3(_move.x, 0, _move.y));
            gObject.Beamed = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GravityObject gObject = other.GetComponent<GravityObject>();

        if (gObject == null || _beamableObjects.Contains(gObject))
            return;

        _beamableObjects.Add(gObject);

        if (Firing)
            OnBeam(gObject);
    }

    private void OnTriggerExit(Collider other)
    {
        GravityObject gObject = other.GetComponent<GravityObject>();

        if (gObject == null)
            return;

        if (_beamableObjects.Contains(gObject))
            _beamableObjects.Remove(gObject);
    }

    private void OnBeam(GravityObject gObject)
    {
        if (!gObject.Beamable)
            return;

        gObject.ApplyForce(new Vector3(), new Vector3(0, _rotateSpeed, 0));
        gObject.Beamed = true;
    }
}