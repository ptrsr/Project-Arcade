using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobeObject : MonoBehaviour
{
    private Globe _globe;

    [SerializeField]
    private Vector3 _globePosition;

    private bool _active = true;

    public GlobeObject(bool active = true)
    {
        _active = active;
        Globe.onGlobeChange += OnGlobeChanged;
    }

    private void OnGlobeChanged()
    {
        GlobePosition = GlobePosition;
    }

    protected virtual void OnValidate()
    {
        GlobePosition = GlobePosition;
    }

    public Vector3 GlobePosition
    {
        get { return _globePosition; }
        set
        {
            _globePosition = value;

            if (!_active)
                return;

            transform.position = new Vector3(Mathf.Sin(_globePosition.x), (Mathf.Cos(_globePosition.x) * Mathf.Cos(_globePosition.z)), Mathf.Sin(_globePosition.z)) * (Globe.Radius + _globePosition.y);
            transform.up = GlobeUp;
        }
    }

    public Vector3 WorldPosition
    {
        get { return Globe.GlobeToScenePosition(_globePosition + new Vector3(0, GlobeRadius, 0)); }
    }


    private Globe Globe
    {
        get
        {
            if (_globe == null)
                _globe = ServiceLocator.Locate<Globe>();

            return _globe;
        }
    }

    public float GlobeRadius
    {
        get { return Globe.Radius; }
    }

    public Vector3 GlobeUp
    {
        get { return transform.position.normalized; }
    }
}
