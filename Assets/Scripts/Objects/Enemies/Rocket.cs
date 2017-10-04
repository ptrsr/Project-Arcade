using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    private GlobeObject _target;
    private MeshRenderer _mr;

	public void Fire(GlobeObject target)
    {
        _target = target;
    }

    public void SetTransparancy(float value)
    {

    }
}