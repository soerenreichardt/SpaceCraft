﻿/*
Copyright (c) 2018 Sebastian Lague
*/
namespace Noise
{
    public static class NoiseFilterFactory
    {
        public static INoiseFilter CreateNoiseFilter(NoiseSettings settings)
        {
            switch (settings.filterType)
            {
                case NoiseSettings.FilterType.Simple:
                    return new SimpleNoiseFilter(settings.simpleNoiseSettings);
                case NoiseSettings.FilterType.Ridgid:
                    return new RidgidNoiseFilter(settings.ridgidNoiseSettings);
                case NoiseSettings.FilterType.Continent:
                    return new ContinentNoiseFilter(settings.continentNoiseSettings);
                case NoiseSettings.FilterType.SmoothRidgid:
                    return new SmoothRidgidNoiseFilter(settings.smoothRidgidNoiseSettings);
            }

            return null;
        }
    }
}