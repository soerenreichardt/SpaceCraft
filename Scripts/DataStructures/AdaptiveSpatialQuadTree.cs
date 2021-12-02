using DataStructures;
using SpaceCraft;
using UnityEngine;

public abstract class AdaptiveSpatialQuadTree<T> : QuadTree<T> 
where T : AdaptiveSpatialQuadTree<T>
{
    protected Vector3 center { get; }
    protected float chunkLength { get; }

    protected float viewDistance { get; }

    public readonly Vector3 face;

    public T[] neighbors;

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
        this.neighbors = new T[4];
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

    public void setNeighbor(int direction, T neighbor)
    {
        neighbors[direction] = neighbor;
        onNeighborSet();
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

    protected abstract void onNeighborSet();
    
    protected void computeNeighbors()
    {
        var leftNeighbor = TreeLocationNodeLookup.findLeftNeighbor((T) this);
        setNeighbor(Directions.LEFT, leftNeighbor);
        leftNeighbor?.setNeighbor(Directions.RIGHT, (T) this);
        
        var rightNeighbor = TreeLocationNodeLookup.findRightNeighbor((T) this);
        setNeighbor(Directions.RIGHT, rightNeighbor);
        rightNeighbor?.setNeighbor(Directions.LEFT, (T) this);
        
        var topNeighbor = TreeLocationNodeLookup.findTopNeighbor((T) this);
        setNeighbor(Directions.TOP, topNeighbor);
        topNeighbor?.setNeighbor(Directions.BOTTOM, (T) this);
        
        var bottomNeighbor = TreeLocationNodeLookup.findBottomNeighbor((T) this);
        setNeighbor(Directions.BOTTOM, bottomNeighbor);
        bottomNeighbor?.setNeighbor(Directions.TOP, (T) this);
    }
}