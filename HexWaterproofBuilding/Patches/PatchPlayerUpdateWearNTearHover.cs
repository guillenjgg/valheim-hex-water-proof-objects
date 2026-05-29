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
            if (__instance == null || Plugin.Instance == null || !Plugin.Instance.IsModEnabled)
            {
                return;
            }

            var hoveringPieceField = Traverse.Create(__instance).Field("m_hoveringPiece");

            if (!__instance.InPlaceMode())
            {
                hoveringPieceField.SetValue(null);
                return;
            }

            hoveringPieceField.SetValue(null);

            if(GameCamera.instance == null || GameCamera.instance.transform == null)
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

            hoveringPieceField.SetValue(piece);

            var wearNTear = piece.GetComponent<WearNTear>();

            if(wearNTear == null)
            {
                return;
            }

            wearNTear.Highlight();
        }
    }
}