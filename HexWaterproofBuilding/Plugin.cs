using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Managers;
using System;
using System.Reflection;

namespace HexWaterproofBuilding
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    public class Plugin : BaseUnityPlugin
    {
        private const string PluginGuid = "hex.waterproofbuilding";
        private const string PluginName = "HexWaterproofBuilding";
        private const string PluginVersion = "1.2.3";

        private Harmony _harmony;
        private ConfigEntry<bool> _modEnabled;
        private ConfigEntry<bool> _extendedPlacementRangeEnabled;

        internal static Plugin Instance { get; private set; }
        internal bool IsModEnabled => _modEnabled != null && _modEnabled.Value;
        internal bool IsExtendedPlacementRangeEnabled => _extendedPlacementRangeEnabled != null && _extendedPlacementRangeEnabled.Value;

        private void Awake()
        {
            Instance = this;

            _modEnabled = Config.Bind("General", "Enabled", true, "Enable or disable the Waterproof Building mod.");
            _extendedPlacementRangeEnabled = Config.Bind("Extended Placement Range", "Enabled", true, "Enable extended placement range for waterproof pieces.");
            _modEnabled.SettingChanged += OnModEnabledSettingChanged;
            _extendedPlacementRangeEnabled.SettingChanged += OnModEnabledSettingChanged;

            Assembly asembly = Assembly.GetExecutingAssembly();
            _harmony = new Harmony(PluginGuid);
            _harmony.PatchAll(asembly);

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
            Logger.LogInfo($"{PluginName} v{PluginVersion} unloaded.");

            _harmony?.UnpatchSelf();
            _harmony = null;

            if (_modEnabled != null)
            {
                _modEnabled.SettingChanged -= OnModEnabledSettingChanged;
            }

            if (_extendedPlacementRangeEnabled != null)
            {
                _extendedPlacementRangeEnabled.SettingChanged -= OnModEnabledSettingChanged;
            }

            PrefabManager.OnVanillaPrefabsAvailable -= Core.WaterproofPieceRegistrar.RegisterPieces;

            Instance = null;
        }

        private void OnModEnabledSettingChanged(object sender, EventArgs args)
        {
            Jotunn.Logger.LogInfo($"Mod enabled: {IsModEnabled}");
            Jotunn.Logger.LogInfo($"Extended placement range enabled: {IsExtendedPlacementRangeEnabled}");

            Jotunn.Logger.LogWarning("Changes require a restart to take effect.");
        }
    }
}