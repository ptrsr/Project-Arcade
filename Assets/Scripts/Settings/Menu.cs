using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour {

	void Start ()
    {
        ServiceLocator.Locate<ObjectSafe>().Start();
	}
	
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.O))
            ServiceLocator.Locate<ObjectSafe>().Delete();

        if (Input.GetKeyDown(KeyCode.P))
            ServiceLocator.Locate<ObjectSafe>().Spawn();
    }
}
