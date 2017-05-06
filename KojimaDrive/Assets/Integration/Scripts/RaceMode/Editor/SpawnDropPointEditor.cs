using UnityEngine;
using System.Collections;
using UnityEditor;
namespace Kojima
{
    [System.Serializable]
    [CustomEditor(typeof(DropPointSpawn))]
    public class SpawnDropPointEditor : Editor
    {

        DropPointSpawn spawnScript;
        public override void OnInspectorGUI()
        {


            DrawDefaultInspector();
            spawnScript = target as DropPointSpawn;

            if (GUILayout.Button("Build Object"))
            {
                Undo.RecordObject(spawnScript, "Build Object");
                spawnScript.BuildObject();
                EditorUtility.SetDirty(spawnScript);
            }
            if (GUILayout.Button("Print List"))
            {
                Undo.RecordObject(spawnScript, "Print List");
                spawnScript.PrintList();
                EditorUtility.SetDirty(spawnScript);
            }
            if (GUILayout.Button("Name Objects"))
            {
                Undo.RecordObject(spawnScript, "Name Objects");
                spawnScript.NameObjects();
                EditorUtility.SetDirty(spawnScript);
            }
            if (GUILayout.Button("Reset Names"))
            {
                Undo.RecordObject(spawnScript, "Reset Names");
                spawnScript.ResetNames();
                EditorUtility.SetDirty(spawnScript);
            }
            if (GUILayout.Button("Standardise Size"))
            {
                Undo.RecordObject(spawnScript, "Standardise Size");
                spawnScript.StandardiseSize();
                EditorUtility.SetDirty(spawnScript);
                
            }



        }
    }
}

