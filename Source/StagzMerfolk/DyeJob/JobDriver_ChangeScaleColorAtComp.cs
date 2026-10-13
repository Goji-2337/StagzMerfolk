using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Verse;
using Verse.AI;

namespace StagzMerfolk;

[PublicAPI]
public class JobDriver_ChangeScaleColorAtComp : JobDriver
{
    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(job.targetA, job, errorOnFailed: errorOnFailed) && pawn.Reserve(job.targetB, job, errorOnFailed: errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch)
            .FailOnDespawnedNullOrForbidden(TargetIndex.A)
            .FailOnSomeonePhysicallyInteracting(TargetIndex.A);
        yield return Toils_Haul.StartCarryThing(TargetIndex.A);
        yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.InteractionCell)
            .FailOnDespawnedOrNull(TargetIndex.B);
        
        Color pendingColor = pawn.genes?.GetFirstGeneOfType<Gene_WithScaleColor>()?.PendingColor ?? Color.white;
        yield return Toils_General.WaitWith(TargetIndex.B, 300, true, face: TargetIndex.B)
            .FailOnDespawnedOrNull(TargetIndex.B)
            .WithEffect(StagzDefOf.Stagz_DyeingTail, TargetIndex.B, pendingColor);
        
        yield return FinalizeLookChange();
    }
    
    public Toil FinalizeLookChange()
    {
        Toil toil = ToilMaker.MakeToil();
        toil.initAction = delegate
        {
            Thing thing = job.GetTarget(TargetIndex.A).Thing.SplitOff(1);
            if (thing is { Destroyed: false })
            {
                thing.Destroy();
            }

            if (pawn.genes?.GetFirstGeneOfType<Gene_WithScaleColor>()?.PendingColor is { } pendingColor)
            {
                pawn.TrySetMerrenScaleColor(pendingColor);
                //may not be resetting properly to null after being used, but right now it doesn't matter
                pawn.genes.GetFirstGeneOfType<Gene_WithScaleColor>()?.PendingColor = null;
            }

            //pawns who don't have tail but can still change color of fins probably shouldn't drip
            if (pawn?.genes?.GetFirstGeneOfType<Gene_Fishtail>() != null)
            {
                pawn.health.AddHediff(StagzDefOf.Stagz_CoveredInDye);
            }
        };
        return toil;
    }
}