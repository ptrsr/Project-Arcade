using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode] [CustomEditor(typeof(GroundEnemy))]
public class EnemyController : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        
    }

    
}
