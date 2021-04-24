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
        return EditorGUIUtility.singleLineHeight * 2 + 2;
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
        var tLw = 80;
        var w = position.width * 0.4f;
        var h = position.height * 0.5f;
        var offsetRect = new Rect(position.x, position.y, w,h );
        var lengthRect = new Rect(position.x + w + 10, position.y, w, h);
        var sideOffstRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, w, h);
        var sidRect = new Rect(position.x + w + 10, position.y + EditorGUIUtility.singleLineHeight, w, h);

        var lw = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = tLw;
        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(lengthRect, property.FindPropertyRelative(nameof(PlatformData.length)));
        EditorGUI.PropertyField(offsetRect, property.FindPropertyRelative(nameof(PlatformData.offset)));
        EditorGUI.PropertyField(sidRect, property.FindPropertyRelative(nameof(PlatformData.sideCount)));
        EditorGUI.PropertyField(sideOffstRect, property.FindPropertyRelative(nameof(PlatformData.sideOffset)));
        EditorGUIUtility.labelWidth = lw;

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
}
