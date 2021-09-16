using SpaceCraft;
using UnityEngine;

public abstract class AdaptiveSpatialQuadTree<T> : QuadTree<T> 
where T : AdaptiveSpatialQuadTree<T>
{

    public const int TOP = 1 << 0;
    public const int LEFT = 1 << 1;
    public const int RIGHT = 1 << 2;
    public const int BOTTOM = 1 << 3;

    public const int TOP_LEFT = TOP | LEFT;
    public const int TOP_RIGHT = TOP | RIGHT;
    public const int BOTTOM_LEFT = BOTTOM | LEFT;
    public const int BOTTOM_RIGHT = BOTTOM | RIGHT;

    public static int[] position = { TOP_LEFT, TOP_RIGHT, BOTTOM_LEFT, BOTTOM_RIGHT };

    public long treeLocation;

    public Vector3 center { get; }
    public float chunkLength { get; }

    public float viewDistance { get; }

    public AdaptiveSpatialQuadTree(
        Vector3 center, 
        float chunkLength, 
        float viewDistance, 
        int maxLevel,
        int level,
        T parent,
        long treeLocation
    ) : base(level, maxLevel, parent)
    {
        this.center = center;
        this.chunkLength = chunkLength;
        this.viewDistance = viewDistance * Mathf.Pow(2, -level) + (4 * chunkLength);
        this.treeLocation = treeLocation;
    }

    protected abstract float distance();

    public void update()
    {
        if (hasChildren) {
            if (distance() > viewDistance) {
                merge();
                return;
            }
            foreach (T child in children)
            {
                child.update();
            }
        } else {
            if (distance() <= viewDistance) {
                split();
            }
        }
    }
}