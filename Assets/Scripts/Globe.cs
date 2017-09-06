using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globe : MonoBehaviour
{
    [SerializeField]
    private float
        _size = 10,
        _rotation = 0;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnValidate()
    {
        ServiceLocator.Provide(this);
        SetWorldSize();
        SetRotation();
    }

    private void SetRotation()
    {
        transform.eulerAngles = new Vector3(0, 0, _rotation);
    }

    private void SetWorldSize()
    {
        if (_size < 0.1f)
            _size = 0.1f;

        transform.localScale = new Vector3(_size, _size, _size);

        SpaceShip ship = ServiceLocator.Locate<SpaceShip>();
        
        if (ship != null)
            ship.UpdatePosition();
    }

    public float Radius
    {
        get { return _size; }
    }
}
