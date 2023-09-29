using System;
using Terrain;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor
{
    [CustomEditor(typeof(SebastianLaguePlanet))]
    public class SebastianLaguePlanetEditor : UnityEditor.Editor
    {
        private SebastianLaguePlanet planet;
        private UnityEditor.Editor terrainEditor;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawSettingsEditor(planet.earthTerrainSettings, planet.OnTerrainSettingsUpdated,  ref planet.earthTerrainSettingsFoldout, ref terrainEditor);
            DrawSettingsEditor(planet.shadingSettings, planet.OnShadingSettingsUpdated,  ref planet.shadingSettingsFoldout, ref terrainEditor);
            DrawSettingsEditor(planet.oceanEffectSettings, planet.OnOceanEffectSettingsUpdated,  ref planet.oceanEffectSettingsFoldout, ref terrainEditor);
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
            planet = (SebastianLaguePlanet) target;
        }
    }
}