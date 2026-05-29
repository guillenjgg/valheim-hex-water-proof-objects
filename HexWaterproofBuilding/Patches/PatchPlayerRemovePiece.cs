using HarmonyLib;
using HexWaterproofBuilding.Services;

namespace HexWaterproofBuilding.Patches
{
    [HarmonyPatch(typeof(Player), nameof(Player.RemovePiece))]
    internal static class PatchPlayerRemovePiece
    {
        private static bool Prefix(Player __instance, ref bool __result)
        {
            if (PieceRemovalService.TryRemoveLongRangeWaterproofPiece(__instance))
            {
                __result = true;
                return false;
            }

            return true;
        }
    }
}