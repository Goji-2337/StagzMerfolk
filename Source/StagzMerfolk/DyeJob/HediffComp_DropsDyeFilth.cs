using System;
using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse;

namespace StagzMerfolk;

[UsedImplicitly]
public class HediffComp_DropsDyeFilth : HediffComp_Disappears
{
    public new HediffCompProperties_DropsDyeFilth Props => (HediffCompProperties_DropsDyeFilth) props;

    private Vector3? lastSmearDropPos;
    private Color filthColor;
    private int ticksForSmearing;

    public override void CompPostPostAdd(DamageInfo? dinfo)
    {
        base.CompPostPostAdd(dinfo);
        filthColor = parent.pawn.GetMerrenScaleColorOrFailsafe();
        ticksForSmearing = Props.directionalFilthTicks;
        CreateDyeSplatter();
    }
    
    public override void CompPostTick(ref float severityAdjustment)
    {
        base.CompPostTick(ref severityAdjustment);
        if (!parent.pawn.SpawnedOrAnyParentSpawned) return;

        Pawn pawn = parent.pawn;

        float rand;
        //part of the trail which includes directional smears
        if (ticksForSmearing-- > 0 &&
            (!lastSmearDropPos.HasValue || 
            Vector3.Distance(pawn.DrawPos, lastSmearDropPos.Value) > 
            0.5f * MathF.Pow(1f - (float) ticksForSmearing / Props.directionalFilthTicks, 2)))
        {
            FilthMaker.TryMakeFilth(pawn.PositionHeld, pawn.MapHeld, StagzDefOf.Stagz_Filth_DyeSmear, out var outFilth, shouldPropagate: false);
            if (outFilth == null) return;
            outFilth.DrawColor = filthColor;
            float rotation = !lastSmearDropPos.HasValue
                ? pawn.pather.lastMoveDirection
                : (lastSmearDropPos.Value - pawn.DrawPos).AngleFlat();
            outFilth.SetOverrideDrawPositionAndRotation(pawn.DrawPos.WithY(StagzDefOf.Stagz_Filth_DyeSmear.Altitude), rotation);
            lastSmearDropPos = pawn.DrawPos;
            
            rand = 0.2f * ticksForSmearing / Props.directionalFilthTicks + 0.1f;
            if (Rand.Chance(rand))
            {
                FilthMaker.TryMakeFilth(pawn.PositionHeld, pawn.MapHeld, StagzDefOf.Stagz_Filth_Dye, out outFilth);
                if (outFilth == null) return;
                outFilth.DrawColor = filthColor;
            }
        }
        
        //additional non-directional splatters, those are dropped for the whole duration of the hediff
        rand = 0.03f * ticksToDisappear / disappearsAfterTicks + 0.005f;
        if (Rand.Chance(rand))
        {
            FilthMaker.TryMakeFilth(pawn.PositionHeld, pawn.MapHeld, StagzDefOf.Stagz_Filth_Dye, out var outFilth);
            if (outFilth == null) return;
            outFilth.DrawColor = filthColor;
        }
    }

    private void CreateDyeSplatter()
    {
        if (!parent.pawn.SpawnedOrAnyParentSpawned) return;
        foreach (IntVec3 item in GenRadial.RadialCellsAround(parent.pawn.Position, 1f, useCenter: true))
        {
            if (item.InBounds(parent.pawn.Map) && Rand.Bool)
            {
                FilthMaker.TryMakeFilth(item, parent.pawn.Map, StagzDefOf.Stagz_Filth_Dye, out var outFilth, Rand.Range(1, 3));
                outFilth.DrawColor = filthColor;
            }
        }
    }
    
    public override void CompExposeData()
    {
        if (Scribe.EnterNode("StagzMerfolk_DropsDyeFilth"))
        {
            try
            {
                base.CompExposeData();
                Scribe_Values.Look(ref lastSmearDropPos, "filthPos");
                Scribe_Values.Look(ref ticksForSmearing, "ticksForSmearing");
                Scribe_Values.Look(ref filthColor, "filthColor", Color.white);
            }
            finally
            {
                Scribe.ExitNode();
            }
        }
    }
}

[PublicAPI]
public class HediffCompProperties_DropsDyeFilth : HediffCompProperties_Disappears
{
    public int directionalFilthTicks = 150;
    public HediffCompProperties_DropsDyeFilth() => compClass = typeof(HediffComp_DropsDyeFilth);
}