using RimWorld;
using Verse;

namespace StagzMerfolk;

public class FocusStrengthOffset_Powered : FocusStrengthOffset
{
    public override string GetExplanation(Thing parent)
    {
        return CanApply(parent, null)
            ? "StagzMerfolk_StatsReport_Powered".Translate() + ": " + GetOffset(parent, null).ToStringWithSign("0%")
            : GetExplanationAbstract(null);
    }

    public override string GetExplanationAbstract(ThingDef def = null)
    {
        return "StagzMerfolk_StatsReport_Powered".Translate() + ": " + this.offset.ToStringWithSign("0%");
    }

    public override float GetOffset(Thing parent, Pawn user = null) => offset;

    public override bool CanApply(Thing parent, Pawn user = null)
    {
        return BuildingPoweredHelper.BuildingPowered(parent) && base.CanApply(parent, user);
    }
}