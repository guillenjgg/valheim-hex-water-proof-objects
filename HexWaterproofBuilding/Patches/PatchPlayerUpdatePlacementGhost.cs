using HarmonyLib;
using HexWaterproofBuilding.Core;
using UnityEngine;

namespace HexWaterproofBuilding.Patches
{
    [HarmonyPatch(typeof(Player), nameof(Player.UpdatePlacementGhost))]
    internal static class PatchPlayerUpdatePlacementGhost
    {
        private const float RotationStep = 22.5f;

        private static void Postfix(Player __instance)
        {
            if (__instance == null || Plugin.Instance == null || !Plugin.Instance.IsModEnabled)
            {
                return;
            }

            GameObject placementGhost = Traverse.Create(__instance)
                .Field("m_placementGhost")
                .GetValue<GameObject>();

            if (placementGhost == null)
            {
                return;
            }

            string ghostName = placementGhost.name.Replace("(Clone)", "");

            if (!ghostName.StartsWith(Constants.PrefabPrefix + "_"))
            {
                return;
            }

            Player.PlacementStatus status = Traverse.Create(__instance)
                .Field("m_placementStatus")
                .GetValue<Player.PlacementStatus>();

            if (status != Player.PlacementStatus.NoRayHits)
            {
                return;
            }

            Transform cameraTransform = GameCamera.instance.transform;
            Vector3 cameraPosition = cameraTransform.position;
            Vector3 cameraForward = cameraTransform.forward;

            int mask = LayerMask.GetMask("Default", "static_solid", "terrain", "piece", "piece_nonsolid");

            RaycastHit aimHit;
            bool hasAimHit = Physics.Raycast(cameraPosition, cameraForward, out aimHit, 50f, mask);

            if (!hasAimHit)
            {
                return;
            }

            placementGhost.SetActive(true);

            int placeRotation = Traverse.Create(__instance)
                .Field("m_placeRotation")
                .GetValue<int>();

            placementGhost.transform.rotation = Quaternion.Euler(
                0f,
                placeRotation * RotationStep,
                0f);

            Piece targetPiece = aimHit.collider.GetComponentInParent<Piece>();

            if (targetPiece == null)
            {
                targetPiece = FindNearestPiece(aimHit.point, 2f);
            }

            if (targetPiece != null && TrySnapToTargetPiece(placementGhost, targetPiece, aimHit.point))
            {
                SetPlacementValid(__instance);
                return;
            }

            if (TrySnapToFoundation(placementGhost, aimHit.point, mask))
            {
                SetPlacementValid(__instance);
            }
        }

        private static bool TrySnapToTargetPiece(GameObject placementGhost, Piece targetPiece, Vector3 aimPoint)
        {
            Transform targetSnap = FindNearestSnapPoint(targetPiece.transform, aimPoint);

            if (targetSnap == null)
            {
                return false;
            }

            Transform ghostBottomSnap = placementGhost.transform.Find("$hud_snappoint_bottom");

            if (ghostBottomSnap == null)
            {
                return false;
            }

            Vector3 offset = targetSnap.position - ghostBottomSnap.position;
            placementGhost.transform.position += offset;

            return true;
        }

        private static bool TrySnapToFoundation(GameObject placementGhost, Vector3 aimPoint, int mask)
        {
            Transform bottomSnap = placementGhost.transform.Find("$hud_snappoint_bottom");

            if (bottomSnap == null)
            {
                return false;
            }

            RaycastHit foundationHit;
            bool hasFoundationHit = Physics.Raycast(
                aimPoint + Vector3.up,
                Vector3.down,
                out foundationHit,
                100f,
                mask
            );

            if (!hasFoundationHit)
            {
                return false;
            }

            Vector3 offset = foundationHit.point - bottomSnap.position;
            placementGhost.transform.position += offset;

            return true;
        }

        private static Transform FindNearestSnapPoint(Transform root, Vector3 position)
        {
            Transform nearestSnap = null;
            float nearestDistance = float.MaxValue;

            Transform[] children = root.GetComponentsInChildren<Transform>(true);

            foreach (Transform child in children)
            {
                if (child == null || !child.CompareTag("snappoint"))
                {
                    continue;
                }

                float distance = Vector3.Distance(position, child.position);

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestSnap = child;
                }
            }

            return nearestSnap;
        }

        private static void SetPlacementValid(Player player)
        {
            Traverse.Create(player)
                .Field("m_placementStatus")
                .SetValue(Player.PlacementStatus.Valid);
        }

        private static Piece FindNearestPiece(Vector3 position, float radius)
        {
            Collider[] colliders = Physics.OverlapSphere(
                position,
                radius,
                LayerMask.GetMask("piece", "piece_nonsolid"));

            Piece nearestPiece = null;
            float nearestDistance = float.MaxValue;

            foreach (Collider collider in colliders)
            {
                if (collider == null)
                {
                    continue;
                }

                Piece piece = collider.GetComponentInParent<Piece>();

                if (piece == null)
                {
                    continue;
                }

                float distance = Vector3.Distance(position, piece.transform.position);

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestPiece = piece;
                }
            }

            return nearestPiece;
        }
    }
}