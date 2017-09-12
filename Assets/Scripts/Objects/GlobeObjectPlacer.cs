using UnityEngine;
using System.Collections;
using UnityEditor;

[ExecuteInEditMode] [CustomEditor(typeof(GlobeObject), true)]
public class GlobeObjectPlacer : Editor
{
    private bool _placing = false;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Place Object"))
            _placing = true;
    }

    void OnSceneGUI()
    {
        if (!_placing)
            return;

        Vector3 mousePosition = Event.current.mousePosition;

        Camera cam = SceneView.lastActiveSceneView.camera;
        mousePosition.y = cam.pixelHeight - mousePosition.y;

        Ray ray = cam.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit))
            return;

        GlobeObject building = (GlobeObject)target;
        building.SetPosition(hit.point);

        if (Event.current.type == EventType.mouseDown)
            _placing = false;
    }
}