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
        public const int FilterEnemyShip = 32;
        public const int FilterEnemyRay = 64;
        public const int FilterEnemyTurret = 128;

        public const int MaskPlayerShip = FilterObstacle | FilterRingHealth | FilterRingPower | FilterEnemyShip | FilterEnemyRay | FilterEnemyTurret;
        public const int MaskPlayerRay = FilterObstacle | FilterEnemyShip | FilterEnemyTurret;
        public const int MaskObstacle = FilterPlayerShip | FilterPlayerRay | FilterEnemyShip | FilterEnemyRay;
        public const int MaskRingHealth = FilterPlayerShip;
        public const int MaskRingPower = FilterPlayerShip;
        public const int MaskEnemyShip = FilterPlayerShip | FilterPlayerRay | FilterObstacle;
        public const int MaskEnemyRay = FilterPlayerShip | FilterObstacle;
        public const int MaskEnemyTurret = FilterPlayerShip | FilterPlayerRay;
    }
}
