using JetBrains.Annotations;
using UnityEngine;
using Verse;

namespace StagzMerfolk;

[UsedImplicitly]
public class PawnRenderNode_Fishfin(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
    : PawnRenderNode(pawn, props, tree)
{
    private new PawnRenderNodeProperties_HasDesiccatedGraphics Props => props as PawnRenderNodeProperties_HasDesiccatedGraphics;
    public override Graphic GraphicFor(Pawn pawn)
    {
        switch (pawn.Drawer.renderer.CurRotDrawMode)
        {
            case RotDrawMode.Dessicated:
                return GraphicDatabase.Get<Graphic_Multi>(
                    Props.desiccatedTexPath,
                    ShaderFor(pawn)
                );
            default:
                return base.GraphicFor(pawn);
        }
    }

    public override Color ColorFor(Pawn pawn)
    {
        return pawn.GetMerrenScaleColorOrFailsafe();
    }
}
