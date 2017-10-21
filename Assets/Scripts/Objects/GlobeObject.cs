using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobeObject : MonoBehaviour
{
    [SerializeField]
    protected Vector3 _globePosition;

    private Globe _globe;

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
        Vector3 tempPosition = Globe.SceneToGlobePosition(ScenePosition);
        tempPosition.y = GlobePosition.y;
        GlobePosition = tempPosition;
    }

    protected virtual void OnValidate()
    {
        GlobePosition = GlobePosition;
        transform.up = GlobeUp;
    }

    public virtual Vector3 GlobePosition
    {
        get { return _globePosition; }
        set
        {
            _globePosition = value;

            transform.position = Globe.GlobeToScenePosition(value);
            transform.up = GlobeUp;
        }
    }

    public Vector3 ScenePosition
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

    public float GlobeRadius
    {
        get { return Globe.Radius; }
    }

    public Vector3 GlobeUp
    {
        get { return transform.position.normalized; }
    }
}
