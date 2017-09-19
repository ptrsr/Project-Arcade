﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShip : MovingObject
{
    [SerializeField]
    private Transform _weaponAttachment;

    [SerializeField]
    private GameObject[] _weaponPrefabs;

    [SerializeField]
    private GameObject _currentWeapon;
    private Weapon _weaponComponent;

    int _weaponSelector = 0;

    private SpaceShip()
    {
        ServiceLocator.Provide(this);
    }

    protected override void Start()
    {
        base.Start();
        SelectWeapon(0);
    }

    private void Update()
    {
        SelectWeapon();
    }

    void FixedUpdate ()
    {
        Vector2 nextMove = Movement();
        Move(nextMove);

        if (_weaponComponent != null)
        {
            _weaponComponent.Aim(new Vector2(nextMove.x * MovementSpeed.x, nextMove.y * MovementSpeed.y));

            if (Input.GetKey(KeyCode.Space))
                _weaponComponent.Fire();
            else
                _weaponComponent.Hold();
        }
    }

    private void SelectWeapon(int weapon = -1)
    {
        if (weapon >= 0 && weapon < _weaponPrefabs.Length)
        {
            SetWeapon(_weaponPrefabs[weapon]);
            _weaponSelector = weapon;
            return;
        }

        int temp = _weaponSelector;

        if (Input.GetKeyDown(KeyCode.Q))
            _weaponSelector--;

        if (Input.GetKeyDown(KeyCode.E))
            _weaponSelector++;

        if (_weaponSelector < 0)
            _weaponSelector += _weaponPrefabs.Length;

        _weaponSelector %= _weaponPrefabs.Length;

        if (temp != _weaponSelector)
            SetWeapon(_weaponPrefabs[_weaponSelector]);
    }

    public void SetWeapon(GameObject prefab)
    {
        if (!Application.isPlaying)
            return;

        if (_weaponComponent != null)
            Destroy(_weaponComponent.gameObject);

        if (prefab == null || prefab.GetComponent<Weapon>() == null)
            return;

        GameObject newPrefab = GameObject.Instantiate(prefab);
        _weaponComponent = newPrefab.GetComponent<Weapon>();

        newPrefab.transform.parent = _weaponAttachment;
        newPrefab.transform.localPosition = new Vector3();
    }

    private Vector2 Movement()
    {
        Vector2 move = new Vector2();

        if (Input.GetKey(KeyCode.S))
            move += new Vector2(0, 1);

        if (Input.GetKey(KeyCode.W))
            move += new Vector2(0, -1);

        if (Input.GetKey(KeyCode.A))
            move += new Vector2(1, 0);

        if (Input.GetKey(KeyCode.D))
            move += new Vector2(-1, 0);

        return move;
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        SetWeapon(_currentWeapon);
    }
}
