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
    }
}
