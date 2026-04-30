using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;

namespace HexWaterproofBuilding
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private const string PluginGuid = "hex.waterproofbuilding";
        private const string PluginName = "Waterproof Building";
        private const string PluginVersion = "1.0.0";

        private Harmony _harmony;
        private ConfigEntry<bool> _modEnabled;

        public static Plugin Instance { get; private set; }
        public bool IsModEnabled => _modEnabled != null && _modEnabled.Value;
        internal static ManualLogSource Log { get; private set; }

        private void Awake()
        {
            Instance = this;
            Log = Logger;

            _modEnabled = Config.Bind("General", "Enabled", true, "Enable or disable the Waterproof Building mod.");
            _modEnabled.SettingChanged += OnModEnabledSettingChanged;

            _harmony = new Harmony(PluginGuid);
            _harmony.PatchAll();

            Log.LogInfo($"{PluginName} v{PluginVersion} loaded.");
        }

        private void OnDestroy()
        {
            if (_modEnabled != null)
            {
                _modEnabled.SettingChanged -= OnModEnabledSettingChanged;
            }

            Instance = null;
            _harmony?.UnpatchSelf();

            Log.LogInfo($"{PluginName} v{PluginVersion} unloaded.");
        }

        private void OnModEnabledSettingChanged(object sender, EventArgs args)
        {
            Log.LogInfo($"Enabled changed to: {IsModEnabled}.");
        }
    }
}