using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : TurretHead
{
    [SerializeField]
    private Transform _rocketSpawn;

    [SerializeField]
    private Rocket _currentRocket;

    protected override void Update()
    {
        base.Update();
        _currentRocket.SetTransparancy(ReloadStatus);

        if (_currentRocket == null)
            GetComponent<DestroyableObject>().Explode();
    }

    protected override void Fire()
    {
        _currentRocket.Fire(_target);

        _currentRocket = Instantiate(_projectilePrefab, _rocketSpawn).GetComponent<Rocket>();

        _currentRocket.transform.localRotation = Quaternion.identity;
        _currentRocket.transform.localPosition = new Vector3();
    }
}
