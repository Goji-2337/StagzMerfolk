using System;
using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace StagzMerfolk;

public class CompToggleableFireOverlay : CompFireOverlayBase
{
    private CompRefuelable refuelableComp;
    private CompGlower glowerComp;
    private Action<ColorInt?> cachedGlowerInt;
    
    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        refuelableComp = parent.GetComp<CompRefuelable>();
        glowerComp = parent.GetComp<CompGlower>();
        cachedGlowerInt = AccessTools.MethodDelegate<Action<ColorInt?>>("Verse.CompGlower:SetGlowColorInternal", glowerComp);
    }
    
    public override void PostDraw()
    {
        base.PostDraw();
        if (refuelableComp is { HasFuel: false }) return;
        Vector3 drawPos = parent.DrawPos;
        drawPos.y += 0.03658537f;
        if (glowerComp != null && DarklightUtility.IsDarklight(glowerComp.GlowColor.ToColor))
        {
            CompDarklightOverlay.DarklightGraphic.Draw(drawPos, parent.Rotation, parent);
        }
        else
        {
            CompFireOverlay.FireGraphic.Draw(drawPos, parent.Rotation, parent);
        }
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        //if I am putting !ModsConfig.IdeologyActive, it will be here - but blue lights are pretty
        if (!parent.Spawned || glowerComp == null || cachedGlowerInt == null) yield break;
        
        bool darklight = DarklightUtility.IsDarklight(glowerComp.GlowColor.ToColor);
        yield return new Command_Action
        {
            icon = ContentFinder<Texture2D>.Get(darklight ? "Things/Special/Fire/FireA" : "Things/Special/Darklight/DarkFireA"),
            defaultLabel = (darklight ? "ToggleDarklightOff" : "ToggleDarklightOn").Translate(),
            defaultDesc = (darklight ? "ToggleDarklightOffDesc" : "ToggleDarklightOnDesc").Translate(),
            action = delegate
            {
                SoundDefOf.Tick_High.PlayOneShotOnCamera();
                if (darklight)
                {
                    cachedGlowerInt(null);
                }
                else
                {
                    glowerComp.GlowColor = new ColorInt(DarklightUtility.DefaultDarklight);
                }
            }
        };
    }
}

[PublicAPI]
public class CompProperties_ToggleableFireOverlay : CompProperties_FireOverlay
{
    public CompProperties_ToggleableFireOverlay() => compClass = typeof (CompToggleableFireOverlay);
}