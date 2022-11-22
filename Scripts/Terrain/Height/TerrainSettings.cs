using UnityEngine;

namespace Terrain
{
    public abstract class TerrainSettings : ScriptableObject
    {
        [Range(1, 25)]
        public int planetSize = 8;
    }
}