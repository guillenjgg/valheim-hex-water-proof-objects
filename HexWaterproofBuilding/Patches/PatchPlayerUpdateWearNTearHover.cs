using HarmonyLib;
using HexWaterproofBuilding.Core;
using HexWaterproofBuilding.Utils;
using UnityEngine;

namespace HexWaterproofBuilding.Patches
{
    [HarmonyPatch(typeof(Player), nameof(Player.UpdateWearNTearHover))]
    internal static class PatchPlayerUpdateWearNTearHover
    {
        private static void Postfix(Player __instance)
        {
            if(__instance == null || !FeatureFlags.CanUseExtendedPlacement())
            {
                return;
            }

            if (!__instance.InPlaceMode())
            {
                return;
            }

            if (GameCamera.instance == null || GameCamera.instance.transform == null)
            {
                return;
            }

            RaycastHit hit;
            var origin = GameCamera.instance.transform.position;
            var direction = GameCamera.instance.transform.forward;
            
            var mask = Traverse.Create(__instance)
                .Field("m_removeRayMask")
                .GetValue<int>();

            var hasHit = Physics.Raycast(origin, direction, out hit, Constants.ExtendedHoverDistance, mask);

            if (!hasHit || hit.collider == null)
            {
                return;
            }

            Piece piece = hit.collider.GetComponentInParent<Piece>();

            if (!PieceUtility.IsWaterproofPiece(piece))
            {
                return;
            }

            Traverse.Create(__instance)
                .Field("m_hoveringPiece")
                .SetValue(piece);

            var wearNTear = piece.GetComponent<WearNTear>();

            if (wearNTear == null)
            {
                return;
            }

            wearNTear.Highlight();
        }
    }
}