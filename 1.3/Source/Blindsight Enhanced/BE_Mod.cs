using Verse;
using UnityEngine;
using System;

namespace Blindsight_Enhanced
{
    public class BE_Settings : ModSettings
    {
        // Psysight Base Sight
        public static float BaseSight
        {
            get { return _baseSight; }
            set { _baseSight = Round(value); }
        }
        private static float _baseSight = 0.2f;
        // Psysight Levels
        public static float Lvl0
        {
            get { return _lvl0; }
            set { _lvl0 = Round(value); }
        }
        private static float _lvl0 = 0.0f;
        public static float Lvl1
        {
            get { return _lvl1; }
            set { _lvl1 = Round(value); }
        }
        private static float _lvl1 = 0.05f;
        public static float Lvl2
        {
            get { return _lvl2; }
            set { _lvl2 = Round(value); }
        }
        private static float _lvl2 = 0.3f;
        public static float Lvl3
        {
            get { return _lvl3; }
            set { _lvl3 = Round(value); }
        }
        private static float _lvl3 = 0.55f;
        public static float Lvl4
        {
            get { return _lvl4; }
            set { _lvl4 = Round(value); }
        }
        private static float _lvl4 = 0.8f;
        public static float Lvl5
        {
            get { return _lvl5; }
            set { _lvl5 = Round(value); }
        }
        private static float _lvl5 = 1.05f;
        public static float Lvl6
        {
            get { return _lvl6; }
            set { _lvl6 = Round(value); }
        }
        private static float _lvl6 = 1.3f;

        private static float Round(float f)
        {
            return (float)(Math.Round(f, 2));
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref _baseSight, "BE_Psysight_BaseSight");
            Scribe_Values.Look(ref _lvl0, "BE_Psysight_Level_0");
            Scribe_Values.Look(ref _lvl1, "BE_Psysight_Level_1");
            Scribe_Values.Look(ref _lvl2, "BE_Psysight_Level_2");
            Scribe_Values.Look(ref _lvl3, "BE_Psysight_Level_3");
            Scribe_Values.Look(ref _lvl4, "BE_Psysight_Level_4");
            Scribe_Values.Look(ref _lvl5, "BE_Psysight_Level_5");
            Scribe_Values.Look(ref _lvl6, "BE_Psysight_Level_6");
        }

        public static float Max = 3f;
        public static bool isChanged = false;

