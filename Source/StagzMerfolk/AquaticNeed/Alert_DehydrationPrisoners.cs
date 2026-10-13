using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace StagzMerfolk;

[UsedImplicitly]
public class Alert_DehydrationPrisoners : Alert
{
    private readonly List<Pawn> dehydratedPawns = [];
    private readonly StringBuilder sb = new();

    public Alert_DehydrationPrisoners()
    {
        defaultLabel = "StagzMerfolk_DehydrationPrisoners".Translate();
        defaultPriority = AlertPriority.High;
    }

    private List<Pawn> DehydratedPawns
    {
        get
        {
            dehydratedPawns.Clear();
            foreach (Pawn pawn in PawnsFinder.AllMaps_PrisonersOfColonySpawned)
            {
                if (pawn.needs.TryGetNeed(StagzDefOf.Stagz_NeedAquatic) is Need_Aquatic need && need.Dehydrating)
                {
                    dehydratedPawns.Add(pawn);
                }
            }
            return dehydratedPawns;
        }
    }
    
    public override TaggedString GetExplanation()
    {
        sb.Length = 0;
        foreach (Pawn pawn in dehydratedPawns) sb.AppendLine("  - " + pawn.NameShortColored.Resolve());
        return "StagzMerfolk_DehydrationPrisonersDesc".Translate((NamedArgument) sb.ToString().TrimEndNewlines());
    }

    public override AlertReport GetReport() => AlertReport.CulpritsAre(DehydratedPawns);
}