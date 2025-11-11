using System;
using System.Linq;
using System.Reflection;
using Verse;

namespace StagzMerfolk;

public class HediffComp_RemovedWhenGeneRemoved : HediffComp
{
    public HediffCompProperties_RemovedWhenGeneRemoved Props
    {
        get
        {
            return (HediffCompProperties_RemovedWhenGeneRemoved)this.props;
        }
    }

    public override void CompPostPostRemoved()
    {
        base.CompPostPostRemoved();
        {
            foreach (var gene in parent.pawn.genes.GenesListForReading.OfType<Stagz_Gene_Tail_Fish>())
            {
                parent.pawn.genes.RemoveGene(gene);
            }
        }
    }
}

public class HediffCompProperties_RemovedWhenGeneRemoved : HediffCompProperties
{
    
    public HediffCompProperties_RemovedWhenGeneRemoved()
    {
        this.compClass = typeof(HediffComp_RemovedWhenGeneRemoved);
    }
}