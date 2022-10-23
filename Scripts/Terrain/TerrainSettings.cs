using Noise;
using UnityEngine;

namespace Terrain
{
    [CreateAssetMenu]
    public class TerrainSettings : ScriptableObject
    {
        
        public int planetRadius = 1;
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