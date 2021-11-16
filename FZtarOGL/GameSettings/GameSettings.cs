namespace FZtarOGL.GameSettings
{
    public class GameSettings
    {
        public static bool DebugRenderDebugGui = true;
        public static bool DebugRenderBoundingBoxes = true;
        public static bool DebugUpdateWindowTitle = false;
        
        private static float _musicVolume = 1;
        private static float _sfxVolume = 1;

        private static bool _invertVerticalMovement = false;

        public static bool InvertVerticalMovement
        {
            get => _invertVerticalMovement;
            set => _invertVerticalMovement = value;
        }

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
        
        public static float SfxVolume
        {
            get { return _sfxVolume; }
            set
            {
                if (value > 1)
                {
                    value = 1;
                    _sfxVolume = value;
                }
                else if (value < 0)
                {
                    value = 0;
                    _sfxVolume = value;
                }
                else
                {
                    _sfxVolume = value;
                }
            }
        }
    }
}