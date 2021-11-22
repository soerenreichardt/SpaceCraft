namespace DataStructures
{
    public static class TreeLocationHelper
    {
        public const long NO_NEIGHBOR_FOUND = -1;
        private const byte LEVEL_MASK = 0b11;
        
        public static long childTreeLocation(long currentTreeLocation, int quadrant, int currentLevel, int maxLevel)
        {
            var level = maxLevel - currentLevel;
            return currentTreeLocation | quadrant << (2 * level);
        }

        public static long computeCommonAncestor(long treeLocation1, long treeLocation2)
        {
            var highestSetBitPosition = computeHighestSetBitPosition(treeLocation1, treeLocation2);
            // align to blocks of 2 bits
            if (highestSetBitPosition % 2 == 1) highestSetBitPosition++;

            long commonAncestorMask = 0;
            for (int level = highestSetBitPosition; level > 0; level -= 2)
            {
                long levelMask = LEVEL_MASK << level;
                if ((treeLocation1 & levelMask) == (treeLocation2 & levelMask))
                {
                    commonAncestorMask |= levelMask;
                }
            }

            return treeLocation1 & commonAncestorMask;
        }

        public static long leftNeighborLocation(long currentTreeLocation, int currentLevel, int maxLevel)
        {
            return neighborPosition(currentTreeLocation, currentLevel, maxLevel,
                new[] {Directions.TOP_RIGHT, Directions.BOTTOM_RIGHT}, 1);
        }

        public static long rightNeighborLocation(long currentTreeLocation, int currentLevel, int maxLevel)
        {
            return neighborPosition(currentTreeLocation, currentLevel, maxLevel,
                new[] {Directions.TOP_LEFT, Directions.BOTTOM_LEFT}, -1);
        }

        public static long topNeighborLocation(long currentTreeLocation, int currentLevel, int maxLevel)
        {
            return neighborPosition(currentTreeLocation, currentLevel, maxLevel,
                new[] {Directions.BOTTOM_LEFT, Directions.BOTTOM_RIGHT}, 2);
        }

        public static long bottomNeighborLocation(long currentTreeLocation, int currentLevel, int maxLevel)
        {
            return neighborPosition(currentTreeLocation, currentLevel, maxLevel,
                new[] {Directions.TOP_LEFT, Directions.TOP_RIGHT}, -2);
        }

        private static long neighborPosition(long currentTreeLocation, int currentLevel, int maxLevel,
            int[] directNeighbors, int offsetToNeighborLocation)
        {
            if (currentLevel < 0)
            {
                return NO_NEIGHBOR_FOUND;
            }
            var level = (maxLevel - currentLevel) * 2;
            long mask = LEVEL_MASK << level;
            var quadrant = (currentTreeLocation & mask) >> level;
            if (quadrant == directNeighbors[0] || quadrant == directNeighbors[1])
            {
                return setLocationOnLevel(currentTreeLocation, quadrant - offsetToNeighborLocation, mask, level);
            }

            var reflectedPosition = setLocationOnLevel(currentTreeLocation, quadrant + 1, mask, level);
            return neighborPosition(reflectedPosition, currentLevel - offsetToNeighborLocation, maxLevel, directNeighbors, offsetToNeighborLocation);
        }

        private static long setLocationOnLevel(long treeLocation, long newLocation, long mask, int level)
        {
            return (treeLocation & ~mask) | (newLocation << level);
        }

        private static int computeHighestSetBitPosition(long treeLocation1, long treeLocation2)
        {
            var temp1 = treeLocation1;
            var temp2 = treeLocation2;
            var highestSetBitPosition = 0;
            while (temp1 > 0 || temp2 > 0)
            {
                temp1 >>= 1;
                temp2 >>= 1;
                highestSetBitPosition++;
            }

            return highestSetBitPosition;
        }
    }
}