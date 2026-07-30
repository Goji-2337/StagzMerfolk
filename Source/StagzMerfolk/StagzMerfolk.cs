using HarmonyLib;
using UnityEngine;
using Verse;

namespace StagzMerfolk;
    public class StagzMerfolk : Mod
    {
        private readonly StagzMerfolkSettings settings;

        public StagzMerfolk(ModContentPack content) : base(content)
        {
            settings = GetSettings<StagzMerfolkSettings>();
            var harmony = new Harmony("com.arquebus.rimworld.mod.stagzmerfolk");
            harmony.PatchAll();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            settings.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            if ("StagzMerfolk_Settings_Category".CanTranslate())
            {
                return "StagzMerfolk_Settings_Category".Translate();
            }
            else
            {
                return "Goji's Fantasy Race: Merren";
            }
        }
    }

    public class StagzMerfolkSettings : ModSettings
    {
        public static bool dbhCleaningCountsAsHydration;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref dbhCleaningCountsAsHydration, "StagzMerfolkDbhCleaningCountsAsHydration");
            base.ExposeData();
        }

        public void DoSettingsWindowContents(Rect inRect)
        {
            string label = "StagzMerfolk_Settings_DbhCleaningCountsAsHydrationLabel".CanTranslate() ? "StagzMerfolk_Settings_DbhCleaningCountsAsHydrationLabel".Translate() : "Dub's Bad Hygiene cleaning counts as hydration";
            string tooltip = "StagzMerfolk_Settings_DbhCleaningCountsAsHydrationTooltip".CanTranslate()
                ? "StagzMerfolk_Settings_DbhCleaningCountsAsHydrationTooltip".Translate()
                : "Enables aquatic hydration needs to be met when a pawn cleans themselves in Dub's Bad Hygiene.";
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.CheckboxLabeled(label, ref dbhCleaningCountsAsHydration, tooltip);
            listingStandard.End();
        }
    }