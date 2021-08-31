using Verse;
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

        //Extra Options
        public static bool isCapped = true;
        public static float Max
        {
            get
            {
                if (isCapped) return 3f;
                return _max;
            }
            set
            {
                if (Round(value) <= 3f) _max = 3f;
                else _max = (float)Math.Round(value, 1);
            }
        }
        public static float _max = 3f;

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
            Scribe_Values.Look(ref isCapped, "BE_Is_Capped");
            Scribe_Values.Look(ref _max, "BE_Max");
        }



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
            isCapped = true;
            Max = 3f;
            Log.Message("[Blindsight Enhanced] Settings Reset");
        }
    }
}
