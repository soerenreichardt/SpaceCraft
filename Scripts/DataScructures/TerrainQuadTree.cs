using System;
using Terrain;
using UnityEngine;

public class TerrainQuadTree : AdaptiveSpatialQuadTree<TerrainQuadTree>
{

    public GameObject terrain;

    public readonly Vector3 face;
    private readonly Material material;

    private Vector3 planetPosition;
    public TerrainChunk terrainComponent;

    private static readonly Camera camera = Camera.main;
    static TerrainQuadTree()
    {
        camera.nearClipPlane *= Planet.SCALE;
    }

    public TerrainQuadTree(
        Vector3 planetPosition,
        float length,
        Vector3 face,
        float threshold,
        int maxLevel,
        Material material
    ) : this(face * length, planetPosition, length, face, threshold, maxLevel, 0, null, 0xF, material)
    {
        MeshGenerator.Data data = new MeshGenerator.Data(terrainComponent, center, face, length, level == maxLevel - 1);
        MeshGenerator.pushData(data);
    }

    public TerrainQuadTree(
        Vector3 center, 
        Vector3 planetPosition,
        float chunkLength, 
        Vector3 face, 
        float threshold, 
        int maxLevel,
        int level,
        TerrainQuadTree parent,
        int border,
        Material material
    ) : base(center, chunkLength, threshold, maxLevel, level, parent, border)
    {
        this.face = face;
        this.material = material;
        this.planetPosition = planetPosition;
        this.terrain = new GameObject(level.ToString());

        if (!hasChildren)
        {
            this.terrainComponent = this.terrain.AddComponent<TerrainChunk>();
            this.terrainComponent.center = this.center;
            this.terrainComponent.tree = this;
            this.terrainComponent.border = border;
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

    protected override TerrainQuadTree initialize(int id, TerrainQuadTree parent)
    {
        Vector3 newCenter = computeCenter(face, position[id]);
        int newBorder = computeBorder(position[id], parent);
        return new TerrainQuadTree(newCenter, parent.planetPosition, chunkLength / 2, face, threshold, maxLevel, level + 1,
            parent, newBorder, parent.material);
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

    private int computeBorder(int id, TerrainQuadTree parent) {
        int newBorder = 0;
        if ((parent.border & TOP) != 0) {
            newBorder |= id & TOP;
        }
        if ((parent.border & LEFT) != 0) {
            newBorder |= id & LEFT;
        }
        if ((parent.border & RIGHT) != 0) {
            newBorder |= id & RIGHT;
        }
        if ((parent.border & BOTTOM) != 0) {
            newBorder |= id & BOTTOM;
        }
        return newBorder;
    }

    private Vector2 compute2DCenter(float anchorX, float anchorY, int id)
    {
        Vector2 result;
        float halfLength = (chunkLength / 2);
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
    private Vector3 computeCenter(Vector3 face, int id)
    {
        Vector3 result;
        Vector2 centerOnPlane;
        if (face == Vector3.up || face == Vector3.down)
        {
            centerOnPlane = compute2DCenter(this.center.x, this.center.z, id);
            result = new Vector3(centerOnPlane.x, this.center.y, centerOnPlane.y);
        } 
        else if (face == Vector3.left || face == Vector3.right)
        {
            centerOnPlane = compute2DCenter(this.center.y, this.center.z, id);
            result = new Vector3(this.center.x, centerOnPlane.x, centerOnPlane.y);
        }
        else if (face == Vector3.forward || face == Vector3.back)
        {
            centerOnPlane = compute2DCenter(this.center.x, this.center.y, id);
            result = new Vector3(centerOnPlane.x, centerOnPlane.y, this.center.z);
        }
        else throw new ArgumentException("Parameter face should be one of the 6 following directions: [left, right, up, down, forward, back]");
        return result;
    }

    public void debugSplit()
    {
        split();
    }

    public void debugMerge() {
        merge();
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