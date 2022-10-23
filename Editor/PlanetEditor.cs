using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : UnityEditor.Editor
{
    private Planet planet;
    private UnityEditor.Editor terrainEditor;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DrawSettingsEditor(planet.terrainSettings, ref planet.terrainSettingsFoldout, ref terrainEditor);
    }

    void DrawSettingsEditor(Object settings, ref bool foldout, ref UnityEditor.Editor editor)
    {
        if (settings != null)
        {
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
            if (foldout)
            {
                CreateCachedEditor(settings, null, ref editor);
                editor.OnInspectorGUI();
            }
        }
    }

    private void OnEnable()
    {
        planet = (Planet) target;
    }
}