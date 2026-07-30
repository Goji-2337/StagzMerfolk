using RimWorld;
using Verse;

namespace StagzMerfolk;

public class CompAbility_RequiresOnWater : AbilityComp
{
    public override bool GizmoDisabled(out string reason)
    {
        if (!this.parent.pawn.OnWater())
        {
            reason = "AbilityDisabledNotOnWater".Translate();
            return true;
        }
        reason = null;
        return false;
    }
}