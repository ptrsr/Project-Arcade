using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    protected virtual void Start()
    {
        ServiceLocator.Locate<ObjectSafe>().AddTemporaryObject(gameObject);
    }

    public void IgnoreSpawnObject(Collider spawnerCollider)
    {
        Physics.IgnoreCollision(GetComponent<Collider>(), spawnerCollider, true);
    }
}
