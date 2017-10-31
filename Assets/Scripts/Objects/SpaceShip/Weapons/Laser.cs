using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : Weapon
{
    [SerializeField]
    private Shader _shader;

    [SerializeField]
    private float
        _maxAngle, // max angle for each side in degrees
        _rotateSpeed,
        _diameter,
        _dps,
        _effectMulti,
        _timeMulti = 1,
        _darken = 1;

    [SerializeField]
    private Color _color;

    private Vector2  _currentAngle;

    private MeshRenderer _mr;
    private Material _mat;

    private Transform _attachmentPoint;

    private void Start()
    {
        _mr = GetComponent<MeshRenderer>();
        _mr.enabled = false;
        _mr.material = InitShader(_shader);
        _attachmentPoint = transform.parent;

        transform.localScale = new Vector3(_diameter, 0, _diameter);
    }

    private Material InitShader(Shader shader)
    {
        Material mat = new Material(shader);
        _mat = mat;
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
        mat.SetFloat("_darken", _darken);
    }

    public override void Aim(Vector2 movement)
    {
        Vector2 desiredAngle = movement * _maxAngle;
        _currentAngle = new Vector2(Mathf.MoveTowards(_currentAngle.x, desiredAngle.x, _rotateSpeed), Mathf.MoveTowards(_currentAngle.y, desiredAngle.y, _rotateSpeed));

    }

    protected override void OnFireEnabled()
    {
        _mr.enabled = true;
    }

    protected override void OnFireDisabled()
    {
        _mr.enabled = false;
    }

    protected override void OnFire()
    {
        Quaternion rotation = new Quaternion();
        rotation.eulerAngles = new Vector3(-_currentAngle.y, 0, _currentAngle.x);
        rotation *= transform.parent.rotation;

        Ray ray = new Ray(transform.parent.position, rotation * Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            transform.position = (hit.point + transform.parent.position) / 2;
            transform.rotation = rotation;
            transform.localScale = new Vector3(_diameter, hit.distance / 2, _diameter);

            DestroyableObject destroyableObject = hit.transform.gameObject.GetComponent<DestroyableObject>();

            if (destroyableObject != null)
                destroyableObject.Damage(_dps * Time.deltaTime);
        }

        _mat.SetFloat("_time", Time.fixedTime * _timeMulti);
    }
}
