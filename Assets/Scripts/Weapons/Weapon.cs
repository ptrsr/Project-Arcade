using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Weapon : MonoBehaviour
{
    public float _angle;
    public float _rotateSpeed;

    public abstract void Fire();

    public void Aim(Vector3 weaponPos, Vector3 aimPos)
    {
        Quaternion desired = Quaternion.LookRotation((aimPos - weaponPos).normalized);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, desired, _rotateSpeed);
    }
}
