using ImGuiNET;
using ImGuiScene;
using System;
using System.Numerics;

namespace NameplateSignResizer
{
    class PluginUI : IDisposable
    {
        private Configuration config;
        private bool visible = false;
        public bool Visible
        {
            get { return this.visible; }
            set { this.visible = value; }
        }

        private bool settingsVisible = false;
        public bool SettingsVisible
        {
            get { return this.settingsVisible; }
            set { this.settingsVisible = value; }
        }

        public PluginUI(Configuration configuration)
        {
            this.config = configuration;
        }

        public void Dispose()
        {
            
        }

        public void Draw()
        {
            DrawSettingsWindow();
        }

        public void DrawSettingsWindow()
        {
            if (!SettingsVisible)
            {
                return;
            }
            var windowHeight = 245;
            if (this.config.experimentalFeatures)
            {
                windowHeight = 360;
            }

            ImGui.SetNextWindowSize(new Vector2(365, windowHeight), ImGuiCond.Always);
            //ImGui.SetNextWindowSizeConstraints(new Vector2(365, windowHeight), new Vector2(float.MaxValue, float.MaxValue));
            if (ImGui.Begin("Nameplate Sign Resizer", ref this.settingsVisible, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
            {
                ImGui.TextWrapped("Changes may take a few seconds to be shown. Moving character or camera helps force a refresh.");
                ImGui.Spacing();

                var enabled = this.config.enabled;
                if (ImGui.Checkbox("Enable Plugin", ref enabled))
                {
                    this.config.enabled = enabled;
                    this.config.Save();
                }
                if (this.config.experimentalFeatures)
                {
                    ImGui.SameLine();
                    var syncOthers = this.config.syncOthersWithSelf;
                    if (ImGui.Checkbox("Sync others", ref syncOthers))
                    {
                        this.config.syncOthersWithSelf = syncOthers;
                        this.config.Save();
                    }
                    ImGui.SameLine();
                    ImGui.TextColored(new Vector4(0.8f, 0.8f, 0.8f, 1f), "?");
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetTooltip("Applies your nameplate settings to other players\' nameplates. Does not reset any settings.");
                    }
                }

                ImGui.Spacing();

                // Self
                var hideOnSelf = this.config.hideSignOnSelf;
                if (ImGui.Checkbox("Hide Sign on Me", ref hideOnSelf))
                {
                    this.config.hideSignOnSelf = hideOnSelf;
                    this.config.Save();
                }

                var resetSelfSize = new Vector2(85, 23);
                ImGui.SameLine(255);
                if (ImGui.Button("Reset Self", resetSelfSize))
                {
                    this.config.resetSelf();
                    this.config.Save();
                }

                var ownSign = this.config.ownSignScale;
                if (ImGui.SliderFloat("My Sign Scale", ref ownSign, 0.1f, 2.0f, "%.1f", ImGuiSliderFlags.AlwaysClamp))
                {
                    this.config.ownSignScale = ownSign;
                    this.config.Save();
                }

                var xOffset = this.config.xOffset;
                if (ImGui.SliderInt("My X Offset", ref xOffset, -100, 100, "%.0f", ImGuiSliderFlags.AlwaysClamp))
                {
                    this.config.xOffset = xOffset;
                    this.config.Save();
                }

                var yOffset = this.config.yOffset;
                if (ImGui.SliderInt("My Y Offset", ref yOffset, -100, 100, "%.0f", ImGuiSliderFlags.AlwaysClamp))
                {
                    this.config.yOffset = yOffset;
                    this.config.Save();
                }

                var exp = this.config.experimentalFeatures;
                if (ImGui.Checkbox("Experimental Features", ref exp))
                {
                    this.config.experimentalFeatures = exp;
                    this.config.Save();
                }

                ImGui.SameLine();
                ImGui.TextColored(new Vector4(0.8f, 0.8f, 0.8f, 1f), "?");
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("Enables resizing options for other players. Warning: this is buggy and will apply to some NPC quest markers.");
                }


                if (exp)
                {
                    ImGui.Spacing();
                    ImGui.Spacing();

                    // Others
                    var hideOnOthers = this.config.hideSignOnOthers;
                    if (ImGui.Checkbox("Hide Sign on Others", ref hideOnOthers))
                    {
                        this.config.hideSignOnOthers = hideOnOthers;
                        this.config.Save();
                    }
                    var resetOthersSize = new Vector2(85, 23);
                    ImGui.SameLine(255);
                    if (ImGui.Button("Reset Others", resetOthersSize))
                    {
                        this.config.resetOthers();
                        this.config.Save();
                    }

                    var othersSign = this.config.otherSignScale;
                    if (ImGui.SliderFloat("Others Sign Scale", ref othersSign, 0.1f, 2.0f, "%.1f", ImGuiSliderFlags.AlwaysClamp))
                    {
                        this.config.otherSignScale = othersSign;
                        this.config.Save();
                    }

                    var xOffsetOthers = this.config.xOffsetOthers;
                    if (ImGui.SliderInt("Others X Offset", ref xOffsetOthers, -100, 100, "%.0f", ImGuiSliderFlags.AlwaysClamp))
                    {
                        this.config.xOffsetOthers = xOffsetOthers;
                        this.config.Save();
                    }

                    var yOffsetOthers = this.config.yOffsetOthers;
                    if (ImGui.SliderInt("Others Y Offset", ref yOffsetOthers, -100, 100, "%.0f", ImGuiSliderFlags.AlwaysClamp))
                    {
                        this.config.yOffsetOthers = yOffsetOthers;
                        this.config.Save();
                    }
                }
                
            }
            ImGui.End();
        }
    }
}