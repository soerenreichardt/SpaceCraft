﻿using UnityEngine;

namespace Terrain.Mesh
{
    public static class UVLookup
    {
        public static Vector2[] ChunkUVs()
        {
            var uvs = new Vector2[(MeshGenerator.CHUNK_SIZE + 1) * (MeshGenerator.CHUNK_SIZE + 1)];
            for (int y = 0; y < MeshGenerator.CHUNK_SIZE + 1; y++)
            {
                for (int x = 0; x < MeshGenerator.CHUNK_SIZE + 1; x++)
                {
                    var xVal = x / (float) MeshGenerator.CHUNK_SIZE;
                    var yVal = y / (float) MeshGenerator.CHUNK_SIZE;
                    uvs[y * (MeshGenerator.CHUNK_SIZE + 1) + x] = new Vector2(
                        xVal,
                        yVal
                    );
                }
            }

            return uvs;
        }
    }
}