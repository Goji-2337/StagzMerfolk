using JetBrains.Annotations;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace StagzMerfolk;

[UsedImplicitly]
public class Stagz_Need_Aquatic : Need
{
    private const int needIntervalTicks = 150;
    private const float BaseGainRatePerInterval = 0.0075f;
    private const float BaseFallRatePerInterval = 0.0003f;
    
    private float CachedFallRateForTooltip
    {
        get
        {
            if (field == 0) field = BaseFallRatePerInterval * FallFactor;
            return field;
        }
        set;
    }

    private float FallFactor
    {
        get
        {
            field = 1;
            if (ModsConfig.OdysseyActive)
                field -= pawn.GetStatValue(StatDefOf.VacuumResistance) / 1.5f;
            CachedFallRateForTooltip = BaseFallRatePerInterval * field;
            return field;
        }
    }
    
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
            CurLevel += BaseGainRatePerInterval;
        }
        else
        {
            CurLevel -= BaseFallRatePerInterval * FallFactor;
        }
        if (Dehydrating)
            HealthUtility.AdjustSeverity(pawn, StagzDefOf.Stagz_Dehydration, 20 * BaseFallRatePerInterval);
        else
        {
            HealthUtility.AdjustSeverity(pawn, StagzDefOf.Stagz_Dehydration, -20 * BaseGainRatePerInterval);
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
    
    public override string GetTipString()
    {
        string tipString = $"{$"{LabelCap}: {CurLevelPercentage.ToStringPercent()}".Colorize(ColoredText.TipSectionTitleColor)}\n\n";

        float num = CurLevel / (CachedFallRateForTooltip * GenDate.TicksPerDay / needIntervalTicks);
        tipString += "StagzMerfolk_DehydrationEstimationOnTooltip".Translate(pawn.Named("PAWN"), "PeriodDays".Translate(num.ToString("F1")).Named("DURATION")).Resolve().CapitalizeFirst();
        
        tipString += $"\n\n{def.description}";
        if (pawn.genes != null && pawn.genes.TryGetNeedEnablingGene(def, out var gene))
        {
            tipString += $"\n\n{"ComesFromGene".Translate()}: {gene.LabelCap}";
        }
        return tipString;
    }

    public void Hydrate(float val)
    {
        CurLevel = Mathf.Min(CurLevel + val, 1f);
    }
}
