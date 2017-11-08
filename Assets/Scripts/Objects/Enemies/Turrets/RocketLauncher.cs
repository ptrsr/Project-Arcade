using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : TurretHead
{
    [SerializeField]
    private Transform _rocketSpawn;
    private Rocket _currentRocket;

    protected override void Start()
    {
        SpawnRocket(ProjectileSpawnPoints[0]);
    }

    protected override void Update()
    {
        base.Update();

        if (_currentRocket != null)
            _currentRocket.SetTransparancy(ReloadStatus);

        if (_currentRocket == null)
            GetComponent<DestroyableObject>().Explode();
    }

    void SpawnRocket(Transform spawnPoint)
    {
        _currentRocket = Instantiate(_projectilePrefab, spawnPoint).GetComponent<Rocket>();
        _currentRocket.transform.localRotation = Quaternion.identity;
        _currentRocket.transform.localPosition = new Vector3();
    }

    protected override void Fire(Transform projectileSpawnpoint)
    {
        _currentRocket.Fire(Target);
        SpawnRocket(projectileSpawnpoint);
    }
}
