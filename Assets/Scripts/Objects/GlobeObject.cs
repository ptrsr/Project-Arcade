using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobeObject : MonoBehaviour
{
    private Globe _globe;

    [SerializeField]
    private Vector3 _globePosition;

    [SerializeField]
    protected bool _active = true;

    private bool _beamed = false;

    public GlobeObject()
    {
        Globe.onGlobeChange += OnGlobeChanged;
    }

    private void OnGlobeChanged()
    {
        GlobePosition = GlobePosition;
    }

    public void SetPosition(Vector3 ScenePosition)
    {
        GlobePosition = Globe.SceneToGlobePosition(ScenePosition) - new Vector3(0, GlobeRadius + GlobePosition.y, 0);
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

            try
            {
                transform.position = new Vector3(Mathf.Sin(_globePosition.x) * Mathf.Cos(_globePosition.z), (Mathf.Cos(_globePosition.x) * Mathf.Cos(_globePosition.z)), Mathf.Sin(_globePosition.z)) * (Globe.Radius + _globePosition.y);
                transform.up = GlobeUp;
            } catch { }
        }
    }

    public Vector3 WorldPosition
    {
        get
        {
            try { return transform.position; }
            catch { return Globe.GlobeToScenePosition(_globePosition + new Vector3(0, GlobeRadius, 0)); }
        }
    }


    public Globe Globe
    {
        get
        {
            if (_globe == null)
                _globe = ServiceLocator.Locate<Globe>();

            return _globe;
        }
    }

    public bool Beamed
    {
        get { return _beamed;  }
        set { _beamed = value; }
    }

    public virtual bool Active
    {
        get { return _active;  }
        set { _active = value; }
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
