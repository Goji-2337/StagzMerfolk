using HarmonyLib;
using RimWorld;
using Verse;

namespace StagzMerfolk.HarmonyPatches;

[HarmonyPatch(typeof(MeditationFocusDef), nameof(MeditationFocusDef.EnablingThingsExplanation))]
public class MeditationFocusDef_EnablingThingsExplanation_Patch
{
    private static bool Prepare() => ModsConfig.RoyaltyActive;

    public static void Postfix(Pawn pawn, MeditationFocusDef __instance, ref string __result)
    {
        if (__instance == StagzDefOf.Stagz_Water && pawn.genes?.HasActiveGene(StagzDefOf.Stagz_Raincaller) == true)
        {
            if (!__result.NullOrEmpty()) __result += "\n";
            __result += $"  - {"StagzMerfolk_UnlockedByGene".Translate()} {StagzDefOf.Stagz_Raincaller.LabelCap}.";
        }
    }
}

[HarmonyPatch(typeof(MeditationFocusTypeAvailabilityCache), "PawnCanUseInt")]
public class MeditationFocusTypeAvailabilityCache_PawnCanUseInt_Patch
{
    private static bool Prepare() => ModsConfig.RoyaltyActive;

    public static void Postfix(Pawn p, MeditationFocusDef type, ref bool __result)
    {
        if (type == StagzDefOf.Stagz_Water && p.genes?.HasActiveGene(StagzDefOf.Stagz_Raincaller) == true)
        {
            __result = true;
        }
    }
}
