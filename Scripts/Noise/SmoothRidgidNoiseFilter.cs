using Noise.Api;
using UnityEngine;

namespace Noise
{
    public class SmoothRidgidNoiseFilter : INoiseFilter
    {
        private readonly NoiseSettings.RidgidNoiseSettings ridgidNoiseSettings;
        private readonly RidgidNoiseFilter ridgidNoiseFilter;

        public SmoothRidgidNoiseFilter(NoiseSettings.RidgidNoiseSettings ridgidNoiseSettings)
        {
            this.ridgidNoiseSettings = ridgidNoiseSettings;
            this.ridgidNoiseFilter = new RidgidNoiseFilter(ridgidNoiseSettings);
        }

        public float Evaluate(Vector3 point)
        {
            var sphereNormal = Vector3.Normalize(point);
            var axisA = Vector3.Cross(sphereNormal, new Vector3(0, 1, 0));
            var axisB = Vector3.Cross(sphereNormal, axisA);

            var offsetDistance = ridgidNoiseSettings.multiplier * 0.01f;
            var sample0 = ridgidNoiseFilter.Evaluate(point);
            var sample1 = ridgidNoiseFilter.Evaluate(point - axisA * offsetDistance);
            var sample2 = ridgidNoiseFilter.Evaluate(point + axisA * offsetDistance);
            var sample3 = ridgidNoiseFilter.Evaluate(point - axisB * offsetDistance);
            var sample4 = ridgidNoiseFilter.Evaluate(point + axisB * offsetDistance);
            return (sample0 + sample1 + sample2 + sample3 + sample4) / 5;
        }
    }
}
