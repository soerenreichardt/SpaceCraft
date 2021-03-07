using System.Collections.Generic;
using UnityEngine;

namespace Terrain
{
    public delegate Vector3[] VerticesGenerator(MeshGenerator.Data data, Vector3 axisA, Vector3 axisB);

    public delegate List<int> IndicesGenerator();
    
    public interface MeshGeneratorStrategy
    {
        VerticesGenerator verticesGenerator();
        IndicesGenerator indicesGenerator();
    }
}