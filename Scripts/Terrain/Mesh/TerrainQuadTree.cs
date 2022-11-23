using DataStructures;
using UnityEngine;

namespace Terrain.Mesh
{
    public class TerrainQuadTree : AdaptiveSpatialQuadTree<TerrainQuadTree>
    {

        public readonly GameObject terrain;
    
        private readonly Material material;
        private readonly MeshGenerator meshGenerator;
    
        private readonly Vector3 planetPosition;
        private readonly TerrainChunk terrainComponent;

        private static readonly Camera Camera = Camera.main;
        static TerrainQuadTree()
        {
            Camera.nearClipPlane *= Planet.SCALE;
        }

        public TerrainQuadTree(
            Vector3 planetPosition,
            float chunkLength,
            Vector3 face,
            float viewDistance,
            int maxLevel,
            Material material,
            MeshGenerator meshGenerator
        ) : this(face * chunkLength, planetPosition, chunkLength, face, viewDistance, maxLevel, 0, null, material, 0, meshGenerator)
        {
            ComputeTerrain();
        }

        private TerrainQuadTree(
            Vector3 center, 
            Vector3 planetPosition,
            float chunkLength, 
            Vector3 face, 
            float viewDistance, 
            int maxLevel,
            int level,
            TerrainQuadTree parent,
            Material material,
            long treeLocation,
            MeshGenerator meshGenerator
        ) : base(center, chunkLength, viewDistance, maxLevel, level, parent, face, treeLocation)
        {
            this.material = material;
            this.planetPosition = planetPosition;
            this.terrain = new GameObject(level.ToString());
            this.meshGenerator = meshGenerator;

            if (!HasChildren)
            {
                this.terrainComponent = this.terrain.AddComponent<TerrainChunk>();
                this.terrainComponent.material = material;
                this.terrainComponent.indicesFunction = GetIndicesFunction();
                if (IsBlockLevel())
                {
                    var meshCollider = this.terrain.AddComponent<MeshCollider>();
                    this.terrainComponent.meshCollider = meshCollider;
                }
                if (parent != null) {
                    this.terrainComponent.parentMeshRenderer = parent.terrainComponent.meshRenderer;
                }
            }

            if (parent != null)
            {
                this.terrain.transform.parent = parent.terrain.transform;
                this.terrain.transform.localPosition = Vector3.zero;
                this.terrain.transform.localRotation = Quaternion.identity;
            }
        }

        public void RecomputeTerrain()
        {
            if (HasChildren)
            {
                foreach (var child in children)
                {
                    child.RecomputeTerrain();
                }            
            }
            else
            {
                ComputeTerrain();
            }
        }
    
        protected override TerrainQuadTree Initialize(int quadrant)
        {
            var nextLevel = level + 1;
            var newChunkLength = chunkLength / 2;
            var newCenter = TerrainQuadTreeHelper.ComputeCenter(face, center, quadrant, newChunkLength);
            return new TerrainQuadTree(
                newCenter, 
                planetPosition, 
                newChunkLength, 
                face, 
                viewDistance, 
                maxLevel, 
                nextLevel,
                this,
                material,
                TreeLocationHelper.ChildTreeLocation(treeLocation, quadrant, nextLevel),
                meshGenerator
            );
        }

        protected override float Distance()
        {
            if (terrainComponent.vertices == null)
            {
                return float.MaxValue;
            }
            // TODO: consider Vector3.SqrtMagnitude to avoid sqrt computation
            var terrainCenter = terrainComponent.transform.TransformPoint(terrainComponent.vertices[8 * (MeshGenerator.CHUNK_SIZE + 1) + 8]);
            return Mathf.Abs(Vector3.Distance(Camera.transform.position, terrainCenter));
        }

        protected override void AdaptiveTreeOnSplit()
        {
            foreach (TerrainQuadTree child in children) {
                child.ComputeTerrain();
                child.ComputeNeighbors();
            }
        }

        protected override void AdaptiveTreeOnMerge()
        {
            terrainComponent.meshRenderer.enabled = true;
            foreach (TerrainQuadTree child in children) {
                child.terrainComponent.Destroy();
                child.RemoveNeighbors();
            }
        }

        protected override void OnNeighborSet()
        {
            UpdateMeshIndices();
        }

        protected override void OnNeighborRemoved()
        {
            UpdateMeshIndices();
        }

        private void ComputeTerrain()
        {
            MeshGenerator.Data childData = new MeshGenerator.Data(terrainComponent, center, face, chunkLength, IsBlockLevel());
            meshGenerator.PushData(childData);
        }
    
        private void UpdateMeshIndices()
        {
            if (!IsBlockLevel())
            {
                terrainComponent.updatedMesh = true;
            }
        }

        private bool IsBlockLevel()
        {
            return level == maxLevel;
        }
    
        private TerrainChunk.IndicesFunction GetIndicesFunction()
        {
            if (IsBlockLevel())
            {
                return indices => indices;
            }

            return _ => IndicesLookup.Get(neighbors);
        }
    }
}