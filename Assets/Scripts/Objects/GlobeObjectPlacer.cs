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

        if (!Physics.Raycast(ray, out hit, 1 << 10))
            return;

        GlobeObject building = (GlobeObject)target;

        if (!Event.current.alt)
        {
            building.SetPosition(hit.point);
            building.transform.up = hit.normal;
        }
        else
        {
            Vector3 delta = building.transform.InverseTransformDirection(hit.point - building.transform.position);
            delta.y = 0;
            Vector3 lookPos = building.transform.TransformDirection(delta);

            building.transform.rotation = Quaternion.LookRotation(lookPos.normalized, building.transform.up);
        }

        if (Event.current.type == EventType.mouseDown)
            _placing = false;
    }
}