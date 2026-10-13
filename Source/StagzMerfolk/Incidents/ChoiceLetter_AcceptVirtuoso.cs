using System.Collections.Generic;
using RimWorld;
using Verse;

namespace StagzMerfolk;

public class ChoiceLetter_AcceptVirtuoso : ChoiceLetter
{
    public Pawn asker;
    
    public override bool CanDismissWithRightClick => false;
    public override bool CanShowInLetterStack => base.CanShowInLetterStack && asker is { Dead: false };

    public override IEnumerable<DiaOption> Choices
    {
        get
        {
            if (ArchivedOnly)
            {
                yield return Option_Close;
                yield break;
            }

            if (lookTargets.IsValid())
            {
                yield return Option_JumpToLocationAndPostpone;
            }

            yield return AcceptOption;
            yield return RejectOption;
            yield return Option_Postpone;
        }
    }
    
    protected virtual DiaOption AcceptOption => new ("Accept".Translate())
    {
        action = delegate
        {
            if (!asker.Spawned)
            {
                Map map = Find.AnyPlayerHomeMap;
                CellFinder.TryFindRandomEdgeCellWith(c=> map.reachability.CanReachColony(c) && !c.Fogged(map), map, CellFinder.EdgeRoadChance_Neutral, out var cell);
                GenSpawn.Spawn(asker, cell, map);
            }

            RecruitUtility.Recruit(asker, Faction.OfPlayer);
            Find.LetterStack.RemoveLetter(this);
        },
        resolveTree = true
    };

    protected virtual DiaOption RejectOption => new ("RejectLetter".Translate())
        {
            action = delegate
            {
                //TODO: need a custom job probably
                asker.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.PanicFlee);
                Find.LetterStack.RemoveLetter(this);
            },
            resolveTree = true
        };

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look(ref asker, "asker");
    }
}