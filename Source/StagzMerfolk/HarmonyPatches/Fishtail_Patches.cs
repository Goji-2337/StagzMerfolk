using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace StagzMerfolk.HarmonyPatches;

//Tricks Rimworld into thinking that merfolk don't have leg bodyparts suitable for wearing pants
[HarmonyPatch(typeof(ApparelUtility), nameof(ApparelUtility.HasPartsToWear))]
public static class FishtailPatch_ApparelUtility
{
    public static bool Prefix(Pawn p, ThingDef apparel, ref bool __result)
    {
        if (p?.genes?.GetFirstGeneOfType<Gene_Fishtail>() == null) return true;
        if (apparel.apparel.bodyPartGroups.CoversMoreThanJustLegs()) return true;
        __result = false;
        return false;
    }
}

[HarmonyPatch(typeof(PawnTechHediffsGenerator), nameof(PawnTechHediffsGenerator.GenerateTechHediffsFor))]
public static class FishtailPatch_PawnTechHediffsGenerator
{
    public static void Postfix(Pawn pawn)
    {
        if (pawn?.genes?.GetFirstGeneOfType<Gene_Fishtail>() is null) return;
        
        //If pawn generated with a leg implant, fishtail and bodyfins are removed/not added.
        //For now only skips full leg implants, if e.g. a foot is replaced it still gets overwritten by fishtail.
        foreach (var hediff in pawn.health.hediffSet.hediffs)
        {
            if (hediff.def.countsAsAddedPartOrImplant && hediff.Part.def == BodyPartDefOf.Leg)
            {
                var finGene = pawn.genes.GetGene(StagzDefOf.Stagz_BodyFin);
                if (finGene is not null) pawn.genes.RemoveGene(finGene);
                var tailGene = pawn.genes.GetFirstGeneOfType<Gene_Fishtail>();
                pawn.genes.RemoveGene(tailGene);

                return;
            }
        }
        
        //otherwise finish adding tail
        pawn.RemoveLegOnlyApparel();
        foreach (var leg in pawn.RaceProps.body.GetPartsWithDef(BodyPartDefOf.Leg))
        {
            pawn.health.RestorePart(leg, null, false);
            pawn.health.AddHediff(StagzDefOf.Stagz_Tail, leg);
        }
    }
}

//fix for vanilla bug which otherwise makes statue's fakePawn merren be created as downed, leading to error spam
[HarmonyPatch(typeof(CompStatue), "InitFakePawn_HookForMods")]
public static class FishtailPatch_CompStatue
{
    private static readonly AccessTools.FieldRef<Pawn_HealthTracker, PawnHealthState> healthStateRef =
        AccessTools.FieldRefAccess<Pawn_HealthTracker, PawnHealthState>("healthState");
    public static void Postfix(Pawn fakePawn, Dictionary<string, object> additionalSavedPawnDataForMods)
    {
        if (fakePawn?.health?.Downed == true)
        {
            healthStateRef(fakePawn.health) = PawnHealthState.Mobile;
        }
    }
}

//not just for anomaly obelisk but some mods use it
[HarmonyPatch(typeof(GameComponent_PawnDuplicator), "Duplicate")]
public static class FishtailPatch_Duplicate
{
    public static void Postfix(Pawn pawn, ref Pawn __result)
    {
        if (pawn.MerrenScaleColor != null)
        {
            __result.MerrenScaleColor = pawn.MerrenScaleColor;
        }
    }
}