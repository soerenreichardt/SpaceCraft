using UnityEngine;

namespace Terrain.Height
{
    public abstract class BaseTerrainSettings : ScriptableObject
    {
        [Range(1, 25)]
        public int planetSize = 8;
    }
}