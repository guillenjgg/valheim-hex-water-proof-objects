using Jotunn.Managers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HexWaterproofBuilding.Discovery
{
    internal static class PrefabDiscovery
    {
        internal static IEnumerable<GameObject> GetPrefabs(Func<GameObject, bool> predicate)
        {
            var prefabs = PrefabManager.Cache.GetPrefabs(typeof(GameObject));

            foreach (var prefab in prefabs)
            {
                var currentPrefab = prefab.Value as GameObject;

                if (currentPrefab == null)
                {
                    continue;
                }

                if (predicate == null || predicate(currentPrefab))
                {
                    yield return currentPrefab;
                }
            }
        }

        internal static Dictionary<string, int> GetHammerPieceIndexes()
        {
            var indexes = new Dictionary<string, int>();
            var hammer = PrefabManager.Cache.GetPrefab(typeof(GameObject),"Hammer") as GameObject;

            if(hammer == null)
            {
                return indexes;
            }

            var itemDrop = hammer.GetComponent<ItemDrop>();

            if(itemDrop == null || itemDrop.m_itemData.m_shared.m_buildPieces == null || itemDrop.m_itemData.m_shared.m_buildPieces.m_pieces == null)
            {
                return indexes;
            }

            var pieces = itemDrop.m_itemData.m_shared.m_buildPieces.m_pieces;
            
            for (int i = 0; i < pieces.Count; i++)
            {
                var prefab = pieces[i];

                if (prefab == null || indexes.ContainsKey(prefab.name))
                {
                    continue;
                }
                
                indexes.Add(prefab.name, i);
            }

            return indexes;
        }
    }
}
