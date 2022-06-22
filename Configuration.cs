using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace NameplateSignResizer
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;
        public bool enabled { get; set; } = true;
        public bool syncOthersWithSelf { get; set; } = false;

        public bool hideSignOnSelf { get; set; } = false;
        public float ownSignScale { get; set; } = 1.0f;
        public int xOffset { get; set; } = 0;
        public int yOffset { get; set; } = 0;

        public bool hideSignOnOthers { get; set; } = false;
        public float otherSignScale { get; set; } = 1.0f;
        public int xOffsetOthers { get; set; } = 0;
        public int yOffsetOthers { get; set; } = 0;

        private DalamudPluginInterface pluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
        }

        public void Save()
        {
            this.pluginInterface!.SavePluginConfig(this);
        }

        public void resetSelf()
        {
            hideSignOnSelf = false;
            ownSignScale = 1.0f;
            xOffset = 0;
            yOffset = 0;
        }

        public void resetOthers()
        {
            hideSignOnOthers = false;
            otherSignScale = 1.0f;
            xOffsetOthers = 0;
            yOffsetOthers = 0;

        }
    }
}
