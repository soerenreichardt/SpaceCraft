namespace SpaceCraft
{

    public abstract class QuadTree<T> where T : QuadTree<T>
    {
        private T parent { get; }

        public readonly long treeLocation;
        
        protected int level { get; }
        protected readonly int maxLevel;

        protected T[] children { get; }
        protected bool hasChildren { get; private set; }

        protected QuadTree(int level, int maxLevel, T parent, long treeLocation)
        {
            this.parent = parent;
            this.children = new T[4];
            this.level = level;
            this.maxLevel = maxLevel;
            this.treeLocation = treeLocation;
            this.hasChildren = false;
        }

        protected abstract T initialize(int quadrant);

        protected void split()
        {
            if (cannotSplit()) return;

            this.hasChildren = true;
            for (int quadrant = 0; quadrant < this.children.Length; quadrant++)
            {
                this.children[quadrant] = initialize(quadrant);
            }
            onSplit();
        }

        protected void merge()
        {
            if (!hasChildren) return;

            onMerge();
            for (int i = 0; i < this.children.Length; i++)
            {
                this.children[i].merge();
                this.children[i] = null;
            }
            this.hasChildren = false;
        }

        protected abstract void onSplit();
        protected abstract void onMerge();
        
        private bool cannotSplit() {
            return this.level >= this.maxLevel || hasChildren;
        }
    }
}
