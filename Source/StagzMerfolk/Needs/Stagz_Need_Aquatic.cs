using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace StagzMerfolk;

public class Stagz_Need_Aquatic : Need
{
    public Stagz_Need_Aquatic(Pawn pawn) : base(pawn)
    {
        this.threshPercents = new List<float>
        {
            0.1f
        };
    }

    private const float FallRate = 0.0003f;
    private float tempFallRate;
    private bool IsCaravanOnWaterFeatures(Pawn pawn)
    {
        if (!pawn.IsCaravanMember())
        {
            return false;
        }

        var caravan = pawn.GetCaravan();
        if (caravan == null)
        {
            return false;
        }
        var tile = Find.WorldGrid[caravan.Tile] as SurfaceTile;
        bool isCoastal = tile?.IsCoastal == true;
        bool hasRivers = tile?.Rivers != null && tile.Rivers.Any();

        return isCoastal || hasRivers;
    }
    public override void NeedInterval()
    {
        if (pawn == null) return;
        if (IsFrozen)
        {
            return;
        }
        if (pawn.IsWet())
        {
            this.CurLevel += 0.0075f;
        }
        else
        {
            tempFallRate = FallRate;
            if (pawn.Map != null && pawn.Map.gameConditionManager.ConditionIsActive(GameConditionDefOf.HeatWave))
            {
                tempFallRate *= 2;
            }

            this.CurLevel -= tempFallRate;
        }
        if (this.CurLevel < 0.1f)
        {
            HealthUtility.AdjustSeverity(this.pawn, StagzDefOf.Stagz_Dehydration, 0.0075f);
        }
        else
        {
            var dehydrationHediff = pawn.health.hediffSet.GetFirstHediffOfDef(StagzDefOf.Stagz_Dehydration);
            if (dehydrationHediff != null)
            {
                dehydrationHediff.Severity -= 0.15f;
            }
        }
    }

    public override bool IsFrozen
    {
        get
        {
            if (this.pawn.IsCaravanMember())
            {
                return IsCaravanOnWaterFeatures(this.pawn);
            }

            return base.IsFrozen || this.pawn.Deathresting;
        }
    }

    public override int GUIChangeArrow
    {
        get
        {
            if (IsFrozen)
            {
                return 0;
            }

            if (pawn.IsWet())
            {
                return 1;
            }

            return -1;
        }
    }

    public void Hydrate(float val)
    {
        this.CurLevel = Mathf.Min(CurLevel + val, 1f);
    }
}
