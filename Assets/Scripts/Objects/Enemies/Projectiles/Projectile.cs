using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float _damage = 1;

    protected virtual void Start()
    {
        ServiceLocator.Locate<ObjectSafe>().AddTemporaryObject(gameObject);
    }

    public void IgnoreSpawnObject(Collider spawnerCollider)
    {
        Physics.IgnoreCollision(GetComponent<Collider>(), spawnerCollider, true);
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        DestroyableObject destroyableObject = collision.gameObject.GetComponent<DestroyableObject>();

        if (destroyableObject == null)
            return;

        destroyableObject.Damage(_damage);
    }
}
