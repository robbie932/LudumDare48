﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlatformCreator))]
public class PlatformCreatorEditor : Editor
{
    PlatformCreator creator;
    private Editor globalDisplaySettingsEditor;

    private void OnEnable()
    {
        creator = target as PlatformCreator;
    }
    int buttonSize = 32;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        DrawSections();
        DrawGlobalDisplaySettingsInspector();
        serializedObject.ApplyModifiedProperties();
    }

    void DrawSections()
    {
        var sectionsProp = serializedObject.FindProperty(nameof(PlatformCreator.sections));
        var lw = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 60;
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        {
            EditorGUILayout.LabelField("Sections", EditorStyles.boldLabel);

            for (int i = 0; i < sectionsProp.arraySize; i++)
            {
                var current = sectionsProp.GetArrayElementAtIndex(i);
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PropertyField(current, new GUIContent("Section " + i));
                    EditorGUILayout.BeginHorizontal(GUILayout.Width(buttonSize * 3));
                    {
                        if (GUILayout.Button("^", EditorStyles.miniButtonLeft, GUILayout.Width(buttonSize)))
                        {
                            if (i > 0)
                            {
                                sectionsProp.MoveArrayElement(i, i - 1);
                                break;
                            }
                        }
                        if (GUILayout.Button("v", EditorStyles.miniButtonMid, GUILayout.Width(buttonSize)))
                        {
                            if (i < sectionsProp.arraySize)
                            {
                                sectionsProp.MoveArrayElement(i, i + 1);
                                break;
                            }
                        }
                        if (GUILayout.Button("x", EditorStyles.miniButtonRight, GUILayout.Width(buttonSize)))
                        {
                            if (i > 0)
                            {
                                sectionsProp.DeleteArrayElementAtIndex(i);
                                break;
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndVertical();
        EditorGUIUtility.labelWidth = lw;
    }
    void DrawGlobalDisplaySettingsInspector()
    {
        var uniqueSections = creator.sections.Distinct().ToArray();
        var usedPlaces = new List<string>();
        for (int i = 0; i < uniqueSections.Length; i++)
        {
            usedPlaces.Add("");
            for (int s = 0; s < creator.sections.Length; s++)
            {
                if (uniqueSections[i] == creator.sections[s])
                {
                    usedPlaces[i] += s + ", ";
                }
            }
        }

        for (int i = 0; i < uniqueSections.Length; i++)
        {
            var data = uniqueSections[i];
            if (data == null)
            {
                continue;
            }
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.LabelField(usedPlaces[i], EditorStyles.boldLabel);
                    data.globalDisplaySettingsFoldout = EditorGUILayout.InspectorTitlebar(data.globalDisplaySettingsFoldout, data);
                    if (data.globalDisplaySettingsFoldout)
                    {
                        CreateCachedEditor(data, null, ref globalDisplaySettingsEditor);
                        globalDisplaySettingsEditor.OnInspectorGUI();
                    }
                    if (check.changed)
                    {
                        SceneView.RepaintAll();
                    }
                }
                EditorGUILayout.EndVertical();
            }
        }
    }
}
