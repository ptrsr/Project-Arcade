using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects : MonoBehaviour
{
    [SerializeField]
    private GameObject
        _explosion,
        _splash;

    private Effects()
    {
        ServiceLocator.Provide(this);
    }
    
    public GameObject Explosion
    {
        get { return _explosion; }
    }

    public GameObject Splash
    {
        get { return _splash; }
    }
}
