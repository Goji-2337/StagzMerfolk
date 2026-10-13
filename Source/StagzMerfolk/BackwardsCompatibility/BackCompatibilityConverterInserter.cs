using System.Collections.Generic;
using HarmonyLib;
using Verse;

namespace StagzMerfolk;

[StaticConstructorOnStartup]
public static class BackCompatibilityConverterInserter
{
    static BackCompatibilityConverterInserter()
    {
        List<BackCompatibilityConverter> list = (List<BackCompatibilityConverter>) typeof(BackCompatibility).GetField("conversionChain", AccessTools.all)!.GetValue(null);
        list.Add(new BackCompatibilityConverter_1_6());
    }
}