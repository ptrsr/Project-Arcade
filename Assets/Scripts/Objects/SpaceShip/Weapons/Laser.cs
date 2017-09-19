using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : Weapon
{
    [SerializeField]
    private float
        _maxAngle, // max angle for each side in degrees
        _rotateSpeed,
        _diameter,
        _dps;

    private float
        _currentAngle;

    private MeshRenderer _mr;
    private Transform _attachmentPoint;

    private void Start()
    {
        _mr = GetComponent<MeshRenderer>();
        _mr.enabled = false;

        _attachmentPoint = transform.parent;

        transform.localScale = new Vector3(_diameter, 0, _diameter);
    }

    public override void Aim(Vector2 movement)
    {
        float desiredAngle = movement.x * _maxAngle;
        _currentAngle = Mathf.MoveTowards(_currentAngle, desiredAngle, _rotateSpeed);
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
        rotation.eulerAngles = new Vector3(0, 0, _currentAngle);
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
    }
}
