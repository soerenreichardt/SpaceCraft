/*
Copyright (c) 2018 Sebastian Lague
*/

namespace Noise.Api
{
    public static class NoiseFilterFactory
    {
        public static INoiseFilter CreateNoiseFilter(NoiseSettings settings)
        {
            return settings.filterType switch
            {
                NoiseSettings.FilterType.Simple => new SimpleNoiseFilter(settings.simpleNoiseSettings),
                NoiseSettings.FilterType.Ridgid => new RidgidNoiseFilter(settings.ridgidNoiseSettings),
                NoiseSettings.FilterType.Continent => new ContinentNoiseFilter(settings.continentNoiseSettings),
                NoiseSettings.FilterType.SmoothRidgid =>
                    new SmoothRidgidNoiseFilter(settings.smoothRidgidNoiseSettings),
                _ => null
            };
        }
    }
}