using HarmonyLib;
using RimWorld;
using Verse;

namespace StagzMerfolk.HarmonyPatches;

[HarmonyPatch(typeof(MeditationFocusDef), nameof(MeditationFocusDef.EnablingThingsExplanation))]
public class MeditationFocusDef_EnablingThingsExplanation_Patch
{
    public static void Postfix(Pawn pawn, MeditationFocusDef __instance, ref string __result)
    {
        if (!ModsConfig.RoyaltyActive) return;

        if (__instance == StagzDefOf.Stagz_Water && pawn.genes?.HasActiveGene(StagzDefOf.Stagz_Raincaller) == true)
        {
            __result += "\n  - " + "Stagz_UnlockedByGene".Translate() + " " + StagzDefOf.Stagz_Raincaller.LabelCap + ".";
        }
    }
}

[HarmonyPatch(typeof(MeditationFocusTypeAvailabilityCache), "PawnCanUseInt")]
public class MeditationFocusTypeAvailabilityCache_PawnCanUseInt_Patch
{
    public static void Postfix(Pawn p, MeditationFocusDef type, ref bool __result)
    {
        if (type == StagzDefOf.Stagz_Water)
        {
            __result = false;
            if (!ModsConfig.RoyaltyActive) return;

            if (p.genes?.HasActiveGene(StagzDefOf.Stagz_Raincaller) == true)
            {
                __result = true;
            }
        }
    }
}
