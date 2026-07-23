using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace StagzMerfolk;

public class WorkGiver_HydrateAquatic : WorkGiver_Scanner
{
    public override PathEndMode PathEndMode
    {
        get
        {
            return PathEndMode.InteractionCell;
        }
    }
    public override ThingRequest PotentialWorkThingRequest
    {
        get
        {
            return ThingRequest.ForGroup(ThingRequestGroup.Pawn);
        }
    }
    public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
    {
        foreach (var potentialTarget in pawn.Map.mapPawns.AllPawnsSpawned)
        {
            var needAquatic = potentialTarget.needs.TryGetNeed<Stagz_Need_Aquatic>();
            if (needAquatic != null && needAquatic.CurLevel < 0.3f)
            {
                yield return potentialTarget;
            }
        }
        yield break;
    }
    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        Pawn target = t as Pawn;
        if (target == null || target == pawn)
        {
            return false;
        }

        Stagz_Need_Aquatic needAquatic = target.needs.TryGetNeed<Stagz_Need_Aquatic>();
        return needAquatic != null && needAquatic.CurLevel < 0.3f && WorkGiver_Tend.GoodLayingStatusForTend(target, pawn) && pawn.CanReserve(target, 1, -1, null, forced);
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        Pawn pawn2 = t as Pawn;
        return JobMaker.MakeJob(StagzDefOf.Stagz_HydrateAquaticJob, pawn2);
    }
}