using HarmonyLib;
using HexWaterproofBuilding.Core;
using HexWaterproofBuilding.Utils;
using UnityEngine;

namespace HexWaterproofBuilding.Patches
{
    [HarmonyPatch(typeof(Player), nameof(Player.PieceRayTest))]
    internal static class PatchPlayerPieceRayTest
    {
        private static bool Prefix(
            Player __instance,
            ref bool __result,
            out Vector3 point,
            out Vector3 normal,
            out Piece piece,
            out Heightmap heightmap,
            out Collider waterSurface,
            bool water)
        {
            point = Vector3.zero;
            normal = Vector3.zero;
            piece = null;
            heightmap = null;
            waterSurface = null;

            if (__instance == null || !FeatureFlags.CanUseExtendedPlacement())
            {
                return true;
            }

            GameObject placementGhost = Traverse.Create(__instance)
                .Field("m_placementGhost")
                .GetValue<GameObject>();

            if (placementGhost == null)
            {
                return true;
            }

            if (!PieceUtility.IsWaterproofPiece(placementGhost.GetComponent<Piece>()))
            {
                return true;
            }

            int rayMask = water
                ? Traverse.Create(__instance).Field("m_placeWaterRayMask").GetValue<int>()
                : Traverse.Create(__instance).Field("m_placeRayMask").GetValue<int>();

            if (GameCamera.instance == null)
            {
                return true;
            }

            Transform cameraTransform = GameCamera.instance.transform;

            RaycastHit hit;

            if (!Physics.Raycast(
                cameraTransform.position,
                cameraTransform.forward,
                out hit,
                Constants.ExtendedPlacementDistance,
                rayMask))
            {
                __result = false;
                return false;
            }

            if (hit.collider == null || hit.collider.attachedRigidbody != null)
            {
                __result = false;
                return false;
            }

            point = hit.point;
            normal = hit.normal;
            piece = hit.collider.GetComponentInParent<Piece>();
            heightmap = hit.collider.GetComponent<Heightmap>();

            waterSurface = hit.collider.gameObject.layer == LayerMask.NameToLayer("Water")
                ? hit.collider
                : null;

            __result = true;
            return false;
        }
    }
}