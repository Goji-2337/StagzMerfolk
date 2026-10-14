using System;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace StagzMerfolk;

public class CompAbilityEffect_SirenSong : CompAbilityEffect_GiveHediff
{
    private new CompProperties_AbilitySirenSong Props => (CompProperties_AbilitySirenSong)props;

    protected override bool TryResist(Pawn pawn)
    {
        var resistChance = MathF.Min(pawn.GetStatValue(StatDefOf.PsychicSensitivity), pawn.health.capacities.GetLevel(PawnCapacityDefOf.Hearing));
        var resistChanceAdjusted = Props.chanceFromCurve.Evaluate(resistChance);
        var roll = Rand.Chance(1f - resistChanceAdjusted);
        return roll;
    }
}

[PublicAPI]
public class CompProperties_AbilitySirenSong : CompProperties_AbilityGiveHediff
{
    public SimpleCurve chanceFromCurve = [];
    public CompProperties_AbilitySirenSong() => compClass = typeof(CompAbilityEffect_SirenSong);
}
