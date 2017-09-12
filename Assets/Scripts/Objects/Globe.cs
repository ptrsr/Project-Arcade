using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globe : MonoBehaviour
{
    public delegate void OnGlobeChange();
    public static event OnGlobeChange onGlobeChange;

    [SerializeField]
    private float
        _size = 10,
        _rotation = 0,
        _gravityAcceleration = 10;


    private Globe()
    {
        ServiceLocator.Provide(this);
    }

    private void OnValidate()
    {
        SetWorldSize();
        SetRotation();

        OnGlobeChanged();
    }

    void OnGlobeChanged()
    {
        if (onGlobeChange == null)
            return;

        foreach (OnGlobeChange change in onGlobeChange.GetInvocationList())
            if (change.Target == null)
                onGlobeChange -= change;

        onGlobeChange();
    }

    private void SetRotation()
    {
        transform.eulerAngles = new Vector3(0, 0, _rotation);
    }

    private void SetWorldSize()
    {
        if (_size < 0.1f)
            _size = 0.1f;

        transform.localScale = new Vector3(_size, _size, _size);
    }

    public float Radius
    {
        get { return _size / 2; }
    }

    public float Gravity
    {
        get { return _gravityAcceleration; }
    }

    public static Vector3 SceneToGlobePosition(Vector3 scenePosition)
    {
        return new Vector3(Mathf.Atan2(scenePosition.x, scenePosition.y), scenePosition.magnitude, Mathf.Sin(scenePosition.z / ServiceLocator.Locate<Globe>().Radius));
    }

    public static Vector3 GlobeToScenePosition(Vector3 globePosition)
    {
        return new Vector3(Mathf.Sin(globePosition.x), Mathf.Cos(globePosition.x), 0) * globePosition.y;
    }
}
