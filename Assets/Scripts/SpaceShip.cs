using System.Collections;
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

	void Start ()
    {
        _radius = ServiceLocator.Locate<Globe>().Radius / 2;
        UpdatePosition();
    }
	
	void Update ()
    {
        Movement();
    }

    #region Movement
    private void Movement()
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


        _position2D += _move * Time.deltaTime;

        if (_move != new Vector2())
            UpdatePosition();

        transform.position = Vector3.Lerp(transform.position, _moveTarget, _acceleration);
        transform.up = transform.position.normalized;
    }

    public void UpdatePosition(bool set = false)
    {
        _moveTarget = new Vector3(Mathf.Sin(_position2D.x), Mathf.Cos(_position2D.x), 0) * (_radius + _position2D.y);

        if (set)
        {
            transform.position = _moveTarget;
            transform.up = transform.position.normalized;
        }
    }
    #endregion

    private void OnValidate()
    {
        ServiceLocator.Provide(this);

        Globe globe = ServiceLocator.Locate<Globe>();

        if (globe == null)
            return;

        _radius = globe.Radius / 2;
        UpdatePosition(true);
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
