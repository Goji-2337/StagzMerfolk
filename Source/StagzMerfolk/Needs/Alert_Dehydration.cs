using System.Collections.Generic;
using System.Text;
using RimWorld;
using Verse;

namespace StagzMerfolk;

public class Alert_Dehydration : Alert
{
    private readonly List<Pawn> dehydratedColonists = [];
    private readonly StringBuilder sb = new StringBuilder();

    public Alert_Dehydration()
    {
        defaultLabel = "StagzMerfolk_Dehydration".Translate();
        defaultPriority = AlertPriority.High;
    }

    private List<Pawn> DehydratedColonists
    {
        get
        {
            dehydratedColonists.Clear();
            foreach (Pawn pawn in PawnsFinder.AllMapsCaravansAndTravellingTransporters_AliveSpawned_FreeColonists_NoSuspended)
            {
                Stagz_Need_Aquatic need = pawn.needs.TryGetNeed(StagzDefOf.Stagz_NeedAquatic) as Stagz_Need_Aquatic;
                if (need != null && need.Dehydrating)
                    dehydratedColonists.Add(pawn);
            }
            return dehydratedColonists;
        }
    }

    public override TaggedString GetExplanation()
    {
        sb.Length = 0;
        foreach (Pawn pawn in dehydratedColonists)
            this.sb.AppendLine("  - " + pawn.NameShortColored.Resolve());
        return "StagzMerfolk_DehydrationDesc".Translate((NamedArgument) sb.ToString().TrimEndNewlines());
    }

    public override AlertReport GetReport() => AlertReport.CulpritsAre(DehydratedColonists);
}