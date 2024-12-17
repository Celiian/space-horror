using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerminalInteraction))]
public class TerminalInteractionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TerminalInteraction terminalInteraction = (TerminalInteraction)target;

        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("terminal"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("terminalName"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultInteractible"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("terminalPageText"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("terminalNameText"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("onInteract"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("nextPageButton"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("previousPageButton"));

        SerializedProperty pagesProperty = serializedObject.FindProperty("pages");
        EditorGUILayout.LabelField("Pages", EditorStyles.boldLabel);
        for (int i = 0; i < pagesProperty.arraySize; i++)
        {
            SerializedProperty pageProperty = pagesProperty.GetArrayElementAtIndex(i);
            pageProperty.stringValue = EditorGUILayout.TextArea(pageProperty.stringValue, GUILayout.Height(250));
        }


        if (GUILayout.Button("Add Page"))
        {
            pagesProperty.InsertArrayElementAtIndex(pagesProperty.arraySize);
        }

        if (GUILayout.Button("Remove Last Page") && pagesProperty.arraySize > 0)
        {
            pagesProperty.DeleteArrayElementAtIndex(pagesProperty.arraySize - 1);
        }

        serializedObject.ApplyModifiedProperties();
    }
}