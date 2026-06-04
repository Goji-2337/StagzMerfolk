using JetBrains.Annotations;
using UnityEngine;
using Verse;

namespace StagzMerfolk;

[UsedImplicitly]
public class PawnRenderNode_Fishtail(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
    : PawnRenderNode(pawn, props, tree)
{
    private new PawnRenderNodeProperties_HasDesiccatedGraphics Props => props as PawnRenderNodeProperties_HasDesiccatedGraphics;
    public override Graphic GraphicFor(Pawn pawn)
    {
        if (pawn.story?.furDef == null)
        {
            return null;
        }

        switch (pawn.Drawer.renderer.CurRotDrawMode)
        {
            case RotDrawMode.Fresh:
                return GraphicDatabase.Get<Graphic_Multi>(
                    pawn.story.furDef.GetFurBodyGraphicPath(pawn),
                    ShaderDatabase.CutoutComplex,
                    Vector2.one,
                    pawn.story.SkinColor,
                    ColorFor(pawn)
                    );
            case RotDrawMode.Rotting:
                return GraphicDatabase.Get<Graphic_Multi>(
                    pawn.story?.furDef.GetFurBodyGraphicPath(pawn),
                    ShaderDatabase.CutoutComplex,
                    Vector2.one,
                    PawnRenderUtility.GetRottenColor(pawn.story!.SkinColor),
                    ColorFor(pawn)
                    );
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