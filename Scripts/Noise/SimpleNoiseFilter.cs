/*
Copyright (c) 2018 Sebastian Lague
*/

using Noise.Api;
using UnityEngine;

namespace Noise
{
    public class SimpleNoiseFilter : INoiseFilter
    {
        private readonly NoiseSettings.SimpleNoiseSettings settings;
        private readonly SimplexNoise noise;

        public SimpleNoiseFilter(NoiseSettings.SimpleNoiseSettings settings)
        {
            this.settings = settings;
            this.noise = new SimplexNoise();
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