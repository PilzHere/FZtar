using ImGuiHandler;
using ImGuiNET;

namespace FZtarOGL.GUI.Debug.Element
{
    public class ElementGameStats : ImGuiElement
    {
        private int _screens, _entities, _bodies;
        private float _virtTravelDist;

        protected override void CustomRender()
        {
            var isOpen = false;
            ImGui.Begin("Game", ref isOpen, ImGuiWindowFlags.NoCollapse);

            ImGui.Text("Screens: " + _screens);
            ImGui.Text("Entities: " + _entities);
            ImGui.Text("Bodies: " + _bodies);
            ImGui.Text("Virtual travel distance: " + _virtTravelDist);

            ImGui.End();
        }

        public void Tick(int screens, int entities, int bodies, float virtTravelDist)
        {
            _screens = screens;
            _entities = entities;
            _bodies = bodies;
            _virtTravelDist = virtTravelDist;
        }
    }
}