namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class ParticleSystemStyles
    {
        public GUIStyle checkmark = FindStyle("ShurikenCheckMark");
        public GUIStyle effectBgStyle = FindStyle("ShurikenEffectBg");
        public GUIStyle emitterHeaderStyle = FindStyle("ShurikenEmitterTitle");
        public GUIStyle label = FindStyle("ShurikenLabel");
        public GUIStyle line = FindStyle("ShurikenLine");
        public GUIStyle minMaxCurveStateDropDown = FindStyle("ShurikenDropdown");
        public GUIStyle minus = FindStyle("ShurikenMinus");
        public GUIStyle moduleBgStyle = FindStyle("ShurikenModuleBg");
        public GUIStyle moduleHeaderStyle = FindStyle("ShurikenModuleTitle");
        public GUIStyle modulePadding = new GUIStyle();
        public GUIStyle numberField = FindStyle("ShurikenValue");
        public GUIStyle objectField = FindStyle("ShurikenObjectField");
        public GUIStyle plus = FindStyle("ShurikenPlus");
        public GUIStyle popup = FindStyle("ShurikenPopUp");
        private static ParticleSystemStyles s_ParticleSystemStyles;
        public GUIStyle selectionMarker = FindStyle("IN ThumbnailShadow");
        public GUIStyle toggle = FindStyle("ShurikenToggle");
        public GUIStyle toolbarButtonLeftAlignText = new GUIStyle(FindStyle("ToolbarButton"));
        public Texture2D warningIcon;

        private ParticleSystemStyles()
        {
            this.emitterHeaderStyle.clipping = TextClipping.Clip;
            this.emitterHeaderStyle.padding.right = 0x2d;
            this.warningIcon = EditorGUIUtility.LoadIcon("console.infoicon.sml");
            this.toolbarButtonLeftAlignText.alignment = TextAnchor.MiddleLeft;
            this.modulePadding.padding = new RectOffset(3, 3, 4, 2);
        }

        private static GUIStyle FindStyle(string styleName)
        {
            return styleName;
        }

        public static ParticleSystemStyles Get()
        {
            if (s_ParticleSystemStyles == null)
            {
                s_ParticleSystemStyles = new ParticleSystemStyles();
            }
            return s_ParticleSystemStyles;
        }
    }
}

