using System.Collections.Generic;
using UnityEngine;
using static DataStructures.Directions;

namespace DataStructures
{
    public static class TreeLocationHelper
    {
        
        public static void updateNeighbors<T>(T[] children, T parent) where T : AdaptiveSpatialQuadTree<T>
        {
            for (var childLocation = 0; childLocation < children.Length; childLocation++)
            {
                var child = children[childLocation];

                var (leftNeighbor, leftRotation) = computeLeftNeighbor(childLocation, children, parent);
                child.setNeighbor(LEFT, leftNeighbor);
                leftNeighbor?.setNeighbor(rotateNeighborDirection(RIGHT, -leftRotation), child);

                var (topNeighbor, topRotation) = computeTopNeighbor(childLocation, children, parent);
                child.setNeighbor(TOP, topNeighbor);
                topNeighbor?.setNeighbor(rotateNeighborDirection(BOTTOM, -topRotation), child);
                
                var (rightNeighbor, rightRotation) = computeRightNeighbor(childLocation, children, parent);
                child.setNeighbor(RIGHT, rightNeighbor);
                rightNeighbor?.setNeighbor(rotateNeighborDirection(LEFT, -rightRotation), child);
                
                var (bottomNeighbor, bottomRotation) = computeBottomNeighbor(childLocation, children, parent);
                child.setNeighbor(BOTTOM, bottomNeighbor);
                bottomNeighbor?.setNeighbor(rotateNeighborDirection(TOP, -bottomRotation), child);
            }
        }

        public static void removeFromNeighbors<T>(T[] children, T parent) where T : AdaptiveSpatialQuadTree<T>
        {
            for (var childLocation = 0; childLocation < children.Length; childLocation++)
            {
                var (leftNeighbor, leftRotation) = computeLeftNeighbor(childLocation, children, parent);
                leftNeighbor?.setNeighbor(rotateNeighborDirection(RIGHT, -leftRotation), null);
            
                var (topNeighbor, topRotation) = computeTopNeighbor(childLocation, children, parent);
                topNeighbor?.setNeighbor(rotateNeighborDirection(BOTTOM, -topRotation), null);
                
                var (rightNeighbor, rightRotation) = computeRightNeighbor(childLocation, children, parent);
                rightNeighbor?.setNeighbor(rotateNeighborDirection(LEFT, -rightRotation), null);
                
                var (bottomNeighbor, bottomRotation) = computeBottomNeighbor(childLocation, children, parent);
                bottomNeighbor?.setNeighbor(rotateNeighborDirection(TOP, -bottomRotation), null);
            }
        }
        
        private static (T, int) computeLeftNeighbor<T>(int childLocation, T[] children, T parent) where T : AdaptiveSpatialQuadTree<T>
        {
            return computeAndNeighborAndDirectionRotation(childLocation, LEFT, children, parent, TOP_RIGHT, BOTTOM_RIGHT, 1);
        }

        private static (T, int) computeTopNeighbor<T>(int childLocation, T[] children, T parent) where T : AdaptiveSpatialQuadTree<T>
        {
            return computeAndNeighborAndDirectionRotation(childLocation, TOP, children, parent, BOTTOM_LEFT, BOTTOM_RIGHT, 2);
        }
        
        private static (T, int) computeRightNeighbor<T>(int childLocation, T[] children, T parent) where T : AdaptiveSpatialQuadTree<T>
        {
            return computeAndNeighborAndDirectionRotation(childLocation, RIGHT, children, parent, TOP_LEFT, BOTTOM_LEFT, -1);
        }

        private static (T, int) computeBottomNeighbor<T>(int childLocation, T[] children, T parent) where T : AdaptiveSpatialQuadTree<T>
        {
            return computeAndNeighborAndDirectionRotation(childLocation, BOTTOM, children, parent, TOP_LEFT, TOP_RIGHT, -2);
        }
        
