using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace StagzMerfolk;

public class Stagz_Gene_Tail_Fish : Gene_WithScaleColor
{
    public override void PostMake()
    {
        base.PostMake();
        
        //harmony patch has to handle this during pawn generation, otherwise if techhediff code happens to generate leg implant it will throw error
        if (PawnGenerator.IsBeingGenerated(pawn)) return;
        
        pawn.RemoveLegOnlyApparel(true);
        //Only run this on first tail added - otherwise can lead to cursed loop with fishtail hediffcomp
        if (pawn.genes.GenesListForReading.OfType<Stagz_Gene_Tail_Fish>().Count() == 0)
            foreach (var leg in pawn.RaceProps.body.GetPartsWithDef(BodyPartDefOf.Leg))
            {
                pawn.health.RestorePart(leg, null, false);
                pawn.health.AddHediff(StagzDefOf.Stagz_Tail, leg);
            }
    }

    public override void PostRemove()
    {
        base.PostRemove();
        //Only run this on last tail removed - otherwise can lead to cursed loop with the fishtail hediffcomp
        if (pawn.genes.GenesListForReading.OfType<Stagz_Gene_Tail_Fish>().Count() == 0)
        {
            List<Hediff> parts = pawn.health.hediffSet.hediffs
                .Where(h => h.def == StagzDefOf.Stagz_Tail).ToList();

            foreach (var part in parts)
            {
                pawn.health.RestorePart(part.Part);
            }
        }
    }
}