using HarmonyLib;
using System;

namespace HexWaterproofBuilding.Patches
{
    [HarmonyPatch(typeof(WearNTear), nameof(WearNTear.Awake))]
    internal static class PatchWearNTearAwake
    {
        [HarmonyPostfix]
        internal static void Postfix(WearNTear __instance)
        {
            if (__instance == null || Plugin.Instance == null || !Plugin.Instance.IsModEnabled || Plugin.Instance.IsWoodAndBoneStacksTakeRainDamage)
            {
                return;
            }

            var piece = __instance.GetComponent<Piece>();

            if (piece == null)
            {
                return;
            }

            string prefabName = __instance.gameObject.name;

            var isWood = __instance.m_materialType == WearNTear.MaterialType.Wood || __instance.m_materialType == WearNTear.MaterialType.HardWood;

            if (isWood && prefabName.IndexOf("stack", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                __instance.m_noRoofWear = false;
            }
        }
    }
}