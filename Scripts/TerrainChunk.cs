using UnityEngine;

public class TerrainChunk : MonoBehaviour
{

    public Mesh mesh;
    public MeshRenderer meshRenderer;
    public MeshRenderer parentMeshRenderer;
    public Material material;
    
    public Vector3[] vertices { get; set; }
    public int[] indices { get; set; }

    public bool updatedMesh { get; set; }

    public TerrainChunk leftNeighbor;
    public TerrainChunk topNeighbor;
    public TerrainChunk rightNeighbor;
    public TerrainChunk bottomNeighbor;
    
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
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = indices;
            if (parentMeshRenderer != null) {
                parentMeshRenderer.enabled = false;
            } 
        }
    }
}
