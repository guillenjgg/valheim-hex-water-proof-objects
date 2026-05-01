using HexWaterproofBuilding.Discovery;
using HexWaterproofBuilding.Rules;
using Jotunn;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HexWaterproofBuilding.Core
{
    internal static class WaterproofPieceRegistrar
    {
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

            //CreateWaterproofPieces(TestPrefabName);

            _registered = true;
            PrefabManager.OnVanillaPrefabsAvailable -= RegisterPieces;

            Plugin.Log.LogInfo("Waterproof pieces registered.");
        }

        private static void CreateWaterproofPieces(string vanillaPrefabName)
        {
            string customPrefabName = WaterproofPrefabRules.PrefabPrefix + vanillaPrefabName;

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

        public static void LogWaterproofPrefabCandidates()
        {
            Plugin.Log.LogInfo("Available waterproof prefab candidates:");

            int validCount = 0;

            foreach (var prefab in PrefabDiscovery.GetPrefabs(WaterproofPrefabRules.IsValidPrefab))
            {
                var wear = prefab.GetComponent<WearNTear>();
                var piece = prefab.GetComponent<Piece>();

                validCount++;

                Plugin.Log.LogInfo(
                    $"Valid Prefab: {prefab.name} | Material: {wear.m_materialType} | Category: {piece.m_category}"
                );
            }

            Plugin.Log.LogInfo($"Total valid waterproof candidates: {validCount}");

            PrefabManager.OnVanillaPrefabsAvailable -= LogWaterproofPrefabCandidates;
        }
    }
}