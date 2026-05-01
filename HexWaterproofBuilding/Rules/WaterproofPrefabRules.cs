using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HexWaterproofBuilding.Rules
{
    internal static class WaterproofPrefabRules
    {
        internal const string PrefabPrefix = "Hex_Waterproof_";

        private static readonly string LowerPrefabPrefix = PrefabPrefix.ToLower();
        private static readonly string[] InvalidTokens =
        {
            "door",
            "stack",
            "gate",
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

            var prefabName = prefab.name;
            var lowerName = prefabName.ToLower();

            if (lowerName.StartsWith(LowerPrefabPrefix))
            {
                return false;
            }

            bool hasInvalidToken = InvalidTokens.Any(token => lowerName.Contains(token));

            if (hasInvalidToken)
            {
                return false;
            }

            if (piece.m_category != Piece.PieceCategory.BuildingWorkbench)
            {
                return false;
            }

            if (wear.m_materialType != WearNTear.MaterialType.Wood && wear.m_materialType != WearNTear.MaterialType.HardWood)
            {
                return false;
            }

            return true;
        }
    }
}
