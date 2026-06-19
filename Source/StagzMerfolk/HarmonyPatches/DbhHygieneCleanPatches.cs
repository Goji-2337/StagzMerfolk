using System;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace StagzMerfolk.HarmonyPatches;

[HarmonyPatch]
public class DbhHygieneCleanPatches
{
    private static bool Prepare()
    {
        return ModLister.AnyModActiveNoSuffix(["Dubwise.DubsBadHygiene", "Dubwise.DubsBadHygiene.Lite"]);
    }

    private static MethodInfo TargetMethod()
    {
        return AccessTools.Method("DubsBadHygiene.Need_Hygiene:clean");
    }

    private static void Postfix(float val, ref Pawn ___pawn)
    {
        if (StagzMerfolkSettings.dbhCleaningCountsAsHydration && ___pawn?.needs?.TryGetNeed(StagzDefOf.Stagz_NeedAquatic) is {} need)
        {
            need.CurLevel = Math.Min(___pawn.needs.TryGetNeed(StagzDefOf.Stagz_NeedAquatic).CurLevel + val, 1f);
        }
    }
}
