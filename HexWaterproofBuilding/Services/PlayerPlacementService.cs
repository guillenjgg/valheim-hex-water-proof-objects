using HarmonyLib;
using HexWaterproofBuilding.Core;
using UnityEngine;

namespace HexWaterproofBuilding.Services
{
    internal static class PlayerPlacementService
    {
        private const float ForwardAimDistance = 50f;

        internal static bool TryGetPlacementGhost(Player player, out GameObject placementGhost)
        {
            placementGhost = Traverse.Create(player)
                .Field("m_placementGhost")
                .GetValue<GameObject>();

            return ValidateGhost(placementGhost);
        }

        internal static bool IsPlacementStatusNoRayHits(Player player)
        {
            Player.PlacementStatus placementStatus = Traverse.Create(player)
                .Field("m_placementStatus")
                .GetValue<Player.PlacementStatus>();

            return placementStatus == Player.PlacementStatus.NoRayHits;
        }

        internal static bool TryGetPlaceRayMask(Player player, out int placeRayMask)
        {
            placeRayMask = Traverse.Create(player)
                .Field("m_placeRayMask")
                .GetValue<int>();

            return placeRayMask != 0;
        }

        internal static bool TryGetFoundationSnapAimHit(int mask, out RaycastHit aimHit)
        {
            aimHit = default(RaycastHit);

            if (GameCamera.instance == null)
            {
                return false;
            }

            Transform cameraTransform = GameCamera.instance.transform;

            return Physics.Raycast(
                cameraTransform.position,
                cameraTransform.forward,
                out aimHit,
                ForwardAimDistance,
                mask);
        }

        internal static void SetPlacementGhostRotation(Player player, GameObject placementGhost)
        {
            if (player == null || placementGhost == null)
            {
                return;
            }

            int placeRotation = Traverse.Create(player)
                .Field("m_placeRotation")
                .GetValue<int>();

            float rotationDegrees = Traverse.Create(player)
                .Field("m_placeRotationDegrees")
                .GetValue<float>();

            placementGhost.transform.rotation = Quaternion.Euler(
                0f,
                placeRotation * rotationDegrees,
                0f);
        }

        internal static void SetPlacementValid(Player player)
        {
            if (player == null)
            {
                return;
            }

            Traverse.Create(player)
                .Field("m_placementStatus")
                .SetValue(Player.PlacementStatus.Valid);
        }

        internal static void SetPlacementGhostValid(Player player)
        {
            if (player == null)
            {
                return;
            }

            Traverse.Create(player)
                .Method("SetPlacementGhostValid", true)
                .GetValue();
        }

        private static bool ValidateGhost(GameObject placementGhost)
        {
            if (placementGhost == null)
            {
                return false;
            }

            string ghostName = placementGhost.name?.Replace("(Clone)", "");

            return !string.IsNullOrEmpty(ghostName)
                && ghostName.StartsWith($"{Constants.PrefabPrefix}_");
        }
    }
}