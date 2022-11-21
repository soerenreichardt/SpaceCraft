using UnityEngine;

namespace Terrain.Color
{
    [CreateAssetMenu]
    public class ColorSettings : ScriptableObject
    {
        public Material material;
        public Gradient gradient;
    }
}
