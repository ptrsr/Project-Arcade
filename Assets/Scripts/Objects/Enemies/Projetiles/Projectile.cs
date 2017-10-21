using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public void IgnoreSpawnObject(Collider spawnerCollider)
    {
        Physics.IgnoreCollision(GetComponent<Collider>(), spawnerCollider, true);
    }
}
