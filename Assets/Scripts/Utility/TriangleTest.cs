using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode] [CustomEditor(typeof(Globe))]
public class TriangleTest : Editor
{
    bool _picking = false;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Globe globe = (Globe)target;

        if (GUILayout.Button("Place Object"))
            _picking ^= true;
    }

    private void OnSceneGUI()
    {
        UpdateGlobe();

        if (!_picking)
            return;

        Vector3 mousePosition = Event.current.mousePosition;

        Camera cam = SceneView.lastActiveSceneView.camera;
        mousePosition.y = cam.pixelHeight - mousePosition.y;

        Ray ray = cam.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, 1 << 10))
            return;

        Mesh mesh = ((MonoBehaviour)target).GetComponent<MeshFilter>().sharedMesh;

        Vector3 p1 = hit.transform.TransformPoint(mesh.vertices[mesh.triangles[hit.triangleIndex * 3]]);
        Vector3 p2 = hit.transform.TransformPoint(mesh.vertices[mesh.triangles[hit.triangleIndex * 3 + 1]]);
        Vector3 p3 = hit.transform.TransformPoint(mesh.vertices[mesh.triangles[hit.triangleIndex * 3 + 2]]);

        Handles.DrawLine(p1, p2);
        Handles.DrawLine(p2, p3);
        Handles.DrawLine(p3, p1);
        
        SceneView.RepaintAll();

        //if (Event.current.type == EventType.mouseDown && Event.current.button == 0)
        //    _picking = false;
    }

    private void UpdateGlobe()
    {
        Globe globe = (Globe)target;

        if (globe.Updated)
            return;

        globe.CreateWorld();
        globe.Updated = true;
    }
}
