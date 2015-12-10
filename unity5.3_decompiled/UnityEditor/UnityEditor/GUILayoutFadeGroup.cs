namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal sealed class GUILayoutFadeGroup : GUILayoutGroup
    {
        public float fadeValue;
        public Color guiColor;
        public bool wasGUIEnabled;

        public override void CalcHeight()
        {
            base.CalcHeight();
            base.minHeight *= this.fadeValue;
            base.maxHeight *= this.fadeValue;
        }
    }
}

