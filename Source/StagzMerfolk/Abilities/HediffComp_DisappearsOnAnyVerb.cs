using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace StagzMerfolk;

public class HediffComp_DisappearsOnAnyVerb : HediffComp
{
    private bool verbUsed;
    public HediffCompProperties_DisappearsOnAnyVerb Props => (HediffCompProperties_DisappearsOnAnyVerb)props;

    public override bool CompShouldRemove => base.CompShouldRemove || verbUsed;
    
    public override void Notify_PawnUsedVerb(Verb verb, LocalTargetInfo target)
    {
        base.Notify_PawnUsedVerb(verb, target);

        if (verb is not Verb_CastAbility aVerb || Props.excludeAbilityDef == null || Props.excludeAbilityDef != aVerb.ability.def)
        {
            verbUsed = true;
        }
    }
}

[PublicAPI]
public class HediffCompProperties_DisappearsOnAnyVerb : HediffCompProperties
{
    public AbilityDef excludeAbilityDef;
    public HediffCompProperties_DisappearsOnAnyVerb()
    {
        compClass = typeof(HediffComp_DisappearsOnAnyVerb);
    }
}