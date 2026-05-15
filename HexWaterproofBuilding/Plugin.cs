using BepInEx;
using BepInEx.Configuration;
using Jotunn.Managers;
using System;

namespace HexWaterproofBuilding
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    public class Plugin : BaseUnityPlugin
    {
        private const string PluginGuid = "hex.waterproofbuilding";
        private const string PluginName = "HexWaterproofBuilding";
        private const string PluginVersion = "1.1.0";

        private ConfigEntry<bool> _modEnabled;

        internal static Plugin Instance { get; private set; }
        internal bool IsModEnabled => _modEnabled != null && _modEnabled.Value;

        private void Awake()
        {
            Instance = this;

            _modEnabled = Config.Bind("General", "Enabled", true, "Enable or disable the Waterproof Building mod.");
            _modEnabled.SettingChanged += OnModEnabledSettingChanged;

            if (IsModEnabled)
            {
                PrefabManager.OnVanillaPrefabsAvailable += Core.WaterproofPieceRegistrar.RegisterPieces;
            }
            else
            {
                Jotunn.Logger.LogInfo("Mod is disabled. No pieces will be registered.");
            }

            Jotunn.Logger.LogInfo($"{PluginName} v{PluginVersion} loaded.");
        }

        private void OnDestroy()
        {
            if (_modEnabled != null)
            {
                _modEnabled.SettingChanged -= OnModEnabledSettingChanged;
            }

            PrefabManager.OnVanillaPrefabsAvailable -= Core.WaterproofPieceRegistrar.RegisterPieces;

            Instance = null;

            Jotunn.Logger.LogInfo($"{PluginName} v{PluginVersion} unloaded.");
        }

        private void OnModEnabledSettingChanged(object sender, EventArgs args)
        {
            Jotunn.Logger.LogInfo($"Enabled changed to: {IsModEnabled}");

            Jotunn.Logger.LogWarning("Changes require a restart to take effect.");
        }
    }
}