using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Beam : Weapon
{
    private List<Transform> _beamableObjects;

    private void Start()
    {
        _beamableObjects = new List<Transform>();
        _rotateSpeed = 3;
    }

    public override void Fire()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Beamable")
            return;

        if (!_beamableObjects.Contains(other.transform))
            _beamableObjects.Add(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Beamable")
            return;

        if (_beamableObjects.Contains(other.transform))
            _beamableObjects.Remove(other.transform);
    }
}
