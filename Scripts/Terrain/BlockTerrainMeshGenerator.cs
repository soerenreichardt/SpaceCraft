using System;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain
{
    public class BlockTerrainMeshGenerator : MeshGeneratorStrategy
    {
        public VerticesGenerator verticesGenerator()
        {
            return blockTerrainVertices;
        }

        public IndicesGenerator indicesGenerator()
        {
            return blockTerrainIndices;
        }

        private static Vector3[] blockTerrainVertices(MeshGenerator.Data data, Vector3 axisA, Vector3 axisB)
        {
            List<Vector3> vertices = new List<Vector3>();
            float stepSize = (data.chunkLength + data.chunkLength) / MeshGenerator.CHUNK_SIZE;
            var axisAOffset = (axisA * data.chunkLength);
            var axisBOffset = (axisB * data.chunkLength);
            var planetRadius = (float) (Math.Pow(2, Planet.PLANET_SIZE) * Planet.SCALE);

            for (int y = 0; y < MeshGenerator.CHUNK_SIZE; y++)
            {
                for (int x = 0; x < MeshGenerator.CHUNK_SIZE; x++)
                {
                    var middlePointOnCube = axisA * ((y + 0.5f) * stepSize) + axisB * ((x + 0.5f) * stepSize) + data.center - axisAOffset - axisBOffset;
                    var middlePointOnSphere = Vector3.Normalize(middlePointOnCube) * planetRadius;

                    var topLeftPointOnCube = middlePointOnCube + axisA * (stepSize * 0.5f) - axisB * (stepSize * 0.5f);
                    var topRightPointOnCube = middlePointOnCube + axisA * (stepSize * 0.5f) + axisB * (stepSize * 0.5f);
                    var bottomLeftPointOnCube = middlePointOnCube - axisA * (stepSize * 0.5f) - axisB * (stepSize * 0.5f);
                    var bottomRightPointOnCube = middlePointOnCube - axisA * (stepSize * 0.5f) + axisB * (stepSize * 0.5f);

                    var topLeftPointOnSphere = Vector3.Normalize(topLeftPointOnCube) * planetRadius;
                    var topRightPointOnSphere = Vector3.Normalize(topRightPointOnCube) * planetRadius;
                    var bottomLeftPointOnSphere = Vector3.Normalize(bottomLeftPointOnCube) * planetRadius;
                    var bottomRightPointOnSphere = Vector3.Normalize(bottomRightPointOnCube) * planetRadius;

                    var elevatedTopLeft = topLeftPointOnSphere + Vector3.Normalize(topLeftPointOnSphere) * MeshGenerator.elevation(middlePointOnSphere);
                    var elevatedTopRight = topRightPointOnSphere + Vector3.Normalize(topRightPointOnSphere) * MeshGenerator.elevation(middlePointOnSphere);
                    var elevatedBottomLeft = bottomLeftPointOnSphere + Vector3.Normalize(bottomLeftPointOnSphere) * MeshGenerator.elevation(middlePointOnSphere);
                    var elevatedBottomRight = bottomRightPointOnSphere + Vector3.Normalize(bottomRightPointOnSphere) * MeshGenerator.elevation(middlePointOnSphere);

                    vertices.Add(elevatedTopLeft);
                    vertices.Add(elevatedTopRight);
                    vertices.Add(elevatedBottomLeft);
                    vertices.Add(elevatedBottomRight);
                }
            }

            return vertices.ToArray();
        }
        
        private static List<int> blockTerrainIndices()
        {
            List<int> indices = new List<int>();
            for (int y = 0; y < MeshGenerator.CHUNK_SIZE; y++)
            {
                for (int x = 0; x < MeshGenerator.CHUNK_SIZE; x++)
                {
                    indices.Add((x + 16 * y) * 4 + 0);
                    indices.Add((x + 16 * y) * 4 + 1);
                    indices.Add((x + 16 * y) * 4 + 2);
                
                    indices.Add((x + 16 * y) * 4 + 1);
                    indices.Add((x + 16 * y) * 4 + 3);
                    indices.Add((x + 16 * y) * 4 + 2);
                }
            }

            return indices;
        }
    }
}