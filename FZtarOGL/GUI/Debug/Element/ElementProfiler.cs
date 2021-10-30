using System;
using ImGuiHandler;
using ImGuiNET;

namespace FZtarOGL.GUI.Debug.Element
{
    public class ElementProfiler : ImGuiElement
    {
        private uint _fps, _ups;
        private float _dt;

        protected override void CustomRender()
        {
            var isOpen = false;
            ImGui.Begin("Profiler", ref isOpen, ImGuiWindowFlags.NoCollapse);

            var frameTime = 1f / _fps;
            if (!Double.IsInfinity(frameTime))
                ImGui.Text("FPS: " + _fps + " / " + frameTime.ToString("0.#######") + " ms");
            else ImGui.Text("FPS: " + _fps + " / 0.0000000 ms");

            ImGui.Text("UPS: " + _ups + " / " + _dt + " ms");
            //ImGui.Text("DeltaTime: " + _dt + " ms");

            ImGui.End();
        }

        public void Tick(uint fps, uint ups, float dt)
        {
            _fps = fps;
            _ups = ups;
            _dt = dt;
        }
    }
}