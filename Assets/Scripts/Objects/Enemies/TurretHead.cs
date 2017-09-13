using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretHead : MonoBehaviour
{
    [SerializeField][Range(0, 10)]
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
    private GameObject _projectilePrefab;

    [SerializeField]
    private float _reloadSpeed;

    private float _reloadTime;

    private Quaternion _headRotation;
    private GroundEnemy _parent;

    public void Aim(GlobeObject target)
    {
        // head rotation
        _head.LookAt(target.transform);
        _head.localEulerAngles = new Vector3(0, _head.localEulerAngles.y, 0);
        Quaternion desiredHeadRotation = _head.rotation;

        _headRotation = Quaternion.Slerp(_headRotation, desiredHeadRotation, _headRotationSpeed * Time.deltaTime);
        _head.rotation = _headRotation;

        // barrel rotation
        Quaternion desiredBarrelRotation = Quaternion.LookRotation((target.transform.position - _barrel.position).normalized);
        _barrel.rotation = Quaternion.Slerp(_barrel.rotation, desiredBarrelRotation, _barrelRotationSpeed * Time.deltaTime);
        _barrel.localEulerAngles = new Vector3(_barrel.localEulerAngles.x, 0, 0);

        // shooting
        _reloadTime = Mathf.Clamp(_reloadTime - Time.deltaTime, 0, _reloadSpeed);

        if (Quaternion.Angle(_barrel.rotation, desiredBarrelRotation) < 2 && _reloadTime == 0)
        {
            _reloadTime = _reloadSpeed;

            if (_projectilePrefab != null)
                foreach (Transform spawnPos in _projectileSpawnPoints)
                    Fire(spawnPos);
        }
    }

    protected virtual void Fire(Transform spawnPos)
    {
        Instantiate(_projectilePrefab, spawnPos.position, _barrel.rotation);
    }

    public GroundEnemy Parent
    {
        get { return _parent;  }
        set { _parent = value; }
    }
}
