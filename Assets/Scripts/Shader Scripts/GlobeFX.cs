using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobeFX : MonoBehaviour
{
    [SerializeField]
    private Texture _texture;

    [SerializeField]
    private Shader _shader;

    private Material _mat;

	void Start ()
    {
        _mat = new Material(_shader);
        GetComponent<MeshRenderer>().material = _mat;

        _mat.SetTexture("_splatmap", _texture);
	}
	
	void Update ()
    {
		
	}
}
