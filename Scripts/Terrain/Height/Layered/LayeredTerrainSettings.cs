using Noise;
using UnityEngine;

namespace Terrain.Height.Layered
{
    [CreateAssetMenu]
    public class LayeredTerrainSettings : BaseTerrainSettings
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
