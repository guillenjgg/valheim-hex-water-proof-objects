using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
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

        private Harmony _harmony;
        private ConfigEntry<bool> _modEnabled;

        internal static Plugin Instance { get; private set; }
        internal bool IsModEnabled => _modEnabled != null && _modEnabled.Value;

        private void Awake()
        {
            Instance = this;

            _modEnabled = Config.Bind("General", "Enabled", true, "Enable or disable the Waterproof Building mod.");
            _modEnabled.SettingChanged += OnModEnabledSettingChanged;

            _harmony = new Harmony(PluginGuid);
            _harmony.PatchAll();

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
            Jotunn.Logger.LogInfo($"{PluginName} v{PluginVersion} unloaded.");

            _harmony?.UnpatchSelf();
            _harmony = null;

            if (_modEnabled != null)
            {
                _modEnabled.SettingChanged -= OnModEnabledSettingChanged;
            }

            PrefabManager.OnVanillaPrefabsAvailable -= Core.WaterproofPieceRegistrar.RegisterPieces;
            
            Instance = null;

        }

        private void OnModEnabledSettingChanged(object sender, EventArgs args)
        {
            Jotunn.Logger.LogInfo($"Enabled changed to: {IsModEnabled}");

            Jotunn.Logger.LogWarning("Changes require a restart to take effect.");
        }
    }
}