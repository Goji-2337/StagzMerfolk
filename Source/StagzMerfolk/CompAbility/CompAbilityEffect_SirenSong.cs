using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace StagzMerfolk.CompAbility;

public class CompAbilityEffect_SirenSong : CompAbilityEffect
{
    private new CompProperties_AbilitySirenSong Props
    {
        get
        {
            return (CompProperties_AbilitySirenSong)this.props;
        }
    }

    public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
    {
        base.Apply(target, dest);
        var things = GenRadial.RadialDistinctThingsAround(this.parent.pawn.Position, this.parent.pawn.Map, this.parent.def.EffectRadius, true);
        foreach (var thing in things)
        {
            if (thing is Pawn pawn && pawn.health.capacities.CapableOf(PawnCapacityDefOf.Hearing) && pawn.psychicEntropy?.PsychicSensitivity >= 0.1f && pawn.Faction.HostileTo(Faction.OfPlayer))
            {
                pawn.health.AddHediff(Props.hediff);
            }
        }
        if (this.Props.casterEffect != null)
        {
            Effecter effecter = this.Props.casterEffect.SpawnAttached(this.parent.pawn, this.parent.pawn.MapHeld, 1f);
            effecter.Trigger(this.parent.pawn, null, -1);
            effecter.Cleanup();
        }

    }

    public override void DrawEffectPreview(LocalTargetInfo target)
    {
        GenDraw.DrawFieldEdges(this.AffectedCells(target, this.parent.pawn.Map).ToList<IntVec3>(), this.Valid(target, false) ? Color.white : Color.red, null);
    }

    private IEnumerable<IntVec3> AffectedCells(LocalTargetInfo target, Map map)
    {
        if (target.Cell.Filled(this.parent.pawn.Map))
        {
            yield break;
        }
        foreach (IntVec3 intVec in GenRadial.RadialCellsAround(target.Cell, this.parent.def.EffectRadius, true))
        {
            if (intVec.InBounds(map) && GenSight.LineOfSightToEdges(target.Cell, intVec, map, true, null))
            {
                yield return intVec;
            }
        }
    }
}

public class CompProperties_AbilitySirenSong : CompProperties_AbilityEffect
{
    public CompProperties_AbilitySirenSong()
    {
        this.compClass = typeof(CompAbilityEffect_SirenSong);
    }

    public EffecterDef casterEffect;

    public HediffDef hediff;
}
