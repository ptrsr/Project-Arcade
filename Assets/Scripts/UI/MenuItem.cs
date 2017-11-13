using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuItem : MonoBehaviour {

    [SerializeField]
    private Shader _shader;

    [SerializeField]
    private Color _color;

    [SerializeField]
    private float
        _strobeWidth = 1,
        _strobeSpeed = 1;

    [SerializeField] [Range(0, 1)]
    private float _min = 0.1f;

    private Material _mat;

    private void Awake()
    {
        _mat = new Material(_shader);
        GetComponent<UnityEngine.UI.Text>().material = _mat;
    }

    private void Start()
    {
        SetUniforms();
    }

    private void SetUniforms()
    {
        if (_mat == null)
            return;

        _mat.SetColor("_color", _color);
        _mat.SetFloat("_width", _strobeWidth);
        _mat.SetFloat("_min", _min);
    }

    private void OnValidate()
    {
        SetUniforms();
    }

    void Update ()
    {
        _mat.SetFloat("_timer", Time.time * _strobeSpeed);
	}

    public bool Selected
    {
        set { _mat.SetFloat("_on", value ? 1 : _min); }
    }
}
