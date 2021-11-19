using DataStructures;
using SpaceCraft;
using UnityEngine;

public abstract class AdaptiveSpatialQuadTree<T> : QuadTree<T> 
where T : AdaptiveSpatialQuadTree<T>
{
    public Vector3 center { get; }
    public float chunkLength { get; }

    public float viewDistance { get; }

    public T[] neighbors { get; set; }
    
    public readonly Vector3 face;
    
    public AdaptiveSpatialQuadTree(
        Vector3 center, 
        float chunkLength, 
        float viewDistance, 
        int maxLevel,
        int level,
        T parent,
        Vector3 face
    ) : base(level, maxLevel, parent)
    {
        this.center = center;
        this.chunkLength = chunkLength;
        this.viewDistance = viewDistance * Mathf.Pow(2, -level) + (4 * chunkLength);
        this.neighbors = new T[4];
        this.face = face;
    }

    protected abstract float distance();

    public void setNeighbor(int position, T neighbor)
    {
        neighbors[position] = neighbor;
    }

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
        updateNeighbors();
        adaptiveTreeOnSplit();
    }

    protected override void onMerge()
    {
        adaptiveTreeOnMerge();
        removeFromNeighbors();
    }

    protected abstract void adaptiveTreeOnSplit();
    
    protected abstract void adaptiveTreeOnMerge();

    private void updateNeighbors()
    {
        TreeLocationHelper.updateNeighbors(children, (T) this);
    }

    private void removeFromNeighbors()
    {
        TreeLocationHelper.removeFromNeighbors(children, (T) this);
    }
}