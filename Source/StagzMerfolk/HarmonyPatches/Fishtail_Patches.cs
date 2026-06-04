using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace StagzMerfolk.HarmonyPatches;

public static class TailHelpers
{
    public static BodyPartGroupDef[] LegsOrFeetGroups = new[] { BodyPartGroupDefOf.Legs, StagzDefOf.Feet };

    public static bool GroupsContainsLegsOrFeet(this List<BodyPartGroupDef> bodyPartGroups)
    {
        return bodyPartGroups.Exists(group => LegsOrFeetGroups.Contains(group));
    }

    public static bool CoversMoreThanJustLegs(this List<BodyPartGroupDef> bodyPartGroups)
    {
        return bodyPartGroups.Any(group => !LegsOrFeetGroups.Contains(group));
    }
}

[HarmonyPatch(typeof(ApparelProperties), "PawnCanWear", new Type[] { typeof(Pawn), typeof(bool) })]
public static class ApparelProperties_PawnCanWear_FishtailPatch
{
    private static bool Prefix(Pawn pawn, ref bool __result, ApparelProperties __instance)
    {
        // Log.Message(__instance);
        if (pawn.genes != null && pawn.genes.GetFirstGeneOfType<Stagz_Gene_Tail_Fish>() != null && !__instance.bodyPartGroups.CoversMoreThanJustLegs())
        {
            __result = false;
            return false;
        }

        // Log.Message("can wear, continue");
        return true;
    }
}

[HarmonyPatch(typeof(Pawn_ApparelTracker), "Wear")]
public static class Pawn_ApparelTracker_Wear_FishtailPatch
{
    private static bool Prefix(Pawn ___pawn, Apparel newApparel)
    {
        if (___pawn.genes != null && ___pawn.genes.GetFirstGeneOfType<Stagz_Gene_Tail_Fish>() != null && !newApparel.def.apparel.bodyPartGroups.CoversMoreThanJustLegs())
        {
            // Log.Message("trying to wear: " + newApparel.LabelShort);
            Messages.Message("StagzMerfolk_CannotWearBecauseOfTail".Translate(___pawn.LabelShort), MessageTypeDefOf.NeutralEvent);
            return false;
        }

        return true;
    }
}

[HarmonyPatch(typeof(PawnGenerator), "GeneratePawn", new Type[] { typeof(PawnGenerationRequest) })]
public static class PawnGenerator_GeneratePawn_FishtailPatch
{
    public static void Postfix(Pawn __result)
    {
        if (__result.genes != null && __result.genes.GetFirstGeneOfType<Stagz_Gene_Tail_Fish>() != null)
        {
            for (int i = __result.apparel.WornApparel.Count - 1; i >= 0; i--)
            {
                var apparel = __result.apparel.WornApparel[i];
                if (!apparel.def.apparel.bodyPartGroups.CoversMoreThanJustLegs())
                {
                    __result.apparel.Remove(apparel);
                }
            }
        }
    }
}