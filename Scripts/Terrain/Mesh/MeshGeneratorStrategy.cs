using UnityEngine;

namespace Terrain
{
    public delegate Mesh MeshComputer(MeshGenerator.Data data, Vector3 axisA, Vector3 axisB);
    
    public interface MeshGeneratorStrategy
    {
        MeshComputer meshComputer();
    }

    public struct Mesh
    {
        public Vector3[] vertices;
        public int[] indices;
        public Vector3[] normals;
        public Vector2[] uvs;
    }
}