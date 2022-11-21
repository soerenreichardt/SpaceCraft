/*
Copyright (c) 2018 Sebastian Lague
*/

using UnityEngine;

namespace Noise
{
    public class SimpleNoiseFilter : INoiseFilter
    {
        private NoiseSettings.SimpleNoiseSettings settings;
        private SimplexNoise noise = new SimplexNoise();

        public SimpleNoiseFilter(NoiseSettings.SimpleNoiseSettings settings)
        {
            this.settings = settings;
        }

        public float Evaluate(Vector3 point)
        {
            float noiseValue = 0;
            float frequency = settings.scale;
            float amplitude = 1;

            for (int i = 0; i < settings.numLayers; i++)
            {
                noiseValue = noise.Evaluate(point * frequency + settings.offset) * amplitude;
                amplitude *= settings.persistence;
                frequency *= settings.lacunarity;
            }

            return noiseValue * settings.multiplier;
        }
    }
}