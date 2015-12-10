namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class VelocityModuleUI : ModuleUI
    {
        private SerializedProperty m_InWorldSpace;
        private SerializedMinMaxCurve m_X;
        private SerializedMinMaxCurve m_Y;
        private SerializedMinMaxCurve m_Z;
        private static Texts s_Texts;

        public VelocityModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "VelocityModule", displayName)
        {
            base.m_ToolTip = "Controls the velocity of each particle during its lifetime.";
        }

        protected override void Init()
        {
            if (this.m_X == null)
            {
                if (s_Texts == null)
                {
                    s_Texts = new Texts();
                }
                this.m_X = new SerializedMinMaxCurve(this, s_Texts.x, "x", ModuleUI.kUseSignedRange);
                this.m_Y = new SerializedMinMaxCurve(this, s_Texts.y, "y", ModuleUI.kUseSignedRange);
                this.m_Z = new SerializedMinMaxCurve(this, s_Texts.z, "z", ModuleUI.kUseSignedRange);
                this.m_InWorldSpace = base.GetProperty("inWorldSpace");
            }
        }

        public override void OnInspectorGUI(ParticleSystem s)
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            base.GUITripleMinMaxCurve(GUIContent.none, s_Texts.x, this.m_X, s_Texts.y, this.m_Y, s_Texts.z, this.m_Z, null);
            ModuleUI.GUIBoolAsPopup(s_Texts.space, this.m_InWorldSpace, s_Texts.spaces);
        }

        public override void UpdateCullingSupportedString(ref string text)
        {
            this.Init();
            if ((!this.m_X.SupportsProcedural() || !this.m_Y.SupportsProcedural()) || !this.m_Z.SupportsProcedural())
            {
                text = text + "\n\tLifetime velocity curves use too many keys.";
            }
        }

        private class Texts
        {
            public GUIContent space = new GUIContent("Space", "Specifies if the velocity values are in local space (rotated with the transform) or world space.");
            public string[] spaces = new string[] { "Local", "World" };
            public GUIContent x = new GUIContent("X");
            public GUIContent y = new GUIContent("Y");
            public GUIContent z = new GUIContent("Z");
        }
    }
}

