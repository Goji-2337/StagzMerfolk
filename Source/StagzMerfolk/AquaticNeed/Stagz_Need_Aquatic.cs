using JetBrains.Annotations;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace StagzMerfolk;

[UsedImplicitly]
public class Stagz_Need_Aquatic : Need
{
    private const float BaseGainRatePerTick = 0.0075f;
    private const float BaseFallRatePerTick = 0.0003f;
    
    //TODO: Normally this is implemented through a stat but idk if we need to go that far yet
    private float FallFactor =>
        pawn.Map != null && pawn.Map.gameConditionManager.ConditionIsActive(GameConditionDefOf.HeatWave) ? 2 : 1;
    
    public bool Dehydrating => CurLevelPercentage <= 0.0;

    private bool GainingHydration =>
        pawn.OnWater()
        || pawn.InRain()
        || pawn.health.hediffSet.HasHediff(StagzDefOf.IntheStandaloneHotSpring);
    
    public Stagz_Need_Aquatic(Pawn pawn) : base(pawn)
    {
        threshPercents = [0.1f];
    }
    private bool IsInCaravanOnWaterFeatures()
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
        if (IsFrozen)
        {
            return;
        }
        if (GainingHydration)
        {
            CurLevel += BaseGainRatePerTick;
        }
        else
        {
            CurLevel -= BaseFallRatePerTick * FallFactor;
        }
        if (Dehydrating)
            HealthUtility.AdjustSeverity(pawn, StagzDefOf.Stagz_Dehydration, 0.0075f);
        else
        {
            HealthUtility.AdjustSeverity(pawn, StagzDefOf.Stagz_Dehydration, -0.15f);
        }
    }

    protected override bool IsFrozen => base.IsFrozen || pawn.Deathresting || IsInCaravanOnWaterFeatures();

    public override int GUIChangeArrow
    {
        get
        {
            if (IsFrozen)
            {
                return 0;
            }
            return GainingHydration ? 1 : -1;
        }
    }
    public override void OnNeedRemoved()
    {
        if (!pawn.health.hediffSet.TryGetHediff(StagzDefOf.Stagz_Dehydration, out Hediff hediff))
            return;
        pawn.health.RemoveHediff(hediff);
    }

    public void Hydrate(float val)
    {
        this.CurLevel = Mathf.Min(CurLevel + val, 1f);
    }
}
