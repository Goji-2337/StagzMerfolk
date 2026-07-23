using RimWorld;
using Verse;

namespace StagzMerfolk;

public static class BuildingPoweredHelper
{
    public static bool BuildingPowered(Thing b)
    {
        CompPowerTrader compPowerTrader = b.TryGetComp<CompPowerTrader>();
        return compPowerTrader != null && compPowerTrader.PowerOn;
    }
}