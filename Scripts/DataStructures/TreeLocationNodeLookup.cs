using System;
using SpaceCraft;
using UnityEngine;
using UnityEngine.Assertions;

namespace DataStructures
{
    public static class TreeLocationNodeLookup
    {
        
        public static T findLeftNeighbor<T>(T currentNode) where T : QuadTree<T>
        {
            var leftNeighborLocation = TreeLocationHelper.leftNeighborLocation(currentNode.treeLocation, currentNode.level);
            return findNode(currentNode, leftNeighborLocation);
        }
        
        // TODO set reverse neighbors
        public static T findRightNeighbor<T>(T currentNode) where T : QuadTree<T>
        {
            var rightNeighborLocation = TreeLocationHelper.rightNeighborLocation(currentNode.treeLocation, currentNode.level);
            return findNode(currentNode, rightNeighborLocation);
        }
        
        public static T findTopNeighbor<T>(T currentNode) where T : QuadTree<T>
        {
            var topNeighborLocation = TreeLocationHelper.topNeighborLocation(currentNode.treeLocation, currentNode.level);
            return findNode(currentNode, topNeighborLocation);
        }
        
        public static T findBottomNeighbor<T>(T currentNode) where T : QuadTree<T>
        {
            var bottomNeighborLocation = TreeLocationHelper.bottomNeighborLocation(currentNode.treeLocation, currentNode.level);
            return findNode(currentNode, bottomNeighborLocation);
        }
        
        public static T findNode<T>(T currentNode, long targetTreeLocation) where T : QuadTree<T>
        {
            if (targetTreeLocation == TreeLocationHelper.NO_NEIGHBOR_FOUND || TreeLocationHelper.quadrantForLevel(targetTreeLocation, 0) != 0)
            {
                // TODO: lookup neighboring planet face
                return null;
            }
            var currentNodeTreeLocation = currentNode.treeLocation;
            var currentNodeLevel = currentNode.level;
            var commonPathLength = TreeLocationHelper.commonPathLength(
                currentNodeTreeLocation,
                targetTreeLocation,
                currentNodeLevel
            );

            T ancestorNode = currentNode.parent;
            for (int i = currentNodeLevel; i > commonPathLength; i--)
            {
                ancestorNode = ancestorNode.parent;
            }

            T node = ancestorNode;
            for (int level = ancestorNode.level; level < currentNodeLevel; level++)
            {
                if (!node.hasChildren) return null;
                var quadrantForLevel = TreeLocationHelper.quadrantForLevel(targetTreeLocation, level + 1);
                node = node.children[quadrantForLevel];
                Assert.IsTrue(Convert.ToString(targetTreeLocation, 2).Contains(Convert.ToString(node.treeLocation, 2)));
            }

            // if (node.treeLocation != targetTreeLocation)
            // {
            //     Debug.Log("" + Convert.ToString(currentNode.treeLocation, 2) + " | " + Convert.ToString(targetTreeLocation, 2) + " ^^ " + Convert.ToString(ancestorNode.treeLocation, 2));
            //     Debug.Log(Convert.ToString(targetTreeLocation, 2) + " | " + Convert.ToString(node.treeLocation, 2));
            // }
            return node;
        }
    }
}