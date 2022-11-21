using Noise;
using UnityEngine;

namespace Terrain
{
    public class LayeredTerrainNoiseEvaluator : INoiseEvaluator
    {
        
        private readonly LayeredTerrainSettings settings;
        private readonly INoiseFilter[] noiseFilters;

        public LayeredTerrainNoiseEvaluator(LayeredTerrainSettings settings)
        {
            this.settings = settings;
            noiseFilters = new INoiseFilter[settings.noiseLayers.Length];
            for (var i = 0; i < noiseFilters.Length; i++)
            {
                noiseFilters[i] = NoiseFilterFactory.CreateNoiseFilter(settings.noiseLayers[i].noiseSettings);
            }
        }

        public float CalculateElevation(Vector3 pointOnUnitSphere, Vector3 pointOnScaledSphere)
        {
            float firstLayerValue = 0;
            float elevation = 0;

            if (noiseFilters.Length > 0)
            {
                var pointOnSphere = settings.noiseLayers[0].scaled ? pointOnScaledSphere : pointOnUnitSphere;
                firstLayerValue = noiseFilters[0].Evaluate(pointOnSphere);
                if (settings.noiseLayers[0].enabled)
                {
                    var scale = settings.noiseLayers[0].scaled ? Planet.SCALE : 1.0f;
                    elevation = firstLayerValue * scale;
                }
            }

            for (var i = 1; i < noiseFilters.Length; i++)
            {
                if (settings.noiseLayers[i].enabled)
                {
                    var pointOnSphere = settings.noiseLayers[0].scaled ? pointOnScaledSphere : pointOnUnitSphere;
                    var mask = settings.noiseLayers[i].useFirstLayerAsMask ? firstLayerValue : 1;
                    var scale = settings.noiseLayers[i].scaled ? Planet.SCALE : 1.0f; 
                    elevation += noiseFilters[i].Evaluate(pointOnSphere) * mask * scale;
                }
            }
            return elevation - 0.05f;
        }
    }
}