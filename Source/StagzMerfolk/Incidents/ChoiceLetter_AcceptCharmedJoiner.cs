using System.Collections.Generic;
using RimWorld;
using Verse;

namespace StagzMerfolk;

public class ChoiceLetter_AcceptCharmedJoiner : ChoiceLetter
{
    public Pawn asker;
    public bool requiresAliveAsker;
    public override bool CanDismissWithRightClick => false;

    public override bool CanShowInLetterStack
    {
        get
        {
            if (!base.CanShowInLetterStack)
            {
                return false;
            }

            if (this.requiresAliveAsker && (this.asker == null || this.asker.Dead))
            {
                return false;
            }

            return true;
        }
    }

    public override IEnumerable<DiaOption> Choices
    {
        get
        {
            if (base.ArchivedOnly)
            {
                yield return base.Option_Close;
                yield break;
            }

            if (this.lookTargets.IsValid())
            {
                yield return base.Option_JumpToLocationAndPostpone;
            }

            var accept = new DiaOption("Accept")
            {
                action = delegate
                {
                    if (asker.Spawned)
                    {
                        asker.mindState.mentalStateHandler.Reset();
                    }
                    else
                    {
                        IntVec3 cell;
                        Map map = Find.AnyPlayerHomeMap;
                        CellFinder.TryFindRandomEdgeCellWith((IntVec3 c) => map.reachability.CanReachColony(c) && !c.Fogged(map), map, CellFinder.EdgeRoadChance_Neutral, out cell);
                        GenSpawn.Spawn(asker, cell, map);
                    }

                    RecruitUtility.Recruit(asker, Faction.OfPlayer);
                    Find.LetterStack.RemoveLetter(this);
                },
                resolveTree = true
            };
            var reject = RejectOption();

            yield return accept;
            yield return reject;
            yield return base.Option_Postpone;
        }
    }

    public virtual DiaOption RejectOption()
    {
        return new DiaOption("Reject")
        {
            action = delegate
            {
                asker.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.PanicFlee);
                Find.LetterStack.RemoveLetter(this);
            },
            resolveTree = true
        };
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look<Pawn>(ref this.asker, "asker", false);
        Scribe_Values.Look<bool>(ref this.requiresAliveAsker, "requiresAliveAsker", false, false);
    }
}