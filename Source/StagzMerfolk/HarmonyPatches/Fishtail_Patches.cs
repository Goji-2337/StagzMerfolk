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