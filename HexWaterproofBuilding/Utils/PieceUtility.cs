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
    }
}
