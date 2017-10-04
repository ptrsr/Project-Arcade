using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : TurretHead
{
    [SerializeField]
    private Transform _rocketSpawn;

    private GameObject _currentRocket;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();


    }

    protected override void Fire()
    {
        _currentRocket.GetComponent<Rocket>().Fire(_target);
        _currentRocket = Instantiate(_projectilePrefab, _rocketSpawn);

        _currentRocket.transform.rotation = Quaternion.identity;
        _currentRocket.transform.localPosition = new Vector3();
    }
}
