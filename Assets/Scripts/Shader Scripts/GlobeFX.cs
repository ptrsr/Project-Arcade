using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobeFX : MonoBehaviour
{
    [SerializeField]
    private Texture
        _splatMap,
        _grass,
        _water;

    [SerializeField]
    private Shader _shader;

    private Material _mat;

	void Start ()
    {
        _mat = new Material(_shader);
        GetComponent<MeshRenderer>().material = _mat;

	}

    private void OnValidate()
    {

    }

    private void SetUniforms()
    {
        _mat.SetTexture("_splatmap", _splatMap);
        _mat.SetTexture("_grass", _grass);
        _mat.SetTexture("_water", _water);
    }
}
