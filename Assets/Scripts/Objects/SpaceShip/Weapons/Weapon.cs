using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Weapon : MonoBehaviour
{
    private bool _firing = false;

    public void Fire()
    {
        if (!_firing)
            OnFireEnabled();

        _firing = true;
        OnFire();
    }

    public void Hold()
    {
        if (_firing)
            OnFireDisabled();

        _firing = false;
    }

    protected virtual void OnFireEnabled() { }
    protected virtual void OnFire() { }
    protected virtual void OnFireDisabled() { }

    public virtual void Aim(Vector2 movement) { }

    //simple rotation if needed later

    //Quaternion desired = Quaternion.LookRotation((aimPos2D - weaponPos).normalized);
    //transform.rotation = Quaternion.RotateTowards(transform.rotation, desired, _rotateSpeed);
}
