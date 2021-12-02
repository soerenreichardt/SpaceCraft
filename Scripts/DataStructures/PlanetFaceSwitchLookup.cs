using System.Collections.Generic;
using UnityEngine;

namespace DataStructures
{
    public static class PlanetFaceSwitchLookup
    {

        public static int rotationForNeighborOfFace(Vector3 face, int neighborDirection)
        {
            return rootNeighborRotationLookup[face][neighborDirection];
        }
        
        private static readonly Dictionary<Vector3, Dictionary<int, int>> rootNeighborRotationLookup =
            new Dictionary<Vector3, Dictionary<int, int>>
            {
                {
                    Vector3.left,
                    new Dictionary<int, int>
                    {
                        {Directions.LEFT, 0},
                        {Directions.RIGHT, 0},
                        {Directions.TOP, -1},
                        {Directions.BOTTOM, -1}
                    }
                },
                {
                    Vector3.right,
                    new Dictionary<int, int>
                    {
                        {Directions.LEFT, 0},
                        {Directions.RIGHT, 0},
                        {Directions.TOP, 1},
                        {Directions.BOTTOM, 1}
                    }
                },
                {
                    Vector3.back,
                    new Dictionary<int, int>
                    {
                        {Directions.LEFT, 0},
                        {Directions.RIGHT, 0},
                        {Directions.TOP, 0},
                        {Directions.BOTTOM, 2}
                    }
                },
                {
                    Vector3.forward,
                    new Dictionary<int, int>
                    {
                        {Directions.LEFT, 0},
                        {Directions.RIGHT, 0},
                        {Directions.TOP, 2},
                        {Directions.BOTTOM, 0}
                    }
                },
                {
                    Vector3.up,
                    new Dictionary<int, int>
                    {
                        {Directions.LEFT, 1},
                        {Directions.RIGHT, -1},
                        {Directions.TOP, 2},
                        {Directions.BOTTOM, 0}
                    }
                },
                {
                    Vector3.down,
                    new Dictionary<int, int>
                    {
                        {Directions.LEFT, -1},
                        {Directions.RIGHT, 1},
                        {Directions.TOP, 0},
                        {Directions.BOTTOM, 2}
                    }
                }
            };
    }
}