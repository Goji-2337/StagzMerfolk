using RimWorld;
using Verse;

namespace StagzMerfolk;

public class ThoughtWorker_Charming : ThoughtWorker
{
    public override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn other)
    {
        return other?.genes != null && other.genes.HasActiveGene(StagzDefOf.Stagz_Charming);
    }
}
