using UnityEngine;

namespace Terrain.Height
{
    public interface INoiseEvaluator
    {
        float CalculateElevation(Vector3 pointOnUnitSphere, Vector3 pointOnScaledSphere);
    }
}
