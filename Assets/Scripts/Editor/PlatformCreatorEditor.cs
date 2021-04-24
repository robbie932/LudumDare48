using System.Collections;
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

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DrawGlobalDisplaySettingsInspector();
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
            var data = creator.sections[i];
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
