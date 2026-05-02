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

            int registeredCount = CreateWaterproofPieces();

            _registered = true;
            PrefabManager.OnVanillaPrefabsAvailable -= RegisterPieces;

            Plugin.Log.LogInfo($"Waterproof pieces registered. Count: {registeredCount}");
        }

        private static int CreateWaterproofPieces()
        {
            int registeredCount = 0;

            foreach (var prefab in PrefabDiscovery.GetPrefabs(WaterproofPrefabRules.IsValidPrefab))
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
                Plugin.Log.LogWarning($"{vanillaPrefab.name} has no Piece component. Skipping.");
                return false;
            }

            string customPrefabName = WaterproofPrefabRules.PrefabPrefix + vanillaPrefab.name;

            GameObject customPrefab = PrefabManager.Instance.CreateClonedPrefab(
                customPrefabName,
                vanillaPrefab.name
            );

            if (customPrefab == null)
            {
                Plugin.Log.LogWarning($"Failed to clone prefab: {vanillaPrefab.name}");
                return false;
            }

            ApplyModifiersToCustomPrefab(customPrefab);

            var requirements = BuildRequirementConfigs(
                vanillaPiece,
                new Dictionary<string, int>
                {
                    { ItemList.Resin, 1 }
                }
            );

            var config = new PieceConfig
            {
                Name = $"Waterproof {vanillaPiece.m_name}",
                Description = "A waterproof building piece that resists water damage.",
                PieceTable = PieceTables.Hammer,
                Category = PieceCategories.Building,
                Requirements = requirements
            };

            var customPiece = new CustomPiece(customPrefab, true, config);

            PieceManager.Instance.AddPiece(customPiece);

            Plugin.Log.LogInfo($"Registered waterproof piece: {customPrefabName}");

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
                Plugin.Log.LogWarning($"{customPrefab.name} has no WearNTear component.");
                return;
            }

            wear.m_noRoofWear = false;
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
                    bool alreadyExists = requirements.Any(requirement => string.Equals(requirement.Item, customRequirement.Key, System.StringComparison.OrdinalIgnoreCase));

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