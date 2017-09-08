﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShip : MonoBehaviour
{
    [SerializeField] [Range(0, 1)]
    private float _acceleration = 0.9f;

    [SerializeField]
    private Vector2
        _movementSpeed = new Vector2(),
        _position2D    = new Vector2();

    private Vector2 
        _move          = new Vector2();

    private Vector3 _moveTarget;

    private float _radius;

    [SerializeField]
    private Transform _weaponAttachment;

    [SerializeField]
    private GameObject _weaponPrefab;
    private Weapon _weapon;

	void Start ()
    {
        _radius = ServiceLocator.Locate<Globe>().Radius;
        UpdatePosition(true);
    }
	
	void FixedUpdate ()
    {
        Vector2 nextMove = Movement();

        if (_weapon != null)
        {
            _weapon.Aim(nextMove);

            if (Input.GetKey(KeyCode.Space))
                _weapon.Fire();
            else
                _weapon.Hold();
        }
    }

    public void SetWeapon(GameObject prefab)
    {
        if (!Application.isPlaying)
            return;

        if (_weapon != null)
            Destroy(_weapon.gameObject);

        if (prefab == null || prefab.GetComponent<Weapon>() == null)
            return;

        GameObject newPrefab = GameObject.Instantiate(_weaponPrefab);
        _weapon = newPrefab.GetComponent<Weapon>();

        newPrefab.transform.parent = _weaponAttachment;
        newPrefab.transform.localPosition = new Vector3();
    }

    #region Movement
    private Vector2 Movement()
    {
        _move = new Vector2();

        if (Input.GetKey(KeyCode.S))
            _move -= new Vector2(0, _movementSpeed.y);

        if (Input.GetKey(KeyCode.W))
            _move += new Vector2(0, _movementSpeed.y);

        if (Input.GetKey(KeyCode.A))
            _move -= new Vector2(_movementSpeed.x, 0);

        if (Input.GetKey(KeyCode.D))
            _move += new Vector2(_movementSpeed.x, 0);

        _move /= _radius; // so the speed doesn't change when you scale the planet

        _position2D += _move * Time.deltaTime;

        if (_move != new Vector2())
            UpdatePosition();

        Vector3 nextMove = transform.position;
        transform.position = Vector3.Lerp(transform.position, _moveTarget, _acceleration);
        nextMove -= transform.position;

        transform.up = transform.position.normalized;

        return new Vector2(nextMove.x, nextMove.y);
    }



    public void UpdatePosition(bool set = false)
    {
        _moveTarget = new Vector3(Mathf.Sin(_position2D.x), Mathf.Cos(_position2D.x), 0) * (_radius + _position2D.y);

        if (set) // when the position of the spaceship shouldn't interpolate
        {
            transform.position = _moveTarget;
            transform.up = transform.position.normalized;

            // also update the camera position
            FollowCam cam = ServiceLocator.Locate<FollowCam>();

            if (cam != null)
                cam.SetCameraTransform(transform);
        }
    }
    #endregion

    private void OnValidate()
    {
        ServiceLocator.Provide(this);
        UpdatePosition(true);

        SetWeapon(_weaponPrefab);
    }

    public float Radius
    {
        set
        {
            _radius = value;
            UpdatePosition(true); // update the position when the radius is changed
        }
    }

    public Vector2 Move
    {
        get { return _move; }
    }

    public Vector2 Position2D
    {
        get { return _position2D; }
    }
}
