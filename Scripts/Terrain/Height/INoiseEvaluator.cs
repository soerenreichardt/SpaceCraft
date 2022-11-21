using UnityEngine;

namespace Terrain
{
    public interface INoiseEvaluator
    {
        float CalculateElevation(Vector3 pointOnUnitSphere, Vector3 pointOnScaledSphere);
    }
}
