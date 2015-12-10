namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class SizeByVelocityModuleUI : ModuleUI
    {
        private SerializedMinMaxCurve m_Curve;
        private SerializedProperty m_Range;
        private static Texts s_Texts;

        public SizeByVelocityModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "SizeBySpeedModule", displayName)
        {
            base.m_ToolTip = "Controls the size of each particle based on its speed.";
        }

        protected override void Init()
        {
            if (this.m_Curve == null)
            {
                if (s_Texts == null)
                {
                    s_Texts = new Texts();
                }
                this.m_Curve = new SerializedMinMaxCurve(this, s_Texts.size);
                this.m_Curve.m_AllowConstant = false;
                this.m_Range = base.GetProperty("range");
            }
        }

        public override void OnInspectorGUI(ParticleSystem s)
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            ModuleUI.GUIMinMaxCurve(s_Texts.size, this.m_Curve);
            ModuleUI.GUIMinMaxRange(s_Texts.velocityRange, this.m_Range);
        }

        private class Texts
        {
            public GUIContent size = new GUIContent("Size", "Controls the size of each particle based on its speed.");
            public GUIContent velocityRange = new GUIContent("Speed Range", "Remaps speed in the defined range to a size.");
        }
    }
}

