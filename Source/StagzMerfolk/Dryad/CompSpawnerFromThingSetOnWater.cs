using System.Collections.Generic;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace StagzMerfolk;

public class CompSpawnerFromThingSetOnWater : ThingComp
{
    private int ticksUntilSpawn;
    public CompProperties_SpawnerFromThingSetOnWater Props => (CompProperties_SpawnerFromThingSetOnWater)props;

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        if (!respawningAfterLoad)
        {
            ResetTicksUntilSpawn();
        }
    }
    public override void CompTickInterval(int delta)
    {
        if (!parent.Spawned) return;
        if (parent is Pawn p && !p.OverOrInWater()) return;
        ticksUntilSpawn -= delta;
        CheckShouldSpawn();
    }

    private void CheckShouldSpawn()
    {
        if (ticksUntilSpawn <= 0)
        {
            ResetTicksUntilSpawn();
            TryDoSpawn();
        }
    }

    private void ResetTicksUntilSpawn()
    {
        ticksUntilSpawn = Props.ticksToSpawnRange.RandomInRange;
    }

    public bool TryDoSpawn()
    {
        ThingSetMakerParams parms = default;
        parms.totalMarketValueRange = Props.marketValueRange;
        parms.countRange = Props.thingsToSpawn;

        List<Thing> loot = Props.thingSetMakerDef.root.Generate(parms);
        Log.Message(loot.ToStringSafeEnumerable());

        foreach (var thing in loot)
        {
            GenSpawn.Spawn(thing, parent.Position, parent.Map);
        }

        Messages.Message("MessageCompSpawnerSpawnedItem".Translate(loot.FirstOrFallback().LabelCap), loot.FirstOrFallback(), MessageTypeDefOf.PositiveEvent);
        return true;
    }
    
    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        if (DebugSettings.ShowDevGizmos)
        {
            yield return new Command_Action
            {
                defaultLabel = "DEV: Spawn items",
                action = delegate
                {
                    ResetTicksUntilSpawn();
                    TryDoSpawn();
                }
            };
        }
    }

    public override string CompInspectStringExtra()
    {
        {
            return $"{"NextSpawnedResourceIn".Translate()}: {ticksUntilSpawn.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor)}";
        }
    }
    public override void PostExposeData()
    {
        Scribe_Values.Look(ref ticksUntilSpawn, "ticksUntilSpawn");
    }
}

[PublicAPI]
public class CompProperties_SpawnerFromThingSetOnWater : CompProperties
{
    public CompProperties_SpawnerFromThingSetOnWater()
    {
        compClass = typeof(CompSpawnerFromThingSetOnWater);
    }

    public ThingSetMakerDef thingSetMakerDef;
    public IntRange ticksToSpawnRange;
    public FloatRange marketValueRange;
    public IntRange thingsToSpawn;
}