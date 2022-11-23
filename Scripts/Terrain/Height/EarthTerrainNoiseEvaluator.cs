using Noise;
using UnityEngine;

namespace Terrain.Height
{
    public class EarthTerrainNoiseEvaluator : INoiseEvaluator
    {
        private readonly EarthTerrainSettings terrainSettings;
        private readonly INoiseFilter continentNoiseFilter;
        private readonly INoiseFilter ridgeNoiseFilter;
        private readonly INoiseFilter maskNoiseFilter;
    
        public EarthTerrainNoiseEvaluator(EarthTerrainSettings terrainSettings)
        {
            this.terrainSettings = terrainSettings;
            this.continentNoiseFilter = NoiseFilterFactory.CreateNoiseFilter(terrainSettings.continentNoiseSettings);
            this.ridgeNoiseFilter = NoiseFilterFactory.CreateNoiseFilter(terrainSettings.ridgeNoiseSettings);
            this.maskNoiseFilter = NoiseFilterFactory.CreateNoiseFilter(terrainSettings.maskNoiseSettings);
        }

        public float CalculateElevation(Vector3 pointOnUnitSphere, Vector3 pointOnScaledSphere)
        {
            var continentShape = continentNoiseFilter.Evaluate(pointOnUnitSphere);
            continentShape = SmoothMax(continentShape, -terrainSettings.oceanFloorDepth, terrainSettings.oceanFloorSmoothing);
            
            continentShape *= continentShape < 0 ? 1 + terrainSettings.oceanDepthMultiplier : 1;
            
            var mountainShape = ridgeNoiseFilter.Evaluate(pointOnUnitSphere);
            
            var mask = Mathf.SmoothStep(0, terrainSettings.mountainBlend, maskNoiseFilter.Evaluate(pointOnUnitSphere));
            
            return continentShape * Planet.SCALE + mountainShape * Planet.SCALE * mask;
        }

        private static float SmoothMax(float a, float b, float k)
        {
            k = Mathf.Min(0, -k);
            var h = Mathf.Max(0, Mathf.Min(1, (b - a + k) / (2 * k)));
            return a * h + b * (1 - h) - k * h * (1 - h);
        }
    }
}
