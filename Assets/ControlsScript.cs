using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlsScript : MonoBehaviour {

	void Update ()
    {
	    if (Input.GetKeyDown(KeyCode.I))
        {
            SceneManager.LoadScene("Main-Menu");
        }
	}
}
