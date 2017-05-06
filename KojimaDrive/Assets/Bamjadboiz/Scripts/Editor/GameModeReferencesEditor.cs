using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(GameModeReferences), true)]
public class GameModeReferencesEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DrawDictionary(serializedObject.FindProperty("m_gameObjects"), "GameObjects");
        DrawDictionary(serializedObject.FindProperty("m_components"), "Components", true);

        serializedObject.ApplyModifiedProperties();
    }

    private static void DrawDictionary(SerializedProperty dictionaryProp, string name, bool allowObjectTypeModifying = false)
    {
        dictionaryProp.isExpanded = EditorGUILayout.Foldout(dictionaryProp.isExpanded, name);
        if (dictionaryProp.isExpanded)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Add"))
            {
                dictionaryProp.arraySize++;
            }

            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < dictionaryProp.arraySize; i++)
            {
                SerializedProperty objRefByNameProp = dictionaryProp.GetArrayElementAtIndex(i);

                EditorGUILayout.BeginHorizontal();

                float oldLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth / (allowObjectTypeModifying ? 8f : 7f);

                EditorGUILayout.PropertyField(objRefByNameProp.FindPropertyRelative("name"));

                if (allowObjectTypeModifying)
                {
                    List<System.Type> allComponentTypes = Bam.GameMode.GameMode.GetAllClassesInheritingClass<Component>(true);
                    string[] names = new string[allComponentTypes.Count];

                    int selectedIndex = 0;
                    int j = 0;
                    foreach(System.Type componentType in allComponentTypes)
                    {
                        names[j] = componentType.Name;

                        if(objRefByNameProp.FindPropertyRelative("obj").objectReferenceValue != null && objRefByNameProp.FindPropertyRelative("obj").objectReferenceValue.GetType() == componentType)
                        {
                            selectedIndex = j;
                        }

                        j++;
                    }

                    selectedIndex = EditorGUILayout.Popup(selectedIndex, names);

                    objRefByNameProp.FindPropertyRelative("obj").objectReferenceValue = EditorGUILayout.ObjectField(objRefByNameProp.FindPropertyRelative("obj").objectReferenceValue, allComponentTypes[selectedIndex], true);
                }
                else
                {
                    EditorGUILayout.PropertyField(objRefByNameProp.FindPropertyRelative("obj"));
                }

                if (GUILayout.Button("Del"))
                {
                    //Shift all elements down, overwriting element at index i
                    for (int j = i + 1; j < dictionaryProp.arraySize; j++)
                    {
                        SerializedProperty a = dictionaryProp.GetArrayElementAtIndex(j - 1);
                        SerializedProperty b = dictionaryProp.GetArrayElementAtIndex(j);

                        a.FindPropertyRelative("name").stringValue = b.FindPropertyRelative("name").stringValue;
                        a.FindPropertyRelative("obj").objectReferenceValue = b.FindPropertyRelative("obj").objectReferenceValue;
                    }

                    //Remove the last element
                    dictionaryProp.arraySize--;
                }

                EditorGUIUtility.labelWidth = oldLabelWidth;

                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel--;
        }
    }
}
