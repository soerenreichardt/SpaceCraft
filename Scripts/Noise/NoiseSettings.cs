/*
Copyright (c) 2018 Sebastian Lague
*/

using System;
using UnityEditor;
using UnityEngine;

namespace Noise
{
    [CreateAssetMenu()]
    public class NoiseSettings : ScriptableObject {

        public delegate void SettingsUpdateHandler();
        public event SettingsUpdateHandler settingsUpdated;

        private long lastUpdate = 0;
        
        public enum FilterType { Simple, Ridgid };
        public FilterType filterType;

        [ConditionalHide("filterType", 0)]
        public SimpleNoiseSettings simpleNoiseSettings;
        [ConditionalHide("filterType", 1)]
        public RidgidNoiseSettings ridgidNoiseSettings;

        [System.Serializable]
        public class SimpleNoiseSettings
        {
            public float strength = 1;
            [Range(1, 8)]
            public int numLayers = 1;
            public float baseRoughness = 1;
            public float roughness = 2;
            public float persistence = .5f;
            public Vector3 centre;
            public float minValue;
        }

        [System.Serializable]
        public class RidgidNoiseSettings : SimpleNoiseSettings
        {
            public float weightMultiplier = .8f;
        }

        public void OnValidate()
        {
            if (EditorApplication.isPlaying)
            {
                var currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                if (currentTime - lastUpdate > 1000)
                {
                    if (settingsUpdated != null) settingsUpdated();
                    lastUpdate = currentTime;
                }
            }
        }
    }
}