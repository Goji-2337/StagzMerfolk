using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace StagzMerfolk;

public class CompSpawnerFromThingSetOnWater : ThingComp
{
    private int ticksUntilSpawn;

    public CompProperties_SpawnerFromThingSetOnWater Props
    {
        get { return (CompProperties_SpawnerFromThingSetOnWater)props; }
    }

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        if (!respawningAfterLoad)
        {
            ResetTicksUntilSpawn();
        }
    }

    public override void CompTick()
    {
        TickInterval(1);
    }

    public override void CompTickRare()
    {
        TickInterval(250);
    }

    private void TickInterval(int interval)
    {
        if (!parent.Spawned)
        {
            return;
        }

        if (parent is Pawn p && !p.OnWater())
        {
            return;
        }

        ticksUntilSpawn -= interval;
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
        ThingSetMakerParams parms = default(ThingSetMakerParams);
        parms.totalMarketValueRange = Props.marketValueRange;
        parms.countRange = Props.thingsToSpawn;

        List<Thing> loot = Props.thingSetMakerDef.root.Generate(parms);
        Log.Message(loot.ToStringSafeEnumerable());

        foreach (var thing in loot)
        {
            GenSpawn.Spawn(thing, parent.Position, parent.Map);
        }

        Messages.Message("MessageCompSpawnerSpawnedItem".Translate(loot.First().LabelCap), loot.First(), MessageTypeDefOf.PositiveEvent, true);
        return true;
    }
    
    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        if (DebugSettings.ShowDevGizmos)
        {
            yield return new Command_Action
            {
                defaultLabel = "DEV: Spawn from" + this.Props.thingSetMakerDef.label,
                icon = TexCommand.DesirePower,
                action = delegate()
                {
                    this.ResetTicksUntilSpawn();
                    this.TryDoSpawn();
                }
            };
        }
        yield break;
    }

    public override string CompInspectStringExtra()
    {
        {
            return "NextSpawnedResourceIn".Translate().Resolve() + ": " + ticksUntilSpawn.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
        }
    }
    public override void PostExposeData()
    {
        Scribe_Values.Look<int>(ref this.ticksUntilSpawn, "ticksUntilSpawn", 0, false);
    }
}

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