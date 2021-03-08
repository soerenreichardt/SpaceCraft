using System;
using System.Collections.Generic;
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
            return new Mesh()
            {
                vertices = smoothTerrainVertices(data, axisA, axisB),
                indices = smoothTerrainIndices()
            };
        }

        private static Vector3[] smoothTerrainVertices(MeshGenerator.Data data, Vector3 axisA, Vector3 axisB)
        {
            Vector3[] vertices = new Vector3[(MeshGenerator.CHUNK_SIZE + 1) * (MeshGenerator.CHUNK_SIZE + 1)];
            float stepSize = (data.chunkLength + data.chunkLength) / MeshGenerator.CHUNK_SIZE;
            var axisAOffset = (axisA * data.chunkLength);
            var axisBOffset = (axisB * data.chunkLength);
            var planetRadius = (float) (Math.Pow(2, Planet.PLANET_SIZE) * Planet.SCALE);
        
            for (int y = 0; y < MeshGenerator.CHUNK_SIZE + 1; y++)
            {
                for (int x = 0; x < MeshGenerator.CHUNK_SIZE + 1; x++)
                {
                    Vector3 pointOnCube = axisA * (y * stepSize) + axisB * (x * stepSize) + data.center - axisAOffset - axisBOffset;
                    Vector3 pointOnSphere = Vector3.Normalize(pointOnCube) * planetRadius;
                    vertices[y * (MeshGenerator.CHUNK_SIZE + 1) + x] = pointOnSphere + Vector3.Normalize(pointOnSphere) * MeshGenerator.elevation(pointOnSphere);
                }
            }

            return vertices;
        }
        
        private static int[] smoothTerrainIndices()
        {
            int[] indices = new int[MeshGenerator.CHUNK_SIZE * MeshGenerator.CHUNK_SIZE * 6];
            for (int y = 0; y < MeshGenerator.CHUNK_SIZE; y++)
            {
                for (int x = 0; x < MeshGenerator.CHUNK_SIZE; x++)
                {
                    indices[(x + MeshGenerator.CHUNK_SIZE * y) * 6 + 0] = x + 0 + 17 * (y + 0);
                    indices[(x + MeshGenerator.CHUNK_SIZE * y) * 6 + 1] = x + 0 + 17 * (y + 1);
                    indices[(x + MeshGenerator.CHUNK_SIZE * y) * 6 + 2] = x + 1 + 17 * (y + 0);

                    indices[(x + MeshGenerator.CHUNK_SIZE * y) * 6 + 3] = x + 1 + 17 * (y + 0);
                    indices[(x + MeshGenerator.CHUNK_SIZE * y) * 6 + 4] = x + 0 + 17 * (y + 1);
                    indices[(x + MeshGenerator.CHUNK_SIZE * y) * 6 + 5] = x + 1 + 17 * (y + 1);
                }
            }

            return indices;
        }
    }
}