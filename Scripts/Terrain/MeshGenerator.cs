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
        public readonly float length;

        public Data(TerrainChunk terrainChunk, Vector3 center, Vector3 face, float length) {
            this.terrainChunk = terrainChunk;
            this.center = center;
            this.face = face;
            this.length = length;
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

            Vector3[] vertices = new Vector3[(CHUNK_SIZE+1) * (CHUNK_SIZE+1)];
            float stepSize = (data.length + data.length) / CHUNK_SIZE;
            for (int y=0; y<CHUNK_SIZE+1; y++) {
                for (int x=0; x<CHUNK_SIZE+1; x++) {
                    Vector3 pointOnCube = axisA * (y * stepSize) + axisB * (x * stepSize) + data.center - (axisA * data.length) - (axisB * data.length);
                    Vector3 pointOnSphere = Vector3.Normalize(pointOnCube) * (float)(Math.Pow(2, Planet.PLANET_SIZE) * Planet.SCALE);
                    vertices[y * (CHUNK_SIZE+1) + x] = pointOnSphere + Vector3.Normalize(pointOnSphere) * Mathf.PerlinNoise(pointOnSphere.x, pointOnSphere.y);
                }
            }

            data.terrainChunk.vertices = vertices;
            data.terrainChunk.indices = computeIndices();
            data.terrainChunk.updatedMesh = true;
        }
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

    private static List<int> computeIndices() {
        List< int > indices = new List< int >();
		for( int y=0; y<CHUNK_SIZE; y++ ) {
			for( int x=0; x<CHUNK_SIZE; x++ ) {
				indices.Add( x+0 + 17*(y+0) );
				indices.Add( x+0 + 17*(y+1) );
				indices.Add( x+1 + 17*(y+0) );
                
				indices.Add( x+1 + 17*(y+0) );
				indices.Add( x+0 + 17*(y+1) );
				indices.Add( x+1 + 17*(y+1) );
			}
		}
        return indices;
    }

    void LateUpdate() {
        consume();
    }
}