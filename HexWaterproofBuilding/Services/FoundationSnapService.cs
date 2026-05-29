using System;
using System.Collections.Generic;
using UnityEngine;

namespace HexWaterproofBuilding.Services
{
    internal static class FoundationSnapService
    {
        private const float NearbySnapPointSearchRadius = 10f;
        private const float MaxSnapDistance = 0.5f;
        private const float PieceAimSphereRadius = 0.45f;
        private const float PieceAimDistance = 50f;

        internal static bool TrySnapPlacementGhost(GameObject placementGhost, RaycastHit aimHit, int mask)
        {
            if (placementGhost == null)
            {
                return false;
            }

            Piece ghostPiece = placementGhost.GetComponent<Piece>();

            if (ghostPiece == null)
            {
                return false;
            }

            Vector3 placementPoint = aimHit.point;
            Vector3 placementNormal = aimHit.normal;

            Jotunn.Logger.LogInfo(
                $"Aim Hit | Collider: {aimHit.collider?.name} | Point: {aimHit.point} | Normal: {aimHit.normal}");

            if (TryGetSnapAnchorFromPieceAim(
                placementGhost,
                mask,
                out Transform targetSnapPoint))
            {
                placementPoint = targetSnapPoint.position;
                placementNormal = Vector3.up;

                Jotunn.Logger.LogInfo(
                    $"Using nearest target snap point | SnapPoint: {targetSnapPoint.name} | Point: {targetSnapPoint.position}");
            }
            else
            {
                Jotunn.Logger.LogInfo("Using aim hit as placement anchor.");
            }

            ApplyInitialGhostPlacement(
                placementGhost,
                placementPoint,
                placementNormal);

            TryApplyVanillaStyleSnapCorrection(
                placementGhost,
                ghostPiece);

            return true;
        }

        private static bool TryGetSnapAnchorFromPieceAim(
            GameObject placementGhost,
            int mask,
            out Transform targetSnapPoint)
        {
            targetSnapPoint = null;

            if (!TryGetPieceAimHit(
                placementGhost,
                mask,
                out RaycastHit pieceAimHit,
                out Piece targetPiece))
            {
                Jotunn.Logger.LogInfo("Piece Aim Hit | none");
                return false;
            }

            Jotunn.Logger.LogInfo(
                $"Piece Aim Hit | Collider: {pieceAimHit.collider?.name} | Piece: {targetPiece.name} | Point: {pieceAimHit.point} | Normal: {pieceAimHit.normal}");

            if (!TryGetNearestSnapPoint(
                targetPiece,
                pieceAimHit.point,
                out targetSnapPoint))
            {
                Jotunn.Logger.LogInfo(
                    $"Nearest Target Snap Point | none | Piece: {targetPiece.name}");

                return false;
            }

            Jotunn.Logger.LogInfo(
                $"Nearest Target Snap Point | Piece: {targetPiece.name} | SnapPoint: {targetSnapPoint.name} | Point: {targetSnapPoint.position}");

            return true;
        }

        private static bool TryGetPieceAimHit(GameObject placementGhost, int mask, out RaycastHit pieceHit, out Piece targetPiece)
        {
            pieceHit = default(RaycastHit);
            targetPiece = null;

            Camera camera = Camera.main;

            if (camera == null)
            {
                Jotunn.Logger.LogInfo("Piece Aim Hit | Camera.main was null.");
                return false;
            }

            Transform cameraTransform = camera.transform;

            RaycastHit[] hits = Physics.SphereCastAll(
                cameraTransform.position,
                PieceAimSphereRadius,
                cameraTransform.forward,
                PieceAimDistance,
                mask);

            if (hits == null || hits.Length == 0)
            {
                Jotunn.Logger.LogInfo("Piece Aim SphereCast returned no hits.");
                return false;
            }

            Jotunn.Logger.LogInfo($"Piece Aim SphereCast returned {hits.Length} hits.");

            Array.Sort(hits, (left, right) => left.distance.CompareTo(right.distance));

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider == null)
                {
                    continue;
                }

