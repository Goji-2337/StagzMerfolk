using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace StagzMerfolk;

public class Stagz_GeneWithScaleColor : Gene
{
    public static readonly List<Color> defaultColors = [
        new (0f / 255f, 126f / 255f, 126f / 255f),     // teal
        new (241f / 255f, 99f / 255f, 35f / 255f),     // orange
        new (43f / 255f, 43f / 255f, 43f / 255f),      // black
        new (242f / 255f, 243f / 255f, 244f / 255f),   // white
        new (106f / 255f, 231f / 255f, 254f / 255f),   // cyan
        new (180f / 255f, 78f / 255f, 175f / 255f),    // magenta
        new (212f / 255f, 76f / 255f, 73f / 255f),     // salmon
    ];
    private Color? _chosenColor;
    
    public Color ChosenColor
    {
        get
        {
            if (_chosenColor == null)
            {
                if (this is Stagz_Gene_Tail_Fish && def.RenderNodeProperties?.First()?.color != null)
                {
                    _chosenColor = def.RenderNodeProperties.First().color;
                } else if (pawn?.genes?.GetFirstGeneOfType<Stagz_Gene_Tail_Fish>()?._chosenColor != null)
                {
                    _chosenColor = pawn.genes.GetFirstGeneOfType<Stagz_Gene_Tail_Fish>()._chosenColor;
                } else if (pawn?.genes?.GetFirstGeneOfType<Stagz_GeneWithScaleColor>()?._chosenColor != null)
                {
                    _chosenColor = pawn.genes.GetFirstGeneOfType<Stagz_GeneWithScaleColor>()._chosenColor;
                }
            }
            _chosenColor ??= defaultColors.RandomElement(); 
            return (Color) _chosenColor;
        }
        set
        {
            foreach (var gene in pawn.genes.GenesListForReading.OfType<Stagz_GeneWithScaleColor>())
            {
                gene._chosenColor = value;
            }
            pawn.Drawer.renderer.SetAllGraphicsDirty();
        }
    }

    public override void PostMake()
    {
        base.PostMake();
        if (this is Stagz_Gene_Tail_Fish)
        {
            if (def.RenderNodeProperties.First().color is { } color && color != Color.clear)
            {
                ChosenColor = color;
            } else {
                Log.Error("StagzMerfolk: Incorrectly defined XML, Merren tail has no color, assigning random");
                ChosenColor = (Color)(_chosenColor = defaultColors.RandomElement());
            }
            return;
        }
        _chosenColor = ChosenColor;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref _chosenColor, "chosenColor");
    }
    
    public override IEnumerable<Gizmo> GetGizmos()
    {
        if (!DebugSettings.ShowDevGizmos || !pawn.Spawned)
        {
            yield break;
        }
        yield return new Command_Action
        {
            defaultLabel = "DEV: Change scale color",
            action = delegate
            {
                Find.WindowStack.Add(new Dialog_ColorPickerForGenesWithScales(pawn));
            }
        };
    }
}