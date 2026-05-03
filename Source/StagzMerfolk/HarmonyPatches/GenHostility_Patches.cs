using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace StagzMerfolk.HarmonyPatches;

internal static class CharmedHostilityPatcher
{
    private const string HarmonyId = "com.arquebus.rimworld.mod.stagzmerfolk.charmedhostility";

    private static readonly Harmony Harmony = new Harmony(HarmonyId);
    private static readonly HashSet<Pawn> ActiveCharmedPawns = new HashSet<Pawn>();
    private static readonly MethodInfo HostileToThingMethod = AccessTools.Method(typeof(GenHostility), nameof(GenHostility.HostileTo), new[] { typeof(Thing), typeof(Thing) });
    private static readonly MethodInfo HostileToFactionMethod = AccessTools.Method(typeof(GenHostility), nameof(GenHostility.HostileTo), new[] { typeof(Thing), typeof(Faction) });
    private static readonly HarmonyMethod HostileToThingPostfix = new HarmonyMethod(typeof(CharmedHostilityPatcher), nameof(PostfixHostileToThing));
    private static readonly HarmonyMethod HostileToFactionPostfix = new HarmonyMethod(typeof(CharmedHostilityPatcher), nameof(PostfixHostileToFaction));

    private static bool patched;

    public static void Register(Pawn pawn)
    {
        if (pawn == null)
        {
            return;
        }

        ActiveCharmedPawns.Add(pawn);
        EnsurePatched();
    }

    public static void Unregister(Pawn pawn)
    {
        if (pawn != null)
        {
            ActiveCharmedPawns.Remove(pawn);
        }

        PruneInactivePawns();
        if (ActiveCharmedPawns.Count == 0)
        {
            Unpatch();
        }
    }

    private static void EnsurePatched()
    {
        if (patched)
        {
            return;
        }

        Harmony.Patch(HostileToThingMethod, postfix: HostileToThingPostfix);
        Harmony.Patch(HostileToFactionMethod, postfix: HostileToFactionPostfix);
        patched = true;
    }

    private static void Unpatch()
    {
        if (!patched)
        {
            return;
        }

        Harmony.Unpatch(HostileToThingMethod, HarmonyPatchType.Postfix, HarmonyId);
        Harmony.Unpatch(HostileToFactionMethod, HarmonyPatchType.Postfix, HarmonyId);
        patched = false;
    }

    private static void PruneInactivePawns()
    {
        ActiveCharmedPawns.RemoveWhere(pawn => pawn == null || !IsCharmed(pawn));
    }

    private static bool IsCharmed(Pawn pawn)
    {
        var def = pawn?.MentalState?.def;
        return def == StagzDefOf.Stagz_Charmed || def == StagzDefOf.Stagz_VeryCharmed;
    }

    private static void PostfixHostileToThing(Thing a, Thing b, ref bool __result)
    {
        if (a.Destroyed || b.Destroyed || a == b)
        {
            return;
        }

        if (a is Pawn { Faction: not null } aPawn && b is Pawn { Faction: not null } bPawn &&
            (IsCharmed(aPawn) || IsCharmed(bPawn)))
        {
            __result = aPawn.Faction == bPawn.Faction;
        }
    }

    private static void PostfixHostileToFaction(Thing t, Faction fac, ref bool __result)
    {
        if (t.Destroyed || fac == null)
        {
            return;
        }

        if (t is Pawn { Faction: not null } pawn && IsCharmed(pawn))
        {
            __result = pawn.Faction == fac;
        }
    }
}