                Piece piece = hit.collider.GetComponentInParent<Piece>(true);

                Jotunn.Logger.LogInfo(
                    $"SphereCast Hit | Collider: {hit.collider.name} | Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)} | Point: {hit.point} | Distance: {hit.distance}");

                Jotunn.Logger.LogInfo(
                    $"Collider Hierarchy | Transform: {hit.collider.transform.name} | Parent: {(hit.collider.transform.parent != null ? hit.collider.transform.parent.name : "null")} | Root: {hit.collider.transform.root.name}");

                Jotunn.Logger.LogInfo(
                    $"SphereCast Piece Lookup | Collider: {hit.collider.name} | Piece: {(piece != null ? piece.name : "none")}");

                Component[] parentComponents =
                    hit.collider.GetComponentsInParent<Component>(true);

                foreach (Component parentComponent in parentComponents)
                {
                    if (parentComponent == null)
                    {
                        continue;
                    }

                    Jotunn.Logger.LogInfo(
                        $"Parent Chain Component | GameObject: {parentComponent.gameObject.name} | Component: {parentComponent.GetType().Name}");
                }

                if (piece == null)
                {
                    continue;
                }

                if (piece.gameObject == placementGhost || piece.transform.root == placementGhost.transform.root)
                {
                    Jotunn.Logger.LogInfo(
                        $"Piece Aim Hit rejected because it was the placement ghost | Piece: {piece.name}");

                    continue;
                }

                pieceHit = hit;
                targetPiece = piece;

                Jotunn.Logger.LogInfo(
                    $"Piece Aim Hit accepted | Collider: {hit.collider.name} | Piece: {piece.name} | Point: {hit.point} | Normal: {hit.normal}");

                return true;
            }

            Jotunn.Logger.LogInfo("Piece Aim Hit | none");

