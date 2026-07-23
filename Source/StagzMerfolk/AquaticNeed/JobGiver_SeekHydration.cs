using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace StagzMerfolk;

public class JobGiver_SeekHydration : ThinkNode_JobGiver
{
    protected override Job TryGiveJob(Pawn pawn)
    {
        if (pawn.needs.TryGetNeed(StagzDefOf.Stagz_NeedAquatic) != null && pawn.health.hediffSet.HasHediff(StagzDefOf.Stagz_Dehydration))
        {
            if (pawn.Map.terrainGrid.TerrainAt(pawn.Position).IsWater)
            {
                return JobMaker.MakeJob(StagzDefOf.Stagz_Wait_Hydrate, 500, true);
            }
            
            
            Region region = ClosestRegionWithWaterTile(pawn.Position, pawn.Map, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false, false, false), RegionType.Set_Passable);
            if (region != null)
            {
                return JobMaker.MakeJob(StagzDefOf.Stagz_GotoWaterCell, region.Cells.First(c => pawn.Map.terrainGrid.TerrainAt(c).IsWater));
            }
        }
        
        return null;
    }

    public override float GetPriority(Pawn pawn)
    {
        if (pawn.needs.TryGetNeed(StagzDefOf.Stagz_NeedAquatic) != null && pawn.health.hediffSet.HasHediff(StagzDefOf.Stagz_Dehydration))
        {
            return 9.25f;
        }
        return 0f;
    }
    
    public static Region ClosestRegionWithWaterTile(IntVec3 root, Map map, TraverseParms traverseParms, RegionType traversableRegionTypes = RegionType.Set_Passable)
    {        
        Region region = root.GetRegion(map, traversableRegionTypes);
        if (region == null)
        {
            return null;
        }
        
        RegionEntryPredicate entryCondition = (Region from, Region r) => r.Allows(traverseParms, false);
        Region foundReg = null;
        RegionProcessor regionProcessor = delegate(Region r)
        {
            if (r.IsDoorway)
            {
                return false;
            }

            if (r.Cells.Any(c => map.terrainGrid.TerrainAt(c).IsWater))
            {
                foundReg = r;
                return true;
            }

            return false;
        };
        RegionTraverser.BreadthFirstTraverse(region, entryCondition, regionProcessor, 9999, traversableRegionTypes);
        return foundReg;
        
    }
}