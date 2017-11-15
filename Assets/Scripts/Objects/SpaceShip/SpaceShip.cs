using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShip : MovingObject
{
    [SerializeField]
    private Transform _weaponAttachment;

    [SerializeField]
    private GameObject[] _weaponPrefabs;
    private Weapon[]     _spawnedWeapons;

    private Weapon _currentWeapon;

    int _weaponSelector = 0;

    protected override void Awake()
    {
        base.Awake();
        ServiceLocator.Provide(this);

        _spawnedWeapons = new Weapon[_weaponPrefabs.Length];

        for (int i = 0; i < _weaponPrefabs.Length; i++)
        {
            GameObject newPrefab = GameObject.Instantiate(_weaponPrefabs[i]);
            newPrefab.transform.parent = _weaponAttachment;
            newPrefab.transform.localPosition = new Vector3();

            _spawnedWeapons[i] = newPrefab.GetComponent<Weapon>();
            newPrefab.SetActive(false);
        }
        SetWeapon(_spawnedWeapons[0]);
    }

    void FixedUpdate ()
    {
        Vector2 move2D = Movement();
        Vector3 move = new Vector3(move2D.x, 0, move2D.y);

        Move(move);
        RotateTo(move, false);

        SelectWeapon();

        if (_currentWeapon != null)
        {
            _currentWeapon.Aim(new Vector2(move2D.x * MovementSpeed.x, move2D.y * MovementSpeed.z));

            if (Input.GetButton("Select / Shoot") || Input.GetButton("Beam"))
                _currentWeapon.Fire();
            else
                _currentWeapon.Release();
        }
    }

    private void SelectWeapon()
    {
        if (Input.GetButton("Beam"))
        {
            if (_currentWeapon.GetType() != typeof(Beam))
                SetWeapon(typeof(Beam));

            return;
        }

        if (Input.GetButton("Laser"))
        {
            if (_currentWeapon.GetType() != typeof(Laser))
                SetWeapon(typeof(Laser));

            return;
        }

        if (Input.GetKeyDown(KeyCode.Q))
            _weaponSelector--;

        else if (Input.GetKeyDown(KeyCode.E))
            _weaponSelector++;
        else
            return;

        if (_weaponSelector < 0)
            _weaponSelector += _weaponPrefabs.Length;

        _weaponSelector %= _weaponPrefabs.Length;

        SetWeapon(_spawnedWeapons[_weaponSelector]);
    }

    public void SetWeapon(System.Type weaponType)
    {
        for (int i = 0; i < _spawnedWeapons.Length; i++)
        {
            Weapon weapon = _spawnedWeapons[i];

            if (weapon.GetType() != weaponType)
                continue;

            if (_currentWeapon != null)
                _currentWeapon.gameObject.SetActive(false);

            _currentWeapon = weapon;
            weapon.gameObject.SetActive(true);

            _weaponSelector = i;
            return;
        }
    }

    private void SetWeapon(Weapon weapon)
    {
        if (_currentWeapon != null)
            _currentWeapon.gameObject.SetActive(false);

        _currentWeapon = weapon;
        _currentWeapon.gameObject.SetActive(true);
    }

    private Vector2 Movement()
    {
        Vector2 move = new Vector2();

        move += new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        return move;
    }

    protected override void OnValidate()
    {
        base.OnValidate();
    }

    private void OnDestroy()
    {
        if (_currentWeapon != null)
            Destroy(_currentWeapon.gameObject);

        ServiceLocator.Locate<Menu>().Stop(2);
    }

    IEnumerator ReturnToMenu()
    {
        yield return new WaitForSeconds(2);
        ServiceLocator.Locate<Menu>().Stop();
    }
}
