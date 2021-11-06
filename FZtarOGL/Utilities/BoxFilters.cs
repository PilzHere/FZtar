namespace FZtarOGL.Utilities
{
    public class BoxFilters
    {
        public const int filterNothing = 0;
        public const int filterPlayerShip = 1;
        public const int filterPlayerRay = 2;
        public const int filterTower = 4;

        public const int maskPlayerShip = filterTower;
        public const int maskPlayerRay = filterTower;
        public const int maskTower = filterPlayerShip | filterPlayerRay;
    }
}
