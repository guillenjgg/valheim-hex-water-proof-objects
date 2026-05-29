using HarmonyLib;
using HexWaterproofBuilding.Services;

namespace HexWaterproofBuilding.Patches
{
    [HarmonyPatch(typeof(Player), nameof(Player.UpdateWearNTearHover))]
    internal static class PatchPlayerUpdateWearNTearHover
    {
        private static void Postfix(Player __instance)
        {
            PieceRemovalService.TryApplyLongRangeHover(__instance);
        }
    }
}