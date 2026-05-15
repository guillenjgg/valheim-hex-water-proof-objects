using HexWaterproofBuilding.Discovery;
using HexWaterproofBuilding.Rules;
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

        internal static void RegisterPieces()
        {
            if (_registered)
            {
                return;
            }

            if (Plugin.Instance == null || !Plugin.Instance.IsModEnabled)
            {
                PrefabManager.OnVanillaPrefabsAvailable -= RegisterPieces;
                return;
            }

            var indexes = PrefabDiscovery.GetHammerPieceIndexes();
            int registeredCount = CreateWaterproofPieces(indexes);

            _registered = true;

            PrefabManager.OnVanillaPrefabsAvailable -= RegisterPieces;

            Jotunn.Logger.LogInfo($"Waterproof pieces registered. Count: {registeredCount}");
        }

        private static int CreateWaterproofPieces(Dictionary<string, int> hammerIndexes)
        {
            int registeredCount = 0;

            var sortedPrefabs = PrefabDiscovery
                .GetPrefabs(WaterproofPrefabRules.IsValidPrefab)
                .OrderBy(prefab =>
                {
                    int index;

                    if (hammerIndexes != null && hammerIndexes.TryGetValue(prefab.name, out index))
                    {
                        return index;
                    }

                    return int.MaxValue;
                })
                .ThenBy(prefab => prefab.name)
                .ToList();

            foreach (var prefab in sortedPrefabs)
            {
                if (CreateWaterproofPiece(prefab))
                {
                    registeredCount++;
                }
            }

            return registeredCount;
        }

        private static bool CreateWaterproofPiece(GameObject vanillaPrefab)
        {
            if (vanillaPrefab == null)
            {
                return false;
            }

            var vanillaPiece = vanillaPrefab.GetComponent<Piece>();

            if (vanillaPiece == null)
            {
                Jotunn.Logger.LogWarning($"{vanillaPrefab.name} has no Piece component. Skipping.");
                return false;
            }

            string customPrefabName = $"{Constants.PrefabPrefix}_{vanillaPrefab.name}";

            GameObject customPrefab = PrefabManager.Instance.CreateClonedPrefab(
                customPrefabName,
                vanillaPrefab.name
            );

            if (customPrefab == null)
            {
                Jotunn.Logger.LogWarning($"Failed to clone prefab: {vanillaPrefab.name}");
                return false;
            }

            ApplyModifiersToCustomPrefab(customPrefab);

            PieceConfig config = BuildPieceConfig(customPrefabName, vanillaPiece);
            var customPiece = new CustomPiece(customPrefab, true, config);

            PieceManager.Instance.AddPiece(customPiece);

            Jotunn.Logger.LogInfo($"Registered waterproof building piece: {customPrefabName}");

            return true;
        }

        private static void ApplyModifiersToCustomPrefab(GameObject customPrefab)
        {
            if (customPrefab == null)
            {
                return;
            }

            var wear = customPrefab.GetComponent<WearNTear>();

            if (wear == null)
            {
                Jotunn.Logger.LogWarning($"{customPrefab.name} has no WearNTear component.");
                return;
            }

            wear.m_noRoofWear = false;
        }

        private static PieceConfig BuildPieceConfig(string customPrefabName, Piece vanillaPiece)
        {
            return new PieceConfig
            {
                Name = customPrefabName,
                Description = "A waterproof building piece that resists water damage.",
                PieceTable = PieceTables.Hammer,
                Category = Constants.WaterproofCategory,
                Requirements = BuildRequirementConfigs(vanillaPiece, new Dictionary<string, int>
                {
                    { ItemList.Resin, 1 }
                })
            };
        }

        private static RequirementConfig[] BuildRequirementConfigs(Piece vanillaPiece, Dictionary<string, int> customRequirements, bool keepVanillaRequirements = true)
        {
            var requirements = new List<RequirementConfig>();

            if (keepVanillaRequirements && vanillaPiece != null && vanillaPiece.m_resources != null)
            {
                foreach (var requirement in vanillaPiece.m_resources)
                {
                    if (requirement == null || requirement.m_resItem == null)
                    {
                        continue;
                    }

                    requirements.Add(new RequirementConfig(
                        requirement.m_resItem.name,
                        requirement.m_amount,
                        requirement.m_amountPerLevel,
                        requirement.m_recover
                    ));
                }
            }

            if (customRequirements != null)
            {
                foreach (var customRequirement in customRequirements)
                {
                    bool alreadyExists = requirements.Any(requirement =>
                        string.Equals(requirement.Item, customRequirement.Key, System.StringComparison.OrdinalIgnoreCase));

                    if (alreadyExists)
                    {
                        continue;
                    }

                    requirements.Add(new RequirementConfig(
                        customRequirement.Key,
                        customRequirement.Value,
                        0,
                        true
                    ));
                }
            }

            return requirements.ToArray();
        }
    }
}