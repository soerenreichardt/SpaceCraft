using Noise;
using UnityEngine;

namespace Terrain
{
    [CreateAssetMenu]
    public class LayeredTerrainSettings : TerrainSettings
    {
        public NoiseLayer[] noiseLayers;

        [System.Serializable]
        public class NoiseLayer
        {
            public bool enabled = true;
            public bool useFirstLayerAsMask;
            public bool scaled;
            public NoiseSettings noiseSettings;
        }
    }
}
