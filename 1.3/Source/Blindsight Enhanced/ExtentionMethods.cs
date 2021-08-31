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

        public static bool RedButton(this Listing_Standard l, Rect rect ,string label, Color textColor, Color bgColor, Color mouseoverColor, bool doMouseoverSound = true)
        {
            Color color = GUI.color;
            Rect btnRect = rect.BottomPartPixels(Text.LineHeight * 4).RightPart(0.2f).TopPartPixels(Text.LineHeight * 2);
            TextAnchor anchor = Text.Anchor;

            Texture2D btnBGMouseover = ContentFinder<Texture2D>.Get("UI/Widgets/ButtonBGMouseover", true);
            Texture2D btnBGAtlas = ContentFinder<Texture2D>.Get("UI/Widgets/ButtonBG", true);
            Texture2D btnBGAtlasClick = ContentFinder<Texture2D>.Get("UI/Widgets/ButtonBGClick", true);

            Texture2D atlas = btnBGAtlas;
            if (Mouse.IsOver(btnRect))
            {
                atlas = btnBGMouseover;
                GUI.color = mouseoverColor;
                if (Input.GetMouseButton(0))
                {
                    atlas = btnBGAtlasClick;
                }
            }
            if (doMouseoverSound) Verse.Sound.MouseoverSounds.DoRegion(btnRect);

            GUI.color = bgColor;
            Widgets.DrawAtlas(btnRect, atlas);
            GUI.color = color;

            Text.Anchor = TextAnchor.MiddleCenter;
            bool wordWrap = Text.WordWrap;
            if (btnRect.height < Text.LineHeight * 2f)
            {
                Text.WordWrap = false;
            }
            TaggedString str = new TaggedString(label).Colorize(textColor);
            Widgets.Label(btnRect, str);
            Text.Anchor = anchor;
            Text.WordWrap = wordWrap;
            return Widgets.ButtonInvisible(btnRect, false);
        }
    }
}
