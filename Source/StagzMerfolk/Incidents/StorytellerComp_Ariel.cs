using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace StagzMerfolk;

//Ref to Stranger In Black comp - class with funny name of StorytellerComp_Triggered
public class StorytellerComp_Ariel : StorytellerComp
{
    private StorytellerCompProperties_Ariel Props => (StorytellerCompProperties_Ariel) props;

    public override void Notify_PawnEvent(Pawn pawn, AdaptationEvent ev, DamageInfo? dinfo = null)
    {
        if (!pawn.RaceProps.Humanlike || !pawn.IsColonist || ev != AdaptationEvent.Downed) return;

        FindCellAndPassToWorker(pawn);
    }

    //Split into its own func so I can pull it through debug
    public void FindCellAndPassToWorker(Pawn pawn, bool debug = false)
    {
        IntVec3 waterCell = CellFinder.StandableCellNear(pawn.Position, pawn.Map, 11.9f, c => !c.Fogged(pawn.Map) && pawn.Map.terrainGrid.TerrainAt(c).IsWater && c != pawn.Position);
        
        if (waterCell != IntVec3.Invalid || debug)
        {
            var incidentParams = StorytellerUtility.DefaultParmsNow(Props.incident.category, pawn.Map);
            if (debug) incidentParams.forced = true;
            incidentParams.controllerPawn = pawn;
            incidentParams.spawnCenter = waterCell;
            
            if (Props.incident.Worker.CanFireNow(incidentParams))
            {
                Find.Storyteller.incidentQueue.Add(new QueuedIncident(new FiringIncident(Props.incident, this, incidentParams), Find.TickManager.TicksGame + Props.delayTicks));
            }
        }
    }
}

[PublicAPI]
public class StorytellerCompProperties_Ariel : StorytellerCompProperties
{
    public IncidentDef incident;
    public int delayTicks = 30;
    public StorytellerCompProperties_Ariel()
    {
        compClass = typeof(StorytellerComp_Ariel);
    }
}