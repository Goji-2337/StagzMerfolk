using System.Collections.Generic;
using UnityEngine;
using Verse.AI;

namespace StagzMerfolk;

public class JobDriver_ChangeScaleColorAtDye : JobDriver_ChangeScaleColorAtComp
{
    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(job.targetA, job, errorOnFailed: errorOnFailed);
    }
    
    protected override IEnumerable<Toil> MakeNewToils()
    {
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell)
            .FailOnDespawnedNullOrForbidden(TargetIndex.A)
            .FailOnSomeonePhysicallyInteracting(TargetIndex.A);
        
        Color pendingColor = pawn.PendingMerrenScaleColor ?? Color.white;
        yield return Toils_General.WaitWith(TargetIndex.A,300, true, face: TargetIndex.A)
            .FailOnDespawnedOrNull(TargetIndex.A)
            .WithEffect(StagzDefOf.Stagz_DyeingTail, TargetIndex.A, pendingColor);
        
        yield return FinalizeLookChange();
    }
}