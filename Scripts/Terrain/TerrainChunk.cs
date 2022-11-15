using UnityEngine;

public class TerrainChunk : MonoBehaviour
{

    public delegate int[] IndicesFunction(int[] currentIndices);
    
    public Mesh mesh;
    public MeshRenderer meshRenderer;
    public MeshRenderer parentMeshRenderer;
    public Material material;
    public MeshCollider meshCollider;
    
    public Vector3[] vertices { get; set; }
    public Vector3[] normals { get; set; }
    public int[] indices { get; set; }
    public IndicesFunction indicesFunction { get; set; }

    public bool updatedMesh { get; set; }

    void Start()
    {
        mesh = new Mesh();

        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = material;
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }

    public void destroy() {
        Destroy(gameObject);
    }

    void Update()
    {
        if (updatedMesh) {
            updatedMesh = false;
            mesh.vertices = vertices;
            mesh.triangles = indicesFunction(indices);
            mesh.normals = normals;
            if (parentMeshRenderer != null) {
                parentMeshRenderer.enabled = false;
            }

            if (meshCollider != null)
            {
                meshCollider.sharedMesh = mesh;
            }
        }
    }
}
