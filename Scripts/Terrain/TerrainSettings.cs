using Noise;
using UnityEngine;

namespace Terrain
{
    [CreateAssetMenu]
    public class TerrainSettings : ScriptableObject
    {
        
        [Range(1, 20)]
        public int planetRadius = 8;
        public NoiseLayer[] noiseLayers;

        [System.Serializable]
        public class NoiseLayer
        {
            public bool enabled = true;
            public bool useFirstLayerAsMask;
            public NoiseSettings noiseSettings;
        }
    }
}