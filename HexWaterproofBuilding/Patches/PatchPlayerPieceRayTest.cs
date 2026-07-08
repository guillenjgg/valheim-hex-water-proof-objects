using HarmonyLib;
using HexWaterproofBuilding.Core;
using HexWaterproofBuilding.Utils;
using UnityEngine;

namespace HexWaterproofBuilding.Patches
{
    [HarmonyPatch(typeof(Player), nameof(Player.PieceRayTest))]
    internal static class PatchPlayerPieceRayTest
    {
        private static readonly AccessTools.FieldRef<Player, GameObject> PlacementGhostRef = AccessTools.FieldRefAccess<Player, GameObject>("m_placementGhost");

        private static readonly AccessTools.FieldRef<Player, int> PlaceRayMaskRef = AccessTools.FieldRefAccess<Player, int>("m_placeRayMask");

        private static readonly AccessTools.FieldRef<Player, int> PlaceWaterRayMaskRef = AccessTools.FieldRefAccess<Player, int>("m_placeWaterRayMask");

        private static readonly int WaterLayer = LayerMask.NameToLayer("Water");

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

            GameObject placementGhost = PlacementGhostRef(__instance);

            if (placementGhost == null)
            {
                return true;
            }

            Piece ghostPiece = placementGhost.GetComponent<Piece>();

            if (!PieceUtility.CanUseExtendedPlacementForPiece(ghostPiece))
            {
                return true;
            }

            int rayMask = water ? PlaceWaterRayMaskRef(__instance) : PlaceRayMaskRef(__instance);

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

            waterSurface = WaterLayer >= 0 && hit.collider.gameObject.layer == WaterLayer ? hit.collider : null;

            __result = true;
            return false;
        }
    }
}