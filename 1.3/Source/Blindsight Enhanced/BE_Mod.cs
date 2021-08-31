using Verse;
using UnityEngine;
using System;

namespace Blindsight_Enhanced
{
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
                string text = "ResetSettingsMsg".Translate();
                string title = "";
                Find.WindowStack.Add(new Dialog_MessageBox(text, "Confirm".Translate(), new Action(BE_Settings.ResetSettings), "GoBack".Translate(), null, title, true, new Action(BE_Settings.ResetSettings), null, WindowLayer.Dialog));
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

        private float Max
        {
            get { return BE_Settings.Max; }
            set { BE_Settings.Max = value; }
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

            l.CheckboxLabeled("IsCapped".Translate(), ref BE_Settings.isCapped);
            if (!BE_Settings.isCapped)
            {
                Max = (float)l.LabeledSlider("Max".Translate() + ": " + (BE_Settings.Max * 100).ToString() + "%", Max, 3f, 25f);
                l.Gap(Text.LineHeight * 2);
            }
            else l.Gap(Text.LineHeight * 3 + 4f);




            ResetButton(l.RedButton(inRect ,"ResetSettings".Translate(), Color.white, new Color(1f, 0.3f, 0.3f, 1f), new Color(1f, 0.5f, 0.5f, 1f)));
            l.Gap(8f);

            TaggedString str = new TaggedString("SettingsApplyOnRestart".Translate()).Colorize(new Color(0.8f, 0.4f, 0.4f, 1f));

            TextAnchor anchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleRight;
            Widgets.Label(inRect.BottomPartPixels(Text.LineHeight * 6).LeftPart(0.78f), str);
            Text.Anchor = anchor;

            l.End();
        }

        public override string SettingsCategory()
        {
            return "BlindsightEnhanced".Translate();
        }
    }
}
