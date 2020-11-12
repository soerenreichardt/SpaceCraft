using System.Collections.Generic;
using UnityEngine;

public class TerrainChunk : MonoBehaviour
{

    public Mesh mesh;
    public MeshRenderer meshRenderer;
    public MeshRenderer parentMeshRenderer;
    public Material material;

    public Vector3 center;
    public TerrainQuadTree tree;

    public Vector3[] vertices { get; set; }
    public List<int> indices { get; set; }

    public bool updatedMesh { get; set; }

    public int border;

    void Start()
    {
        mesh = new Mesh();

        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = material;
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        meshFilter.mesh.RecalculateNormals();

        indices = new List<int>();
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
            mesh.triangles = indices.ToArray();
            if (parentMeshRenderer != null) {
                parentMeshRenderer.enabled = false;
            } 
        }
    }
}
