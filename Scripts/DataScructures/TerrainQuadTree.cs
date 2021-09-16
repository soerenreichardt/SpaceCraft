using System;
using Terrain;
using UnityEngine;

public class TerrainQuadTree : AdaptiveSpatialQuadTree<TerrainQuadTree>
{

    public GameObject terrain;

    private readonly Vector3 face;
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
        Material material
    ) : this(face * chunkLength, planetPosition, chunkLength, face, viewDistance, maxLevel, 0, null, 0, material)
    {
        MeshGenerator.Data data = new MeshGenerator.Data(terrainComponent, center, face, chunkLength, level == maxLevel - 1);
        MeshGenerator.pushData(data);
    }

    public TerrainQuadTree(
        Vector3 center, 
        Vector3 planetPosition,
        float chunkLength, 
        Vector3 face, 
        float viewDistance, 
        int maxLevel,
        int level,
        TerrainQuadTree parent,
        long treeLocation,
        Material material
    ) : base(center, chunkLength, viewDistance, maxLevel, level, parent, treeLocation)
    {
        this.face = face;
        this.material = material;
        this.planetPosition = planetPosition;
        this.terrain = new GameObject(level.ToString());

        if (!hasChildren)
        {
            this.terrainComponent = this.terrain.AddComponent<TerrainChunk>();
            this.terrainComponent.material = material;
            if (parent != null) {
                this.terrainComponent.parentMeshRenderer = parent.terrainComponent.meshRenderer;
            }
        }

        if (parent != null)
        {
            this.terrain.transform.parent = parent.terrain.transform;
        }
    }

    protected override TerrainQuadTree initialize(int quadrant)
    {
        Vector3 newCenter = computeCenter(quadrant);
        var quadrantTreeLocation = treeLocation * 10 + quadrant;
        return new TerrainQuadTree(
            newCenter, 
            planetPosition, 
            chunkLength / 2, 
            face, 
            viewDistance, 
            maxLevel, 
            level + 1,
            this, 
            quadrantTreeLocation, 
            material
        );
    }

    protected override float distance()
    {
        if (terrainComponent.vertices == null)
        {
            return float.MaxValue;
        }
        // TODO: consider Vector3.SqrtMagnitude to avoid sqrt computation
        return Mathf.Abs(Vector3.Distance(camera.transform.position, terrainComponent.vertices[8 * (MeshGenerator.CHUNK_SIZE+1) + 8]));
    }

    private Vector2 compute2DCenter(float anchorX, float anchorY, int id)
    {
        Vector2 result;
        float halfLength = chunkLength / 2;
        switch (id)
        {
            case TOP_LEFT:
                result = new Vector2(anchorX - halfLength, anchorY + halfLength);
                break;
            case TOP_RIGHT:
                result = new Vector2(anchorX + halfLength, anchorY + halfLength);
                break;
            case BOTTOM_LEFT:
                result = new Vector2(anchorX - halfLength, anchorY - halfLength);
                break;
            case BOTTOM_RIGHT:
                result = new Vector2(anchorX + halfLength, anchorY - halfLength);
                break;
            default:
                throw new ArgumentException("Parameter id should be within 0 and 3");
        }
        return result;
    }

    // TODO: refactor to use axis
    private Vector3 computeCenter(int quadrant)
    {
        Vector3 result;
        Vector2 centerOnPlane;
        if (face == Vector3.up || face == Vector3.down)
        {
            centerOnPlane = compute2DCenter(center.x, center.z, quadrant);
            result = new Vector3(centerOnPlane.x, center.y, centerOnPlane.y);
        } 
        else if (face == Vector3.left || face == Vector3.right)
        {
            centerOnPlane = compute2DCenter(center.y, center.z, quadrant);
            result = new Vector3(center.x, centerOnPlane.x, centerOnPlane.y);
        }
        else if (face == Vector3.forward || face == Vector3.back)
        {
            centerOnPlane = compute2DCenter(center.x, center.y, quadrant);
            result = new Vector3(centerOnPlane.x, centerOnPlane.y, center.z);
        }
        else throw new ArgumentException("Parameter face should be one of the 6 following directions: [left, right, up, down, forward, back]");
        return result;
    }

    protected override void onSplit()
    {
        foreach (TerrainQuadTree child in children) {
            MeshGenerator.Data childData = new MeshGenerator.Data(child.terrainComponent, child.center, child.face, child.chunkLength, level == maxLevel - 1);
            MeshGenerator.pushData(childData);
        }
    }

    protected override void onMerge()
    {
        terrainComponent.meshRenderer.enabled = true;
        foreach (TerrainQuadTree child in children) {
            child.terrainComponent.destroy();
        }
    }
}