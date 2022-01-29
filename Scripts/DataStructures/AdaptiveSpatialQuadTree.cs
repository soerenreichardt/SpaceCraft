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

    private void setNeighbor(int direction, T neighbor)
    {
        neighbors[direction] = neighbor;
        onNeighborSet();
    }

    private void removeNeighbor(int direction)
    {
        neighbors[direction] = null;
        onNeighborRemoved();
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

    protected abstract void onNeighborRemoved();
    
    protected void computeNeighbors()
    {
        var (leftNeighbor, rotationLN) = TreeLocationNodeLookup.findLeftNeighbor((T) this);
        setNeighbor(Directions.LEFT, leftNeighbor);
        leftNeighbor?.setNeighbor(Directions.rotateDirection(Directions.RIGHT, rotationLN), (T) this);
        
        var (rightNeighbor, rotationRN) = TreeLocationNodeLookup.findRightNeighbor((T) this);
        setNeighbor(Directions.RIGHT, rightNeighbor);
        rightNeighbor?.setNeighbor(Directions.rotateDirection(Directions.LEFT, rotationRN), (T) this);
        
        var (topNeighbor, rotationTN) = TreeLocationNodeLookup.findTopNeighbor((T) this);
        setNeighbor(Directions.TOP, topNeighbor);
        topNeighbor?.setNeighbor(Directions.rotateDirection(Directions.BOTTOM, rotationTN), (T) this);
        
        var (bottomNeighbor, rotationBN) = TreeLocationNodeLookup.findBottomNeighbor((T) this);
        setNeighbor(Directions.BOTTOM, bottomNeighbor);
        bottomNeighbor?.setNeighbor(Directions.rotateDirection(Directions.TOP, rotationBN), (T) this);
    }

    protected void removeNeighbors()
    {
        var (leftNeighbor, rotationLN) = TreeLocationNodeLookup.findLeftNeighbor((T) this);
        leftNeighbor?.removeNeighbor(Directions.rotateDirection(Directions.RIGHT, rotationLN));
        
        var (rightNeighbor, rotationRN) = TreeLocationNodeLookup.findRightNeighbor((T) this);
        rightNeighbor?.removeNeighbor(Directions.rotateDirection(Directions.LEFT, rotationRN));
        
        var (topNeighbor, rotationTN) = TreeLocationNodeLookup.findTopNeighbor((T) this);
        topNeighbor?.removeNeighbor(Directions.rotateDirection(Directions.BOTTOM, rotationTN));
        
        var (bottomNeighbor, rotationBN) = TreeLocationNodeLookup.findBottomNeighbor((T) this);
        bottomNeighbor?.removeNeighbor(Directions.rotateDirection(Directions.TOP, rotationBN));
    }
}