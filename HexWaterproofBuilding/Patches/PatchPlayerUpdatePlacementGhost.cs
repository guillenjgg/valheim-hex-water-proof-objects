// PatchPlayerUpdatePlacementGhost.cs
using HarmonyLib;
using HexWaterproofBuilding.Services;
using UnityEngine;

namespace HexWaterproofBuilding.Patches
{
    [HarmonyPatch(typeof(Player), nameof(Player.UpdatePlacementGhost))]
    internal static class PatchPlayerUpdatePlacementGhost
    {
        private static void Postfix(Player __instance)
        {
            if (__instance == null || Plugin.Instance == null || !Plugin.Instance.IsModEnabled)
            {
                return;
            }

            if (!PlayerPlacementService.TryGetPlacementGhost(__instance, out GameObject placementGhost))
            {
                return;
            }

            if (!PlayerPlacementService.IsPlacementStatusNoRayHits(__instance))
            {
                return;
            }

            if (!PlayerPlacementService.TryGetPlaceRayMask(__instance, out int mask))
            {
                return;
            }

            if (!PlayerPlacementService.TryGetFoundationSnapAimHit(mask, out RaycastHit aimHit))
            {
                return;
            }

            placementGhost.SetActive(true);

            PlayerPlacementService.SetPlacementGhostRotation(__instance, placementGhost);

            if (FoundationSnapService.TrySnapPlacementGhost(placementGhost, aimHit, mask))
            {
                PlayerPlacementService.SetPlacementValid(__instance);
                PlayerPlacementService.SetPlacementGhostValid(__instance);
            }
        }
    }
}