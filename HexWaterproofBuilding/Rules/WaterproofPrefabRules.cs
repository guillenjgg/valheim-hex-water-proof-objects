using HexWaterproofBuilding.Core;
using System.Linq;
using UnityEngine;

namespace HexWaterproofBuilding.Rules
{
    internal static class WaterproofPrefabRules
    {
        private static readonly string LowerWaterproofPrefabPrefix = Constants.WaterProofPrefabPrefix.ToLowerInvariant();
        private static readonly string LowerPierPrefabPrefix = Constants.PierPrefabPrefix.ToLowerInvariant();

        private static readonly string[] InvalidTokens =
        {
            "stack",
            "spiral",
            "treasure",
            "dvergr",
            "old_",
            "turf"
        };

        internal static bool IsValidPrefab(GameObject prefab)
        {
            if (prefab == null)
            {
                return false;
            }

            var piece = prefab.GetComponent<Piece>();
            var wear = prefab.GetComponent<WearNTear>();

            if (piece == null || wear == null)
            {
                return false;
            }

            var lowerName = prefab.name.ToLowerInvariant();

            if (lowerName.StartsWith(LowerWaterproofPrefabPrefix) || lowerName.StartsWith(LowerPierPrefabPrefix))
            {
                return false;
            }

            bool hasInvalidToken = InvalidTokens.Any(token => lowerName.Contains(token));

            if (hasInvalidToken)
            {
                return false;
            }

            if (piece.name != "piece_workbench" && piece.m_category != Piece.PieceCategory.BuildingWorkbench)
            {
                return false;
            }

            return wear.m_materialType == WearNTear.MaterialType.Wood ||
                   wear.m_materialType == WearNTear.MaterialType.HardWood;
        }
    }
}
