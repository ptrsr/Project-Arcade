using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnGlobeChange();

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class Globe : MonoBehaviour
{
    [SerializeField] [Range(0, 5)]
    private int _recursion;

    [SerializeField]
    private float
        _radius,
        _heightMulti,
        _waterLevel,
        _waveHeight,
        _waveMulti,
        _waveSpeed,
        _specular;

    [SerializeField] [Range(0, 1)]
    private float _ambient;

    [SerializeField]
    private Texture2D
        _heightMap,
        _waveMap;

    [SerializeField]
    private Shader _shader;

    [SerializeField]
    private Color
        _grass,
        _water;

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
    }

    private void OnValidate()
    {
        ServiceLocator.Provide(this);

        CreateWorld();

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

    private void CreateWorld()
    {
        MF.mesh = Create(_recursion);
        SetUniforms();
    }

    private void SetUniforms()
    {
        _mat = new Material(_shader);

        _mat.SetFloat("_ambient", _ambient);
        _mat.SetFloat("_specular", _specular);

        _mat.SetColor("_grass", _grass);
        _mat.SetColor("_water", _water);

        _mat.SetFloat("_waterLevel", _waterLevel + _radius);
        _mat.SetFloat("_waveMulti", _waveMulti);
        _mat.SetFloat("_waveHeight", _waveHeight * _radius);

        _mat.SetTexture("_waveMap", _waveMap);

        MR.material = _mat;
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

        Vector3[] vertArray   = new Vector3[faces.Count * 3];
        Vector3[] normalArray = new Vector3[faces.Count * 3];
        Vector2[] uvArray     = new Vector2[faces.Count * 3];

        Vector3[] tVertArray = new Vector3[vertList.Count];
        Vector2[] tUvArray   = new Vector2[vertList.Count];

        for (int i = 0; i < vertList.Count; i++)
        {
            Vector3 vertex = vertList[i];
            Vector2 uv = CalcUV(vertex);

            tVertArray[i] = vertex * CalcHeight(uv);
            tUvArray[i]   = uv;
        }


        int[] triArray = new int[faces.Count * 3];
        for (int i = 0; i < faces.Count; i++)
        {
            TriangleIndices face = faces[i];

            uvArray[i * 3]     = tUvArray[face.v1];
            uvArray[i * 3 + 1] = tUvArray[face.v2];
            uvArray[i * 3 + 2] = tUvArray[face.v3];

            vertArray[i * 3]     = tVertArray[face.v1];
            vertArray[i * 3 + 1] = tVertArray[face.v2];
            vertArray[i * 3 + 2] = tVertArray[face.v3];

            triArray[i * 3]     = i * 3;
            triArray[i * 3 + 1] = i * 3 + 1;
            triArray[i * 3 + 2] = i * 3 + 2;

            Vector3 normal = Vector3.Cross(tVertArray[face.v2] - tVertArray[face.v1], tVertArray[face.v3] - tVertArray[face.v1]).normalized;

            for (int j = 0; j < 3; j++)
                normalArray[i * 3 + j] = normal;
        }

        mesh.vertices  = vertArray;
        mesh.triangles = triArray;
        mesh.normals   = normalArray;
        mesh.uv        = uvArray;

        mesh.RecalculateBounds();

        return mesh;
    }

    private float CalcHeight(Vector2 uv)
    {
        return _radius + _heightMap.GetPixel((int)(uv.x * _heightMap.width), (int)(uv.y * _heightMap.height)).grayscale * (_heightMulti * _radius);
    }

    private Vector2 CalcUV(Vector3 position)
    {
        Vector3 d = position.normalized;
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

    public float Radius
    {
        get { return _radius; }
    }

    public float MaxHeight
    {
        get { return _heightMulti * _radius; }
    }

    public float Gravity
    {
        get { return _gravityAcceleration; }
    }

    public float LevelWidth
    {
        get { return _levelWidth; }
    }

    public static Vector3 SceneToGlobePosition(Vector3 scenePosition, bool relative = false)
    {
        float radius = ServiceLocator.Locate<Globe>().Radius;
        Vector3 globePosition = new Vector3(Mathf.Atan2(scenePosition.x, scenePosition.y), scenePosition.magnitude, Mathf.Sin(scenePosition.z / radius));

        if (relative)
            globePosition.y -= radius;

        return globePosition;
    }

    public static Vector3 GlobeToScenePosition(Vector3 globePosition)
    {
        return new Vector3(Mathf.Sin(globePosition.x) * Mathf.Cos(globePosition.z), Mathf.Cos(globePosition.x) * Mathf.Cos(globePosition.z), Mathf.Sin(globePosition.z)) * globePosition.y;
    }

    public Vector3 TerrainPosition(Vector3 position)
    {
        Vector3 rayPos = position.normalized * MaxHeight;

        RaycastHit hit;
        if (Physics.Raycast(rayPos, -rayPos.normalized, out hit, _radius - MaxHeight))
            return hit.point;
        }
    }
    #endregion


}
