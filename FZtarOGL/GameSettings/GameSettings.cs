namespace FZtarOGL.GameSettings
{
    public class GameSettings
    {
        public static bool DebugRenderDebugGui = true;
        public static bool DebugRenderBoundingBoxes = true;
        public static bool DebugUpdateWindowTitle = false;
        private static float _musicVolume = 1;

        public static float MusicVolume
        {
            get { return _musicVolume; }
            set
            {
                if (value > 1)
                {
                    value = 1;
                    _musicVolume = value;
                }
                else if (value < 0)
                {
                    value = 0;
                    _musicVolume = value;
                }
                else
                {
                    _musicVolume = value;
                }
            }
        }
    }
}