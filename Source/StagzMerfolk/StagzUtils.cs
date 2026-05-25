using UnityEngine;
using Verse;

namespace StagzMerfolk;

public static class StagzUtils
{
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
        if (pawn?.genes?.GetFirstGeneOfType<Gene_Fishtail>() != null)
        {
            return pawn.genes.GetFirstGeneOfType<Gene_Fishtail>().ChosenColor;
        }
        return pawn?.genes?.GetFirstGeneOfType<Gene_WithScaleColor>()?.ChosenColor;
    }
    
    public static Color GetMerrenScaleColorOrFailsafe(this Pawn pawn)
    {
        return pawn.TryGetMerrenScaleColor() ?? pawn.story?.HairColor ?? Color.white;
    }
    
    public static void TrySetMerrenScaleColor(this Pawn pawn, Color color)
    {
        pawn?.genes?.GetFirstGeneOfType<Gene_WithScaleColor>()?.ChosenColor = color;
    }
}