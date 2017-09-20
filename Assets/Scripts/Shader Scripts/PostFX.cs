﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostFX : MonoBehaviour
{
    [SerializeField] [Range(0, 1)]
    private float _borderFade = 1;

    [SerializeField]
    private Shader _shader;
    private Material _mat;

    private Camera _cam;
    private Globe _globe;

    private void Start()
    {
        _cam = Camera.main;
        _mat = new Material(_shader);
        _globe = ServiceLocator.Locate<Globe>();

        _cam.depthTextureMode = DepthTextureMode.Depth;

        Globe.onGlobeChange += SetShaderUniforms;
        SetShaderUniforms();
    }

    private void OnValidate()
    {
        SetShaderUniforms();
    }

    private void SetShaderUniforms()
    {
        if (_mat == null)
            return;

        _mat.SetFloat("_levelWidth", _globe.LevelWidth);
        _mat.SetFloat("_borderFade", _borderFade);
    }

    [ImageEffectOpaque]
    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        RaycastCornerBlit(src, dst, _mat);
    }

    void RaycastCornerBlit(RenderTexture source, RenderTexture dest, Material mat)
    {
        // Compute Frustum Corners
        float camFar = _cam.farClipPlane;
        float camFov = _cam.fieldOfView;
        float camAspect = _cam.aspect;

        float fovWHalf = camFov * 0.5f;

        Vector3 toRight = _cam.transform.right * Mathf.Tan(fovWHalf * Mathf.Deg2Rad) * camAspect;
        Vector3 toTop = _cam.transform.up * Mathf.Tan(fovWHalf * Mathf.Deg2Rad);

        Vector3 topLeft = (_cam.transform.forward - toRight + toTop);
        float camScale = topLeft.magnitude * camFar;

        topLeft.Normalize();
        topLeft *= camScale;

        Vector3 topRight = (_cam.transform.forward + toRight + toTop);
        topRight.Normalize();
        topRight *= camScale;

        Vector3 bottomRight = (_cam.transform.forward + toRight - toTop);
        bottomRight.Normalize();
        bottomRight *= camScale;

        Vector3 bottomLeft = (_cam.transform.forward - toRight - toTop);
        bottomLeft.Normalize();
        bottomLeft *= camScale;

        // Custom Blit, encoding Frustum Corners as additional Texture Coordinates
        RenderTexture.active = dest;

        mat.SetTexture("_Scene", source);

        GL.PushMatrix();
        GL.LoadOrtho();

        mat.SetPass(0);

        GL.Begin(GL.QUADS);

        GL.MultiTexCoord2(0, 0.0f, 0.0f);
        GL.MultiTexCoord(1, bottomLeft);
        GL.Vertex3(0.0f, 0.0f, 0.0f);

        GL.MultiTexCoord2(0, 1.0f, 0.0f);
        GL.MultiTexCoord(1, bottomRight);
        GL.Vertex3(1.0f, 0.0f, 0.0f);

        GL.MultiTexCoord2(0, 1.0f, 1.0f);
        GL.MultiTexCoord(1, topRight);
        GL.Vertex3(1.0f, 1.0f, 0.0f);

        GL.MultiTexCoord2(0, 0.0f, 1.0f);
        GL.MultiTexCoord(1, topLeft);
        GL.Vertex3(0.0f, 1.0f, 0.0f);

        GL.End();
        GL.PopMatrix();
    }
}
