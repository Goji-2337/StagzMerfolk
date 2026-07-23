using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace StagzMerfolk;

public class CompAbilityEffect_ToggleHediff : CompAbilityEffect
{
    public new CompProperties_AbilityToggleHediff Props => (CompProperties_AbilityToggleHediff) props;
    
    public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
    {
        Pawn pawn = parent.pawn;
        Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(Props.hediffDef);
        if (firstHediffOfDef != null)
        {
            pawn.health.RemoveHediff(firstHediffOfDef);
            return;
        }

        base.Apply(target, dest);
        Hediff hediff = HediffMaker.MakeHediff(Props.hediffDef, pawn);
        pawn.health.AddHediff(hediff);
    }
}

[PublicAPI]
public class CompProperties_AbilityToggleHediff : CompProperties_AbilityEffect
{
    public HediffDef hediffDef;
    public CompProperties_AbilityToggleHediff()
    {
        compClass = typeof(CompAbilityEffect_ToggleHediff);
    }
}