using System;
using System.Runtime.InteropServices;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Hooking;
using Dalamud.IoC;
using Dalamud.Logging;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace NameplateSignResizer
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "NameplateSignResizer";
        private const string configCommand = "/nsr";
        private readonly int defaultXPos = 95;
        private readonly int defaultYPos = 4;

        [PluginService]
        private DalamudPluginInterface PluginInterface { get; init; }
        [PluginService]
        private CommandManager CommandManager { get; init; }
        private Configuration config { get; init; }
        private PluginUI pluginUi { get; init; }
        private Hook<SetNamePlateDelegate> setNamePlateHook;
        internal PluginAddressResolver address;

        [PluginService]
        public SigScanner SigScanner { get; set; }
        [PluginService]
        public ClientState Client { get; init; }
        [PluginService]
        public GameGui GameGui { get; set; }
        public IntPtr AddonNamePlatePtr => GameGui.GetAddonByName("NamePlate", 1);
        

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] CommandManager commandManager)
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;
            address = new PluginAddressResolver();
            address.Setup(SigScanner);
            setNamePlateHook = new Hook<SetNamePlateDelegate>(address.AddonNamePlate_SetNamePlatePtr, SetNamePlateDetour);
            setNamePlateHook.Enable();

            // Setup and initialize config UI.
            this.config = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.config.Initialize(this.PluginInterface);
            this.pluginUi = new PluginUI(this.config);

            // Add plugin commands
            this.CommandManager.AddHandler(configCommand, new CommandInfo(OnConfig)
            {
                HelpMessage = "Open config window for Nameplate Sign Resizer"
            });

            // Draw UI
            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;

        }

        public void Dispose()
        {
            this.pluginUi.Dispose();
            this.CommandManager.RemoveHandler(configCommand);
            setNamePlateHook.Dispose();
        }

        /// <summary>
        /// Open/close plugin settings window.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="args"></param>
        private void OnConfig(string command, string args)
        {
            
            this.pluginUi.SettingsVisible = !this.pluginUi.SettingsVisible;
        }

        private unsafe void UpdateSign(AddonNamePlate.NamePlateObject npObject)
        {
            int xOffset = 0;
            int yOffset = 0;
            float scale = 1;
            // Only apply config if plugin is enabled and nameplate is a non-pvp player nameplate.
            if (config.enabled && npObject.IsPlayerCharacter && npObject.NameplateKind == 0 && npObject.IsPvpEnemy == 0)
            {
                // Nameplate belongs to the local player or sync is enabled.
                if (npObject.IsLocalPlayer ||
                    (config.syncOthersWithSelf && config.experimentalFeatures))
                {
                    // Hide nameplate sign.
                    if (config.hideSignOnSelf)
                        scale = 0;
                    else
                    {
                        scale = config.ownSignScale;
                        xOffset = config.xOffset;
                        yOffset = config.yOffset;
                    }
                        
                }
                else
                {
                    // Skip if experimental features are not enabled.
                    // TODO: Fix bug causing settings for other players to also apply to some NPC quest markers.
                    if (config.experimentalFeatures)
                    {
                        if (config.hideSignOnOthers)
                            scale = 0;
                        else
                        {
                            scale = config.otherSignScale;
                            xOffset = config.xOffsetOthers;
                            yOffset = config.yOffsetOthers;
                        }
                    }
                    
                }
            }
            
            // Change icon size
            npObject.ImageNode2->AtkResNode.SetScale(scale, scale);
            // TODO: Auto position after scale change
            // Offset icon position to compensate for scale change
            //float posOffsetScale = scale < 1 ? scale : -scale;
            npObject.ImageNode2->AtkResNode.SetPositionFloat(this.defaultXPos + xOffset, this.defaultYPos + yOffset);

        }
        private void DrawUI()
        {
            this.pluginUi.Draw();
        }

        private void DrawConfigUI()
        {
            this.pluginUi.SettingsVisible = true;
        }

        internal IntPtr SetNamePlateDetour(IntPtr namePlateObjectPtr, bool isPrefixTitle, bool displayTitle, IntPtr title, IntPtr name, IntPtr fcName, int iconID)
        {
            try
            {
                return SetNamePlate(namePlateObjectPtr, isPrefixTitle, displayTitle, title, name, fcName, iconID);
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, $"SetNamePlateDetour encountered a critical error");
            }

            return setNamePlateHook.Original(namePlateObjectPtr, isPrefixTitle, displayTitle, title, name, fcName, iconID);
        }

        internal IntPtr SetNamePlate(IntPtr namePlateObjectPtr, bool isPrefixTitle, bool displayTitle, IntPtr title, IntPtr name, IntPtr fcName, int iconID)
        {
            var npObject = Marshal.PtrToStructure<AddonNamePlate.NamePlateObject>(namePlateObjectPtr);
            this.UpdateSign(npObject);
            return setNamePlateHook.Original(namePlateObjectPtr, isPrefixTitle, displayTitle, title, name, fcName, iconID);
        }
    }
}
