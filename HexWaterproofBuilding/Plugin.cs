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
        private const string PluginVersion = "1.3.1";

        private Harmony _harmony;
        private ConfigEntry<bool> _modEnabled;
        private ConfigEntry<bool> _extendedPlacementRangeEnabled;
        private ConfigEntry<bool> _extendedPlacementForVanillaPiecesEnabled;

        internal static Plugin Instance { get; private set; }
        internal bool IsModEnabled => _modEnabled != null && _modEnabled.Value;
        internal bool IsExtendedPlacementRangeEnabled => _extendedPlacementRangeEnabled != null && _extendedPlacementRangeEnabled.Value;
        internal bool IsExtendedPlacementForVanillaPiecesEnabled => _extendedPlacementForVanillaPiecesEnabled != null && _extendedPlacementForVanillaPiecesEnabled.Value;

        private void Awake()
        {
            Instance = this;

            _modEnabled = Config.Bind("General", "Enabled", true, "Enable or disable the Waterproof Building mod.");
            _extendedPlacementRangeEnabled = Config.Bind("Extended Placement Range", "Enabled", true, "Enable extended placement range for waterproof pieces.");
            _extendedPlacementForVanillaPiecesEnabled = Config.Bind(
                "Extended Placement Range",
                "VanillaPiecesEnabled",
                false,
                "Enable extended placement range for vanilla building pieces."
            );
            _modEnabled.SettingChanged += OnModEnabledSettingChanged;
            _extendedPlacementRangeEnabled.SettingChanged += OnModEnabledSettingChanged;
            _extendedPlacementForVanillaPiecesEnabled.SettingChanged += OnModEnabledSettingChanged;

            Assembly assembly = Assembly.GetExecutingAssembly();
            _harmony = new Harmony(PluginGuid);
            _harmony.PatchAll(assembly);

            if (IsModEnabled)
            {
                PrefabManager.OnVanillaPrefabsAvailable += Core.WaterproofPieceRegistrar.RegisterPieces;
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

            if (_extendedPlacementForVanillaPiecesEnabled != null)
            {
                _extendedPlacementForVanillaPiecesEnabled.SettingChanged -= OnModEnabledSettingChanged;
            }

            PrefabManager.OnVanillaPrefabsAvailable -= Core.WaterproofPieceRegistrar.RegisterPieces;

            Instance = null;
        }

        private void OnModEnabledSettingChanged(object sender, EventArgs args)
        {
            Jotunn.Logger.LogInfo($"Mod enabled: {IsModEnabled}");
            Jotunn.Logger.LogInfo($"Extended placement range enabled: {IsExtendedPlacementRangeEnabled}");
            Jotunn.Logger.LogInfo($"Extended placement for vanilla pieces enabled: {IsExtendedPlacementForVanillaPiecesEnabled}");
            Jotunn.Logger.LogWarning("Changes require a restart to fully take effect.");
        }
    }
}