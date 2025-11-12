using RimWorld;
using Verse;

namespace StagzMerfolk.CompAbility;

public class HediffComp_DisappearsOnLeavingWater : HediffComp
{
    private bool usedVerb = false;
    private int landDuration = 60;
    private int onLandDuration = 0;

    public HediffCompProperties_DisappearsOnLeavingWater Props
    {
        get { return (HediffCompProperties_DisappearsOnLeavingWater)this.props; }
    }

    public override bool CompShouldRemove
    {
        get { return base.CompShouldRemove || onLandDuration >= landDuration || usedVerb; }
    }

    public override void CompPostTick(ref float severityAdjustment)
    {
        base.CompPostTick(ref severityAdjustment);
        if (!parent.pawn.OnWater())
        {
            onLandDuration++;
        }
        else
        {
            onLandDuration = 0;
        }
    }

    public override void Notify_PawnUsedVerb(Verb verb, LocalTargetInfo target)
    {
        base.Notify_PawnUsedVerb(verb, target);

        var aVerb = verb as Verb_CastAbility;
        if (aVerb?.ability.def != StagzDefOf.Stagz_DeepDive)
        {
            usedVerb = true;
        }
    }
}

public class HediffCompProperties_DisappearsOnLeavingWater : HediffCompProperties
{
    public HediffCompProperties_DisappearsOnLeavingWater()
    {
        this.compClass = typeof(HediffComp_DisappearsOnLeavingWater);
    }

    // public EffecterDef casterEffect;
}