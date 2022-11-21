/*
Copyright (c) 2020 Sebastian Lague
*/

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
