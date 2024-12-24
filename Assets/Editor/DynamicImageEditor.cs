using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(DynamicImage))]
public class DynamicImageEditor : RawImageEditor 
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("rectWidth"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("rectHeight"));

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
