using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;

namespace HexWaterproofBuilding.Core
{
    internal static class WaterproofPieceRegistrar
    {
        private const string WaterproofPrefix = "Hex_Waterproof_";
        private const string TestPrefabName = "wood_pole_log_4";

        private static bool _registered;

        public static void RegisterPieces()
        {
            if (_registered)
            {
                return;
            }

            if (Plugin.Instance == null || !Plugin.Instance.IsModEnabled)
            {
                return;
            }

            RegisterWaterproofPiece(TestPrefabName);

            _registered = true;
            PrefabManager.OnVanillaPrefabsAvailable -= RegisterPieces;

            Plugin.Log.LogInfo("Waterproof pieces registered.");
        }

        private static void RegisterWaterproofPiece(string vanillaPrefabName)
        {
            string customPrefabName = WaterproofPrefix + vanillaPrefabName;

            GameObject customPrefab = PrefabManager.Instance.CreateClonedPrefab(
                customPrefabName,
                vanillaPrefabName
            );

            if (customPrefab == null)
            {
                Plugin.Log.LogWarning($"Failed to clone prefab: {vanillaPrefabName}");
                return;
            }

            var wear = customPrefab.GetComponent<WearNTear>();

            if (wear == null)
            {
                Plugin.Log.LogWarning($"{customPrefabName} has no WearNTear. Skipping.");
                return;
            }

            wear.m_noRoofWear = false;

            var config = new PieceConfig
            {
                Name = "Waterproof Core Wood Pole",
                Description = "A waterproof core wood pole that resists water damage.",
                PieceTable = PieceTables.Hammer,
                Category = PieceCategories.Building,
                Requirements = new[]
                {
                    new RequirementConfig("RoundLog", 1, 0, true),
                    new RequirementConfig("Resin", 1, 0, true)
                }
            };

            var customPiece = new CustomPiece(customPrefab, true, config);

            PieceManager.Instance.AddPiece(customPiece);

            Plugin.Log.LogInfo($"Registered waterproof piece: {customPrefabName}");
        }
    }
}