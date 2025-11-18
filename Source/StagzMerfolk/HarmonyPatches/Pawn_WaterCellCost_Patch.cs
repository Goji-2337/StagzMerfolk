using HarmonyLib;
using Verse;

namespace StagzMerfolk.HarmonyPatches;

[HarmonyPatch(typeof(Pawn), nameof(Pawn.WaterCellCost), MethodType.Getter)]
public static class Pawn_WaterCellCost_Patch
{
    private static void Postfix(Pawn __instance, ref int? __result)
    {
        if (__instance.genes != null && __instance.genes.HasActiveGene(StagzDefOf.Stagz_Aquatic))
        {
            __result = 1;
        }
    }
}
