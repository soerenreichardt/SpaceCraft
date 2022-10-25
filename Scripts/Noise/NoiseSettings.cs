/*
Copyright (c) 2018 Sebastian Lague
*/

using System;
using UnityEngine;

namespace Noise
{
    [Serializable]
    public class NoiseSettings {

        // public delegate void SettingsUpdateHandler();
        // public event SettingsUpdateHandler settingsUpdated;

        // private long lastUpdate;
        
        public enum FilterType { Simple, Ridgid };
        public FilterType filterType;

        [ConditionalHide("filterType", 0)]
        public SimpleNoiseSettings simpleNoiseSettings;
        [ConditionalHide("filterType", 1)]
        public RidgidNoiseSettings ridgidNoiseSettings;

        [Serializable]
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

        [Serializable]
        public class RidgidNoiseSettings : SimpleNoiseSettings
        {
            public float weightMultiplier = .8f;
        }
        //
        // public void OnValidate()
        // {
        //     if (EditorApplication.isPlaying)
        //     {
        //         var currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        //         if (currentTime - lastUpdate > 300)
        //         {
        //             if (settingsUpdated != null) settingsUpdated();
        //             lastUpdate = currentTime;
        //         }
        //     }
        // }
    }
}