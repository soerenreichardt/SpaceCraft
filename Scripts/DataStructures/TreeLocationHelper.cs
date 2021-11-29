namespace DataStructures
{
    public static class TreeLocationHelper
    {
        public const long SWITCH_PLANET_FACE_MASK = 1L << 63;
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
                new[] {Directions.TOP_RIGHT, Directions.BOTTOM_RIGHT}, 1, Directions.LEFT);
        }

        public static long rightNeighborLocation(long currentTreeLocation, int quadTreeLevel)
        {
            return neighborPosition(currentTreeLocation, quadTreeLevel,
                new[] {Directions.TOP_LEFT, Directions.BOTTOM_LEFT}, -1, Directions.RIGHT);
        }

        public static long topNeighborLocation(long currentTreeLocation, int quadTreeLevel)
        {
            return neighborPosition(currentTreeLocation, quadTreeLevel,
                new[] {Directions.BOTTOM_LEFT, Directions.BOTTOM_RIGHT}, 2, Directions.TOP);
        }

        public static long bottomNeighborLocation(long currentTreeLocation, int quadTreeLevel)
        {
            return neighborPosition(currentTreeLocation, quadTreeLevel,
                new[] {Directions.TOP_LEFT, Directions.TOP_RIGHT}, -2, Directions.BOTTOM);
        }

        private static long neighborPosition(long currentTreeLocation, int level, int[] directNeighbors, int offsetToNeighborLocation, int neighborDirection)
        {
            if (level == 0)
            {
                // Set the neighbor search direction as first 2 bits.
                // The direction code differs from the quadrant locations
                // hence this conversion is necessary.
                // Also the first bit is flipped as an indicator that we
                // need to traverse another planet face.
                var planetFaceNeighborInformation = neighborDirection | SWITCH_PLANET_FACE_MASK;
                return (currentTreeLocation & ~LEVEL_MASK) | planetFaceNeighborInformation;
            }
            
            long mask = LEVEL_MASK << (level * 2);
            var quadrant = quadrantForLevel(currentTreeLocation, level);
            if (quadrant == directNeighbors[0] || quadrant == directNeighbors[1])
            {
                return setLocationOnLevel(currentTreeLocation, quadrant - offsetToNeighborLocation, mask, level);
            }

            var reflectedPosition = setLocationOnLevel(currentTreeLocation, quadrant + offsetToNeighborLocation, mask, level);
            return neighborPosition(reflectedPosition, level - 1, directNeighbors, offsetToNeighborLocation, neighborDirection);
        }

        private static long setLocationOnLevel(long treeLocation, long newLocation, long mask, int level)
        {
            var levelShift = level * 2;
            return (treeLocation & ~mask) | (newLocation << levelShift);
        }
    }
}