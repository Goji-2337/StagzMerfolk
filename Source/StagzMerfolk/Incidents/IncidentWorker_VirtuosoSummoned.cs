using JetBrains.Annotations;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace StagzMerfolk;

[UsedImplicitly]
public class IncidentWorker_VirtuosoSummoned : IncidentWorker_ArielSummoned
{
    protected override void AssignLord(ref IncidentParms parms, Pawn pawn, Map map)
    {
        RCellFinder.TryFindRandomSpotJustOutsideColony(pawn, out var chillSpot);
        var lordJobVisitColony = new LordJob_VisitColony(parms.faction, chillSpot, GenDate.TicksPerDay);
        LordMaker.MakeNewLord(parms.faction, lordJobVisitColony, map, [pawn]);
    }

    protected override void ControllerPawnEffects(IncidentParms parms, Map map, Pawn pawn)
    {
    }
}