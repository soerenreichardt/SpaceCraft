using static DataStructures.Directions;

namespace DataStructures
{
    public static class TreeLocationHelper
    {
        public const long SWITCH_PLANET_FACE_MASK = 1L << 63;
        private const long LEVEL_MASK = 0b11;
        
        public static long ChildTreeLocation(long currentTreeLocation, int quadrant, int level)
        {
            return currentTreeLocation | quadrant << (2 * level);
        }

        public static long QuadrantForLevel(long treeLocation, int level)
        {
            var levelShift = 2 * level;
            return (treeLocation & (LEVEL_MASK << levelShift)) >> levelShift;
        }

        public static int CommonPathLength(long treeLocation1, long treeLocation2, int nodeLevel)
        {
            for (int level = 0; level <= nodeLevel; level++)
            {
                if (QuadrantForLevel(treeLocation1, level) == QuadrantForLevel(treeLocation2, level))
                {
                    continue;
                }

                return level;
            }

            return 0;
        }

        public static long LeftNeighborLocation(long currentTreeLocation, int quadTreeLevel)
        {
            return NeighborPosition(currentTreeLocation, quadTreeLevel,
                new[] {TOP_RIGHT, BOTTOM_RIGHT}, 1, LEFT);
        }

        public static long RightNeighborLocation(long currentTreeLocation, int quadTreeLevel)
        {
            return NeighborPosition(currentTreeLocation, quadTreeLevel,
                new[] {TOP_LEFT, BOTTOM_LEFT}, -1, RIGHT);
        }

        public static long TopNeighborLocation(long currentTreeLocation, int quadTreeLevel)
        {
            return NeighborPosition(currentTreeLocation, quadTreeLevel,
                new[] {BOTTOM_LEFT, BOTTOM_RIGHT}, 2, TOP);
        }

        public static long BottomNeighborLocation(long currentTreeLocation, int quadTreeLevel)
        {
            return NeighborPosition(currentTreeLocation, quadTreeLevel,
                new[] {TOP_LEFT, TOP_RIGHT}, -2, BOTTOM);
        }

        public static bool SwitchPlanetFace(long treeLocation)
        {
            return (treeLocation & SWITCH_PLANET_FACE_MASK) != 0;
        }
        
        private static long NeighborPosition(long currentTreeLocation, int level, int[] directNeighbors, int offsetToNeighborLocation, int neighborDirection)
        {
            if (level == 0)
            {
                // Set the neighbor search direction as first 2 bits.
                // The direction code differs from the quadrant locations
                // hence this conversion is necessary.
                // Also the highest bit is flipped as an indicator that we
                // need to traverse another planet face.
                var planetFaceNeighborInformation = neighborDirection | SWITCH_PLANET_FACE_MASK;
                return (currentTreeLocation & ~LEVEL_MASK) | planetFaceNeighborInformation;
            }
            
            long mask = LEVEL_MASK << (level * 2);
            var quadrant = QuadrantForLevel(currentTreeLocation, level);
            if (quadrant == directNeighbors[0] || quadrant == directNeighbors[1])
            {
                return SetLocationOnLevel(currentTreeLocation, quadrant - offsetToNeighborLocation, mask, level);
            }

            var reflectedPosition = SetLocationOnLevel(currentTreeLocation, quadrant + offsetToNeighborLocation, mask, level);
            return NeighborPosition(reflectedPosition, level - 1, directNeighbors, offsetToNeighborLocation, neighborDirection);
        }

        private static long SetLocationOnLevel(long treeLocation, long newLocation, long mask, int level)
        {
            var levelShift = level * 2;
            return (treeLocation & ~mask) | (newLocation << levelShift);
        }
    }
}