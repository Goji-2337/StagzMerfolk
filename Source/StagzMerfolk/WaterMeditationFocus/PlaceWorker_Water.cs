using JetBrains.Annotations;
using Verse;

namespace StagzMerfolk;

[UsedImplicitly]
public class PlaceWorker_OnlyOverWater : PlaceWorker
{
    public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
    {
        foreach (IntVec3 cell in GenAdj.OccupiedRect(loc, rot, checkingDef.Size))
        {
            if (cell.InBounds(map))
            {
                if (!cell.GetTerrain(map).IsWater)
                {
                    return "StagzMerfolk_MessageMustBePlacedOnWater".Translate();
                }
            }
        }
        return AcceptanceReport.WasAccepted;
    }
}