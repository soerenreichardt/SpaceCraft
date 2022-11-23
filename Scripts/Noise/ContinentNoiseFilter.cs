/*
Copyright (c) 2020 Sebastian Lague
*/

using UnityEngine;

namespace Noise
{
    public class ContinentNoiseFilter : INoiseFilter
    {
        private readonly NoiseSettings.SimpleNoiseSettings continentNoiseSettings;
        private readonly SimplexNoise noise;

        public ContinentNoiseFilter(NoiseSettings.SimpleNoiseSettings continentNoiseSettings)
        {
            this.continentNoiseSettings = continentNoiseSettings;
            this.noise = new SimplexNoise();
        }

        public float Evaluate(Vector3 point)
        {
            var frequency = continentNoiseSettings.scale;
            var offset = continentNoiseSettings.offset;
            var persistence = continentNoiseSettings.persistence;
            var lacunarity = continentNoiseSettings.lacunarity;
            var gain = 1.0f;

            var noiseSum = 0.0f;
            var amplitude = 1.0f;
            var weight = 1.0f;

            for (var i = 0; i < continentNoiseSettings.numLayers; i++)
            {
                var noiseValue = noise.Evaluate(point * frequency + offset) * 0.5f + 0.5f;
                noiseValue *= weight;
                weight = Mathf.Clamp01(noiseValue * gain);

                noiseSum += noiseValue * amplitude;
                amplitude *= persistence;
                frequency *= lacunarity;
            }

            return noiseSum - 0.5f;
        }
    }
}
