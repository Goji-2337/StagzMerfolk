using System.Linq;
using UnityEngine;
using Verse;

namespace StagzMerfolk;

public class PawnRenderNode_Fishfin(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
    : PawnRenderNode(pawn, props, tree)
{
    public override Color ColorFor(Pawn pawn)
    {
        return pawn.GetMerrenScaleColorOrFailsafe();
    }
}
