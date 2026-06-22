using HarmonyLib;
using RimWorld;
using Verse;

namespace StagzMerfolk.HarmonyPatches;

//Tricks Rimworld into thinking that merfolk don't have leg bodyparts suitable for wearing pants
[HarmonyPatch(typeof(ApparelUtility), nameof(ApparelUtility.HasPartsToWear))]
public static class ApparelUtility_FishtailPatch
{
    public static bool Prefix(Pawn p, ThingDef apparel, ref bool __result)
    {
        if (p?.genes?.GetFirstGeneOfType<Stagz_Gene_Tail_Fish>() == null) return true;
        if (apparel.apparel.bodyPartGroups.CoversMoreThanJustLegs()) return true;
        __result = false;
        return false;
    }
}