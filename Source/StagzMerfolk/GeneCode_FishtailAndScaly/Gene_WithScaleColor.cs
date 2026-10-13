using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace StagzMerfolk;

public class Gene_WithScaleColor : Gene
{
    public static readonly List<Color> defaultColors = [
        new Color32(0, 126, 126, 255),     // teal
        new Color32(241, 99, 35, 255),     // orange
        new Color32(43, 43, 43, 255),      // black
        new Color32(242, 243, 244, 255),   // white
        new Color32(106, 231, 254, 255),   // cyan
        new Color32(180, 78, 175, 255),    // magenta
        new Color32(212, 76, 73, 255),     // salmon
    ];
    private Color? _chosenColor;
    
    public Color ChosenColor
    {
        get
        {
            _chosenColor ??= pawn?.genes?.GetFirstGeneOfType<Gene_WithScaleColor>()?._chosenColor ?? defaultColors.RandomElement(); 
            return (Color) _chosenColor;
        }
        set
        {
            foreach (var gene in pawn.genes.GenesListForReading.OfType<Gene_WithScaleColor>())
            {
                gene._chosenColor = value;
            }
            pawn.Drawer.renderer.SetAllGraphicsDirty();
        }
    }
    
    private Color? _pendingColor;
    public Color? PendingColor
    {
        get => _pendingColor;
        set
        {
            foreach (var gene in pawn.genes.GenesListForReading.OfType<Gene_WithScaleColor>())
            {
                gene._pendingColor = value;
            }
        }
    }

    public override void PostMake()
    {
        base.PostMake();
        _chosenColor = ChosenColor;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref _chosenColor, "chosenColor");
        Scribe_Values.Look(ref _pendingColor, "pendingColor");
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