using HexWaterproofBuilding.Core;

namespace HexWaterproofBuilding.Utils
{
    internal static class PieceUtility
    {
        internal static bool IsWaterproofPiece(Piece piece)
        {
            if (piece == null)
            {
                return false;
            }

            string pieceName = piece.name.Replace("(Clone)", "");

            return pieceName.StartsWith(Constants.PrefabPrefix + "_");
        }

        internal static bool CanUseExtendedPlacementForPiece(Piece piece)
        {
            if (piece == null)
            {
                return false;
            }

            if (IsWaterproofPiece(piece))
            {
                return true;
            }

            if (!FeatureFlags.CanUseExtendedPlacementForVanillaPieces())
            {
                return false;
            }

            return IsVanillaBuildingPiece(piece);
        }

        private static bool IsVanillaBuildingPiece(Piece piece)
        {
            if (piece == null)
            {
                return false;
            }

            string pieceName = piece.name.Replace("(Clone)", "");

            if (pieceName.StartsWith(Constants.PrefabPrefix + "_"))
            {
                return false;
            }

            bool isWorkbench = pieceName == "piece_workbench";
            bool isBuildingPiece = piece.m_category == Piece.PieceCategory.BuildingWorkbench;

            return isWorkbench || isBuildingPiece;
        }
    }
}
