using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace StagzMerfolk;

public class CompAbility_StartOnCooldown : AbilityComp
{
    public CompProperties_AbilityStartsOnCooldown Props => (CompProperties_AbilityStartsOnCooldown)props;
    public override void Initialize(AbilityCompProperties props)
    {
        base.Initialize(props);
        var ticks = Rand.RangeInclusive(Props.cooldownTicksRange.min, Props.cooldownTicksRange.max);
        parent.StartCooldown(ticks);
    }
}

[PublicAPI]
public class CompProperties_AbilityStartsOnCooldown : AbilityCompProperties
{
    public IntRange cooldownTicksRange;
    public CompProperties_AbilityStartsOnCooldown() => compClass = typeof(CompAbility_StartOnCooldown);
}