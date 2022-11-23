using Noise;
using UnityEngine;

namespace Terrain.Height
{
    [CreateAssetMenu]
    public class EarthTerrainSettings : TerrainSettings
    {
        public float oceanDepthMultiplier;
        public float oceanFloorDepth;
        public float oceanFloorSmoothing;

        [Range(0,1)]
        public float mountainBlend;
        
        public NoiseSettings continentNoiseSettings;
        public NoiseSettings ridgeNoiseSettings;
        public NoiseSettings maskNoiseSettings;
    }
}
