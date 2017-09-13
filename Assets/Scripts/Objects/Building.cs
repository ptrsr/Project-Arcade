using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : GlobeObject
{
    private int buildingHealth;
    private bool destroyed;

    [SerializeField]
    private GameObject destroyedPrefab;

	void Start ()
    {
        buildingHealth = 100;
        destroyed = false;
	}
	
	void Update ()
    {
		if (Input.GetKeyDown(KeyCode.P))
        {
            TakeDamage(100);
        }

        if (destroyed)
        {
            
        }
	}

    public void TakeDamage(int pDamageAmount)
    {
        if (!destroyed)
        {
            buildingHealth -= pDamageAmount;

            if (buildingHealth <= 0)
            {
                Instantiate(destroyedPrefab, transform.position, Quaternion.identity);
                destroyed = true;
                Destroy(this.gameObject);
            }
        }
    }
}
