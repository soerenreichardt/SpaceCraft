﻿/*
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
            float noiseValue = 0;
            float frequency = settings.baseRoughness;
            float amplitude = 1;
            float weight = 1;

            for (int i = 0; i < settings.numLayers; i++)
            {
                float v = 1-Mathf.Abs(noise.Evaluate(point * frequency + settings.centre));
                v *= v;
                v *= weight;
                weight = Mathf.Clamp01(v * settings.weightMultiplier);

                noiseValue += v * amplitude;
                frequency *= settings.roughness;
                amplitude *= settings.persistence;
            }

            noiseValue = Mathf.Max(0, noiseValue - settings.minValue);
            return noiseValue * settings.strength;
        }
    }
}