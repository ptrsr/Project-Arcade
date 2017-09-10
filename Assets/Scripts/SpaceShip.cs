using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShip : GlobeObject
{
    [SerializeField] [Range(0, 1)]
    private float _acceleration = 0.9f;

    [SerializeField]
    private Vector2
        _movementSpeed = new Vector2();

    private GlobeObject _moveTarget;

    [SerializeField]
    private Transform _weaponAttachment;

    [SerializeField]
    private GameObject _weaponPrefab;
    private Weapon _weapon;

    private SpaceShip()
    {
        ServiceLocator.Provide(this);
    }

    private void Start ()
    {
        _moveTarget = new GlobeObject(false);
        _moveTarget.GlobePosition = GlobePosition;
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

    private Vector2 Movement()
    {
        Vector2 move = new Vector2();

        if (Input.GetKey(KeyCode.S))
            move += new Vector2(0, -1);

        if (Input.GetKey(KeyCode.W))
            move += new Vector2(0, 1);

        if (Input.GetKey(KeyCode.A))
            move += new Vector2(-1, 0);

        if (Input.GetKey(KeyCode.D))
            move += new Vector2(1, 0);

        float moveScalar = GlobeRadius + GlobePosition.y; // so the ship's speed doesn't change with altitude
        _moveTarget.GlobePosition += new Vector3((move.x * _movementSpeed.x) / moveScalar, move.y * _movementSpeed.y, 0) * Time.deltaTime;

        GlobePosition = Vector3.Slerp(GlobePosition, _moveTarget.GlobePosition, _acceleration);

        return move;
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        SetWeapon(_weaponPrefab);
    }

    public Vector2 MovementSpeed
    {
        get { return _movementSpeed; }
    }

    public GlobeObject MoveTarget
    {
        get { return _moveTarget; }
    }
}
