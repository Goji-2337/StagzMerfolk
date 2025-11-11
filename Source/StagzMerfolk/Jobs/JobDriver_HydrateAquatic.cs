using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace StagzMerfolk;

public class JobDriver_HydrateAquatic : JobDriver
{
    protected Pawn Patient
    {
        get { return job.targetA.Pawn; }
    }

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return Patient == pawn || pawn.Reserve(Patient, job, 1, -1, null, errorOnFailed);
    }

    public override IEnumerable<Toil> MakeNewToils()
    {
        yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.Touch);

        Toil hydrateToil = Toils_General.WaitWith(TargetIndex.A, 1000, false, true, false, TargetIndex.None);
        hydrateToil.defaultDuration = 100;

        hydrateToil.defaultCompleteMode = ToilCompleteMode.Delay;
        hydrateToil.AddEndCondition(delegate
        {
            if (Patient.needs.TryGetNeed<Stagz_Need_Aquatic>().CurLevel < 0.95f)
            {
                return JobCondition.Ongoing;
            }

            return JobCondition.Succeeded;
        });
        hydrateToil.tickAction = delegate() { Patient.needs.TryGetNeed<Stagz_Need_Aquatic>().Hydrate(0.001f); };
        hydrateToil.WithProgressBar(TargetIndex.A, () => Patient.needs.TryGetNeed<Stagz_Need_Aquatic>().CurLevel, false, -0.5f, false);
        hydrateToil.AddFinishAction(delegate
        {
            if (Patient != null && Patient != pawn && Patient.CurJob != null && (Patient.CurJob.def == JobDefOf.Wait || Patient.CurJob.def == JobDefOf.Wait_MaintainPosture))
            {
                Patient.jobs.EndCurrentJob(JobCondition.InterruptForced, true, true);
            }
        });
        yield return hydrateToil;
    }
}