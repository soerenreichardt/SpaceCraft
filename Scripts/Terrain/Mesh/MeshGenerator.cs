using System.Collections.Concurrent;
using System.Threading;
using Noise.Api;
using Terrain.Height;
using UnityEngine;

namespace Terrain.Mesh
{
    public class MeshGenerator {
        public const int CHUNK_SIZE = 16;
        private const int BATCH_SIZE = 64;

        private readonly SmoothTerrainMeshGenerator smoothTerrainMeshGenerator;
        private readonly BlockTerrainMeshGenerator blockTerrainMeshGenerator;

        private readonly ConcurrentQueue<Data> queue;

        private int dataSize;
        
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

        public MeshGenerator(INoiseEvaluator noiseEvaluator, BaseTerrainSettings terrainSettings)
        {
            this.smoothTerrainMeshGenerator = new SmoothTerrainMeshGenerator(noiseEvaluator, terrainSettings.planetSize);
            this.blockTerrainMeshGenerator = new BlockTerrainMeshGenerator(noiseEvaluator, terrainSettings.planetSize);
            this.queue = new ConcurrentQueue<Data>();
            ThreadPool.SetMaxThreads(4, 4);
        }

        public void PushData(Data data) {
            queue.Enqueue(data);
            Interlocked.Increment(ref dataSize);
        }

        public void Consume() {
            int batchCounter = 0;
            while(batchCounter < BATCH_SIZE || !queue.IsEmpty) {
                ThreadPool.QueueUserWorkItem(ComputeMesh);
                batchCounter++;
            }
        }

        private void ComputeMesh(object stateInfo)
        {
            if (queue.TryDequeue(out var data)) {
                Interlocked.Decrement(ref dataSize);

                var (axisA, axisB) = AxisLookup.GetAxisForFace(data.face);

                IMeshGeneratorStrategy meshGeneratorStrategy = data.blockLevel 
                        ? (IMeshGeneratorStrategy) blockTerrainMeshGenerator 
                        : smoothTerrainMeshGenerator;

                var mesh = meshGeneratorStrategy.MeshComputer()(data, axisA, axisB);
                data.terrainChunk.vertices = mesh.vertices;
                data.terrainChunk.normals = mesh.normals;
                data.terrainChunk.uvs = mesh.uvs;
                if (data.blockLevel) data.terrainChunk.indices = mesh.indices;
                data.terrainChunk.updatedMesh = true;
            }
        }

        private Vector3 computePointOnSphere(Vector3 vertex) {
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
    }
}