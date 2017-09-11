using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShip : MovingObject
{
    [SerializeField]
    private Transform _weaponAttachment;

    [SerializeField]
    private GameObject _weaponPrefab;
    private Weapon _weapon;

    private SpaceShip()
    {
        ServiceLocator.Provide(this);
    }

    protected override void Start()
    {
        base.Start();
    }

    void FixedUpdate ()
    {
        Vector2 nextMove = Movement();
        Move(nextMove);

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

        return move;
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        SetWeapon(_weaponPrefab);
    }
}