        public static void ResetSettings()
        {
            BaseSight = 0.2f;
            Lvl0 = 0f;
            Lvl1 = 0.05f;
            Lvl2 = 0.3f;
            Lvl3 = 0.55f;
            Lvl4 = 0.8f;
            Lvl5 = 1.05f;
            Lvl6 = 1.3f;
        }
    }

    class BE_Mod : Mod
    {
        BE_Settings settings;
        public BE_Mod(ModContentPack mcp) : base(mcp)
        {
            settings = GetSettings<BE_Settings>();
        }

        private static void ResetButton(bool active)
        {
            if (active)
            {
                Log.Message("Reset BE Settings");
                BE_Settings.ResetSettings();
            }
        }

        private static float BE_Max(float f)
        {
            if (f >= BE_Settings.Max) return BE_Settings.Max;
            if (f <= BE_Settings.BaseSight) return BE_Settings.BaseSight;
            return f;
        }

        private float BaseSight
        {
            get { return BE_Settings.BaseSight; }
            set { if (value > 0) BE_Settings.BaseSight = value; }
        }

        private float TotalLvl0
        {
            get { return BE_Max(BE_Settings.BaseSight + BE_Settings.Lvl0); }
            set { BE_Settings.Lvl0 = value - BE_Settings.BaseSight; }
        }
        private float TotalLvl1
        {
            get { return BE_Max(BE_Settings.BaseSight + BE_Settings.Lvl1); }
            set { BE_Settings.Lvl1 = value - BE_Settings.BaseSight; }
        }
        private float TotalLvl2
        {
            get { return BE_Max(BE_Settings.BaseSight + BE_Settings.Lvl2); }
            set { BE_Settings.Lvl2 = value - BE_Settings.BaseSight; }
        }
        private float TotalLvl3
        {
            get { return BE_Max(BE_Settings.BaseSight + BE_Settings.Lvl3); }
            set { BE_Settings.Lvl3 = value - BE_Settings.BaseSight; }
        }
        private float TotalLvl4
        {
            get { return BE_Max(BE_Settings.BaseSight + BE_Settings.Lvl4); }
            set { BE_Settings.Lvl4 = value - BE_Settings.BaseSight; }
        }
        private float TotalLvl5
        {
            get { return BE_Max(BE_Settings.BaseSight + BE_Settings.Lvl5); }
            set { BE_Settings.Lvl5 = value - BE_Settings.BaseSight; }
        }
        private float TotalLvl6
        {
            get { return BE_Max(BE_Settings.BaseSight + BE_Settings.Lvl6); }
            set { BE_Settings.Lvl6 = value - BE_Settings.BaseSight; }
        }


        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard l = new Listing_Standard();
            l.Begin(inRect);
            l.Gap(8f);
            BaseSight = (float)l.LabeledSlider("BaseSight".Translate() + ": " + (BE_Settings.BaseSight * 100).ToString() + "% Sight", BaseSight, 0.01f, BE_Settings.Max);
            l.Gap(24f);

            Text.Font = GameFont.Medium;
            l.Label("PsysightLevels".Translate());
            Text.Font = GameFont.Small;

            TotalLvl0 = (float)l.LabeledSlider("Level".Translate() + " 0: " + ((BE_Settings.BaseSight + BE_Settings.Lvl0) * 100).ToString() + "% Sight", TotalLvl0, 0f, BE_Settings.Max);
            TotalLvl1 = (float)l.LabeledSlider("Level".Translate() + " 1: " + ((BE_Settings.BaseSight + BE_Settings.Lvl1) * 100).ToString() + "% Sight", TotalLvl1, 0f, BE_Settings.Max);
            TotalLvl2 = (float)l.LabeledSlider("Level".Translate() + " 2: " + ((BE_Settings.BaseSight + BE_Settings.Lvl2) * 100).ToString() + "% Sight", TotalLvl2, 0f, BE_Settings.Max);
            TotalLvl3 = (float)l.LabeledSlider("Level".Translate() + " 3: " + ((BE_Settings.BaseSight + BE_Settings.Lvl3) * 100).ToString() + "% Sight", TotalLvl3, 0f, BE_Settings.Max);
            TotalLvl4 = (float)l.LabeledSlider("Level".Translate() + " 4: " + ((BE_Settings.BaseSight + BE_Settings.Lvl4) * 100).ToString() + "% Sight", TotalLvl4, 0f, BE_Settings.Max);
            TotalLvl5 = (float)l.LabeledSlider("Level".Translate() + " 5: " + ((BE_Settings.BaseSight + BE_Settings.Lvl5) * 100).ToString() + "% Sight", TotalLvl5, 0f, BE_Settings.Max);
            TotalLvl6 = (float)l.LabeledSlider("Level".Translate() + " 6: " + ((BE_Settings.BaseSight + BE_Settings.Lvl6) * 100).ToString() + "% Sight", TotalLvl6, 0f, BE_Settings.Max);
            l.Gap(24f);

            ResetButton(l.ButtonText("ResetSettings".Translate()));
            l.Gap(8f);

            TaggedString str = new TaggedString("SettingsApplyOnRestart".Translate()).Colorize(new Color(0.8f, 0.4f, 0.4f, 1f));
            l.Label(str);

            base.DoSettingsWindowContents(inRect);
            l.End();
        }

        public override string SettingsCategory()
        {
            return "BlindsightEnhanced".Translate();
        }
    }
}
