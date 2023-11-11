using System;
using Noise.Api;
using UnityEngine;

namespace Terrain.Mesh
{
    public class SmoothTerrainMeshGenerator : IMeshGeneratorStrategy
    {

        private readonly INoiseEvaluator terrainNoiseEvaluator;
        private readonly int planetSize;

        public SmoothTerrainMeshGenerator(INoiseEvaluator terrainNoiseEvaluator, int planetSize)
        {
            this.terrainNoiseEvaluator = terrainNoiseEvaluator;
            this.planetSize = planetSize;
        }

        public MeshComputer MeshComputer()
        {
            return SmoothTerrainMesh;
        }

        private Mesh SmoothTerrainMesh(MeshGenerator.Data data, Vector3 axisA, Vector3 axisB)
        {
            var (vertices, normals) = SmoothTerrainVerticesAndNormals(data, axisA, axisB);
            return new Mesh
            {
                vertices = vertices,
                indices = IndicesLookup.Indices,
                normals = normals,
                uvs = UVLookup.ChunkUVs()
            };
        }

        private (Vector3[], Vector3[]) SmoothTerrainVerticesAndNormals(MeshGenerator.Data data, Vector3 axisA, Vector3 axisB)
        {
            var vertices = new Vector3[(MeshGenerator.CHUNK_SIZE + 1) * (MeshGenerator.CHUNK_SIZE + 1)];
            var normals = new Vector3[(MeshGenerator.CHUNK_SIZE + 1) * (MeshGenerator.CHUNK_SIZE + 1)];
            
            var stepSize = (data.chunkLength + data.chunkLength) / MeshGenerator.CHUNK_SIZE;
            var axisAOffset = (axisA * data.chunkLength);
            var axisBOffset = (axisB * data.chunkLength);
            var planetRadius = (float) (Math.Pow(2, planetSize) * Planet.SCALE);

            // vertices
            for (var y = 0; y < MeshGenerator.CHUNK_SIZE + 1; y++)
            {
                for (var x = 0; x < MeshGenerator.CHUNK_SIZE + 1; x++)
                {
                    // vertex position
                    var pointOnCube = ComputePointOnCube(x, y);
                    var pointOnUnitSphere = Vector3.Normalize(pointOnCube);
                    var pointOnSphere = pointOnUnitSphere * planetRadius;
                    var elevation = terrainNoiseEvaluator.CalculateElevation(pointOnUnitSphere, pointOnSphere);
                    var elevatedPointOnSphere = pointOnSphere * (1.0f + elevation);
                    vertices[y * (MeshGenerator.CHUNK_SIZE + 1) + x] = elevatedPointOnSphere;
                }
            }

            
            // normals
            for (var y = 0; y < MeshGenerator.CHUNK_SIZE + 1; y++)
            {
                for (var x = 0; x < MeshGenerator.CHUNK_SIZE + 1; x++)
                {
                    var elevatedPointOnSphere = vertices[y * (MeshGenerator.CHUNK_SIZE + 1) + x];
                    
                    Vector3 leftNeighbor;
                    Vector3 rightNeighbor;
                    Vector3 topNeighbor;
                    Vector3 bottomNeighbor;

                    if (x == 0)
                    {
                        var leftNeighborOnUnitSphere = Vector3.Normalize(ComputePointOnCube(x - 1, y));
                        var leftNeighborOnSphere = leftNeighborOnUnitSphere * planetRadius;
                        leftNeighbor = leftNeighborOnSphere * (1.0f + terrainNoiseEvaluator.CalculateElevation(leftNeighborOnUnitSphere, leftNeighborOnSphere));
                    }
                    else
                    {
                        leftNeighbor = vertices[y * (MeshGenerator.CHUNK_SIZE + 1) + (x - 1)];
                    }

                    if (y == 0)
                    {
                        var topNeighborOnUnitSphere = Vector3.Normalize(ComputePointOnCube(x, y - 1));
                        var topNeighborOnSphere = topNeighborOnUnitSphere * planetRadius;
                        topNeighbor = topNeighborOnSphere * (1.0f + terrainNoiseEvaluator.CalculateElevation(topNeighborOnUnitSphere, topNeighborOnSphere));
                    }
                    else
                    {
                        topNeighbor = vertices[(y-1) * (MeshGenerator.CHUNK_SIZE + 1) + x];
                    }

                    if (x == MeshGenerator.CHUNK_SIZE)
                    {
                        var rightNeighborOnUnitSphere = Vector3.Normalize(ComputePointOnCube(x + 1, y));
                        var rightNeighborOnSphere = rightNeighborOnUnitSphere * planetRadius;
                        rightNeighbor = rightNeighborOnSphere * (1.0f + terrainNoiseEvaluator.CalculateElevation(rightNeighborOnUnitSphere, rightNeighborOnSphere));
                    }
                    else
                    {
                        rightNeighbor = vertices[y * (MeshGenerator.CHUNK_SIZE + 1) + (x + 1)];
                    }

                    if (y == MeshGenerator.CHUNK_SIZE)
                    {
                        var bottomNeighborOnUnitSphere = Vector3.Normalize(ComputePointOnCube(x, y + 1));
                        var bottomNeighborOnSphere = bottomNeighborOnUnitSphere * planetRadius;
                        bottomNeighbor = bottomNeighborOnSphere * (1.0f + terrainNoiseEvaluator.CalculateElevation(bottomNeighborOnUnitSphere, bottomNeighborOnSphere));                        
                    }
                    else
                    {
                        bottomNeighbor = vertices[(y+1) * (MeshGenerator.CHUNK_SIZE + 1) + x];
                    }
                    
                    var normal = Vector3.zero;
                    normal += Vector3.Cross(topNeighbor - elevatedPointOnSphere, leftNeighbor - elevatedPointOnSphere);
                    normal += Vector3.Cross(bottomNeighbor - elevatedPointOnSphere, rightNeighbor - elevatedPointOnSphere);
                    normals[y * (MeshGenerator.CHUNK_SIZE + 1) + x] = Vector3.Normalize(normal);
                }
            }

            Vector3 ComputePointOnCube(int x, int y)
            {
                return axisA * (y * stepSize) + axisB * (x * stepSize) + data.center - axisAOffset - axisBOffset;
            }

            return (vertices, normals);
        }
    }
}