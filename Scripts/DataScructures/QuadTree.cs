using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceCraft
{

    public abstract class QuadTree<T> where T : QuadTree<T>
    {

        protected T parent { set; get; }
        protected T[] children;

        protected int level;
        protected int maxLevel;

        protected bool hasChildren;

        public QuadTree(int maxLevel, T parent) : this(0, maxLevel, parent)
        {
        }

        public QuadTree(int level, int maxLevel, T parent)
        {
            this.parent = parent;
            this.children = new T[4];
            this.level = level;
            this.maxLevel = maxLevel;
            this.hasChildren = false;
        }

        protected bool canSplit() {
            return this.level >= this.maxLevel || hasChildren;
        }
        
        protected abstract T initialize(int id, T parent);

        protected virtual void split()
        {
            if (this.level >= this.maxLevel || hasChildren) return; // TODO: prevent update function from calling into split()

            this.hasChildren = true;
            for (int i = 0; i < this.children.Length; i++)
            {
                this.children[i] = initialize(i, (T)this);
            }
            onSplit();
        }

        protected virtual void merge()
        {
            if (!hasChildren) return;

            onMerge();
            for (int i = 0; i < this.children.Length; i++)
            {
                if (this.children[i].hasChildren) {
                    this.children[i].merge();
                }
                this.children[i] = null;
            }
            this.hasChildren = false;
        }

        protected abstract void onSplit();
        protected abstract void onMerge();
    }
}
