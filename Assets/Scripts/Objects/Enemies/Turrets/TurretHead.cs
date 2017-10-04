using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class TurretHead : MonoBehaviour
{
    [SerializeField]
    RotateType _rotateType = RotateType.slerp;

    [SerializeField]
    private float
        _headRotationSpeed,
        _barrelRotationSpeed;

    [SerializeField]
    private Transform
        _head,
        _barrel;

    //[SerializeField]
    //private Transform[]
    //    _projectileSpawnPoints;

    [SerializeField]
    protected GameObject _projectilePrefab;

    [SerializeField]
    private float
        _reloadSpeed,
        _shootAngle = 5,
        _range = 20;

    protected GlobeObject _target;

    private Quaternion
        _idleBarrelRotation,
        _idleHeadRotation;

    private float _reloadTime;

    private void Start()
    {
        _target = ServiceLocator.Locate<SpaceShip>();

        _idleHeadRotation = _head.rotation;
        _idleBarrelRotation = _barrel.rotation;
    }

    protected virtual void Update()
    {
        _reloadTime = Mathf.Clamp(_reloadTime - Time.deltaTime, 0, _reloadSpeed);

        if (CheckInRange(_target))
            Aim(_target);
        else
            Idle();
    }

    private void Idle()
    {
        _head.rotation = _rotateType == RotateType.slerp ?
            Quaternion.Slerp(_head.rotation, _idleHeadRotation, _headRotationSpeed * Time.deltaTime) :
            Quaternion.RotateTowards(_head.rotation, _idleHeadRotation, _headRotationSpeed * Time.deltaTime);

        _barrel.rotation = _rotateType == RotateType.slerp ?
            Quaternion.Slerp(_barrel.rotation, _idleBarrelRotation, _barrelRotationSpeed * Time.deltaTime) :
            Quaternion.RotateTowards(_barrel.rotation, _idleBarrelRotation, _barrelRotationSpeed * Time.deltaTime);
    }

    public void Aim(GlobeObject target)
    {
        // head rotation
        Quaternion lastHeadRotation = _head.rotation;
        _head.LookAt(target.transform);
        _head.localEulerAngles = new Vector3(0, _head.localEulerAngles.y, 0);

        _head.rotation = _rotateType == RotateType.slerp ?
            Quaternion.Slerp(lastHeadRotation, _head.rotation, _headRotationSpeed * Time.deltaTime) :
            Quaternion.RotateTowards(lastHeadRotation, _head.rotation, _headRotationSpeed * Time.deltaTime);

        // barrel rotation
        Quaternion lastBarrelRotation = _barrel.rotation;
        _barrel.LookAt(target.transform);
        Quaternion desiredRotation = _barrel.rotation;
        _barrel.localEulerAngles = new Vector3(_barrel.localEulerAngles.x, 180, 0);

        _barrel.rotation = _rotateType == RotateType.slerp ?
            Quaternion.Slerp(lastBarrelRotation, _barrel.rotation, _barrelRotationSpeed * Time.deltaTime) :
            Quaternion.RotateTowards(lastBarrelRotation, _barrel.rotation, _barrelRotationSpeed * Time.deltaTime);

        // shooting
        if (Quaternion.Angle(_barrel.rotation, desiredRotation) < _shootAngle && _reloadTime == 0)
        {
            _reloadTime = _reloadSpeed;
            Fire();
        }
    }

    protected abstract void Fire();

    //protected virtual void Fire(Transform spawnPos)
    //{
    //    Instantiate(_projectilePrefab, spawnPos.position, _barrel.rotation);
    //}

    private bool CheckInRange(GlobeObject target)
    {
        return Vector3.Distance(transform.position, target.WorldPosition) < _range;
    }
}
