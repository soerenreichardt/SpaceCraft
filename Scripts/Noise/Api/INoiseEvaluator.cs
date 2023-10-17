using UnityEngine;

namespace Noise.Api
{
    public interface INoiseEvaluator
    {
        float CalculateElevation(Vector3 pointOnUnitSphere, Vector3 pointOnScaledSphere);
    }
}
