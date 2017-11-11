using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuItem : MonoBehaviour {

    private Material _material;

    private void Awake()
    {
        _material = GetComponent<UnityEngine.UI.Text>().material;
    }

    void Start ()
    {
        print(_material != null);
	}
	
	void Update () {
		
	}
}
