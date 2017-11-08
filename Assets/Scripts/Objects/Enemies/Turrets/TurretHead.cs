using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretHead : MonoBehaviour
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

    [SerializeField]
    private Transform[]
        _projectileSpawnPoints;

    [SerializeField]
    protected GameObject _projectilePrefab;

    [SerializeField]
    private float
        _reloadSpeed,
        _shootAngle = 5,
        _range = 20;

    private GravityObject _gravityObject;
    private GlobeObject _target;

    private Quaternion
        _idleBarrelRotation,
        _idleHeadRotation;

    private float _reloadTime;

    protected virtual void Start()
    {
        _target = ServiceLocator.Locate<SpaceShip>();
        _gravityObject = GetComponent<GravityObject>();

        _idleHeadRotation = _head.localRotation;
        _idleBarrelRotation = _barrel.localRotation;
    }

    protected virtual void Update()
    {
        _reloadTime = Mathf.Clamp(_reloadTime - Time.deltaTime, 0, _reloadSpeed);

        if (_gravityObject != null && (_gravityObject.Beamed || !_gravityObject.Kinematic))
            return;

        if (CheckInRange(Target))
            Aim(Target);
        else
            Idle();
    }

    private void Idle()
    {
        _head.localRotation = _rotateType == RotateType.slerp ?
            Quaternion.Slerp(_head.localRotation, _idleHeadRotation, _headRotationSpeed * Time.deltaTime) :
            Quaternion.RotateTowards(_head.localRotation, _idleHeadRotation, _headRotationSpeed * Time.deltaTime);

        _barrel.localRotation = _rotateType == RotateType.slerp ?
            Quaternion.Slerp(_barrel.localRotation, _idleBarrelRotation, _barrelRotationSpeed * Time.deltaTime) :
            Quaternion.RotateTowards(_barrel.localRotation, _idleBarrelRotation, _barrelRotationSpeed * Time.deltaTime);
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
        _barrel.localEulerAngles = new Vector3(_barrel.localEulerAngles.x, 0, 0);
        Quaternion desiredRotation = _barrel.rotation;

        _barrel.rotation = _rotateType == RotateType.slerp ?
            Quaternion.Slerp(lastBarrelRotation, _barrel.rotation, _barrelRotationSpeed * Time.deltaTime) :
            Quaternion.RotateTowards(lastBarrelRotation, _barrel.rotation, _barrelRotationSpeed * Time.deltaTime);

        // shooting
        if (Vector3.Angle(_barrel.forward, (target.ScenePosition - _barrel.position).normalized) < _shootAngle && _reloadTime == 0)
        {
            _reloadTime = _reloadSpeed;
            Fire(_projectileSpawnPoints[0]);
        }
    }

    protected virtual void Fire(Transform spawnPos)
    {
        GameObject projectile = Instantiate(_projectilePrefab, spawnPos.position, _barrel.rotation);
        projectile.GetComponent<Projectile>().IgnoreSpawnObject(GetComponent<Collider>());
    }

    private bool CheckInRange(GlobeObject target)
    {
        return Vector3.Distance(transform.position, target.ScenePosition) < _range;
    }

    protected float ReloadStatus
    {
        get { return 1 - _reloadTime / _reloadSpeed; }
    }

    protected Transform[] ProjectileSpawnPoints
    {
        get { return _projectileSpawnPoints; }
    }

    protected GlobeObject Target
    {
        get
        {
            if (_target == null)
                _target = ServiceLocator.Locate<SpaceShip>();

            return _target;
        }
    }
}
