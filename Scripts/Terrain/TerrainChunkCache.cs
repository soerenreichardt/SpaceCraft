using System.Collections.Generic;
using Terrain.Mesh;
using UnityEngine;

namespace Terrain
{
    public class TerrainChunkCache : MonoBehaviour
    {
        private static readonly Stack<(GameObject, TerrainChunk)> cache = new Stack<(GameObject, TerrainChunk)>();

        public static (GameObject, TerrainChunk) GetOrCreate(string name, TerrainQuadTree parent, TerrainChunk.IndicesFunction indicesFunction,
            Material material, bool isBlockLevel)
        {
            TerrainChunk terrainChunk;
            GameObject terrain;
            if (cache.Count > 0)
            {
                (terrain, terrainChunk) = cache.Pop();
                terrain.name = name;
                terrain.SetActive(true);

                var meshCollider = terrain.GetComponent<MeshCollider>();
                var meshColliderExists = meshCollider == null;
                if (isBlockLevel && !meshColliderExists)
                {
                    meshCollider = terrain.AddComponent<MeshCollider>();
                    terrainChunk.meshCollider = meshCollider;
                } else if (meshColliderExists)
                {
                    Destroy(meshCollider);
                }
            }
            else
            {
                terrain = new GameObject(name);
                terrainChunk = terrain.AddComponent<TerrainChunk>();
                terrainChunk.material = material;
                if (isBlockLevel)
                {
                    var meshCollider = terrain.AddComponent<MeshCollider>();
                    terrainChunk.meshCollider = meshCollider;
                }
            }

            if (parent != null) {
                terrainChunk.parentMeshRenderer = parent.terrainComponent.meshRenderer;
                terrain.transform.parent = parent.terrain.transform;
                terrain.transform.localPosition = Vector3.zero;
                terrain.transform.localRotation = Quaternion.identity;
            }

            terrainChunk.indicesFunction = indicesFunction;
            return (terrain, terrainChunk);
        }
        
        public static void Insert(GameObject terrain, TerrainChunk terrainChunk)
        {
            terrain.SetActive(false);
            terrainChunk.mesh.Clear();
            terrainChunk.updatedMesh = false;
            terrainChunk.updateTriangles = false;
            cache.Push((terrain, terrainChunk));
        }
    }
}