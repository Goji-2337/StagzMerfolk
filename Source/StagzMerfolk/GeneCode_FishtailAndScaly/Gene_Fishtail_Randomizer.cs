using System.Collections.Generic;
using System.Linq;
using Verse;

namespace StagzMerfolk;

[StaticConstructorOnStartup]
public static class TailColours
{
    public static IEnumerable<GeneDef> allColours = DefDatabase<GeneDef>.AllDefsListForReading
        .Where(g => g.geneClass == typeof(Stagz_Gene_Tail_Fish));
}

public class Gene_Fishtail_Randomizer : Gene
{
    public override void PostAdd()
    {
        base.PostAdd();

        pawn.genes.AddGene(TailColours.allColours.RandomElement(), pawn.genes.IsXenogene(this));

        pawn.genes.RemoveGene(this);
    }
}