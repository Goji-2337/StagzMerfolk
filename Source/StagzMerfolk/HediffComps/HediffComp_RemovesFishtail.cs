using System.Linq;
using JetBrains.Annotations;
using Verse;

namespace StagzMerfolk;

[UsedImplicitly]
public class HediffComp_RemovesFishtail : HediffComp
{
    public override void CompPostPostRemoved()
    {
        base.CompPostPostRemoved();
        foreach (var gene in parent.pawn.genes.GenesListForReading.OfType<Stagz_Gene_Tail_Fish>())
        {
            parent.pawn.genes.RemoveGene(gene);
        }
    }
}