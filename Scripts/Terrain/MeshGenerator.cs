using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;

namespace Terrain
{
    public class MeshGenerator : MonoBehaviour {

        public static readonly int BATCH_SIZE = 64;
        public static readonly int CHUNK_SIZE = 16;
        public static int dataSize;

        private static readonly SmoothTerrainMeshGenerator SmoothTerrainMeshGenerator = new SmoothTerrainMeshGenerator();
        private static readonly BlockTerrainMeshGenerator BlockTerrainMeshGenerator = new BlockTerrainMeshGenerator();
    
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

        private static readonly ConcurrentQueue<Data> Queue = new ConcurrentQueue<Data>();

        public static void pushData(Data data) {
            Queue.Enqueue(data);
            Interlocked.Increment(ref dataSize);
        }

        public static void consume() {
            int batchCounter = 0;
            ThreadPool.SetMaxThreads(4, 4);
            while(batchCounter < BATCH_SIZE || !Queue.IsEmpty) {
                ThreadPool.QueueUserWorkItem(computeMesh);
                batchCounter++;
            }
        }

        static void computeMesh(object stateInfo)
        {
            if (Queue.TryDequeue(out var data)) {
                Interlocked.Decrement(ref dataSize);

                var axisA = new Vector3(data.face.y, data.face.z, data.face.x);
                var axisB = Vector3.Cross(data.face, axisA);

                MeshGeneratorStrategy meshGeneratorStrategy =
                    data.blockLevel 
                        ? (MeshGeneratorStrategy) BlockTerrainMeshGenerator 
                        : SmoothTerrainMeshGenerator;

                data.terrainChunk.vertices = meshGeneratorStrategy.verticesGenerator()(data, axisA, axisB);
                data.terrainChunk.indices = meshGeneratorStrategy.indicesGenerator()();
                data.terrainChunk.updatedMesh = true;
            }
        }

        public static float elevation(Vector3 pointOnSphere)
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

        void LateUpdate() {
            consume();
        }
    }
}