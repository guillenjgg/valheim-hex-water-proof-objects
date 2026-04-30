using HarmonyLib;

namespace HexWaterproofBuilding.Patches
{
    [HarmonyPatch(typeof(ZNetScene), "Awake")]
    public static class ZnetSceneAwakePatch
    {

        [HarmonyPostfix]
        private static void Postfix(ZNetScene __instance)
        {
            WaterproofPieceRegistrar.TryRegistrar(__instance);
        }
    }
}
