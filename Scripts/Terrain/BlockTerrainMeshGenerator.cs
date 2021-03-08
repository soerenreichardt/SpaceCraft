using System;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain
{
    public class BlockTerrainMeshGenerator : MeshGeneratorStrategy
    {
        private static readonly float PLANET_RADIUS = (float) (Math.Pow(2, Planet.PLANET_SIZE) * Planet.SCALE);
        private static readonly float BLOCK_HEIGHT = Planet.SCALE * (2.0f / MeshGenerator.CHUNK_SIZE);

        public MeshComputer meshComputer()
        {
            return blockTerrainMesh;
        }

        private static Mesh blockTerrainMesh(MeshGenerator.Data data, Vector3 axisA, Vector3 axisB)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int> indices = new List<int>();

            float stepSize = (data.chunkLength + data.chunkLength) / MeshGenerator.CHUNK_SIZE;
            var axisAOffset = (axisA * data.chunkLength);
            var axisBOffset = (axisB * data.chunkLength);

            float[,] elevations = BlockTerrainMeshGenerator.elevations(data, axisA, axisB);

            int vertexOffset = 0;
            for (int y = 0; y < MeshGenerator.CHUNK_SIZE; y++)
            {
                for (int x = 0; x < MeshGenerator.CHUNK_SIZE; x++)
                {
                    var middlePointOnCube = axisA * ((y + 0.5f) * stepSize) + axisB * ((x + 0.5f) * stepSize) +
                        data.center - axisAOffset - axisBOffset;

                    var topLeftPointOnCube = middlePointOnCube + axisA * (stepSize * 0.5f) - axisB * (stepSize * 0.5f);
                    var topRightPointOnCube = middlePointOnCube + axisA * (stepSize * 0.5f) + axisB * (stepSize * 0.5f);
                    var bottomLeftPointOnCube =
                        middlePointOnCube - axisA * (stepSize * 0.5f) - axisB * (stepSize * 0.5f);
                    var bottomRightPointOnCube =
                        middlePointOnCube - axisA * (stepSize * 0.5f) + axisB * (stepSize * 0.5f);

                    var topLeftPointOnSphere = Vector3.Normalize(topLeftPointOnCube) * PLANET_RADIUS;
                    var topRightPointOnSphere = Vector3.Normalize(topRightPointOnCube) * PLANET_RADIUS;
                    var bottomLeftPointOnSphere = Vector3.Normalize(bottomLeftPointOnCube) * PLANET_RADIUS;
                    var bottomRightPointOnSphere = Vector3.Normalize(bottomRightPointOnCube) * PLANET_RADIUS;

                    var elevation = elevations[y, x];
                    var elevatedTopLeft = topLeftPointOnSphere + Vector3.Normalize(topLeftPointOnSphere) * elevation;
                    var elevatedTopRight = topRightPointOnSphere + Vector3.Normalize(topRightPointOnSphere) * elevation;
                    var elevatedBottomLeft =
                        bottomLeftPointOnSphere + Vector3.Normalize(bottomLeftPointOnSphere) * elevation;
                    var elevatedBottomRight = bottomRightPointOnSphere +
                                              Vector3.Normalize(bottomRightPointOnSphere) * elevation;

                    vertices.Add(elevatedTopLeft);
                    vertices.Add(elevatedTopRight);
                    vertices.Add(elevatedBottomLeft);
                    vertices.Add(elevatedBottomRight);

                    indices.Add((x + 16 * y) * 4 + 0 + vertexOffset);
                    indices.Add((x + 16 * y) * 4 + 1 + vertexOffset);
                    indices.Add((x + 16 * y) * 4 + 2 + vertexOffset);

                    indices.Add((x + 16 * y) * 4 + 1 + vertexOffset);
                    indices.Add((x + 16 * y) * 4 + 3 + vertexOffset);
                    indices.Add((x + 16 * y) * 4 + 2 + vertexOffset);
                    
                    if (x > 0)
                    {
                        var leftElevation = elevations[y, x - 1];
                        vertexOffset = computeVerticalCubeFaces( x + 16 * y, (x - 1) + 16 * y, vertexOffset, elevation, leftElevation, 0, 2, 1, 3, vertices, indices);
                    }
                    
                    if (y > 0)
                    {
                        var topElevation = elevations[y - 1, x];
                        vertexOffset = computeVerticalCubeFaces(x + 16 * y, x + 16 * (y - 1), vertexOffset, elevation, topElevation, 2, 3, 0, 1, vertices, indices);
                    }
                }
            }

            return new Mesh()
            {
                vertices = vertices.ToArray(),
                indices = indices.ToArray()
            };
        }

        private static int computeVerticalCubeFaces(
            int vertexIdBlockStart, 
            int neighborVertexIdBlockStart,
            int currentVertexOffset,
            float elevation, 
            float comparedElevation,
            int direction1, 
            int direction2,
            int adjacentDirection1,
            int adjacentDirection2,
            List<Vector3> vertices, 
            List<int> indices)
        {
            int vertexOffset = currentVertexOffset;
            if (elevation < comparedElevation || elevation > comparedElevation)
            {
                int blockDifference = (int) ((comparedElevation - elevation) / BLOCK_HEIGHT);

                int topLeftVertexId = vertexIdBlockStart * 4 + direction1 + vertexOffset;
                int bottomLeftVertexId = vertexIdBlockStart * 4 + direction2 + vertexOffset;
                var leftNeighborTopRightVertexId = neighborVertexIdBlockStart * 4 + adjacentDirection1 + vertexOffset;
                var leftNeighborBottomRightVertexId = neighborVertexIdBlockStart * 4 + adjacentDirection2 + vertexOffset;
                var topLeftRay = vertices[topLeftVertexId] - vertices[leftNeighborTopRightVertexId];
                var bottomLeftRay = vertices[bottomLeftVertexId] - vertices[leftNeighborBottomRightVertexId];
                
                for (int i = 1; i < Math.Abs(blockDifference) - 1; i++)
                {
                    Vector3 intermediateTopLeftVertex = Vector3.Normalize(topLeftRay) * BLOCK_HEIGHT * i;
                    Vector3 intermediateBottomLeftVertex = Vector3.Normalize(bottomLeftRay) * BLOCK_HEIGHT * i;
                    vertices.Add(intermediateTopLeftVertex);
                    vertices.Add(intermediateBottomLeftVertex);
                    vertexOffset += 2;

                    indices.Add(topLeftVertexId);
                    indices.Add(bottomLeftVertexId);
                    indices.Add(vertices.Count - 2);

                    indices.Add(bottomLeftVertexId);
                    indices.Add(vertices.Count - 2);
                    indices.Add(vertices.Count - 1);

                    topLeftVertexId = vertices.Count - 2;
                    bottomLeftVertexId = vertices.Count - 2;
                }

                indices.Add(topLeftVertexId);
                indices.Add(bottomLeftVertexId);
                indices.Add(leftNeighborTopRightVertexId);

                indices.Add(bottomLeftVertexId);
                indices.Add(leftNeighborBottomRightVertexId);
                indices.Add(leftNeighborTopRightVertexId);
            }

            return vertexOffset;
        }
        
        private static float[,] elevations(MeshGenerator.Data data, Vector3 axisA, Vector3 axisB)
        {
            float[,] elevations = new float[MeshGenerator.CHUNK_SIZE, MeshGenerator.CHUNK_SIZE];

            float stepSize = (data.chunkLength + data.chunkLength) / MeshGenerator.CHUNK_SIZE;
            var axisAOffset = (axisA * data.chunkLength);
            var axisBOffset = (axisB * data.chunkLength);

            for (int y = 0; y < MeshGenerator.CHUNK_SIZE; y++)
            {
                for (int x = 0; x < MeshGenerator.CHUNK_SIZE; x++)
                {
                    var middlePointOnCube = axisA * ((y + 0.5f) * stepSize) + axisB * ((x + 0.5f) * stepSize) +
                        data.center - axisAOffset - axisBOffset;
                    var middlePointOnSphere = Vector3.Normalize(middlePointOnCube) * PLANET_RADIUS;
                    var elevatedMiddlePointOnSphere = MeshGenerator.elevation(middlePointOnSphere);
                    int numBlocks = (int) (elevatedMiddlePointOnSphere / BLOCK_HEIGHT);
                    elevations[y, x] = BLOCK_HEIGHT * numBlocks;
                }
            }

            return elevations;
        }
    }
}