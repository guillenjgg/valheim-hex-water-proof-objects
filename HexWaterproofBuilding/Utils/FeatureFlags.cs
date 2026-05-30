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
    }
}
