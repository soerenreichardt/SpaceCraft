using UnityEngine;

namespace DataStructures
{
    public abstract class AdaptiveSpatialQuadTree<T> : QuadTree<T> 
        where T : AdaptiveSpatialQuadTree<T>
    {
        protected readonly Vector3 center;
        protected readonly float chunkLength;
        protected readonly float viewDistance;

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

        protected abstract float Distance();

        public void Update()
        {
            if (HasChildren) {
                if (Distance() > viewDistance) {
                    Merge();
                    return;
                }
                foreach (T child in children)
                {
                    child.Update();
                }
            } else {
                if (Distance() <= viewDistance) {
                    Split();
                }
            }
        }

        private void SetNeighbor(int direction, T neighbor)
        {
            neighbors[direction] = neighbor;
            OnNeighborSet();
        }

        private void RemoveNeighbor(int direction)
        {
            neighbors[direction] = null;
            OnNeighborRemoved();
        }
    
        protected override void OnSplit()
        {
            AdaptiveTreeOnSplit();
        }

        protected override void OnMerge()
        {
            AdaptiveTreeOnMerge();
        }

        protected abstract void AdaptiveTreeOnSplit();
    
        protected abstract void AdaptiveTreeOnMerge();

        protected abstract void OnNeighborSet();

        protected abstract void OnNeighborRemoved();
    
        protected void ComputeNeighbors()
        {
            var (leftNeighbor, rotationLN) = TreeLocationNodeLookup.findLeftNeighbor((T) this);
            SetNeighbor(Directions.LEFT, leftNeighbor);
            leftNeighbor?.SetNeighbor(Directions.rotateDirection(Directions.RIGHT, rotationLN), (T) this);
        
            var (rightNeighbor, rotationRN) = TreeLocationNodeLookup.findRightNeighbor((T) this);
            SetNeighbor(Directions.RIGHT, rightNeighbor);
            rightNeighbor?.SetNeighbor(Directions.rotateDirection(Directions.LEFT, rotationRN), (T) this);
        
            var (topNeighbor, rotationTN) = TreeLocationNodeLookup.findTopNeighbor((T) this);
            SetNeighbor(Directions.TOP, topNeighbor);
            topNeighbor?.SetNeighbor(Directions.rotateDirection(Directions.BOTTOM, rotationTN), (T) this);
        
            var (bottomNeighbor, rotationBN) = TreeLocationNodeLookup.findBottomNeighbor((T) this);
            SetNeighbor(Directions.BOTTOM, bottomNeighbor);
            bottomNeighbor?.SetNeighbor(Directions.rotateDirection(Directions.TOP, rotationBN), (T) this);
        }

        protected void RemoveNeighbors()
        {
            var (leftNeighbor, rotationLN) = TreeLocationNodeLookup.findLeftNeighbor((T) this);
            leftNeighbor?.RemoveNeighbor(Directions.rotateDirection(Directions.RIGHT, rotationLN));
        
            var (rightNeighbor, rotationRN) = TreeLocationNodeLookup.findRightNeighbor((T) this);
            rightNeighbor?.RemoveNeighbor(Directions.rotateDirection(Directions.LEFT, rotationRN));
        
            var (topNeighbor, rotationTN) = TreeLocationNodeLookup.findTopNeighbor((T) this);
            topNeighbor?.RemoveNeighbor(Directions.rotateDirection(Directions.BOTTOM, rotationTN));
        
            var (bottomNeighbor, rotationBN) = TreeLocationNodeLookup.findBottomNeighbor((T) this);
            bottomNeighbor?.RemoveNeighbor(Directions.rotateDirection(Directions.TOP, rotationBN));
        }
    }
}