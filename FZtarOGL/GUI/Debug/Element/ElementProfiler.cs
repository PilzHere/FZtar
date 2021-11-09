using System;
using ImGuiHandler;
using ImGuiNET;
using Microsoft.Xna.Framework;

namespace FZtarOGL.GUI.Debug.Element
{
    public class ElementProfiler : ImGuiElement
    {
        private uint _fps, _ups;
        private float _dt;
        private long _modelsDrawn;

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
            ImGui.Text(" ");
            ImGui.Text("Draw calls: " + Profiler.ProfilerStats.DrawCalls);
            ImGui.Text("Clear calls: " + Profiler.ProfilerStats.ClearCalls);
            ImGui.Text("Target switches: " + Profiler.ProfilerStats.TargetSwitches);
            ImGui.Text("Texture switches: " + Profiler.ProfilerStats.TextureSwitches);
            ImGui.Text("\nShader switches");
            ImGui.Text("Vertex: " + Profiler.ProfilerStats.VertexShaderSwitches);
            ImGui.Text("Fragment: " + Profiler.ProfilerStats.FragmentShaderSwitches);
            ImGui.Text("\nShapes drawn");
            ImGui.Text("Sprites drawn: " + Profiler.ProfilerStats.SpritesDrawn);
            ImGui.Text("Models drawn: " + _modelsDrawn);
            ImGui.Text("Primitives drawn: " + Profiler.ProfilerStats.PrimitivesDrawn);
            
            ImGui.Button("Render bounding boxes");
            if (ImGui.IsItemClicked(0))
            {
                GameSettings.GameSettings.DebugRenderBoundingBoxes =
                    !GameSettings.GameSettings.DebugRenderBoundingBoxes;
            }

            ImGui.End();
        }

        public void Tick(uint fps, uint ups, float dt, long modelsDrawn)
        {
            _fps = fps;
            _ups = ups;
            _dt = dt;
            _modelsDrawn = modelsDrawn;
        }
    }
}