namespace FZtarOGL.Utilities
{
    public class BoxFilters
    {
        public const int FilterNothing = 0;
        public const int FilterPlayerShip = 1;
        public const int FilterPlayerRay = 2;
        public const int FilterObstacle = 4;
        public const int FilterRingHealth = 8;
        public const int FilterRingPower = 16;

        public const int MaskPlayerShip = FilterObstacle | FilterRingHealth | FilterRingPower;
        public const int MaskPlayerRay = FilterObstacle;
        public const int MaskObstacle = FilterPlayerShip | FilterPlayerRay;
        public const int MaskRingHealth = FilterPlayerShip;
        public const int MaskRingPower = FilterPlayerShip;
    }
}
