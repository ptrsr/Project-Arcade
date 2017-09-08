using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : Weapon
{
    [SerializeField]
    private float 
        _angle,
        _rotateSpeed;

    private MeshRenderer _mr;
    private Transform _attachmentPoint;

    private void Start()
    {
        _mr = GetComponent<MeshRenderer>();
        _mr.enabled = false;

        _attachmentPoint = transform.parent;
    }

    public override void Aim(Vector2 movement)
    {
        print(movement.magnitude);
    }

    protected override void OnFireEnabled()
    {
        
    }

    protected override void OnFire()
    {
        //Debug.DrawRay(_attachmentPoint.position, );
    }
}
