using UnityEngine;

namespace Terrain
{
    public class TerrainChunk : MonoBehaviour
    {

        public delegate int[] IndicesFunction(int[] currentIndices);
    
        public UnityEngine.Mesh mesh;
        public MeshRenderer meshRenderer;
        public MeshRenderer parentMeshRenderer;
        public Material material;
        public MeshCollider meshCollider;
    
        public Vector3[] vertices { get; set; }
        public Vector3[] normals { get; set; }
    
        public Vector2[] uvs { get; set; }
    
        public int[] indices { get; set; }
        public IndicesFunction indicesFunction { get; set; }

        public bool updatedMesh { get; set; }
        internal bool updateTriangles { get; set; }

        void Start()
        {
            mesh = new UnityEngine.Mesh();

            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = material;
            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
        }

        public void Destroy() {
            Destroy(gameObject);
        }

        void Update()
        {
            if (updatedMesh) {
                updatedMesh = false;
                mesh.vertices = vertices;
                mesh.triangles = indicesFunction(indices);
                mesh.normals = normals;
                mesh.uv = uvs;
                updateTriangles = true;
                if (parentMeshRenderer != null) {
                    parentMeshRenderer.enabled = false;
                }

                if (meshCollider != null)
                {
                    meshCollider.sharedMesh = mesh;
                }
            }
        }

        public void UpdateTriangles()
        {
            if (updateTriangles)
            {
                mesh.triangles = indicesFunction(indices);                
            }
        }
    }
}
