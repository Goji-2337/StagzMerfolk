using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace StagzMerfolk;

//TODO: should convert to extensions block and maybe rename to StagzExtensions sometime
public static class StagzUtils
{
    public static bool InRain(this Pawn pawn)
    {
        return pawn.Map != null && !pawn.Position.Roofed(pawn.Map) && pawn.Map.weatherManager.RainRate > 0.01f;
    }

    public static bool OnWater(this Pawn pawn)
    {
        //GetTerrain can never be null. Defaults to soil
        return pawn.Map != null && pawn.Position.GetTerrain(pawn.Map).IsWater;
    }

    public static bool InRiver(this Pawn pawn)
    {
        //Same as above
        return pawn.Map != null && pawn.Position.GetTerrain(pawn.Map).IsRiver;
    }
    
    public static Color? TryGetMerrenScaleColor(this Pawn pawn) {
        
        return pawn?.genes?.GetFirstGeneOfType<Stagz_Gene_Tail_Fish>()?.ChosenColor
               ?? pawn?.genes?.GetFirstGeneOfType<Gene_WithScaleColor>()?.ChosenColor;
    }
    
    public static Color GetMerrenScaleColorOrFailsafe(this Pawn pawn)
    {
        return pawn.TryGetMerrenScaleColor() ?? pawn.story?.HairColor ?? Color.white;
    }
    
    public static void TrySetMerrenScaleColor(this Pawn pawn, Color color)
    {
        pawn?.genes?.GetFirstGeneOfType<Gene_WithScaleColor>()?.ChosenColor = color;
    }
    
    public static bool GroupsContainsLegsOrFeet(this List<BodyPartGroupDef> bodyPartGroups)
    {
        BodyPartGroupDef[] LegsOrFeetGroups = [BodyPartGroupDefOf.Legs, StagzDefOf.Feet];
        return bodyPartGroups.Exists(LegsOrFeetGroups.Contains);
    }

    public static bool CoversMoreThanJustLegs(this List<BodyPartGroupDef> bodyPartGroups)
    {
        BodyPartGroupDef[] LegsOrFeetGroups = [BodyPartGroupDefOf.Legs, StagzDefOf.Feet];
        return bodyPartGroups.Any(group => !LegsOrFeetGroups.Contains(group));
    }

    public static void RemoveLegOnlyApparel(this Pawn pawn, bool drop = false)
    {
        if (pawn?.apparel?.WornApparel == null) return;
        for (int i = pawn.apparel.WornApparel.Count - 1; i >= 0; i--)
        {
            var apparel = pawn.apparel.WornApparel[i];
            if (apparel.def.apparel.bodyPartGroups.CoversMoreThanJustLegs()) continue;
            //tries to drop on the floor, otherwise silently deletes from game
            if (drop)
            {
                pawn.apparel.TryDrop(apparel);
                Messages.Message("StagzMerfolk_CannotWearBecauseOfTail".Translate(pawn.LabelShort), MessageTypeDefOf.NeutralEvent);
            } else
            {
                pawn.apparel.WornApparel.Remove(apparel);
            }
        }
    }
}