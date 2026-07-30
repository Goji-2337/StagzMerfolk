using System.Collections.Generic;
using RimWorld;
using Verse;

namespace StagzMerfolk;

public class ChoiceLetter_AcceptCharmedJoiner : ChoiceLetter
{
    public Pawn asker;
    
    //TODO: don't think I need this bool but I need to rework Charm first
    public bool requiresAliveAsker;
    public override bool CanDismissWithRightClick => false;
    public override bool CanShowInLetterStack => base.CanShowInLetterStack && (!requiresAliveAsker || asker is { Dead: false });

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
            if (asker.Spawned)
            {
                asker.mindState.mentalStateHandler.Reset();
            }
            else
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
                asker.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.PanicFlee);
                Find.LetterStack.RemoveLetter(this);
            },
            resolveTree = true
        };

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look(ref asker, "asker");
        Scribe_Values.Look(ref requiresAliveAsker, "requiresAliveAsker");
    }
}