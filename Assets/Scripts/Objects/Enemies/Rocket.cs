using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField]
    private float
        _force,
        _rotationSpeed,
        _missAngle,
        _missTime;

    private float
        _timer,
        _destroyTime = 0;

    [SerializeField]
    private DestroyableObject _destroyableObject;

    [SerializeField]
    private MeshRenderer _mr;

    [SerializeField]
    private Rigidbody _rb;

    private GlobeObject _target;

    private bool _fired = false;

    private void Update()
    {
        if (!_fired)
            return;

        _timer += Time.deltaTime;

        _rb.velocity = transform.forward * _force * _timer;

        if (_destroyTime == 0)
        {

            Quaternion desired = Quaternion.LookRotation((_target.transform.position - transform.position).normalized);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, desired, _rotationSpeed * Time.deltaTime);

            if (Quaternion.Angle(transform.rotation, desired) > _missAngle)
                _destroyTime = _timer + _missTime;
        }
        else if (_timer > _destroyTime)
            _destroyableObject.Explode();

    }


    public void Fire(GlobeObject target)
    {
        _target = target;
        transform.parent = null;
        _fired = true;
    }


    public void SetTransparancy(float value)
    {
        Color color = _mr.material.color;
        color.a = value;

        _mr.material.color = color;
    }
}