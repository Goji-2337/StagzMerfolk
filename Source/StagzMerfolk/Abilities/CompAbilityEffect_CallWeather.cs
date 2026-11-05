using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace StagzMerfolk;

public class CompAbilityEffect_CallWeather : CompAbilityEffect
{
    public new CompProperties_AbilityCallWeather Props => (CompProperties_AbilityCallWeather)props;
    public WeatherDef weatherToBeCalled;
    
    public override bool GizmoDisabled(out string reason)
    {
        if (parent.pawn.MapHeld.weatherDecider.ForcedWeather != null)
        {
            reason = "StagzMerfolk_ForcedWeatherActive".Translate();
            return true;
        }
        reason = null;
        return false;
    }
    
    public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
    {
        base.Apply(target, dest);
        
        //shouldn't happen, but
        if (weatherToBeCalled == null)
        {
            Log.Error("Tried to cast Raincall but weatherDef was null.");
            return;
        }
        
        //sets weather
        parent.pawn.MapHeld.weatherManager.TransitionTo(weatherToBeCalled);
        
        //forces duration
        AccessTools.FieldRefAccess<WeatherDecider, int>("curWeatherDuration").Invoke(parent.pawn.MapHeld.weatherDecider) = Props.weatherDuration;
        
        Messages.Message("StagzMerfolk_RainCall".Translate(parent.pawn.LabelShort, weatherToBeCalled.label) , parent.pawn, MessageTypeDefOf.NeutralEvent);
    }

    public override void PostExposeData()
    {
        if (Scribe.EnterNode("StagzMerfolk_Raincall"))
        {
            try
            {
                base.PostExposeData();
                Scribe_Defs.Look(ref weatherToBeCalled, "weatherToBeCalled");
            }
            finally
            {
                Scribe.ExitNode();
            }
        }
    }
}

[PublicAPI]
public class CompProperties_AbilityCallWeather : CompProperties_AbilityEffect
{
    public List<WeatherDef> weatherDefsForRandomRoll;
    public List<WeatherDef> weatherDefsForSelection;
    public int weatherDuration;
    public CompProperties_AbilityCallWeather() => compClass = typeof(CompAbilityEffect_CallWeather);
}