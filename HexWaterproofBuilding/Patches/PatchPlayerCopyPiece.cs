using HarmonyLib;
using HexWaterproofBuilding.Core;
using HexWaterproofBuilding.Utils;
using System;
using UnityEngine;

namespace HexWaterproofBuilding.Patches
{
    [HarmonyPatch(typeof(Player), nameof(Player.CopyPiece))]
    internal static class PatchPlayerCopyPiece
    {
        [HarmonyPrefix]
        private static bool Prefix(Player __instance, ref bool __result)
        {
            if (__instance == null || !FeatureFlags.CanUseExtendedPlacement())
            {
                return true;
            }

            if (GameCamera.instance == null || GameCamera.instance.transform == null)
            {
                return true;
            }

            var origin = GameCamera.instance.transform.position;
            var direction = GameCamera.instance.transform.forward;

            var mask = Traverse.Create(__instance)
                .Field("m_removeRayMask")
                .GetValue<int>();

            RaycastHit hit;
            var hasHit = Physics.Raycast(origin, direction, out hit, Constants.ExtendedPlacementDistance, mask);

            if (!hasHit || hit.collider == null)
            {
                __result = false;
                return false;
            }

            var piece = hit.collider.GetComponentInParent<Piece>();

            if (piece == null && hit.collider.GetComponent<Heightmap>() != null)
            {
                piece = TerrainModifier.FindClosestModifierPieceInRange(
                    hit.point,
                    Constants.ClosestRangeModifier);
            }

            if (!PieceUtility.IsWaterproofPiece(piece))
            {
                return true;
            }

            var selected = Traverse.Create(__instance)
                .Method("SetSelectedPiece", new object[] { piece })
                .GetValue<bool>();

            if (!selected)
            {
                __instance.Message(MessageHud.MessageType.Center, "$msg_missingrequirement");
                __result = false;
                return false;
            }

            var rotation = piece.transform.rotation;

            var placeRotationDegrees = Traverse.Create(__instance)
                .Field("m_placeRotationDegrees")
                .GetValue<float>();

            Traverse.Create(__instance)
                .Field("m_placeRotation")
                .SetValue((int)Math.Round(rotation.eulerAngles.y / placeRotationDegrees));

            __result = true;
            return false;
        }
    }
}