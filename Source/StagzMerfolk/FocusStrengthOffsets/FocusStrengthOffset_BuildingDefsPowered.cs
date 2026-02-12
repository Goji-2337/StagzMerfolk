using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace StagzMerfolk;

public class FocusStrengthOffset_BuildingDefsPowered : FocusStrengthOffset_BuildingDefs
{
    public override bool CanApply(Thing parent, Pawn user = null)
    {
        return BuildingPoweredHelper.BuildingPowered(parent) && base.CanApply(parent, user);
    }

    public override float OffsetForBuilding(Thing b)
    {
        return BuildingPoweredHelper.BuildingPowered(b) ? base.OffsetFor(b.def) : 0f;
    }

    public override int BuildingCount(Thing parent)
    {
        if (parent == null || !parent.Spawned)
        {
            return 0;
        }

        int num = 0;
        List<Thing> forCell = parent.Map.listerBuldingOfDefInProximity.GetForCell(parent.Position, this.radius, this.defs, parent);
        for (int i = 0; i < forCell.Count; i++)
        {
            Thing b = forCell[i];
            if (BuildingPoweredHelper.BuildingPowered(b))
            {
                num++;
            }
        }
        return Math.Min(num, this.maxBuildings);
    }
}