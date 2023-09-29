using Noise;
using UnityEngine;

namespace Terrain.Height.SebastianLague
{
    [CreateAssetMenu]
    public class EarthTerrainSettings : BaseTerrainSettings
    {
        public float oceanDepthMultiplier;
        public float oceanFloorDepth;
        public float oceanFloorSmoothing;

        [Range(0,1)]
        public float mountainBlend;
        
        public NoiseSettings continentNoiseSettings;
        public NoiseSettings ridgeNoiseSettings;
        public NoiseSettings maskNoiseSettings;

        public EarthTerrainSettings Clone()
        {
            return (EarthTerrainSettings) this.MemberwiseClone();
        }
    }
}
