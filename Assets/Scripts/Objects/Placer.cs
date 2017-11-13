using UnityEngine;
using System.Collections;
using UnityEditor;

[ExecuteInEditMode] [CustomEditor(typeof(GlobeObject), true)]
public class Placer : Editor
{
    private bool _placing = false;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorUtility.SetDirty(target);

        if (GUILayout.Button("Place Object"))
            _placing = true;
    }

    void OnSceneGUI()
    {
        if (!_placing || Application.isPlaying)
            return;
        Vector3 mousePosition = Event.current.mousePosition;

        Camera cam = SceneView.lastActiveSceneView.camera;
        mousePosition.y = cam.pixelHeight - mousePosition.y;

        Ray ray = cam.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, 1 << 10))
            return;

        GlobeObject globeObject = (GlobeObject)target;

        if (!Event.current.alt)
        {

            Vector3 globePos = Globe.SceneToGlobePosition(hit.point);
            Vector3 normal;
            Vector3 worldPos = Globe.GlobeToScenePosition(globePos, out normal);

            globeObject.SetPosition(worldPos);
            globeObject.transform.up = normal;
        }
        else
        {
            Vector3 delta = globeObject.transform.InverseTransformDirection(hit.point - globeObject.transform.position);
            delta.y = 0;
            Vector3 lookPos = globeObject.transform.TransformDirection(delta);

            globeObject.transform.rotation = Quaternion.LookRotation(lookPos.normalized, globeObject.transform.up);
        }

        if (Event.current.type == EventType.mouseDown)
            _placing = false;
    }
}