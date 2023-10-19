using System;
using System.Collections.Generic;
using Noise.Api;
using UnityEngine;

namespace Terrain.Mesh
{
    public class BlockTerrainMeshGenerator : IMeshGeneratorStrategy
    {
        private const float BLOCK_HEIGHT = Planet.SCALE * (2.0f / MeshGenerator.CHUNK_SIZE);
        private const float HALF_BLOCK_HEIGHT = BLOCK_HEIGHT / 2.0f;

        private readonly INoiseEvaluator terrainNoiseEvaluator;
        private readonly float planetRadius;

        public BlockTerrainMeshGenerator(INoiseEvaluator terrainNoiseEvaluator, int planetSize)
        {
            this.terrainNoiseEvaluator = terrainNoiseEvaluator;
            this.planetRadius = (float) (Math.Pow(2, planetSize) * Planet.SCALE);
        }

        public MeshComputer MeshComputer()
        {
            return BlockTerrainMesh;
        }

        private Mesh BlockTerrainMesh(MeshGenerator.Data data, Vector3 axisA, Vector3 axisB)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int> indices = new List<int>();

            var axisAOffset = axisA * data.chunkLength;
            var axisBOffset = axisB * data.chunkLength;

            for (int y = 0; y < MeshGenerator.CHUNK_SIZE; y++)
            {
                for (int x = 0; x < MeshGenerator.CHUNK_SIZE; x++)
                {
                    var middlePointOnCube = axisA * ((y + 0.5f) * BLOCK_HEIGHT) + axisB * ((x + 0.5f) * BLOCK_HEIGHT) +
                        data.center - axisAOffset - axisBOffset;

                    var topLeftPointOnCube = middlePointOnCube + axisA * HALF_BLOCK_HEIGHT - axisB * HALF_BLOCK_HEIGHT;
                    var topRightPointOnCube = middlePointOnCube + axisA * HALF_BLOCK_HEIGHT + axisB * HALF_BLOCK_HEIGHT;
                    var bottomLeftPointOnCube = middlePointOnCube - axisA * HALF_BLOCK_HEIGHT - axisB * HALF_BLOCK_HEIGHT;
                    var bottomRightPointOnCube = middlePointOnCube - axisA * HALF_BLOCK_HEIGHT + axisB * HALF_BLOCK_HEIGHT;

                    var topLeftPointOnSphere = Vector3.Normalize(topLeftPointOnCube);
                    var topRightPointOnSphere = Vector3.Normalize(topRightPointOnCube);
                    var bottomLeftPointOnSphere = Vector3.Normalize(bottomLeftPointOnCube);
                    var bottomRightPointOnSphere = Vector3.Normalize(bottomRightPointOnCube);

                    var scaledTopLeftPointOnSphere = topLeftPointOnSphere * planetRadius;
                    var scaledTopRightPointOnSphere = topRightPointOnSphere * planetRadius;
                    var scaledBottomLeftPointOnSphere = bottomLeftPointOnSphere * planetRadius;
                    var scaledBottomRightPointOnSphere = bottomRightPointOnSphere * planetRadius;

                    var elevation = this.Elevation(middlePointOnCube);
                    var elevatedTopLeft = topLeftPointOnSphere * elevation;
                    var elevatedTopRight = topRightPointOnSphere * elevation;
                    var elevatedBottomLeft = bottomLeftPointOnSphere * elevation;
                    var elevatedBottomRight = bottomRightPointOnSphere * elevation;

                    vertices.Add(elevatedTopLeft);
                    vertices.Add(elevatedTopRight);
                    vertices.Add(elevatedBottomLeft);
                    vertices.Add(elevatedBottomRight);

                    int vertexBlockStart = vertices.Count - 4;

                    indices.Add(vertexBlockStart + 0);
                    indices.Add(vertexBlockStart + 1);
                    indices.Add(vertexBlockStart + 2);

                    indices.Add(vertexBlockStart + 1);
                    indices.Add(vertexBlockStart + 3);
                    indices.Add(vertexBlockStart + 2);

                }
            }

            return new Mesh()
            {
                vertices = vertices.ToArray(),
                indices = indices.ToArray()
            };
        }

        private float Elevation(Vector3 middlePointOnCube)
        {
            var middlePointOnUnitSphere = Vector3.Normalize(middlePointOnCube);
            var middlePointOnSphere = middlePointOnUnitSphere * planetRadius;
            var elevation = (1.0f + terrainNoiseEvaluator.CalculateElevation(middlePointOnUnitSphere, middlePointOnSphere)) * planetRadius;
            var numBlocks = (int) (elevation / BLOCK_HEIGHT) + 1;
            return BLOCK_HEIGHT * numBlocks;
        }
    }
}