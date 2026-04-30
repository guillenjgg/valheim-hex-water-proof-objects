using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HexWaterproofBuilding
{
    internal static class WaterproofPieceRegistrar
    {
        private static readonly HashSet<string> ExcludedNameFragments = new HashSet<string>
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

        public static void TryRegistrar(ZNetScene zNetScene)
        {
            if (zNetScene == null)
            {
                return;
            }

            List<GameObject> allPrefabs = zNetScene.m_prefabs;

            if (allPrefabs == null || allPrefabs.Count == 0)
            {
                return;
            }

            var woodPrefabs = new List<GameObject>();

            foreach (var fab in allPrefabs)
            {
                if (fab == null)
                {
                    continue;
                }

                string nameLower = fab.name.ToLowerInvariant();

                if (nameLower.StartsWith("hex_"))
                {
                    continue;
                }

                var piece = fab.GetComponent<Piece>();
                if (piece == null || piece.m_category != Piece.PieceCategory.BuildingWorkbench)
                {
                    continue;
                }

                var wear = fab.GetComponent<WearNTear>();
                if (wear == null)
                {
                    continue;
                }

                bool isWoodMaterial = 
                    wear.m_materialType == WearNTear.MaterialType.HardWood || 
                    wear.m_materialType == WearNTear.MaterialType.Wood;

                if (!isWoodMaterial)
                {
                    continue;
                }

                bool isExcluded = ExcludedNameFragments.Any(fragment => nameLower.Contains(fragment));

                if (isExcluded)
                {
                    continue;
                }

                woodPrefabs.Add(fab);
            }

            if (woodPrefabs.Any())
            {
                foreach (var item in woodPrefabs)
                {
                    Plugin.Log.LogInfo($"Wood prefab: {item.name}");
                }
            }
            else
            {
                Plugin.Log.LogWarning("No Wood build prefabs found.");
            }
        }
    }
}