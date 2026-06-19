using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace StagzMerfolk;

// Currently unused?
[UsedImplicitly]
public class ConditionalStatEffector_NotWater : ConditionalStatAffecter
{
    public override bool Applies(StatRequest req)
    {
        //Check if request is valid (request is for a pawn and pawn is on a map)
        var pawn = req.Thing as Pawn;
        if (!req.HasThing || pawn?.Map == null) return false;

        //Check if pawn is not on water
        return !pawn.OnWater();
    }

    public override string Label => field ??= "StagzMerfolk_ConditionalStatEffector_NotWater".Translate();
}