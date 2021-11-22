namespace DataStructures
{
    public static class TreeLocationHelper
    {
        private const byte LEVEL_MASK = 0b11;
        
        public static long childTreeLocation(long currentTreeLocation, int quadrant)
        {
            return currentTreeLocation << 2 | quadrant;
        }
        
        public static long leftNeighborLocation(long currentTreeLocation, int currentLevel, int maxLevel)
        {
            if (currentLevel < 0)
            {
                // no neighbor found in this planet face
            }
            var level = (maxLevel - currentLevel) * 2;
            long mask = LEVEL_MASK << level;
            var quadrant = (currentTreeLocation & mask) >> level;
            if (quadrant == Directions.TOP_RIGHT || quadrant == Directions.BOTTOM_RIGHT)
            {
                return setLocationOnLevel(currentTreeLocation, quadrant - 1, mask, level);
            }

            var reflectedPosition = setLocationOnLevel(currentTreeLocation, quadrant + 1, mask, level);
            return leftNeighborLocation(reflectedPosition, currentLevel - 1, maxLevel);
        }
        
        private static long setLocationOnLevel(long treeLocation, long newLocation, long mask, int level)
        {
            return (treeLocation & ~mask) | (newLocation << level);
        }
    }
}