using UnityEngine;

namespace HexWaterproofBuilding.Services
{
    internal static class FoundationSnapService
    {
        private const float DropDistance = 100f;
        private const float NearbyPieceSearchRadius = 2f;
        private const string BottomSnapPoint = "$hud_snappoint_bottom";

        internal static bool TrySnapPlacementGhost(GameObject placementGhost, RaycastHit aimHit, int mask)
        {
            if (placementGhost == null)
            {
                return false;
            }

            Piece targetPiece = aimHit.collider != null
                ? aimHit.collider.GetComponentInParent<Piece>()
                : null;

            if (targetPiece == null)
            {
                targetPiece = FindNearestPiece(aimHit.point, NearbyPieceSearchRadius);
            }

            if (targetPiece != null && TrySnapToTargetPiece(placementGhost, targetPiece, aimHit.point))
            {
                return true;
            }

            return TrySnapToFoundation(placementGhost, aimHit.point, mask);
        }

        private static bool TrySnapToTargetPiece(GameObject placementGhost, Piece targetPiece, Vector3 aimPoint)
        {
            Transform targetSnap = FindNearestSnapPoint(targetPiece.transform, aimPoint);

            if (targetSnap == null)
            {
                return false;
            }

            Transform ghostBottomSnap = placementGhost.transform.Find(BottomSnapPoint);

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
            Transform bottomSnap = placementGhost.transform.Find(BottomSnapPoint);

            if (bottomSnap == null)
            {
                return false;
            }

            RaycastHit foundationHit;
            bool hasFoundationHit = Physics.Raycast(
                aimPoint + Vector3.up,
                Vector3.down,
                out foundationHit,
                DropDistance,
                mask);

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
