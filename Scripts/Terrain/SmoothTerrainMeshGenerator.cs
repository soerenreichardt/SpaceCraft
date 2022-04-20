using System;
using UnityEngine;

namespace Terrain
{
    public class SmoothTerrainMeshGenerator : MeshGeneratorStrategy
    {
        public MeshComputer meshComputer()
        {
            return smoothTerrainMesh;
        }

        private static Mesh smoothTerrainMesh(MeshGenerator.Data data, Vector3 axisA, Vector3 axisB)
        {
            var (vertices, normals) = smoothTerrainVerticesAndNormals(data, axisA, axisB);
            return new Mesh()
            {
                vertices = vertices,
                indices = IndicesLookup.Indices,
                normals = normals
            };
        }

        private static (Vector3[], Vector3[]) smoothTerrainVerticesAndNormals(MeshGenerator.Data data, Vector3 axisA, Vector3 axisB)
        {
            Vector3[] vertices = new Vector3[(MeshGenerator.CHUNK_SIZE + 1) * (MeshGenerator.CHUNK_SIZE + 1)];
            Vector3[] normals = new Vector3[(MeshGenerator.CHUNK_SIZE + 1) * (MeshGenerator.CHUNK_SIZE + 1)];
            
            float stepSize = (data.chunkLength + data.chunkLength) / MeshGenerator.CHUNK_SIZE;
            var axisAOffset = (axisA * data.chunkLength);
            var axisBOffset = (axisB * data.chunkLength);
            var planetRadius = (float) (Math.Pow(2, Planet.PLANET_SIZE) * Planet.SCALE);
        
            for (int y = 0; y < MeshGenerator.CHUNK_SIZE + 1; y++)
            {
                for (int x = 0; x < MeshGenerator.CHUNK_SIZE + 1; x++)
                {
                    var pointOnCube = computePointOnCube(x, y);
                    var pointOnUnitSphere = Vector3.Normalize(pointOnCube);
                    var pointOnSphere = pointOnUnitSphere * planetRadius;
                    var elevatedPointOnSphere = pointOnSphere * (1.0f + MeshGenerator.elevation(pointOnSphere) * Planet.SCALE);
                    vertices[y * (MeshGenerator.CHUNK_SIZE + 1) + x] = elevatedPointOnSphere;

                    Vector3 leftNeighbor;
                    Vector3 topNeighbor;
                    if (x > 0 && y > 0)
                    {
                        leftNeighbor = vertices[y * (MeshGenerator.CHUNK_SIZE + 1) + (x-1)];
                        topNeighbor = vertices[(y-1) * (MeshGenerator.CHUNK_SIZE + 1) + x];
                    }
                    else
                    {
                        var leftNeighborOnSphere = Vector3.Normalize(computePointOnCube(x - 1, y)) * planetRadius;
                        leftNeighbor = leftNeighborOnSphere * (1.0f + MeshGenerator.elevation(leftNeighborOnSphere) * Planet.SCALE);
                        
                        var topNeighborOnSphere = Vector3.Normalize(computePointOnCube(x, y - 1)) * planetRadius;
                        topNeighbor = topNeighborOnSphere * (1.0f + MeshGenerator.elevation(topNeighborOnSphere) * Planet.SCALE);
                    }
                    normals[y * (MeshGenerator.CHUNK_SIZE + 1) + x] += Vector3.Cross(topNeighbor - elevatedPointOnSphere, leftNeighbor - elevatedPointOnSphere);
                    
                    var rightNeighborOnSphere = Vector3.Normalize(computePointOnCube(x + 1, y)) * planetRadius;
                    var rightNeighbor = rightNeighborOnSphere * (1.0f + MeshGenerator.elevation(rightNeighborOnSphere) * Planet.SCALE);
                    
                    var bottomNeighborOnSphere = Vector3.Normalize(computePointOnCube(x, y + 1)) * planetRadius;
                    var bottomNeighbor = bottomNeighborOnSphere * (1.0f + MeshGenerator.elevation(bottomNeighborOnSphere) * Planet.SCALE);
                    normals[y * (MeshGenerator.CHUNK_SIZE + 1) + x] += Vector3.Cross(bottomNeighbor - elevatedPointOnSphere, rightNeighbor - elevatedPointOnSphere);
                }
            }

            Vector3 computePointOnCube(int x, int y)
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