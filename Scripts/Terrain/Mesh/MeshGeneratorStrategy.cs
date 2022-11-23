using UnityEngine;

namespace Terrain.Mesh
{
    public delegate Mesh MeshComputer(MeshGenerator.Data data, Vector3 axisA, Vector3 axisB);
    
    public interface IMeshGeneratorStrategy
    {
        MeshComputer MeshComputer();
    }

    public struct Mesh
    {
        public Vector3[] vertices;
        public int[] indices;
        public Vector3[] normals;
        public Vector2[] uvs;
    }
}