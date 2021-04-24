using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomPropertyDrawer(typeof(PlatformData))]
public class PlatformDataDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        // Draw label
        position = EditorGUI.PrefixLabel(position, new GUIContent(label.text.Replace("Element", "Platform")));

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var tLw = 60;
        var amountRect = new Rect(position.x, position.y, position.width * 0.5f - tLw, position.height);
        var unitRect = new Rect(position.x + position.width * 0.5f, position.y, position.width * 0.5f - tLw, position.height);
        var lw = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = tLw;
        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(amountRect, property.FindPropertyRelative(nameof(PlatformData.length)));
        EditorGUI.PropertyField(unitRect, property.FindPropertyRelative(nameof(PlatformData.offset)));
        EditorGUIUtility.labelWidth = lw;

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
