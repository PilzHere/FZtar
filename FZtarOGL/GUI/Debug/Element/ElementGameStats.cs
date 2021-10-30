using ImGuiHandler;
using ImGuiNET;

namespace FZtarOGL.GUI.Debug.Element
{
    public class ElementGameStats : ImGuiElement
    {
        private int _screens, _entities;

        protected override void CustomRender()
        {
            var isOpen = false;
            ImGui.Begin("Game", ref isOpen, ImGuiWindowFlags.NoCollapse);

            ImGui.Text("Screens: " + _screens);
            ImGui.Text("Entities: " + _entities);

            ImGui.End();
        }

        public void Tick(int screens, int entities)
        {
            _screens = screens;
            _entities = entities;
        }
    }
}