            return false;
        }

        private static void ApplyInitialGhostPlacement(
            GameObject placementGhost,
            Vector3 placementPoint,
            Vector3 placementNormal)
        {
            Collider[] colliders = placementGhost.GetComponentsInChildren<Collider>();

            if (colliders.Length == 0)
            {
                placementGhost.transform.position = placementPoint;
                return;
            }

            placementGhost.transform.position = placementPoint + placementNormal * 50f;

            Vector3 closestPoint = Vector3.zero;
            float closestDistance = float.MaxValue;
            bool foundColliderPoint = false;

            foreach (Collider collider in colliders)
            {
                if (collider == null || collider.isTrigger || !collider.enabled)
                {
                    continue;
                }

                MeshCollider meshCollider = collider as MeshCollider;

                if (meshCollider != null && !meshCollider.convex)
                {
                    continue;
                }

                Vector3 colliderPoint = collider.ClosestPoint(placementPoint);
                float distance = Vector3.Distance(colliderPoint, placementPoint);

                if (distance < closestDistance)
                {
                    closestPoint = colliderPoint;
                    closestDistance = distance;
                    foundColliderPoint = true;
                }
            }

            if (!foundColliderPoint)
            {
                placementGhost.transform.position = placementPoint;
                return;
            }

            Vector3 offset = placementGhost.transform.position - closestPoint;

            placementGhost.transform.position = placementPoint + offset;
        }

        private static bool TryApplyVanillaStyleSnapCorrection(
            GameObject placementGhost,
            Piece ghostPiece)
        {
            List<Transform> ghostSnapPoints = new List<Transform>();
            ghostPiece.GetSnapPoints(ghostSnapPoints);

            if (ghostSnapPoints.Count == 0)
            {
                Jotunn.Logger.LogInfo("Snap Correction | Ghost snap points: none");
                return false;
            }

            List<Transform> worldSnapPoints = new List<Transform>();
            List<Piece> nearbyPieces = new List<Piece>();

            Piece.GetSnapPoints(
                placementGhost.transform.position,
                NearbySnapPointSearchRadius,
                worldSnapPoints,
                nearbyPieces);

            if (worldSnapPoints.Count == 0)
            {
                Jotunn.Logger.LogInfo("Snap Correction | World snap points: none");
                return false;
            }

            Transform bestGhostSnap = null;
            Transform bestWorldSnap = null;
            float bestDistance = float.MaxValue;

            foreach (Transform ghostSnap in ghostSnapPoints)
            {
                if (ghostSnap == null)
                {
                    continue;
                }

                foreach (Transform worldSnap in worldSnapPoints)
                {
                    if (worldSnap == null)
                    {
                        continue;
                    }

                    if (worldSnap.root == placementGhost.transform.root)
                    {
                        continue;
                    }

                    float distance = Vector3.Distance(
                        ghostSnap.position,
                        worldSnap.position);

                    if (distance > MaxSnapDistance || distance >= bestDistance)
                    {
                        continue;
                    }

                    bestDistance = distance;
                    bestGhostSnap = ghostSnap;
                    bestWorldSnap = worldSnap;
                }
            }

            if (bestGhostSnap == null || bestWorldSnap == null)
            {
                Jotunn.Logger.LogInfo(
                    $"Snap Correction | No snap pair found | GhostSnapCount: {ghostSnapPoints.Count} | WorldSnapCount: {worldSnapPoints.Count}");

                return false;
            }

            Vector3 snappedPosition =
                bestWorldSnap.position - (bestGhostSnap.position - placementGhost.transform.position);

            bool overlaps = IsOverlappingOtherPiece(
                snappedPosition,
                placementGhost.transform.rotation,
                placementGhost.name,
                nearbyPieces,
                ghostPiece.m_allowRotatedOverlap);

            Jotunn.Logger.LogInfo(
                $"Applied snap correction candidate | GhostSnap: {bestGhostSnap.name} | WorldSnap: {bestWorldSnap.name} | Distance: {bestDistance} | Overlaps: {overlaps}");

            if (overlaps)
            {
                return false;
            }

            placementGhost.transform.position = snappedPosition;

            Jotunn.Logger.LogInfo(
                $"Applied snap correction | GhostSnap: {bestGhostSnap.name} | WorldSnap: {bestWorldSnap.name} | Distance: {bestDistance}");

            return true;
        }

        private static bool IsOverlappingOtherPiece(
            Vector3 position,
            Quaternion rotation,
            string pieceName,
            List<Piece> nearbyPieces,
            bool allowRotatedOverlap)
        {
            foreach (Piece piece in nearbyPieces)
            {
                if (piece == null)
                {
                    continue;
                }

                Transform pieceTransform = piece.transform;

                if (Vector3.Distance(position, pieceTransform.position) >= 0.05f)
                {
                    continue;
                }

                if (allowRotatedOverlap && Quaternion.Angle(pieceTransform.rotation, rotation) > 10f)
                {
                    continue;
                }

                if (piece.gameObject.name.CustomStartsWith(pieceName))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool TryGetNearestSnapPoint(
            Piece piece,
            Vector3 point,
            out Transform nearestSnapPoint)
        {
            nearestSnapPoint = null;

            if (piece == null)
            {
                return false;
            }

            List<Transform> snapPoints = new List<Transform>();
            piece.GetSnapPoints(snapPoints);

            float closestDistance = float.MaxValue;

            foreach (Transform snapPoint in snapPoints)
            {
                if (snapPoint == null)
                {
                    continue;
                }

                float distance = Vector3.Distance(point, snapPoint.position);

                Jotunn.Logger.LogInfo(
                    $"Target Snap Candidate | Piece: {piece.name} | SnapPoint: {snapPoint.name} | Point: {snapPoint.position} | DistanceToAim: {distance}");

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearestSnapPoint = snapPoint;
                }
            }

            return nearestSnapPoint != null;
        }
    }
}