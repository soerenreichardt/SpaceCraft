using System;
using UnityEngine;
using static DataStructures.Directions;

namespace Terrain
{
    public static class TerrainQuadTreeHelper
    {
        // TODO: refactor to use axis
        internal static Vector3 computeCenter(Vector3 face, Vector3 center, int quadrant, float chunkLength)
        {
            Vector3 result;
            Vector2 centerOnPlane;
            if (face == Vector3.up || face == Vector3.down)
            {
                centerOnPlane = compute2DCenter(center.x, center.z, quadrant, face.y, chunkLength);
                result = new Vector3(centerOnPlane.x, center.y, centerOnPlane.y);
            }
            else if (face == Vector3.left || face == Vector3.right)
            {
                centerOnPlane = compute2DCenter(center.z, center.y, quadrant, face.x, chunkLength);
                result = new Vector3(center.x, centerOnPlane.y, centerOnPlane.x);
            }
            else if (face == Vector3.forward || face == Vector3.back)
            {
                centerOnPlane = compute2DCenter(center.x, center.y, quadrant, -face.z, chunkLength);
                result = new Vector3(centerOnPlane.x, centerOnPlane.y, center.z);
            }
            else
                throw new ArgumentException(
                    "Parameter face should be one of the 6 following directions: [left, right, up, down, forward, back]");

            return result;
        }

        private static Vector2 compute2DCenter(float anchorX, float anchorY, int quadrant, float sign, float chunkLength)
        {
            Vector2 result;
            switch (quadrant)
            {
                case TOP_LEFT:
                    result = new Vector2(anchorX - (chunkLength * sign), anchorY + chunkLength);
                    break;
                case TOP_RIGHT:
                    result = new Vector2(anchorX + (chunkLength * sign), anchorY + chunkLength);
                    break;
                case BOTTOM_LEFT:
                    result = new Vector2(anchorX - (chunkLength * sign), anchorY - chunkLength);
                    break;
                case BOTTOM_RIGHT:
                    result = new Vector2(anchorX + (chunkLength * sign), anchorY - chunkLength);
                    break;
                default:
                    throw new ArgumentException("Parameter quadrant should be within 0 and 3");
            }
            return result;
        }
    }
}