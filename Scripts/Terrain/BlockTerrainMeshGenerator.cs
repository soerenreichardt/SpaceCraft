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

            var axisAOffset = (axisA * data.chunkLength);
            var axisBOffset = (axisB * data.chunkLength);

            float[] lastRowElevations = new float[MeshGenerator.CHUNK_SIZE];
            float lastColumnElevation = 0.0f;
            for (int y = 0; y < MeshGenerator.CHUNK_SIZE; y++)
            {
                for (int x = 0; x < MeshGenerator.CHUNK_SIZE; x++)
                {
                    var middlePointOnCube = axisA * ((y + 0.5f) * BLOCK_HEIGHT) + axisB * ((x + 0.5f) * BLOCK_HEIGHT) + data.center - axisAOffset - axisBOffset;

                    var topLeftPointOnCube = middlePointOnCube + axisA * (BLOCK_HEIGHT * 0.5f) - axisB * (BLOCK_HEIGHT * 0.5f);
                    var topRightPointOnCube = middlePointOnCube + axisA * (BLOCK_HEIGHT * 0.5f) + axisB * (BLOCK_HEIGHT * 0.5f);
                    var bottomLeftPointOnCube = middlePointOnCube - axisA * (BLOCK_HEIGHT * 0.5f) - axisB * (BLOCK_HEIGHT * 0.5f);
                    var bottomRightPointOnCube = middlePointOnCube - axisA * (BLOCK_HEIGHT * 0.5f) + axisB * (BLOCK_HEIGHT * 0.5f);

                    // TODO: reuse points in next iteration
                    var topLeftPointOnSphere = Vector3.Normalize(topLeftPointOnCube);
                    var topRightPointOnSphere = Vector3.Normalize(topRightPointOnCube);
                    var bottomLeftPointOnSphere = Vector3.Normalize(bottomLeftPointOnCube);
                    var bottomRightPointOnSphere = Vector3.Normalize(bottomRightPointOnCube);

                    var scaledTopLeftPointOnSphere = topLeftPointOnSphere * PLANET_RADIUS;
                    var scaledTopRightPointOnSphere = topRightPointOnSphere * PLANET_RADIUS;
                    var scaledBottomLeftPointOnSphere = bottomLeftPointOnSphere * PLANET_RADIUS;
                    var scaledBottomRightPointOnSphere = bottomRightPointOnSphere * PLANET_RADIUS;

                    var elevation = BlockTerrainMeshGenerator.elevation(x, y, axisA, axisB, axisAOffset, axisBOffset, data.center);
                    var elevatedTopLeft = scaledTopLeftPointOnSphere + topLeftPointOnSphere * elevation;
                    var elevatedTopRight = scaledTopRightPointOnSphere + topRightPointOnSphere * elevation;
                    var elevatedBottomLeft = scaledBottomLeftPointOnSphere + bottomLeftPointOnSphere * elevation;
                    var elevatedBottomRight = scaledBottomRightPointOnSphere + bottomRightPointOnSphere * elevation;

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
                    
                    if (y > 0)
                    {
                        var topElevation = lastRowElevations[x];
                        computeVerticalCubeFaces(
                            vertexBlockStart,
                            computeVertexBlockStart(x, y - 1),
                            elevation,
                            topElevation,
                            2,
                            3,
                            0,
                            1,
                            bottomLeftPointOnSphere,
                            bottomRightPointOnSphere,
                            vertices,
                            indices);
                    }
                    
                    if (x > 0)
                    {
                        var leftElevation = lastColumnElevation;
                        computeVerticalCubeFaces(
                            vertexBlockStart,
                            vertices.Count - 8,
                            elevation,
                            leftElevation,
                            0,
                            2,
                            1,
                            3,
                            topLeftPointOnSphere,
                            bottomLeftPointOnSphere,
                            vertices,
                            indices);
                    }

                    lastColumnElevation = elevation;
                    lastRowElevations[x] = elevation;
                }
            }

            int computeVertexBlockStart(int x, int y)
            {
                return (x + MeshGenerator.CHUNK_SIZE * y) * 4;
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
            float elevation, 
            float comparedElevation,
            int direction1, 
            int direction2,
            int adjacentDirection1,
            int adjacentDirection2,
            Vector3 topLeftRay,
            Vector3 bottomLeftRay,
            List<Vector3> vertices, 
            List<int> indices)
        {
            int vertexOffset = 0;
            if (elevation < comparedElevation || elevation > comparedElevation)
            {
                int blockDifference = (int) ((comparedElevation - elevation) / BLOCK_HEIGHT);

                int topLeftVertexId = vertexIdBlockStart + direction1;
                int bottomLeftVertexId = vertexIdBlockStart + direction2;
                var leftNeighborTopRightVertexId = neighborVertexIdBlockStart + adjacentDirection1;
                var leftNeighborBottomRightVertexId = neighborVertexIdBlockStart + adjacentDirection2;

                // for (int i = 1; i < Math.Abs(blockDifference); i++)
                // {
                //     Vector3 intermediateTopLeftVertex = vertices[topLeftVertexId] + topLeftRay * BLOCK_HEIGHT * i;
                //     Vector3 intermediateBottomLeftVertex = vertices[bottomLeftVertexId] + bottomLeftRay * (BLOCK_HEIGHT * i);
                //     vertices.Add(intermediateTopLeftVertex);
                //     vertices.Add(intermediateBottomLeftVertex);
                //     vertexOffset += 2;
                //
                //     indices.Add(topLeftVertexId);
                //     indices.Add(bottomLeftVertexId);
                //     indices.Add(vertices.Count - 1);
                //
                //     indices.Add(vertices.Count - 1);
                //     indices.Add(vertices.Count - 2);
                //     indices.Add(topLeftVertexId);
                //
                //     topLeftVertexId = vertices.Count - 2;
                //     bottomLeftVertexId = vertices.Count - 1;
                // }

                indices.Add(topLeftVertexId);
                indices.Add(bottomLeftVertexId);
                indices.Add(leftNeighborBottomRightVertexId);

                indices.Add(topLeftVertexId);
                indices.Add(leftNeighborBottomRightVertexId);
                indices.Add(leftNeighborTopRightVertexId);
            }

            return vertexOffset;
        }

        private static float elevation(int x, int y, Vector3 axisA, Vector3 axisB, Vector3 axisAOffset, Vector3 axisBOffset, Vector3 center)
        {
            var middlePointOnCube = axisA * ((y + 0.5f) * BLOCK_HEIGHT) + axisB * ((x + 0.5f) * BLOCK_HEIGHT) + center - axisAOffset - axisBOffset;
            var middlePointOnSphere = Vector3.Normalize(middlePointOnCube) * PLANET_RADIUS;
            var elevatedMiddlePointOnSphere = MeshGenerator.elevation(middlePointOnSphere);
            int numBlocks = (int) (elevatedMiddlePointOnSphere / BLOCK_HEIGHT) + 1;
            return BLOCK_HEIGHT * numBlocks;
        }
    }
}