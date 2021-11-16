using FZtarOGL.GUI.Debug.Element;
using ImGuiHandler;

namespace FZtarOGL.GUI.Debug
{
    public class GuiDebugManager : ImGuiManager
    {
        private Game1 _game;
        private Screen.Screen _screen;

        public Screen.Screen Screen
        {
            get => _screen;
            set => _screen = value;
        }

        private ElementGameStats _elementGameStats;
        private ElementProfiler _elementProfiler;
        
        public GuiDebugManager(Game1 game, Screen.Screen screen, ImGuiRenderer renderer) : base(renderer)
        {
            _game = game;
            _screen = screen;

            _elementGameStats = new ElementGameStats();
            _elementProfiler = new ElementProfiler();
            
            AddElement(_elementGameStats);
            AddElement(_elementProfiler);
        }
        
        public void Tick()
        {
            _elementGameStats.Tick(_game.ScreenManager.GameScreens.Count, _screen._Entities.Count, _screen.BoxfCount, _screen.CurrentLevel.VirtualTravelDistance1);
            _elementProfiler.Tick(_game.LastFps, _game.LastUps, _game.DeltaTime, _screen.ModelsDrawn);
        }
    }
}