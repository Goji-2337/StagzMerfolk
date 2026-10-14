using RimWorld;
using Verse;
// ReSharper disable UnassignedField.Global

namespace StagzMerfolk;

[DefOf]
public class StagzDefOf
{
    public static GeneDef Stagz_Charming;
    public static GeneDef Stagz_RainVeil;
    public static GeneDef Stagz_Raincaller;
    public static GeneDef Stagz_BodyFin;
    public static GeneDef Stagz_Aquatic;

    public static MeditationFocusDef Stagz_Water;

    public static HediffDef Stagz_Dehydration;
    public static NeedDef Stagz_NeedAquatic;
    public static JobDef Stagz_HydrateAquaticJob;

    public static BodyPartGroupDef Feet;
    public static HediffDef Stagz_Tail;

    public static JobDef Stagz_GotoWaterCell;
    public static JobDef Stagz_Wait_Hydrate;
    [MayRequire("balistafreak.StandaloneHotSpring")]
    public static HediffDef IntheStandaloneHotSpring;
    
    public static JobDef Stagz_OpenDialog_ColorPickerForGenesWithScales;
    public static JobDef Stagz_ChangeScaleColorAtComp;
    public static JobDef Stagz_ChangeScaleColorAtDye;
    public static ThingDef Stagz_Filth_Dye;
    public static ThingDef Stagz_Filth_DyeSmear;
    public static HediffDef Stagz_CoveredInDye;
    public static EffecterDef Stagz_DyeingTail;
    
    [MayRequireRoyalty]
    public static IncidentDef Stagz_VirtuosoSummoned;
    public static EffecterDef Stagz_DeepDiveSubmerge;
    public static SoundDef Stagz_DeepDiveEmergeSound;
    [MayRequire("GM.Nautian.Style")]
    public static StyleCategoryDef GM_Ocean;

    public static ShaderTypeDef ScalyGenes;
    
    public static MentalStateDef GiveUpExit;
    public static HediffDef Stagz_Infatuated;

    public static AbilityDef Stagz_SirenCallCharm;
    
    static StagzDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(StagzDefOf));
    }
}