/*using System;
using System.Reflection;
using DubsBadHygiene;
using HarmonyLib;
using Verse;

namespace StagzMerfolk.HarmonyPatches;

[HarmonyPatch]
public class DbhHygieneCleanPatches
{
    [HarmonyPrepare]
    private static bool shouldPatchBadHygene()
    {
        return ModsConfig.IsActive("dubwise.dubsbadhygiene");
    }

    [HarmonyTargetMethod]
    public static MethodInfo TargetMethod()
    {
        return AccessTools.Method("DubsBadHygiene.Need_Hygiene:clean");
    }

    [HarmonyPostfix]
    private static void Postfix(float val, ref Pawn ___pawn)
    {
        if (StagzMerfolkSettings.dbhCleaningCountsAsHydration && ___pawn?.genes != null && ___pawn.genes.HasActiveGene(StagzDefOf.Stagz_Aquatic) && ___pawn.needs.TryGetNeed(StagzDefOf.Stagz_NeedAquatic) != null)
        {
            ___pawn.needs.TryGetNeed(StagzDefOf.Stagz_NeedAquatic).CurLevel = Math.Min(___pawn.needs.TryGetNeed(StagzDefOf.Stagz_NeedAquatic).CurLevel + val, 1f);
        }
    }
}*/
