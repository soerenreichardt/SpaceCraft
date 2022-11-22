/*
Copyright (c) 2018 Sebastian Lague
*/

using System;
using UnityEngine;

namespace Noise
{
    [Serializable]
    public class NoiseSettings {

        public enum FilterType { Simple, Ridgid, Continent, SmoothRidgid };
        public FilterType filterType;

        [ConditionalHide("filterType", 0)]
        public SimpleNoiseSettings simpleNoiseSettings;
        [ConditionalHide("filterType", 1)]
        public RidgidNoiseSettings ridgidNoiseSettings;
        [ConditionalHide("filterType", 2)] 
        public SimpleNoiseSettings continentNoiseSettings;
        [ConditionalHide("filterType", 3)] 
        public RidgidNoiseSettings smoothRidgidNoiseSettings;
        
        [Serializable]
        public class SimpleNoiseSettings
        {
            public bool scaled = false;
            public float multiplier = 1;
            [Range(1, 8)]
            public int numLayers = 1;
            public float scale = 1;
            public float lacunarity = 2;
            public float persistence = .5f;
            public Vector3 offset;
        }

        [Serializable]
        public class RidgidNoiseSettings : SimpleNoiseSettings
        {
            public float gain = 0.5f;
            public float power = .8f;
        }
    }
}