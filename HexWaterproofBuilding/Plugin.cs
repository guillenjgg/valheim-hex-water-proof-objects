using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HexWaterproofBuilding
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    public class Plugin : BaseUnityPlugin
    {
        private const string PluginGuid = "hex.waterproofbuilding";
        private const string PluginName = "HexWaterproofBuilding";
        private const string PluginVersion = "1.4.0";

        private Harmony _harmony;
        private ConfigEntry<bool> _modEnabled;
        private ConfigEntry<bool> _extendedPlacementRangeEnabled;
        private ConfigEntry<bool> _extendedPlacementForVanillaPiecesEnabled;
        private ConfigEntry<bool> _workBenchRequireRoof;
        private CustomLocalization _localization;

        internal static Plugin Instance { get; private set; }
        internal bool IsModEnabled => _modEnabled != null && _modEnabled.Value;
        internal bool IsExtendedPlacementRangeEnabled => _extendedPlacementRangeEnabled != null && _extendedPlacementRangeEnabled.Value;
        internal bool IsExtendedPlacementForVanillaPiecesEnabled => _extendedPlacementForVanillaPiecesEnabled != null && _extendedPlacementForVanillaPiecesEnabled.Value;
        internal bool IsWorkBenchRequireRoof => _workBenchRequireRoof != null && _workBenchRequireRoof.Value;
        internal AssetBundle AssetBundle { get; private set; }
        internal GameObject PierLog4Asset { get; private set; }

        private void Awake()
        {
            Instance = this;
            
            LoadAssets();
            AddLocalizations();

            _modEnabled = Config.Bind("General", "Enabled", true, "Enable or disable the Waterproof Building mod.");
            _extendedPlacementRangeEnabled = Config.Bind("Extended Placement Range", "Enabled", true, "Enable extended placement range for waterproof pieces.");
            _extendedPlacementForVanillaPiecesEnabled = Config.Bind(
                "Extended Placement Range",
                "VanillaPiecesEnabled",
                false,
                "Enable extended placement range for vanilla building pieces."
            );
            
            _workBenchRequireRoof = Config.Bind("General", "WorkBenchRequireRoof", false, "Require a roof for workbenches to function.");

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

            PrefabManager.OnVanillaPrefabsAvailable -= Core.WaterproofPieceRegistrar.RegisterPieces;

            Instance = null;
        }

        private bool LoadAssets()
        {
            AssetBundle = AssetUtils.LoadAssetBundleFromResources("piersupport", Assembly.GetExecutingAssembly());

            if(AssetBundle == null)
            {
                return false;
            }

            PierLog4Asset = AssetBundle.LoadAsset<GameObject>("hex_pier_log_4_vertical");

            return PierLog4Asset != null;
        }

        private void AddLocalizations()
        {
            _localization = LocalizationManager.Instance.GetLocalization();

            _localization.AddTranslation("English", new Dictionary<string, string>
            {
                {"piece_hex_pier_log_4_vertical", "4m Vertical Pier Support" },
                {"piece_hex_pier_log_4_vertical_desc", "A pier support that extends to the seabed. Cannot be placed on dry land." },
            });
        }
    }
}