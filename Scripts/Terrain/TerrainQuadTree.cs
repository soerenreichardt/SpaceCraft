using DataStructures;
using Terrain;
using UnityEngine;

public class TerrainQuadTree : AdaptiveSpatialQuadTree<TerrainQuadTree>
{

    public GameObject terrain;
    
    private readonly Material material;

    private Vector3 planetPosition;
    private TerrainChunk terrainComponent;

    private static readonly Camera camera = Camera.main;
    static TerrainQuadTree()
    {
        camera.nearClipPlane *= Planet.SCALE;
    }

    public TerrainQuadTree(
        Vector3 planetPosition,
        float chunkLength,
        Vector3 face,
        float viewDistance,
        int maxLevel,
        Material material,
        long treeLocation
    ) : this(face * chunkLength, planetPosition, chunkLength, face, viewDistance, maxLevel, 0, null, material, treeLocation)
    {
        MeshGenerator.Data data = new MeshGenerator.Data(terrainComponent, center, face, chunkLength, isBlockLevel());
        MeshGenerator.pushData(data);
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
        long treeLocation
    ) : base(center, chunkLength, viewDistance, maxLevel, level, parent, face, treeLocation)
    {
        this.material = material;
        this.planetPosition = planetPosition;
        this.terrain = new GameObject(level.ToString());

        if (!hasChildren)
        {
            this.terrainComponent = this.terrain.AddComponent<TerrainChunk>();
            this.terrainComponent.material = material;
            this.terrainComponent.indicesFunction = getIndicesFunction();
            if (parent != null) {
                this.terrainComponent.parentMeshRenderer = parent.terrainComponent.meshRenderer;
            }
        }

        if (parent != null)
        {
            this.terrain.transform.parent = parent.terrain.transform;
            this.terrain.transform.localPosition = Vector3.zero;
        }
    }

    protected override TerrainQuadTree initialize(int quadrant)
    {
        var nextLevel = level + 1;
        var newChunkLength = chunkLength / 2;
        var newCenter = TerrainQuadTreeHelper.computeCenter(face, center, quadrant, newChunkLength);
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
            TreeLocationHelper.childTreeLocation(treeLocation, quadrant, nextLevel)
        );
    }

    protected override float distance()
    {
        if (terrainComponent.vertices == null)
        {
            return float.MaxValue;
        }
        // TODO: consider Vector3.SqrtMagnitude to avoid sqrt computation
        return Mathf.Abs(Vector3.Distance(camera.transform.position, terrainComponent.vertices[8 * (MeshGenerator.CHUNK_SIZE+1) + 8] + terrainComponent.transform.position));
    }

    protected override void adaptiveTreeOnSplit()
    {
        foreach (TerrainQuadTree child in children) {
            child.computeTerrain();
            child.computeNeighbors();
        }
    }

    protected override void adaptiveTreeOnMerge()
    {
        terrainComponent.meshRenderer.enabled = true;
        foreach (TerrainQuadTree child in children) {
            child.terrainComponent.destroy();
            child.removeNeighbors();
        }
    }

    protected override void onNeighborSet()
    {
        updateMeshIndices();
    }

    protected override void onNeighborRemoved()
    {
        updateMeshIndices();
    }

    private void computeTerrain()
    {
        MeshGenerator.Data childData = new MeshGenerator.Data(terrainComponent, center, face, chunkLength, level == maxLevel);
        MeshGenerator.pushData(childData);
    }
    
    private void updateMeshIndices()
    {
        if (!isBlockLevel())
        {
            terrainComponent.updatedMesh = true;
        }
    }

    private bool isBlockLevel()
    {
        return level == maxLevel;
    }
    
    private TerrainChunk.IndicesFunction getIndicesFunction()
    {
        if (isBlockLevel())
        {
            return indices => indices;
        }

        return _ => IndicesLookup.get(neighbors);
    }
}