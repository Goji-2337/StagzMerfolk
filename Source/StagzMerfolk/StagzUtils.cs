using RimWorld;
using Verse;

namespace StagzMerfolk;

public static class StagzUtils
{
    
    public static bool IsWet(this Pawn pawn)
    {
        return OnWater(pawn) || InRain(pawn) || IsSoakingWet(pawn);
    }

    private static bool IsSoakingWet(Pawn pawn)
    {
        return pawn.needs.mood?.thoughts.memories.GetFirstMemoryOfDef(ThoughtDefOf.SoakingWet) != null;
    }

    public static bool InRain(this Pawn pawn)
    {
        return pawn.Map != null && pawn.Position.GetTerrain(pawn.Map) != null &&  
               !pawn.Position.Roofed(pawn.Map) && pawn.Map.weatherManager.RainRate > 0.01f;
    }

    public static bool OnWater(this Pawn pawn)
    {
        return pawn.Map != null && pawn.Position.GetTerrain(pawn.Map) != null && pawn.Position.GetTerrain(pawn.Map).IsWater;
    }

    public static bool InRiver(this Pawn pawn)
    {
        return pawn.Map != null && pawn.Position.GetTerrain(pawn.Map) != null && pawn.Position.GetTerrain(pawn.Map).IsRiver;
    }
}