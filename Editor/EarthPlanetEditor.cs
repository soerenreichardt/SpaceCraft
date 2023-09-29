using System;
using Terrain;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor
{
    [CustomEditor(typeof(EarthPlanet))]
    public class EarthPlanetEditor : UnityEditor.Editor
    {
        private EarthPlanet planet;
        private UnityEditor.Editor terrainEditor;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawSettingsEditor(planet.earthTerrainSettings, planet.OnTerrainSettingsUpdated,  ref planet.earthTerrainSettingsFoldout, ref terrainEditor);
        }

        private static void DrawSettingsEditor(Object settings, Action onSettingsUpdated, ref bool foldout, ref UnityEditor.Editor editor)
        {
            if (settings != null)
            {
                foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
                if (!foldout) return;
            
                using var check = new EditorGUI.ChangeCheckScope();

                CreateCachedEditor(settings, null, ref editor);
                editor.OnInspectorGUI();

                if (check.changed && Application.isPlaying)
                {
                    onSettingsUpdated?.Invoke();
                }
            }
        }

        private void OnEnable()
        {
            planet = (EarthPlanet) target;
        }
    }
}