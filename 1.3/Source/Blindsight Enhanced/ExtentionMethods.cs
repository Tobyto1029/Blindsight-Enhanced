using Verse;
using UnityEngine;

namespace Blindsight_Enhanced
{
    public static class ExtentionMethods
    {
        public static float LabeledSlider(this Listing_Standard l, string label, float val, float min, float max)
        {
            Rect rect = l.GetRect(Text.LineHeight + 4f).Rounded();
            Rect sliderOffset = rect.RightPart(0.75f);
            Widgets.Label(rect, label);
            float valSetting = Widgets.HorizontalSlider(sliderOffset, val, min, max);
            return valSetting;
        }
    }
}
