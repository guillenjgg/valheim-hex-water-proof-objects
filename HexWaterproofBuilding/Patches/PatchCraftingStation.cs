using HarmonyLib;
using HexWaterproofBuilding;

[HarmonyPatch(typeof(CraftingStation), nameof(CraftingStation.Start))]
internal static class PatchCraftingStation
{
    [HarmonyPrefix]
    internal static void Prefix(CraftingStation __instance)
    {
        if(Plugin.Instance == null)
        {
            return;
        }

        var prefabName = Utils.GetPrefabName(__instance.gameObject);

        if (prefabName.Equals("piece_workbench"))
        {
            __instance.m_craftRequireRoof = false;
        }
    }
}