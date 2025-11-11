using System;
using HarmonyLib;
using Verse;
using Verse.AI;

namespace StagzMerfolk.HarmonyPatches;

[HarmonyPatch(typeof(Pawn_PathFollower))]
[HarmonyPatch("CostToMoveIntoCell")]
[HarmonyPatch(new Type[]
{
    typeof(Pawn),
    typeof(IntVec3)
})]
public static class Pawn_PathFollower_Patches
{
    [HarmonyPrepare]
    private static bool shouldUseInsteadOfPathfindingFramework()
    {
        return !ModsConfig.IsActive("pathfinding.framework");
    }

    private static void Postfix(Pawn pawn, IntVec3 c, ref float __result)
    {
        if (pawn?.genes != null && pawn.genes.HasActiveGene(StagzDefOf.Stagz_Aquatic) && pawn.Map.terrainGrid.TerrainAt(c).IsWater)
        {
            // Log.Message("terrain is water: " + __result);
            //TODO: also add terrain affordance of land in there
            var movingCardinal = c.x == pawn.Position.x || c.z == pawn.Position.z;
            __result = movingCardinal ? pawn.TicksPerMoveCardinal : pawn.TicksPerMoveDiagonal;

            // Log.Message("after patch: " + __result);
        }
    }
}
