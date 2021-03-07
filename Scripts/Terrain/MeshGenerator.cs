using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MeshGenerator : MonoBehaviour {

    public static readonly int BATCH_SIZE = 64;
    public static readonly int CHUNK_SIZE = 16;
    public static int dataSize;

    public struct Data {
        public readonly TerrainChunk terrainChunk;
        public Vector3 center;
        public Vector3 face;
        public readonly float chunkLength;
        public readonly bool blockLevel;

        public Data(TerrainChunk terrainChunk, Vector3 center, Vector3 face, float chunkLength, bool blockLevel) {
            this.terrainChunk = terrainChunk;
            this.center = center;
            this.face = face;
            this.chunkLength = chunkLength;
            this.blockLevel = blockLevel;
        }
    }

    private static readonly ConcurrentQueue<Data> queue = new ConcurrentQueue<Data>();

    public static void pushData(Data data) {
        queue.Enqueue(data);
        Interlocked.Increment(ref dataSize);
    }

    public static void consume() {
        int batchCounter = 0;
        ThreadPool.SetMaxThreads(4, 4);
        while(batchCounter < BATCH_SIZE || !queue.IsEmpty) {
            ThreadPool.QueueUserWorkItem(computeMesh);
            batchCounter++;
        }
    }

    static void computeMesh(object stateInfo)
    {
        if (queue.TryDequeue(out var data)) {
            Interlocked.Decrement(ref dataSize);

            var axisA = new Vector3(data.face.y, data.face.z, data.face.x);
            var axisB = Vector3.Cross(data.face, axisA);

            if (!data.blockLevel)
            {
                var vertices = smoothTerrain(data, axisA, axisB);

                data.terrainChunk.vertices = vertices;
                data.terrainChunk.indices = computeSmoothTerrainIndices();
            }
            else
            {
                var vertices = blockTerrain(data, axisA, axisB);

                data.terrainChunk.vertices = vertices;
                data.terrainChunk.indices = computeBlockTerrainIndices();
            }

            data.terrainChunk.updatedMesh = true;
        }
    }

    private static Vector3[] smoothTerrain(Data data, Vector3 axisA, Vector3 axisB)
    {
        Vector3[] vertices = new Vector3[(CHUNK_SIZE + 1) * (CHUNK_SIZE + 1)];
        float stepSize = (data.chunkLength + data.chunkLength) / CHUNK_SIZE;
        var axisAOffset = (axisA * data.chunkLength);
        var axisBOffset = (axisB * data.chunkLength);
        var planetRadius = (float) (Math.Pow(2, Planet.PLANET_SIZE) * Planet.SCALE);
        
        for (int y = 0; y < CHUNK_SIZE + 1; y++)
        {
            for (int x = 0; x < CHUNK_SIZE + 1; x++)
            {
                Vector3 pointOnCube = axisA * (y * stepSize) + axisB * (x * stepSize) + data.center - axisAOffset - axisBOffset;
                Vector3 pointOnSphere = Vector3.Normalize(pointOnCube) * planetRadius;
                vertices[y * (CHUNK_SIZE + 1) + x] = pointOnSphere + Vector3.Normalize(pointOnSphere) * elevation(pointOnSphere);
            }
        }

        return vertices;
    }

    private static Vector3[] blockTerrain(Data data, Vector3 axisA, Vector3 axisB)
    {
        List<Vector3> vertices = new List<Vector3>();
        float stepSize = (data.chunkLength + data.chunkLength) / CHUNK_SIZE;
        var axisAOffset = (axisA * data.chunkLength);
        var axisBOffset = (axisB * data.chunkLength);
        var planetRadius = (float) (Math.Pow(2, Planet.PLANET_SIZE) * Planet.SCALE);
        
        for (int y = 0; y < CHUNK_SIZE; y++)
        {
            for (int x = 0; x < CHUNK_SIZE; x++)
            {
                var middlePointOnCube = axisA * ((y + 0.5f) * stepSize) + axisB * ((x + 0.5f) * stepSize) + data.center - axisAOffset - axisBOffset;
                var middlePointOnSphere = Vector3.Normalize(middlePointOnCube) * planetRadius;
                
                var topLeftPointOnCube = middlePointOnCube + axisA * (stepSize * 0.5f) - axisB * (stepSize * 0.5f);
                var topRightPointOnCube = middlePointOnCube + axisA * (stepSize * 0.5f) + axisB * (stepSize * 0.5f);
                var bottomLeftPointOnCube = middlePointOnCube - axisA * (stepSize * 0.5f) - axisB * (stepSize * 0.5f);
                var bottomRightPointOnCube = middlePointOnCube - axisA * (stepSize * 0.5f) + axisB * (stepSize * 0.5f);
                
                var topLeftPointOnSphere = Vector3.Normalize(topLeftPointOnCube) * planetRadius;
                var topRightPointOnSphere = Vector3.Normalize(topRightPointOnCube) * planetRadius;
                var bottomLeftPointOnSphere = Vector3.Normalize(bottomLeftPointOnCube) * planetRadius;
                var bottomRightPointOnSphere = Vector3.Normalize(bottomRightPointOnCube) * planetRadius;

                var elevatedTopLeft = topLeftPointOnSphere + Vector3.Normalize(topLeftPointOnSphere) * elevation(middlePointOnSphere);
                var elevatedTopRight = topRightPointOnSphere + Vector3.Normalize(topRightPointOnSphere) * elevation(middlePointOnSphere);
                var elevatedBottomLeft = bottomLeftPointOnSphere + Vector3.Normalize(bottomLeftPointOnSphere) * elevation(middlePointOnSphere);
                var elevatedBottomRight = bottomRightPointOnSphere + Vector3.Normalize(bottomRightPointOnSphere) * elevation(middlePointOnSphere);
                
                vertices.Add(elevatedTopLeft);
                vertices.Add(elevatedTopRight);
                vertices.Add(elevatedBottomLeft);
                vertices.Add(elevatedBottomRight);
            }
        }

        return vertices.ToArray();
    }
    
    private static float elevation(Vector3 pointOnSphere)
    {
        return Mathf.PerlinNoise(pointOnSphere.x, pointOnSphere.y);
    }

    private static Vector3 computePointOnSphere(Vector3 vertex) {
        // spherify
        float x = vertex.x * vertex.x;
        float y = vertex.y * vertex.y;
        float z = vertex.z * vertex.z;
        
        Vector3 sphereVertex;
        sphereVertex.x = vertex.x * Mathf.Sqrt( 1.0f - ( y * 0.5f ) - ( z * 0.5f ) + ( ( y * z ) / 3.0f ) );
        sphereVertex.y = vertex.y * Mathf.Sqrt( 1.0f - ( z * 0.5f ) - ( x * 0.5f ) + ( ( z * x ) / 3.0f ) );
        sphereVertex.z = vertex.z * Mathf.Sqrt( 1.0f - ( x * 0.5f ) - ( y * 0.5f ) + ( ( x * y ) / 3.0f ) );
        return sphereVertex;
    }

    private static List<int> computeSmoothTerrainIndices() {
        List<int> indices = new List<int>();
        for (int y = 0; y < CHUNK_SIZE; y++)
        {
            for (int x = 0; x < CHUNK_SIZE; x++)
            {
                indices.Add(x + 0 + 17 * (y + 0));
                indices.Add(x + 0 + 17 * (y + 1));
                indices.Add(x + 1 + 17 * (y + 0));

                indices.Add(x + 1 + 17 * (y + 0));
                indices.Add(x + 0 + 17 * (y + 1));
                indices.Add(x + 1 + 17 * (y + 1));
            }
        }
        return indices;
    }

    private static List<int> computeBlockTerrainIndices()
    {
        List<int> indices = new List<int>();
        for (int y = 0; y < CHUNK_SIZE; y++)
        {
            for (int x = 0; x < CHUNK_SIZE; x++)
            {
                indices.Add((x + 16 * y) * 4 + 0);
                indices.Add((x + 16 * y) * 4 + 1);
                indices.Add((x + 16 * y) * 4 + 2);
                
                indices.Add((x + 16 * y) * 4 + 1);
                indices.Add((x + 16 * y) * 4 + 3);
                indices.Add((x + 16 * y) * 4 + 2);
            }
        }

        return indices;
    }
    
    void LateUpdate() {
        consume();
    }
}