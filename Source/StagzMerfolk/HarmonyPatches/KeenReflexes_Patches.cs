using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace StagzMerfolk.HarmonyPatches;

[HarmonyPatch(typeof(Pawn), "SpecialDisplayStats")]
public static class Pawn_SpecialDisplayStats_Patch
{
    private static IEnumerable<StatDrawEntry> Postfix(IEnumerable<StatDrawEntry> __result, Pawn __instance)
    {
        if (__instance != null && __instance.RaceProps.Humanlike && __instance.genes.HasActiveGene(StagzDefOf.Stagz_KeenReflexes))
        {
            var keenReflexesStatDrawEntry = new StatDrawEntry(
                StatCategoryDefOf.PawnCombat,
                "StagzMerfolk_KeenReflexes".Translate(),
                "StagzMerfolk_KeenReflexes_Value".Translate(),
                "StagzMerfolk_KeenReflexes_Description".Translate(),
                410000
            );
            return __result.Concat(keenReflexesStatDrawEntry);
        }

        return __result;
    }
}

[HarmonyPatch(typeof(ShotReport), "AimOnTargetChance_IgnoringPosture", MethodType.Getter)]
public static class ShotReport_AimOnTargetChance_IgnoringPosture_Patch
{
    private static float meleeToRangeCoefficient = StagzDefOf.Stagz_KeenReflexes.HasModExtension<KeenReflexModExtension>() ? StagzDefOf.Stagz_KeenReflexes.GetModExtension<KeenReflexModExtension>().MeleeToRangeCoefficient : 1f;

    private static void Postfix(ref float __result, ref TargetInfo ___target)
    {
        if (___target == null) return;

        var pawn = ___target.Thing as Pawn;
        if (pawn != null && pawn.RaceProps.Humanlike && pawn.genes.HasActiveGene(StagzDefOf.Stagz_KeenReflexes) && __result < 1f)
        {
            __result = Math.Max(__result - (pawn.GetStatValue(StatDefOf.MeleeDodgeChance, true, -1) * meleeToRangeCoefficient), 0.02f);
        }
    }
}

[HarmonyPatch(typeof(ShotReport), "GetTextReadout")]
public static class ShotReport_GetTextReadout_Patch
{
    private static void Postfix(ref string __result, ref TargetInfo ___target)
    {
        if (___target == null) return;

        var pawn = ___target.Thing as Pawn;
        if (pawn != null && pawn.RaceProps.Humanlike && pawn.genes.HasActiveGene(StagzDefOf.Stagz_KeenReflexes))
        {
            __result += "   " + "StagzMerfolk_KeenReflexes".Translate() + " " + (pawn.GetStatValue(StatDefOf.MeleeDodgeChance, true, -1) * 1f).ToStringPercent() + "\n";
        }
    }
}
