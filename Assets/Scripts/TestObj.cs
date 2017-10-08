using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObj : GlobeObject
{
    public Vector3 _test;

	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update () {
        GlobePosition = TerrainPosition(_test);
	}
}
