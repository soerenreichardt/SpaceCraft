using System.Collections.Generic;
using UnityEngine;

namespace Terrain
{
    public class AxisLookup
    {
        public static (Vector3, Vector3) getAxisForFace(Vector3 face)
        {
            return AxisPerFace[face];
        }
        
        private static readonly Dictionary<Vector3, (Vector3, Vector3)> AxisPerFace =
            new Dictionary<Vector3, (Vector3, Vector3)>
            {
                {Vector3.back, (Vector3.right, Vector3.down)},
                {Vector3.forward, (Vector3.left, Vector3.down)},
                {Vector3.up, (Vector3.right, Vector3.back)},
                {Vector3.down, (Vector3.left, Vector3.back)},
                {Vector3.left, (Vector3.back, Vector3.down)},
                {Vector3.right, (Vector3.forward, Vector3.down)}
            };
    }
}