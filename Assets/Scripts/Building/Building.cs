using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class Building : GlobeObject
{
	void Start ()
    {
		
	}

    public void SetPosition(Vector3 ScenePosition)
    {
        GlobePosition = Globe.SceneToGlobePosition(ScenePosition) - new Vector3(0, GlobeRadius, 0);
    }
}
