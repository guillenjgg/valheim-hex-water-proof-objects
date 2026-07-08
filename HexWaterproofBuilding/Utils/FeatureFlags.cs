namespace HexWaterproofBuilding.Utils
{
    internal static class FeatureFlags
    {
        internal static bool CanUseExtendedPlacement()
        {
            return Plugin.Instance != null 
                && Plugin.Instance.IsModEnabled 
                && Plugin.Instance.IsExtendedPlacementRangeEnabled;
        }

        internal static bool CanUseExtendedPlacementForVanillaPieces()
        {
            return CanUseExtendedPlacement()
                && Plugin.Instance.IsExtendedPlacementForVanillaPiecesEnabled;
        }
    }
}
