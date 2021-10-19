using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// https://www.youtube.com/watch?v=pF2IWWUzuIQ

[CustomPropertyDrawer(typeof(ReadOnlyInspectorAttribute))]
public class ReadOnlyInspectorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label);
        GUI.enabled = true;
    }
}

