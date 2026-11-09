using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
// ReSharper disable MemberCanBePrivate.Global

namespace StagzMerfolk;

//don't forget to nullcheck "this" when adding an extension method
public static class Utils
{
    extension(Pawn pawn)
    {
        public bool InRain() => pawn?.Map != null && !pawn.Position.Roofed(pawn.Map) && pawn.Map.weatherManager.RainRate > 0.01f;
        //GetTerrain can never be null. Defaults to soil
        public bool OverWater() => pawn?.Map != null && pawn.Position.GetTerrain(pawn.Map).IsWater;
        public bool OverOrInWater() => pawn.OverWater() || pawn.IsSubmerged();
        public bool IsInCaravanOnWaterFeatures() =>
            pawn.IsCaravanMember() 
            && pawn.GetCaravan() is { } caravan 
            && (Find.WorldGrid[caravan.Tile] is SurfaceTile tile && (tile.IsCoastal || tile.Rivers is [_, ..]) 
                || Find.WorldGrid[caravan.Tile].IsSubmerged());
        public Color GetMerrenScaleColorOrFailsafe() => pawn.MerrenScaleColor ?? pawn?.story?.HairColor ?? Color.white;
        public Color? MerrenScaleColor
        {
            get => pawn?.genes?.GetFirstGeneOfType<Gene_WithScaleColor>()?.ChosenColor;
            set => pawn?.genes?.GetFirstGeneOfType<Gene_WithScaleColor>()?.ChosenColor = value;
        }

        public Color? PendingMerrenScaleColor
        {
            get => pawn?.genes?.GetFirstGeneOfType<Gene_WithScaleColor>()?.PendingColor;
            set => pawn?.genes?.GetFirstGeneOfType<Gene_WithScaleColor>()?.PendingColor = value;
        }

        public bool HasActiveFishtail() => pawn?.genes?.HasActiveGene(StagzDefOf.Stagz_Fishtail) == true;
        
        public void RemoveLegOnlyApparel()
        {
            if (pawn?.apparel?.WornApparel == null) return;
            for (int i = pawn.apparel.WornApparel.Count - 1; i >= 0; i--)
            {
                var apparel = pawn.apparel.WornApparel[i];
                if (apparel.def.apparel.bodyPartGroups.CoversMoreThanJustLegs()) continue;
                //tries to drop on the floor, otherwise silently deletes from game
                if (!pawn.Spawned)
                {
                    pawn.apparel.WornApparel.Remove(apparel);
                }
                else
                {
                    pawn.apparel.TryDrop(apparel);
                    Messages.Message("StagzMerfolk_CannotWearBecauseOfTail".Translate(pawn.LabelShort), MessageTypeDefOf.NeutralEvent);
                }
            }
        }
    }
    extension(List<BodyPartGroupDef> bodyPartGroups)
    {
        public bool GroupsContainsLegsOrFeet()
        {
            List<BodyPartGroupDef> LegsOrFeetGroups = [BodyPartGroupDefOf.Legs, StagzDefOf.Feet];
            return bodyPartGroups?.Exists(LegsOrFeetGroups.Contains) == true;
        }
        public bool CoversMoreThanJustLegs()
        {
            List<BodyPartGroupDef> LegsOrFeetGroups = [BodyPartGroupDefOf.Legs, StagzDefOf.Feet];
            return bodyPartGroups?.Any(group => !LegsOrFeetGroups.Contains(group)) == true;
        }
    }
}