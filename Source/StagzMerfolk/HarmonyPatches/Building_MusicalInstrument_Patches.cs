using HarmonyLib;
using RimWorld;
using Verse;

namespace StagzMerfolk.HarmonyPatches;

[HarmonyPatch(typeof(Building_MusicalInstrument), "StopPlaying")]
public class Building_MusicalInstrument_Patches
{
    private static float spawnChance = StagzDefOf.Stagz_VirtuosoSummoned.HasModExtension<ArielSpawnModExtension>()
        ? StagzDefOf.Stagz_VirtuosoSummoned.GetModExtension<ArielSpawnModExtension>().SpawnChance
        : 0f;

    private static void Prefix(Pawn ___currentPlayer)
    {
        FindCellAndPassToWorker(___currentPlayer);
    }

    //Split into its own func so I can pull it through debug
    public static void FindCellAndPassToWorker(Pawn pawn, bool debug = false)
    {
        var mapTemp = pawn?.Map;
        if (mapTemp != null && Rand.Chance(spawnChance))
        {
            var incidentParams = StorytellerUtility.DefaultParmsNow(StagzDefOf.Stagz_VirtuosoSummoned.category, mapTemp);
            incidentParams.controllerPawn = pawn;
            if (debug) incidentParams.forced = true;
            var raidArrivalMode = PawnsArrivalModeDefOf.EmergeFromWater;
            //assigns parms.spawnCenter if valid cells exist, otherwise considers incident attempt invalid
            if (!raidArrivalMode.Worker.CanUseWith(incidentParams) ||
                !raidArrivalMode.Worker.TryResolveRaidSpawnCenter(incidentParams))
            {
                if (!debug) return;
            }

            if (StagzDefOf.Stagz_VirtuosoSummoned.Worker.CanFireNow(incidentParams) &&
                StagzDefOf.Stagz_VirtuosoSummoned.Worker.TryExecute(incidentParams))
            {
                incidentParams.target.StoryState.lastFireTicks[StagzDefOf.Stagz_VirtuosoSummoned] =
                    Find.TickManager.TicksGame;
            }
        }
    }

}