using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace StagzMerfolk;

[DefOf]
[PublicAPI]
public class StagzDefOf
{

    public static GeneDef Stagz_KeenReflexes;
    public static GeneDef Stagz_Charming;
    public static GeneDef Stagz_RainVeil;
    public static GeneDef Stagz_Raincaller;
    public static GeneDef Stagz_BodyFin;
    public static GeneDef Stagz_Aquatic;

    public static MeditationFocusDef Stagz_Water;

    public static HediffDef Stagz_Dehydration;
    public static NeedDef Stagz_NeedAquatic;
    public static JobDef Stagz_HydrateAquaticJob;

    public static AbilityDef Stagz_DeepDive;

    public static BodyPartGroupDef Feet;
    public static HediffDef Stagz_Tail;

    public static JobDef Stagz_GotoWaterCell;
    public static JobDef Stagz_Wait_Hydrate;
    
    public static MentalStateDef Stagz_Charmed;
    public static MentalStateDef Stagz_VeryCharmed;

    public static LetterDef Stagz_AcceptCharmedJoiner;
    public static LetterDef Stagz_AcceptAriel;

    public static PawnKindDef Stagz_Ariel;
    public static IncidentDef Stagz_ArielSummoned;
    [MayRequireRoyalty] public static IncidentDef Stagz_VirtuosoSummoned;
    
    static StagzDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(StagzDefOf));
    }
}

public static class StagzCollections
{
    public static readonly MentalStateDef[] StateDefs = { StagzDefOf.Stagz_Charmed, StagzDefOf.Stagz_VeryCharmed };
}