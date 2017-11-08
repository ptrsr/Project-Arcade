using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Callbacks;
using System;

public delegate void OnGlobeChange();

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class Globe : MonoBehaviour
{
    [Serializable]
    struct GlobeSettings
    {
        [Range(0, 6)]
         public int recursion;

        public float
            radius,
            heightMulti;

        public Texture2D
            heightMap,
            waveMap;

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(GlobeSettings))
                return false;

            GlobeSettings other = (GlobeSettings)obj;

            return recursion == other.recursion &&
                   radius == other.radius &&
                   heightMulti == other.heightMulti &&
                   heightMap == other.heightMap &&
                   waveMap == other.waveMap;
        }

        public static bool operator != (GlobeSettings A, GlobeSettings B)
        {
            return !A.Equals(B);
        }

        public static bool operator == (GlobeSettings A, GlobeSettings B)
        {
            return A.Equals(B);
        }
    }

    [SerializeField]
    private Shader _shader;

    [SerializeField]
    private GlobeSettings _globeSettings;
    private static GlobeSettings _backup;

    [SerializeField]
    private Color
        _grass,
        _water,
        _sand,
        _paintColor;

    [SerializeField]
    private float
        _waterLevel,
        _waveHeight,
        _waveMulti,
        _waveSpeed,
        _specular,
        _test;

    [SerializeField] [Range(0, 1)]
    private float _ambient;

    [SerializeField]
    private Texture2D
        _heightMap,
        _waveMap,
        _paintMap;

    #region LEVEL SETTINGS
    public static event OnGlobeChange onGlobeChange;

    [SerializeField]
    private float
        _rotation = 0,
        _gravityAcceleration = 10,
        _levelWidth;
    #endregion

    private MeshRenderer _mr;
    private MeshFilter   _mf;
    private MeshCollider _mc;
    private Material     _mat;



    private Globe()
    {
        ServiceLocator.Provide(this);
    }

    private void Start ()
    {
        OnValidate();
    }

    private void Update()
    {
        _mat.SetFloat("_timer", Time.time / _waveSpeed);
        _mat.SetFloat("_test", _test);
    }

    private void OnValidate()
    {
        if (_globeSettings != _backup)
        {
            CreateWorld();
            _backup = _globeSettings;
        }

        SetUniforms();
        OnGlobeChanged();
    }

    void OnGlobeChanged()
    {
        if (onGlobeChange == null)
            return;

        foreach (OnGlobeChange change in onGlobeChange.GetInvocationList())
            try { change(); } catch { onGlobeChange -= change; }
    }

    public void CreateWorld()
    {
        Mesh mesh = Create(_globeSettings.recursion);

        MF.mesh = mesh;
        MC.sharedMesh = mesh;

        _paintMap = CreateWorldTexture(mesh.vertexCount);
    }

    private void SetUniforms()
    {
        _mat = new Material(_shader);
        _mat.shader = _shader;

        _mat.SetFloat("_ambient", _ambient);
        _mat.SetFloat("_specular", _specular);

        _mat.SetColor("_grass", _grass);
        _mat.SetColor("_water", _water);
        _mat.SetColor("_sand", _sand);


        _mat.SetFloat("_waterLevel", _waterLevel + _globeSettings.radius);
        _mat.SetFloat("_waveMulti", _waveMulti);
        _mat.SetFloat("_waveHeight", _waveHeight * _globeSettings.radius);

        _mat.SetTexture("_waveMap", _waveMap);
        _mat.SetTexture("_paintMap", _paintMap);

        _mat.SetVector("_paintMapSize", new Vector2(_paintMap.width, _paintMap.height));

        MR.material = _mat;
    }

    public void Draw(int[] vertices)
    {
        foreach (int vertice in vertices)
        {
            int row = vertice / _paintMap.width;
            int col = vertice % _paintMap.width;

            _paintMap.SetPixel(col, row, _paintColor);
        }

        _paintMap.Apply();
        _mat.SetTexture("_paintMap", _paintMap);

    }

    private Texture2D CreateWorldTexture(int vertices)
    {
        int width = (int)Math.Sqrt(vertices);
        int height = width + 1;

        Texture2D paintMap = new Texture2D(width, height);
        Color[] block = new Color[width * height];

        for (int i = 0; i < block.Length; i++)
            block[i] = Color.clear;

        paintMap.SetPixels(0, 0, paintMap.width, paintMap.height, block);
        paintMap.Apply();

        return paintMap;
    }

    #region SPHERE CREATION

    private struct TriangleIndices
    {
        public int v1;
        public int v2;
        public int v3;

        public TriangleIndices(int v1, int v2, int v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }
    }

    // return index of point in the middle of p1 and p2
    private static int getMiddlePoint(int p1, int p2, ref List<Vector3> vertices, ref Dictionary<long, int> cache)
    {
        // first check if we have it already
        bool firstIsSmaller = p1 < p2;
        long smallerIndex = firstIsSmaller ? p1 : p2;
        long greaterIndex = firstIsSmaller ? p2 : p1;
        long key = (smallerIndex << 32) + greaterIndex;

        int ret;
        if (cache.TryGetValue(key, out ret))
        {
            return ret;
        }

        // not in cache, calculate it
        Vector3 point1 = vertices[p1];
        Vector3 point2 = vertices[p2];
        Vector3 middle = new Vector3
        (
            (point1.x + point2.x) / 2f,
            (point1.y + point2.y) / 2f,
            (point1.z + point2.z) / 2f
        );

        // add vertex makes sure point is on unit sphere
        int i = vertices.Count;
        vertices.Add(middle.normalized);

        // store it, return index
        cache.Add(key, i);

        return i;
    }

    private Mesh Create(int recursion)
    {
        Mesh mesh = new Mesh();

        List<Vector3> vertList = new List<Vector3>();
        Dictionary<long, int> middlePointIndexCache = new Dictionary<long, int>();

        // create 12 vertices of a icosahedron
        float t = (1f + Mathf.Sqrt(5f)) / 2f;

        vertList.Add(new Vector3(-1f, t, 0f).normalized);
        vertList.Add(new Vector3(1f, t, 0f).normalized);
        vertList.Add(new Vector3(-1f, -t, 0f).normalized);
        vertList.Add(new Vector3(1f, -t, 0f).normalized);

        vertList.Add(new Vector3(0f, -1f, t).normalized);
        vertList.Add(new Vector3(0f, 1f, t).normalized);
        vertList.Add(new Vector3(0f, -1f, -t).normalized);
        vertList.Add(new Vector3(0f, 1f, -t).normalized);

        vertList.Add(new Vector3(t, 0f, -1f).normalized);
        vertList.Add(new Vector3(t, 0f, 1f).normalized);
        vertList.Add(new Vector3(-t, 0f, -1f).normalized);
        vertList.Add(new Vector3(-t, 0f, 1f).normalized);

        // create 20 triangles of the icosahedron
        List<TriangleIndices> faces = new List<TriangleIndices>();

        // 5 faces around point 0
        faces.Add(new TriangleIndices(0, 11, 5));
        faces.Add(new TriangleIndices(0, 5, 1));
        faces.Add(new TriangleIndices(0, 1, 7));
        faces.Add(new TriangleIndices(0, 7, 10));
        faces.Add(new TriangleIndices(0, 10, 11));

        // 5 adjacent faces 
        faces.Add(new TriangleIndices(1, 5, 9));
        faces.Add(new TriangleIndices(5, 11, 4));
        faces.Add(new TriangleIndices(11, 10, 2));
        faces.Add(new TriangleIndices(10, 7, 6));
        faces.Add(new TriangleIndices(7, 1, 8));

        // 5 faces around point 3
        faces.Add(new TriangleIndices(3, 9, 4));
        faces.Add(new TriangleIndices(3, 4, 2));
        faces.Add(new TriangleIndices(3, 2, 6));
        faces.Add(new TriangleIndices(3, 6, 8));
        faces.Add(new TriangleIndices(3, 8, 9));

        // 5 adjacent faces 
        faces.Add(new TriangleIndices(4, 9, 5));
        faces.Add(new TriangleIndices(2, 4, 11));
        faces.Add(new TriangleIndices(6, 2, 10));
        faces.Add(new TriangleIndices(8, 6, 7));
        faces.Add(new TriangleIndices(9, 8, 1));

        // refine triangles
        for (int i = 0; i < recursion; i++)
        {
            List<TriangleIndices> faces2 = new List<TriangleIndices>();
            foreach (var tri in faces)
            {
                // replace triangle by 4 triangles
                int a = getMiddlePoint(tri.v1, tri.v2, ref vertList, ref middlePointIndexCache);
                int b = getMiddlePoint(tri.v2, tri.v3, ref vertList, ref middlePointIndexCache);
                int c = getMiddlePoint(tri.v3, tri.v1, ref vertList, ref middlePointIndexCache);

                faces2.Add(new TriangleIndices(tri.v1, a, c));
                faces2.Add(new TriangleIndices(tri.v2, b, a));
                faces2.Add(new TriangleIndices(tri.v3, c, b));
                faces2.Add(new TriangleIndices(a, b, c));
            }
            faces = faces2;
        }

        Vector3[] vertArray = new Vector3[vertList.Count];
        Vector2[] uvArray   = new Vector2[vertList.Count];

        for (int i = 0; i < vertList.Count; i++)
        {
            Vector3 vertex = vertList[i];
            Vector2 uv = CalcUV(vertex);

            vertArray[i] = vertex * CalcHeight(uv);
            uvArray[i]   = uv;
        }

        int[] triArray = new int[faces.Count * 3];
        for (int i = 0; i < faces.Count; i++)
        {
            TriangleIndices face = faces[i];

            triArray[i * 3]     = face.v1;
            triArray[i * 3 + 1] = face.v2;
            triArray[i * 3 + 2] = face.v3;
        }

        mesh.vertices  = vertArray;
        mesh.triangles = triArray;
        mesh.uv        = uvArray;

        mesh.RecalculateBounds();

        return mesh;
    }

    private float CalcHeight(Vector2 uv)
    {
        return Radius + _heightMap.GetPixel((int)(uv.x * _heightMap.width), (int)(uv.y * _heightMap.height)).grayscale * (_globeSettings.heightMulti * Radius);
    }

    private Vector2 CalcUV(Vector3 vertice)
    {
        Vector3 d = vertice.normalized;
        return new Vector2(0.5f + Mathf.Atan2(d.x, d.z) / Mathf.PI / 2, Mathf.Asin(d.y) / Mathf.PI + 0.5f);
    }

    #endregion


    #region GETTERS / SETTERS
    private MeshRenderer MR
    {
        get
        {
            if (_mr == null)
                _mr = GetComponent<MeshRenderer>();

            return _mr;
        }
    }

    private MeshFilter MF
    {
        get
        {
            if (_mf == null)
                _mf = GetComponent<MeshFilter>();

            return _mf;
        }
    }

    private MeshCollider MC
    {
        get
        {
            if (_mc == null)
                _mc = GetComponent<MeshCollider>();

            return _mc;
        }
    }

    public float Radius
    {
        get { return _globeSettings.radius; }
    }

    public float MaxHeight
    {
        get { return Radius + _globeSettings.heightMulti * Radius; }
    }

    public float Gravity
    {
        get { return _gravityAcceleration; }
    }

    public float LevelWidth
    {
        get { return _levelWidth; }
    }

    public float WaterLevel
    {
        get { return Radius + _waterLevel; }
    }

    public static Vector3 SceneToGlobePosition(Vector3 scenePosition)
    {
        Globe globe = ServiceLocator.Locate<Globe>();

        Vector3 normalizedScenePosition = scenePosition.normalized;
        Vector3 rayGlobePos = normalizedScenePosition * (globe.MaxHeight + 1);
        Vector3 globePosition = new Vector3(Mathf.Atan2(normalizedScenePosition.x, normalizedScenePosition.y), scenePosition.magnitude, Mathf.Sin(normalizedScenePosition.z));

        RaycastHit hit;

        if (Physics.Raycast(rayGlobePos, -normalizedScenePosition, out hit, globe.MaxHeight - globe.WaterLevel + 1, 1 << 10))
            globePosition.y = scenePosition.magnitude - hit.point.magnitude;
        else
            globePosition.y = scenePosition.magnitude - globe.WaterLevel;

        return globePosition;
    }

    public static Vector3 GlobeToScenePosition(Vector3 globePosition)
    {
        Vector3 temp;
        return GlobeToScenePosition(globePosition, out temp);
    }

    public static Vector3 GlobeToScenePosition(Vector3 globePosition, out Vector3 normal)
    {
        Globe globe = ServiceLocator.Locate<Globe>();

        Vector3 rayGlobePos = new Vector3(globePosition.x, globe.MaxHeight + 1, globePosition.z);
        Vector3 rayPos = new Vector3(Mathf.Sin(rayGlobePos.x) * Mathf.Cos(rayGlobePos.z), Mathf.Cos(rayGlobePos.x) * Mathf.Cos(rayGlobePos.z), Mathf.Sin(rayGlobePos.z)) * rayGlobePos.y;

        Vector3 GlobeDown = -rayPos.normalized;

        float raylength = globe.MaxHeight - globe.WaterLevel + 1;

        RaycastHit tempHit;
        if (Physics.Raycast(rayPos, GlobeDown, out tempHit, raylength, 1 << 10))
        {
            normal = tempHit.normal;
            return tempHit.point + tempHit.point.normalized * globePosition.y;
        }
        else
        {
            normal = -GlobeDown;
            return rayPos.normalized * (globe.WaterLevel + globePosition.y);
        }
    }
    #endregion

    [DidReloadScripts]
    private static void OnSceneReload()
    {
        ServiceLocator.Locate<Globe>().SetUniforms();
    }
}
