using System.Linq;
using UnityEngine;
using Verse;

namespace StagzMerfolk;

public class PawnRenderNode_Fishfin : PawnRenderNode
{
    public PawnRenderNode_Fishfin(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props,
        tree)
    {
    }

    public override Color ColorFor(Pawn pawn)
    {
        if (pawn.genes != null && pawn.genes.HasActiveGene(StagzDefOf.Stagz_BodyFin) &&
            pawn.genes.GetFirstGeneOfType<Stagz_Gene_Tail_Fish>() != null)
        {
            return pawn.genes.GetFirstGeneOfType<Stagz_Gene_Tail_Fish>().def.renderNodeProperties.First()?.color ??
                   Props.color ?? pawn.story.HairColor;
        }

        return pawn.story.HairColor;
    }
}
