using System;
using System.Xml;
using RimWorld;
using Verse;
using Verse.AI;

namespace StagzMerfolk;

public class BackCompatibilityConverter_1_6 : BackCompatibilityConverter
{
    //TODO TODO TODO
    public override bool AppliesToVersion(int majorVer, int minorVer)
    {
        return majorVer switch
        {
            1 => minorVer <= 6, 
            0 => true, 
            _ => false, 
        };
    }

    public override string BackCompatibleDefName(Type defType, string defName, bool forDefInjections = false, XmlNode node = null)
    {
        if (defType == typeof(GeneDef) && defName == "Stagz_WebbedHands")
        {
            return "WebbedPhalanges";
        }
        
        if (defType == typeof(GeneDef) &&
            defName is "Stagz_Gene_Tail_Fish_Random" or "Stagz_Gene_Tail_Fish_Teal" 
                or "Stagz_Gene_Tail_Fish_Orange" or "Stagz_Gene_Tail_Fish_Black" 
                or "Stagz_Gene_Tail_Fish_White" or "Stagz_Gene_Tail_Fish_Cyan" 
                or "Stagz_Gene_Tail_Fish_Magenta" or "Stagz_Gene_Tail_Fish_Salmon")
        {
            return "Stagz_Fishtail";
        }

        if (defType == typeof(MentalStateDef) && defName is "Stagz_Charmed" or "Stagz_VeryCharmed")
        {
            return "Berserk";
        }
        
        if (defType == typeof(LetterDef) && defName is "Stagz_AcceptCharmedJoiner")
        {
            return "Stagz_AcceptVirtuoso";
        }
        return null;
    }

    public override Type GetBackCompatibleType(Type baseType, string providedClassName, XmlNode node)
    {
        return providedClassName switch
        {
            "StagzMerfolk.Gene_Fishtail_Randomizer" or "StagzMerfolk.Stagz_Gene_Tail_Fish" => typeof(Gene_Fishtail),
            "StagzMerfolk.Stagz_Need_Aquatic" => typeof(Need_Aquatic),
            "StagzMerfolk.MentalState_Charmed" or "StagzMerfolk.MentalState_VeryCharmed" => typeof(MentalState_Berserk),
            "StagzMerfolk.ChoiceLetter_AcceptCharmedJoiner" => typeof(ChoiceLetter_AcceptVirtuoso),
            _ => null
        };
    }

    public override void PostExposeData(object obj)
    {
        if (Scribe.mode == LoadSaveMode.LoadingVars && obj is Pawn pawn)
        {
            Ability ability = pawn.abilities?.abilities.FirstOrFallback(x => x.def != null && x.def == StagzDefOf.Stagz_SirenCallCharm);
            if (ability != null)
            {
                pawn.abilities.RemoveAbility(ability.def);
                pawn.abilities.GainAbility(StagzDefOf.Stagz_SirenCallCharm);
            }
        }
    }
}