using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Beam : Weapon
{
    [SerializeField]
    private Shader _shader;

    [SerializeField]
    private Color _color;

    [SerializeField]
    private float
        _beamSpeed = 0.01f,
        _rotateSpeed = 5,
        _effectSpeed = 1,
        _effectMulti = 1;

    private Material _mat;
    private MeshRenderer _mr;

    private List<GravityObject> _beamableObjects;
    private Transform _attachmentPoint;

    private Vector2 _move;
    private Vector3 _lastPosition;

    private void Start()
    {
        _mr = GetComponent<MeshRenderer>();
        _mr.enabled = false;
        _mat = InitShader(_shader);
        _mr.material = _mat;

        _attachmentPoint = transform.parent;

        _beamableObjects = new List<GravityObject>();

        transform.localEulerAngles = new Vector3(90, 0, 0);
    }

    private Material InitShader(Shader shader)
    {
        Material mat = new Material(shader);
        SetUniforms(mat);
        return mat;
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
            SetUniforms(_mat);
    }

    private void SetUniforms(Material mat)
    {
        mat.SetColor("_color", _color);
        mat.SetFloat("_effectMulti", _effectMulti);
    }

    public override void Aim(Vector2 movement)
    {
        _move = movement;
    }

    protected override void OnFire()
    {
        _mat.SetFloat("_time", Time.fixedTime * _effectSpeed);

        Vector3 deltaPosition = transform.position - _lastPosition;

        for (int i = _beamableObjects.Count - 1; i >= 0; i--)
        {
            GravityObject gravityObject = _beamableObjects[i];

            if (gravityObject == null)
            {
                _beamableObjects.RemoveAt(i);
                continue;
            }

            if (!gravityObject.Beamable)
                continue;

            gravityObject.ApplyForce(new Vector3());
            gravityObject.transform.position += (transform.parent.position - gravityObject.transform.position).normalized * _beamSpeed + deltaPosition;
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
        {
            if (Firing)
                gObject.Beamed = false;
            _beamableObjects.Remove(gObject);
        }
    }

    private void OnBeam(GravityObject gObject)
    {
        if (!gObject.Beamable)
            return;

        gObject.ApplyForce(new Vector3(), new Vector3(0, _rotateSpeed, 0));
        gObject.Beamed = true;
    }
}