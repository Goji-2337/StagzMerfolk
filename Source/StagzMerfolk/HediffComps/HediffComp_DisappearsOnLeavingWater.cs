using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace StagzMerfolk;

public class HediffComp_DisappearsOnLeavingWater : HediffComp
{
    private bool usedVerb = false;
    private int onLandDuration = 0;

    public HediffCompProperties_DisappearsOnLeavingWater Props => (HediffCompProperties_DisappearsOnLeavingWater)props;

    public override bool CompShouldRemove => base.CompShouldRemove || onLandDuration >= Props.landGracePeriodDuration || usedVerb;

    //TODO: transfer to delta logic
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
        //DefOf can be null if relevant DLCs aren't loaded - theoretically this means this code will never be called in the first place but let's be paranoid
        if (StagzDefOf.Stagz_DeepDive != null && aVerb?.ability.def != StagzDefOf.Stagz_DeepDive)
        {
            usedVerb = true;
        }
    }
}

[PublicAPI]
public class HediffCompProperties_DisappearsOnLeavingWater : HediffCompProperties
{
    public int landGracePeriodDuration = 60;
    public HediffCompProperties_DisappearsOnLeavingWater()
    {
        compClass = typeof(HediffComp_DisappearsOnLeavingWater);
    }

    // public EffecterDef casterEffect;
}