using System.Text;
using JetBrains.Annotations;
using LudeonTK;
using Verse;
// ReSharper disable UnusedMember.Local

namespace StagzMerfolk;

[UsedImplicitly]
public class DebugTools
{
    [DebugAction("StagzMerfolk", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    private static void FireArielIncidentWorker()
    {
        if (Find.Storyteller.storytellerComps.FirstOrFallback(c => c is StorytellerComp_Ariel) is not StorytellerComp_Ariel comp)
        {
            Log.Error("StagzMerfolk: StorytellerComp_Ariel is null");
            return;
        }
        foreach (Thing thing in Find.CurrentMap.thingGrid.ThingsAt(UI.MouseCell()))
        {
            if (thing is Pawn { IsColonist: true } pawn)
            {
                comp.FindCellAndPassToWorker(pawn, true);
            }
        }
    }
    
    [DebugAction("StagzMerfolk", actionType = DebugActionType.ToolMap, requiresRoyalty = true, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    private static void FireVirtuosoIncidentWorker()
    {
        if (!ModsConfig.RoyaltyActive)
        {
            Log.Error("Virtuoso requires musical instruments to trigger and cannot work without Royalty.");
            return;
        }
        foreach (Thing thing in Find.CurrentMap.thingGrid.ThingsAt(UI.MouseCell()))
        {
            if (thing is Pawn { IsColonist: true } pawn)
            {
                HarmonyPatches.Building_MusicalInstrument_Patches.FindCellAndPassToWorker(pawn, true);
            }
        }
    }

    [DebugAction("StagzMerfolk", actionType = DebugActionType.Action)]
    private static void DumpCharmTrackerToLog()
    {
        StringBuilder message = new($"Current tick: {Find.TickManager.TicksGame}.");
        if (GameComp_CharmTracker.pawnsWithPendingConsequences == null || GameComp_CharmTracker.pawnsWithPendingConsequences.Count == 0)
        {
            Log.Message(message.Append(" List was empty or null."));
            return;
        }
        foreach (var record in GameComp_CharmTracker.pawnsWithPendingConsequences)
        {
            message.Append($" {record.pawn.Name}, {record.outcomeDef.label}, due at {record.triggerConsequenceTick}");
            message.Append(Find.TickManager.TicksGame < record.triggerConsequenceTick ? "." : ", OVERDUE.");
        } 
        Log.Message(message);
    }
    
}
