using RimWorld;
using Verse;

namespace StagzMerfolk;

public class ThoughtWorker_NeedHydration : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        if (p.needs.TryGetNeed(StagzDefOf.Stagz_NeedAquatic) == null)
        {
            return ThoughtState.Inactive;
        }

        return p.health.hediffSet.GetFirstHediffOfDef(StagzDefOf.Stagz_Dehydration) == null ?
            ThoughtState.Inactive : ThoughtState.ActiveAtStage(p.health.hediffSet.GetFirstHediffOfDef(StagzDefOf.Stagz_Dehydration).CurStageIndex);
    }
}