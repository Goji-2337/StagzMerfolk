using System.Linq;
using JetBrains.Annotations;
using LudeonTK;
using Verse;

namespace StagzMerfolk;

[UsedImplicitly]
public class DebugTools
{
    [DebugAction("StagzMerfolk", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    private static void FireArielIncidentWorker()
    {
        if (Find.Storyteller.storytellerComps.First(c => c is StorytellerComp_Ariel) is not StorytellerComp_Ariel comp)
        {
            Log.Error("StagzMerfolk: StorytellerComp_Ariel is null");
            return;
        }
        foreach (Thing thing in Find.CurrentMap.thingGrid.ThingsAt(UI.MouseCell()).ToList())
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
        foreach (Thing thing in Find.CurrentMap.thingGrid.ThingsAt(UI.MouseCell()).ToList())
        {
            if (thing is Pawn { IsColonist: true } pawn)
            {
                HarmonyPatches.Building_MusicalInstrument_Patches.FindCellAndPassToWorker(pawn, true);
            }
        }
    }
}
