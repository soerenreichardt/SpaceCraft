namespace DataStructures
{

    public abstract class QuadTree<T> where T : QuadTree<T>
    {
        public readonly T parent;
        public readonly T[] children;

        public readonly long treeLocation;
        public readonly int level;
        protected readonly int maxLevel;

        public bool HasChildren { get; private set; }

        protected QuadTree(int level, int maxLevel, T parent, long treeLocation)
        {
            this.parent = parent;
            this.children = new T[4];
            this.level = level;
            this.maxLevel = maxLevel;
            this.treeLocation = treeLocation;
            this.HasChildren = false;
        }

        protected abstract T Initialize(int quadrant);

        protected void Split()
        {
            if (CannotSplit()) return;

            this.HasChildren = true;
            for (int quadrant = 0; quadrant < this.children.Length; quadrant++)
            {
                this.children[quadrant] = Initialize(quadrant);
            }
            OnSplit();
        }

        protected void Merge()
        {
            if (!HasChildren) return;

            OnMerge();
            for (int i = 0; i < this.children.Length; i++)
            {
                this.children[i].Merge();
                this.children[i] = null;
            }
            this.HasChildren = false;
        }

        protected abstract void OnSplit();
        protected abstract void OnMerge();
        
        private bool CannotSplit() {
            return this.level >= this.maxLevel || HasChildren;
        }
    }
}
