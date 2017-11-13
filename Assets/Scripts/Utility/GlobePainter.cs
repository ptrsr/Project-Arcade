using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode] [CustomEditor(typeof(Globe))]
public class GlobePainter : Editor
{
    bool _picking = false;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Paint terrain"))
            _picking ^= true;

        if (GUILayout.Button("Safe terrain"))
            ((Globe)target).SavePaintMap();
    }

    private void OnSceneGUI()
    {
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

        int v1 = mesh.triangles[hit.triangleIndex * 3];
        int v2 = mesh.triangles[hit.triangleIndex * 3 + 1];
        int v3 = mesh.triangles[hit.triangleIndex * 3 + 2];

        Vector3 p1 = hit.transform.TransformPoint(mesh.vertices[v1]);
        Vector3 p2 = hit.transform.TransformPoint(mesh.vertices[v2]);
        Vector3 p3 = hit.transform.TransformPoint(mesh.vertices[v3]);

        Handles.DrawLine(p1, p2);
        Handles.DrawLine(p2, p3);
        Handles.DrawLine(p3, p1);
        
        if (Application.isEditor)
            SceneView.RepaintAll();

        Globe globe = (Globe)target;

        if (Event.current.button == 0 && Event.current.type == EventType.mouseDown)
            globe.Draw(new int[] { v1, v2, v3 });
    }
}
