namespace DataStructures
{
    public static class TreeLocationHelper
    {
        public const long NO_NEIGHBOR_FOUND = -1;
        private const byte LEVEL_MASK = 0b11;
        
        public static long childTreeLocation(long currentTreeLocation, int quadrant, int level)
        {
            return currentTreeLocation | quadrant << (2 * level);
        }

        public static long quadrantForLevel(long treeLocation, int level)
        {
            var levelShift = 2 * level;
            return (treeLocation & (LEVEL_MASK << levelShift)) >> levelShift;
        }
        
        public static int commonPathLength(long treeLocation1, long treeLocation2, int nodeLevel)
        {
            for (int level = 0; level <= nodeLevel; level++)
            {
                if (quadrantForLevel(treeLocation1, level) == quadrantForLevel(treeLocation2, level))
                {
                    continue;
                }

                return level;
            }

            return 0;
        }

        public static long leftNeighborLocation(long currentTreeLocation, int quadTreeLevel)
        {
            return neighborPosition(currentTreeLocation, quadTreeLevel,
                new[] {Directions.TOP_RIGHT, Directions.BOTTOM_RIGHT}, 1);
        }

        public static long rightNeighborLocation(long currentTreeLocation, int quadTreeLevel)
        {
            return neighborPosition(currentTreeLocation, quadTreeLevel,
                new[] {Directions.TOP_LEFT, Directions.BOTTOM_LEFT}, -1);
        }

        public static long topNeighborLocation(long currentTreeLocation, int quadTreeLevel)
        {
            return neighborPosition(currentTreeLocation, quadTreeLevel,
                new[] {Directions.BOTTOM_LEFT, Directions.BOTTOM_RIGHT}, 2);
        }

        public static long bottomNeighborLocation(long currentTreeLocation, int quadTreeLevel)
        {
            return neighborPosition(currentTreeLocation, quadTreeLevel,
                new[] {Directions.TOP_LEFT, Directions.TOP_RIGHT}, -2);
        }

        private static long neighborPosition(long currentTreeLocation, int level, int[] directNeighbors, int offsetToNeighborLocation)
        {
            if (level < 0)
            {
                return NO_NEIGHBOR_FOUND;
            }
            
            long mask = LEVEL_MASK << (level * 2);
            var quadrant = quadrantForLevel(currentTreeLocation, level);
            if (quadrant == directNeighbors[0] || quadrant == directNeighbors[1])
            {
                return setLocationOnLevel(currentTreeLocation, quadrant - offsetToNeighborLocation, mask, level);
            }

            var reflectedPosition = setLocationOnLevel(currentTreeLocation, quadrant + offsetToNeighborLocation, mask, level);
            return neighborPosition(reflectedPosition, level - 1, directNeighbors, offsetToNeighborLocation);
        }

        private static long setLocationOnLevel(long treeLocation, long newLocation, long mask, int level)
        {
            var levelShift = level * 2;
            return (treeLocation & ~mask) | (newLocation << levelShift);
        }
    }
}