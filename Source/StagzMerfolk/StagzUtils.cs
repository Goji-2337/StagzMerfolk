using RimWorld;
using UnityEngine;
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
    
    public static Color? TryGetMerrenScaleColor(this Pawn pawn)
    {
        if (pawn?.genes?.GetFirstGeneOfType<Stagz_Gene_Tail_Fish>() != null)
        {
            return pawn.genes.GetFirstGeneOfType<Stagz_Gene_Tail_Fish>().ChosenColor;
        }
        return pawn?.genes?.GetFirstGeneOfType<Stagz_GeneWithScaleColor>()?.ChosenColor;
    }
    
    public static Color GetMerrenScaleColorOrFailsafe(this Pawn pawn)
    {
        return pawn.TryGetMerrenScaleColor() ?? pawn.story?.HairColor ?? Color.white;
    }
    
    public static void TrySetMerrenScaleColor(this Pawn pawn, Color color)
    {
        pawn?.genes?.GetFirstGeneOfType<Stagz_GeneWithScaleColor>()?.ChosenColor = color;
    }
}