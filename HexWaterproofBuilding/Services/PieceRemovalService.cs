using HarmonyLib;
using HexWaterproofBuilding.Core;
using UnityEngine;

namespace HexWaterproofBuilding.Services
{
    internal static class PieceRemovalService
    {
        internal const float HoverDistance = 50f;

        internal static bool TryApplyLongRangeHover(Player player)
        {
            if (player == null || Plugin.Instance == null || !Plugin.Instance.IsModEnabled)
            {
                return false;
            }

            if (!player.InPlaceMode())
            {
                return false;
            }

            Piece currentHoveringPiece = Traverse.Create(player)
                .Field("m_hoveringPiece")
                .GetValue<Piece>();

            if (currentHoveringPiece != null)
            {
                return false;
            }

            if (GameCamera.instance == null)
            {
                return false;
            }

            int removeRayMask = Traverse.Create(player)
                .Field("m_removeRayMask")
                .GetValue<int>();

            if (removeRayMask == 0)
            {
                return false;
            }

            Transform cameraTransform = GameCamera.instance.transform;

            RaycastHit hit;
            bool hasHit = Physics.Raycast(
                cameraTransform.position,
                cameraTransform.forward,
                out hit,
                HoverDistance,
                removeRayMask);

            if (!hasHit || hit.collider == null)
            {
                return false;
            }

            Piece piece = hit.collider.GetComponentInParent<Piece>();

            if (!IsWaterproofPiece(piece))
            {
                return false;
            }

            Traverse.Create(player)
                .Field("m_hoveringPiece")
                .SetValue(piece);

            WearNTear wearNTear = piece.GetComponent<WearNTear>();

            if (wearNTear != null)
            {
                wearNTear.Highlight();
            }

            return true;
        }

        private static bool IsWaterproofPiece(Piece piece)
        {
            if (piece == null)
            {
                return false;
            }

            string pieceName = piece.name?.Replace("(Clone)", "");

            return !string.IsNullOrEmpty(pieceName)
                && pieceName.StartsWith($"{Constants.PrefabPrefix}_");
        }
    }
}