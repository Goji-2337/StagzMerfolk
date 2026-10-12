using System.Collections.Generic;
using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse;

namespace StagzMerfolk;

[UsedImplicitly]
public class Command_Raincall : Command_Ability
{
    private readonly CompAbilityEffect_CallWeather cachedComp;
    public Command_Raincall(Ability ability, Pawn pawn) : base(ability, pawn)
    {
        try
        {
            for (int i = 0; i < Ability.comps.Count; i++)
            {
                if (Ability.comps[i] is CompAbilityEffect_CallWeather result)
                {
                    cachedComp = result;
                }
            }
        } catch
        {
            Log.Error("CompAbilityEffect_CallWeather is missing from Raincall.");
        }
    }

    public override void ProcessInput(Event ev)
    {
        cachedComp.weatherToBeCalled = cachedComp.Props.weatherDefsForRandomRoll.RandomElement();
        base.ProcessInput(ev);
    }

    public override IEnumerable<FloatMenuOption> RightClickFloatMenuOptions
    {
        get
        {
            foreach (WeatherDef weather in cachedComp.Props.weatherDefsForSelection)
            {
                yield return new FloatMenuOption(weather.label, delegate
                {
                    cachedComp.weatherToBeCalled = weather;
                    //will only ever work as untargetable ability unless rewritten
                    ability.QueueCastingJob((LocalTargetInfo) (Thing) ability.pawn, LocalTargetInfo.Invalid);
                });
            }
        }
    }
}