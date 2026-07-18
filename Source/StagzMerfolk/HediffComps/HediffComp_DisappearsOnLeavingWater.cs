using JetBrains.Annotations;
using Verse;

namespace StagzMerfolk;

public class HediffComp_DisappearsOnLeavingWater : HediffComp
{
    private int onLandDuration = 0;

    public HediffCompProperties_DisappearsOnLeavingWater Props => (HediffCompProperties_DisappearsOnLeavingWater)props;

    public override bool CompShouldRemove => base.CompShouldRemove || onLandDuration > Props.landGracePeriodDuration;

    public override void CompPostTickInterval(ref float severityAdjustment, int delta)
    {
        base.CompPostTickInterval(ref severityAdjustment, delta);
        if (!parent.pawn.OnWater())
        {
            onLandDuration += delta;
        }
        else
        {
            onLandDuration = 0;
        }
    }
}

[PublicAPI]
public class HediffCompProperties_DisappearsOnLeavingWater : HediffCompProperties
{
    public int landGracePeriodDuration = 30;
    public HediffCompProperties_DisappearsOnLeavingWater()
    {
        compClass = typeof(HediffComp_DisappearsOnLeavingWater);
    }
}