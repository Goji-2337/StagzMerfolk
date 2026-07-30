using JetBrains.Annotations;
using Verse;

namespace StagzMerfolk;

public class HediffComp_DeepDiveSFX : HediffComp
{
    public HediffCompProperties_DeepDiveSFX Props => (HediffCompProperties_DeepDiveSFX) props;
    public override void CompPostPostAdd(DamageInfo? dinfo) => Props.addEffecter?.SpawnAttached(Pawn, Pawn.Map);
    public override void CompPostPostRemoved() => Props.removeEffecter?.SpawnAttached(Pawn, Pawn.Map);
}
[PublicAPI]
public class HediffCompProperties_DeepDiveSFX : HediffCompProperties
{
    public EffecterDef addEffecter;
    public EffecterDef removeEffecter;
    public HediffCompProperties_DeepDiveSFX()
    {
        compClass = typeof(HediffComp_DeepDiveSFX);
    }
}