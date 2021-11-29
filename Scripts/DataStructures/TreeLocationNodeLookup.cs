using UnityEngine.Assertions;

namespace DataStructures
{
    public static class TreeLocationNodeLookup
    {
        
        public static T findLeftNeighbor<T>(T currentNode) where T : AdaptiveSpatialQuadTree<T>
        {
            var leftNeighborLocation = TreeLocationHelper.leftNeighborLocation(currentNode.treeLocation, currentNode.level);
            return findNode(currentNode, leftNeighborLocation);
        }
        
        public static T findRightNeighbor<T>(T currentNode) where T : AdaptiveSpatialQuadTree<T>
        {
            var rightNeighborLocation = TreeLocationHelper.rightNeighborLocation(currentNode.treeLocation, currentNode.level);
            return findNode(currentNode, rightNeighborLocation);
        }
        
        public static T findTopNeighbor<T>(T currentNode) where T : AdaptiveSpatialQuadTree<T>
        {
            var topNeighborLocation = TreeLocationHelper.topNeighborLocation(currentNode.treeLocation, currentNode.level);
            return findNode(currentNode, topNeighborLocation);
        }
        
        public static T findBottomNeighbor<T>(T currentNode) where T : AdaptiveSpatialQuadTree<T>
        {
            var bottomNeighborLocation = TreeLocationHelper.bottomNeighborLocation(currentNode.treeLocation, currentNode.level);
            return findNode(currentNode, bottomNeighborLocation);
        }

        private static T findNode<T>(T currentNode, long targetTreeLocation) where T : AdaptiveSpatialQuadTree<T>
        {
            var ancestorNode = (targetTreeLocation & TreeLocationHelper.SWITCH_PLANET_FACE_MASK) != 0
                ? findTreeRootNeighbor(currentNode, targetTreeLocation)
                : findFirstCommonAncestor(currentNode, targetTreeLocation);

            if (TreeLocationHelper.quadrantForLevel(targetTreeLocation, 0) != 0)
            {
                Assert.IsTrue((targetTreeLocation & TreeLocationHelper.SWITCH_PLANET_FACE_MASK) != 0);                
            }
            
            var currentNodeLevel = currentNode.level;
            T node = ancestorNode;
            for (int level = ancestorNode.level; level < currentNodeLevel; level++)
            {
                if (!node.hasChildren) return null;
                var quadrantForNextLevel = TreeLocationHelper.quadrantForLevel(targetTreeLocation, level + 1);
                node = node.children[quadrantForNextLevel];
            }

            return node;
        }

        private static T findTreeRootNeighbor<T>(T currentNode, long targetTreeLocation)
            where T : AdaptiveSpatialQuadTree<T>
        {
            T rootNode = currentNode;
            while (rootNode.parent != null)
            {
                rootNode = rootNode.parent;
            }
         
            var neighborPosition = TreeLocationHelper.quadrantForLevel(targetTreeLocation, 0);
            return rootNode.neighbors[neighborPosition];
        }

        private static T findFirstCommonAncestor<T>(T currentNode, long targetTreeLocation) where T : AdaptiveSpatialQuadTree<T>
        {
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

            return ancestorNode;
        }
    }
}