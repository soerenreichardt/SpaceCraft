using DataStructures;
using SpaceCraft;
using UnityEngine;

public abstract class AdaptiveSpatialQuadTree<T> : QuadTree<T> 
where T : AdaptiveSpatialQuadTree<T>
{
    protected Vector3 center { get; }
    protected float chunkLength { get; }

    protected float viewDistance { get; }

    protected readonly Vector3 face;

    protected AdaptiveSpatialQuadTree(
        Vector3 center, 
        float chunkLength, 
        float viewDistance, 
        int maxLevel,
        int level,
        T parent,
        Vector3 face,
        long treeLocation
    ) : base(level, maxLevel, parent, treeLocation)
    {
        this.center = center;
        this.chunkLength = chunkLength;
        this.viewDistance = viewDistance * Mathf.Pow(2, -level) + (4 * chunkLength);
        this.face = face;
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

    protected override void onSplit()
    {
        adaptiveTreeOnSplit();
    }

    protected override void onMerge()
    {
        adaptiveTreeOnMerge();
    }

    protected abstract void adaptiveTreeOnSplit();
    
    protected abstract void adaptiveTreeOnMerge();
}