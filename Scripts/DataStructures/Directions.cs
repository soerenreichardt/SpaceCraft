namespace DataStructures
{
    public static class Directions
    {
        public const int LEFT = 0;
        public const int TOP = 1;
        public const int RIGHT = 2;
        public const int BOTTOM = 3;

        public const int TOP_LEFT = 0;
        public const int TOP_RIGHT = 1;
        public const int BOTTOM_LEFT = 2;
        public const int BOTTOM_RIGHT = 3;
        
        public static long rotateQuadrant(long quadrant, int rotation)
        {
            rotation = rotation < 0
                ? 4 + rotation
                : rotation;
            
            var rotatedQuadrant = quadrant;
            for (int i = 0; i < rotation; i++)
            {
                if (rotatedQuadrant == TOP_LEFT)
                {
                    rotatedQuadrant = BOTTOM_LEFT;
                    continue;
                }

                if (rotatedQuadrant == TOP_RIGHT)
                {
                    rotatedQuadrant = TOP_LEFT;
                    continue;
                }

                if (rotatedQuadrant == BOTTOM_LEFT)
                {
                    rotatedQuadrant = BOTTOM_RIGHT;
                    continue;
                }

                if (rotatedQuadrant == BOTTOM_RIGHT)
                {
                    rotatedQuadrant = TOP_RIGHT;
                    continue;
                }
            }

            return rotatedQuadrant;
        }
        
        public static int rotateDirection(int direction, int rotation)
        {
            rotation = rotation < 0
                ? 4 + rotation
                : rotation;
            
            var rotatedDirection = direction;
            for (int i = 0; i < rotation; i++)
            {
                if (rotatedDirection == LEFT)
                {
                    rotatedDirection = BOTTOM;
                    continue;
                }

                if (rotatedDirection == TOP)
                {
                    rotatedDirection = LEFT;
                    continue;
                }

                if (rotatedDirection == RIGHT)
                {
                    rotatedDirection = TOP;
                    continue;
                }

                if (rotatedDirection == BOTTOM)
                {
                    rotatedDirection = RIGHT;
                    continue;
                }
            }

            return rotatedDirection;
        }
    }
}