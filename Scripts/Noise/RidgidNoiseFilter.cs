/*
Copyright (c) 2018 Sebastian Lague
*/

using UnityEngine;

namespace Noise
{
    public class RidgidNoiseFilter : INoiseFilter
    {
        NoiseSettings.RidgidNoiseSettings settings;
        SimplexNoise noise = new SimplexNoise();

        public RidgidNoiseFilter(NoiseSettings.RidgidNoiseSettings settings)
        {
            this.settings = settings;
        }

        public float Evaluate(Vector3 point)
        {
            float noiseSum = 0.0f;
            float frequency = settings.scale;
            float amplitude = 1;
            float ridgeWeight = 1;

            for (int i = 0; i < settings.numLayers; i++)
            {
                float noiseValue = 1-Mathf.Abs(noise.Evaluate(point * frequency + settings.offset));
                noiseValue = Mathf.Pow(Mathf.Abs(noiseValue), settings.power);
                noiseValue *= ridgeWeight;
                ridgeWeight = Mathf.Clamp01(noiseValue * settings.gain);

                noiseSum += noiseValue * amplitude;
                amplitude *= settings.persistence;
                frequency *= settings.lacunarity;
            }

            return noiseSum * settings.multiplier;
        }
    }
}