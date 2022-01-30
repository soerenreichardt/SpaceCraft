namespace DataStructures
{
    public static class TreeLocationNodeLookup
    {
        
        public static (T, int) findLeftNeighbor<T>(T currentNode) where T : AdaptiveSpatialQuadTree<T>
        {
            var leftNeighborLocation = TreeLocationHelper.leftNeighborLocation(currentNode.treeLocation, currentNode.level);
            return findNode(currentNode, leftNeighborLocation, Directions.LEFT);
        }
        
        public static (T, int) findRightNeighbor<T>(T currentNode) where T : AdaptiveSpatialQuadTree<T>
        {
            var rightNeighborLocation = TreeLocationHelper.rightNeighborLocation(currentNode.treeLocation, currentNode.level);
            return findNode(currentNode, rightNeighborLocation, Directions.RIGHT);
        }
        
        public static (T, int) findTopNeighbor<T>(T currentNode) where T : AdaptiveSpatialQuadTree<T>
        {
            var topNeighborLocation = TreeLocationHelper.topNeighborLocation(currentNode.treeLocation, currentNode.level);
            return findNode(currentNode, topNeighborLocation, Directions.TOP);
        }
        
        public static (T, int) findBottomNeighbor<T>(T currentNode) where T : AdaptiveSpatialQuadTree<T>
        {
            var bottomNeighborLocation = TreeLocationHelper.bottomNeighborLocation(currentNode.treeLocation, currentNode.level);
            return findNode(currentNode, bottomNeighborLocation, Directions.BOTTOM);
        }

        private static (T, int) findNode<T>(T currentNode, long targetTreeLocation, int targetDirection) where T : AdaptiveSpatialQuadTree<T>
        {
            T ancestorNode;
            int rotation;
            if (TreeLocationHelper.switchPlanetFace(targetTreeLocation))
            {
                ancestorNode = findTreeRootNeighbor(currentNode, targetTreeLocation);
                rotation = PlanetFaceSwitchLookup.rotationForNeighborOfFace(currentNode.face, targetDirection);
            }
            else
            {
                ancestorNode = findFirstCommonAncestor(currentNode, targetTreeLocation);
                rotation = 0;
            }

            var currentNodeLevel = currentNode.level;
            T node = ancestorNode;
            for (int level = ancestorNode.level; level < currentNodeLevel; level++)
            {
                // This code can get executed while unity is in the process of deleting
                // the `node` object. This is why I need to check for null here.
                if (node == null || !node.hasChildren) return (null, 0);
                var quadrantForNextLevel = Directions.rotateQuadrant(TreeLocationHelper.quadrantForLevel(targetTreeLocation, level + 1), rotation);
                node = node.children[quadrantForNextLevel];
            }

            return (node, rotation);
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