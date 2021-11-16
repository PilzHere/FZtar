using System;

namespace FZtarOGL.Utilities
{
    public class MathUtils
    {
        private static readonly Random Random0To1 = new Random();

        public static int GetRandomNumberBetween0And1()
        {
            int random = Random0To1.Next(2);
            
            return random;
        }
    }
}