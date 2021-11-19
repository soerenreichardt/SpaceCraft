using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCraft
{

    public abstract class QuadTree<T> where T : QuadTree<T>
    {

        protected T parent { get; }

        public int level { get; }
        protected int maxLevel;

        public T[] children { get; }
        public bool hasChildren { get; private set; }

        public QuadTree(int level, int maxLevel, T parent)
        {
            this.parent = parent;
            this.children = new T[4];
            this.level = level;
            this.maxLevel = maxLevel;
            this.hasChildren = false;
        }

        protected bool cannotSplit() {
            return this.level >= this.maxLevel || hasChildren;
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
    }
}
