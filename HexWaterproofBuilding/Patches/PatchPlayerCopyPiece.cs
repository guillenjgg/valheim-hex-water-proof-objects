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
        private static readonly AccessTools.FieldRef<Player, int> RemoveRayMaskRef = AccessTools.FieldRefAccess<Player, int>("m_removeRayMask");

        private static readonly AccessTools.FieldRef<Player, float> PlaceRotationDegreesRef = AccessTools.FieldRefAccess<Player, float>("m_placeRotationDegrees");

        private static readonly AccessTools.FieldRef<Player, int> PlaceRotationRef = AccessTools.FieldRefAccess<Player, int>("m_placeRotation");

        private static readonly FastInvokeHandler SetSelectedPieceRef = MethodInvoker.GetHandler(AccessTools.Method(
            typeof(Player),
            "SetSelectedPiece",
            new[] { typeof(Piece) }));

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

            Transform cameraTransform = GameCamera.instance.transform;

            RaycastHit hit;
            bool hasHit = Physics.Raycast(
                cameraTransform.position,
                cameraTransform.forward,
                out hit,
                Constants.ExtendedPlacementDistance,
                RemoveRayMaskRef(__instance));

            if (!hasHit || hit.collider == null)
            {
                __result = false;
                return false;
            }

            Piece piece = hit.collider.GetComponentInParent<Piece>();

            if (piece == null && hit.collider.GetComponent<Heightmap>() != null)
            {
                piece = TerrainModifier.FindClosestModifierPieceInRange(
                    hit.point,
                    Constants.ClosestRangeModifier);
            }

            if (!PieceUtility.CanUseExtendedPlacementForPiece(piece))
            {
                return true;
            }

            bool selected = (bool)SetSelectedPieceRef(__instance, piece);

            if (!selected)
            {
                __instance.Message(MessageHud.MessageType.Center, "$msg_missingrequirement");
                __result = false;
                return false;
            }

            float placeRotationDegrees = PlaceRotationDegreesRef(__instance);

            if (placeRotationDegrees <= 0f)
            {
                __result = true;
                return false;
            }

            Quaternion rotation = piece.transform.rotation;
            PlaceRotationRef(__instance) = (int)Math.Round(rotation.eulerAngles.y / placeRotationDegrees);

            __result = true;
            return false;
        }
    }
}