        private static (T, int) computeAndNeighborAndDirectionRotation<T>(
            int childLocation,
            int neighborDirection,
            T[] children,
            T parent,
            int firstDirectNeighbor,
            int secondDirectNeighbor,
            int offset
        ) where T : AdaptiveSpatialQuadTree<T>
        {
            if (childLocation == firstDirectNeighbor || childLocation == secondDirectNeighbor)
            {
                return (children[childLocation - offset], 0);
            }

            var parentNeighbor = parent.neighbors[neighborDirection];
            if (parentNeighbor != null && parentNeighbor.hasChildren)
            {
                var (rotatedNeighborPosition, rotation) = TreeLocationHelper.rotatedNeighborPosition(parent, parentNeighbor, childLocation, offset);
                return (parentNeighbor.children[rotatedNeighborPosition], rotation);
            }

            return (null, 0);
        }

        private static (int, int) rotatedNeighborPosition<T>(
            T parent,
            T parentNeighbor,
            int childLocation,
            int offset
        ) where T : AdaptiveSpatialQuadTree<T>
        {
            var parentFace = parent.face;
            var parentNeighborFace = parentNeighbor.face;
            var rotation = rootNeighborRotationLookup[parentFace][parentNeighborFace];

            return (rotateNeighborPosition(childLocation + offset, rotation), rotation);
        }

        private static int rotateNeighborPosition(int originalPosition, int rotation)
        {
            var positiveRotation = rotation < 0
                ? 4 + rotation
                : rotation;
            var position = originalPosition;
            for (int i = 0; i < positiveRotation; i++)
            {
                switch (position)
                {
                    case TOP_LEFT:
                        position = BOTTOM_LEFT;
                        break;
                    case TOP_RIGHT:
                        position = TOP_LEFT;
                        break;
                    case BOTTOM_LEFT:
                        position = BOTTOM_RIGHT;
                        break;
                    case BOTTOM_RIGHT:
                        position = TOP_RIGHT;
                        break;
                }
            }

            return position;
        }

        private static int rotateNeighborDirection(int originalPosition, int rotation)
        {
            var positiveRotation = rotation < 0
                ? 4 + rotation
                : rotation;

            return (originalPosition + positiveRotation) % 4;
        }
        
        private static readonly Dictionary<Vector3, Dictionary<Vector3, int>> rootNeighborRotationLookup =
            new Dictionary<Vector3, Dictionary<Vector3, int>>
            {
                {
                    Vector3.left,
                    new Dictionary<Vector3, int>
                    {
                        {Vector3.up, -1},
                        {Vector3.down, -1},
                        {Vector3.forward, 0},
                        {Vector3.back, 0},
                        {Vector3.left, 0}
                    }
                },
                {
                    Vector3.right,
                    new Dictionary<Vector3, int>
                    {
                        {Vector3.up, 1},
                        {Vector3.down, 1},
                        {Vector3.forward, 0},
                        {Vector3.back, 0},
                        {Vector3.right, 0}
                    }
                },
                {
                    Vector3.back, 
                    new Dictionary<Vector3, int>
                    {
                        {Vector3.up, 0},
                        {Vector3.down, 2},
                        {Vector3.left, 0},
                        {Vector3.right, 0},
                        {Vector3.back, 0}
                    }
                },
                {
                    Vector3.forward, 
                    new Dictionary<Vector3, int>
                    {
                        {Vector3.up, 2},
                        {Vector3.down, 0},
                        {Vector3.left, 0},
                        {Vector3.right, 0},
                        {Vector3.forward, 0}
                    }
                },
                {
                    Vector3.up, 
                    new Dictionary<Vector3, int>
                    {
                        {Vector3.forward, 2},
                        {Vector3.back, 0},
                        {Vector3.left, 1},
                        {Vector3.right, -1},
                        {Vector3.up, 0}
                    }
                },
                {
                    Vector3.down, 
                    new Dictionary<Vector3, int>
                    {
                        {Vector3.forward, 0},
                        {Vector3.back, 2},
                        {Vector3.left, -1},
                        {Vector3.right, 1},
                        {Vector3.down, 0}
                    }
                }
            };
    }
}