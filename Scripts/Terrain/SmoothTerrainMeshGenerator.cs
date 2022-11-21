using System;
using UnityEngine;

namespace Terrain
{
    public class SmoothTerrainMeshGenerator : MeshGeneratorStrategy
    {

        private readonly INoiseEvaluator terrainNoiseEvaluator;
        private readonly int planetSize;

        public SmoothTerrainMeshGenerator(INoiseEvaluator terrainNoiseEvaluator, int planetSize)
        {
            this.terrainNoiseEvaluator = terrainNoiseEvaluator;
            this.planetSize = planetSize;
        }

        public MeshComputer meshComputer()
        {
            return smoothTerrainMesh;
        }

        private Mesh smoothTerrainMesh(MeshGenerator.Data data, Vector3 axisA, Vector3 axisB)
        {
            var (vertices, normals) = smoothTerrainVerticesAndNormals(data, axisA, axisB);
            return new Mesh()
            {
                vertices = vertices,
                indices = IndicesLookup.Indices,
                normals = normals,
                uvs = UVLookup.chunkUVs()
            };
        }

        private (Vector3[], Vector3[]) smoothTerrainVerticesAndNormals(MeshGenerator.Data data, Vector3 axisA, Vector3 axisB)
        {
            Vector3[] vertices = new Vector3[(MeshGenerator.CHUNK_SIZE + 1) * (MeshGenerator.CHUNK_SIZE + 1)];
            Vector3[] normals = new Vector3[(MeshGenerator.CHUNK_SIZE + 1) * (MeshGenerator.CHUNK_SIZE + 1)];
            
            float stepSize = (data.chunkLength + data.chunkLength) / MeshGenerator.CHUNK_SIZE;
            var axisAOffset = (axisA * data.chunkLength);
            var axisBOffset = (axisB * data.chunkLength);
            var planetRadius = (float) (Math.Pow(2, planetSize) * Planet.SCALE);
        
            for (int y = 0; y < MeshGenerator.CHUNK_SIZE + 1; y++)
            {
                for (int x = 0; x < MeshGenerator.CHUNK_SIZE + 1; x++)
                {
                    // vertex position
                    var pointOnCube = ComputePointOnCube(x, y);
                    var pointOnUnitSphere = Vector3.Normalize(pointOnCube);
                    var pointOnSphere = pointOnUnitSphere * planetRadius;
                    var elevation = terrainNoiseEvaluator.CalculateElevation(pointOnUnitSphere, pointOnSphere);
                    var elevatedPointOnSphere = pointOnSphere * (1.0f + elevation);
                    vertices[y * (MeshGenerator.CHUNK_SIZE + 1) + x] = elevatedPointOnSphere;

                    // normals
                    Vector3 leftNeighbor;
                    Vector3 topNeighbor;
                    if (x > 0 && y > 0)
                    {
                        leftNeighbor = vertices[y * (MeshGenerator.CHUNK_SIZE + 1) + (x-1)];
                        topNeighbor = vertices[(y-1) * (MeshGenerator.CHUNK_SIZE + 1) + x];
                    }
                    else
                    {
                        var leftNeighborOnUnitSphere = Vector3.Normalize(ComputePointOnCube(x - 1, y));
                        var leftNeighborOnSphere = leftNeighborOnUnitSphere * planetRadius;
                        leftNeighbor = leftNeighborOnSphere * (1.0f + terrainNoiseEvaluator.CalculateElevation(leftNeighborOnUnitSphere, leftNeighborOnSphere));

                        var topNeighborOnUnitSphere = Vector3.Normalize(ComputePointOnCube(x, y - 1));
                        var topNeighborOnSphere = topNeighborOnUnitSphere * planetRadius;
                        topNeighbor = topNeighborOnSphere * (1.0f + terrainNoiseEvaluator.CalculateElevation(topNeighborOnUnitSphere, topNeighborOnSphere));
                    }
                    normals[y * (MeshGenerator.CHUNK_SIZE + 1) + x] += Vector3.Cross(topNeighbor - elevatedPointOnSphere, leftNeighbor - elevatedPointOnSphere);

                    var rightNeighborOnUnitSphere = Vector3.Normalize(ComputePointOnCube(x + 1, y));
                    var rightNeighborOnSphere = rightNeighborOnUnitSphere * planetRadius;
                    var rightNeighbor = rightNeighborOnSphere * (1.0f + terrainNoiseEvaluator.CalculateElevation(rightNeighborOnUnitSphere, rightNeighborOnSphere));

                    var bottomNeighborOnUnitSphere = Vector3.Normalize(ComputePointOnCube(x, y + 1));
                    var bottomNeighborOnSphere = bottomNeighborOnUnitSphere * planetRadius;
                    var bottomNeighbor = bottomNeighborOnSphere * (1.0f + terrainNoiseEvaluator.CalculateElevation(bottomNeighborOnUnitSphere, bottomNeighborOnSphere));
                    normals[y * (MeshGenerator.CHUNK_SIZE + 1) + x] += Vector3.Cross(bottomNeighbor - elevatedPointOnSphere, rightNeighbor - elevatedPointOnSphere);
                }
            }

            Vector3 ComputePointOnCube(int x, int y)
            {
                return axisA * (y * stepSize) + axisB * (x * stepSize) + data.center - axisAOffset - axisBOffset;
            }

            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = Vector3.Normalize(normals[i]);
            }
            
            return (vertices, normals);
        }
    }
}