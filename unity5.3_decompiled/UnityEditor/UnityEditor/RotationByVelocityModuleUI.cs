namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class RotationByVelocityModuleUI : ModuleUI
    {
        private SerializedProperty m_Range;
        private SerializedProperty m_SeparateAxes;
        private SerializedMinMaxCurve m_X;
        private SerializedMinMaxCurve m_Y;
        private SerializedMinMaxCurve m_Z;
        private static Texts s_Texts;

        public RotationByVelocityModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "RotationBySpeedModule", displayName)
        {
            base.m_ToolTip = "Controls the angular velocity of each particle based on its speed.";
        }

        protected override void Init()
        {
            if (this.m_Z == null)
            {
                if (s_Texts == null)
                {
                    s_Texts = new Texts();
                }
                this.m_X = new SerializedMinMaxCurve(this, s_Texts.x, "x", ModuleUI.kUseSignedRange);
                this.m_Y = new SerializedMinMaxCurve(this, s_Texts.y, "y", ModuleUI.kUseSignedRange);
                this.m_Z = new SerializedMinMaxCurve(this, s_Texts.z, "curve", ModuleUI.kUseSignedRange);
                this.m_X.m_RemapValue = 57.29578f;
                this.m_Y.m_RemapValue = 57.29578f;
                this.m_Z.m_RemapValue = 57.29578f;
                this.m_SeparateAxes = base.GetProperty("separateAxes");
                this.m_Range = base.GetProperty("range");
            }
        }

        public override void OnInspectorGUI(ParticleSystem s)
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            EditorGUI.BeginChangeCheck();
            bool flag = ModuleUI.GUIToggle(s_Texts.separateAxes, this.m_SeparateAxes);
            if (EditorGUI.EndChangeCheck())
            {
                if (flag)
                {
                    this.m_Z.RemoveCurveFromEditor();
                }
                else
                {
                    this.m_X.RemoveCurveFromEditor();
                    this.m_Y.RemoveCurveFromEditor();
                    this.m_Z.RemoveCurveFromEditor();
                }
            }
            if (flag)
            {
                this.m_Z.m_DisplayName = s_Texts.z;
                base.GUITripleMinMaxCurve(GUIContent.none, s_Texts.x, this.m_X, s_Texts.y, this.m_Y, s_Texts.z, this.m_Z, null);
            }
            else
            {
                this.m_Z.m_DisplayName = s_Texts.rotation;
                ModuleUI.GUIMinMaxCurve(s_Texts.rotation, this.m_Z);
            }
            ModuleUI.GUIMinMaxRange(s_Texts.velocityRange, this.m_Range);
        }

        public override void UpdateCullingSupportedString(ref string text)
        {
            text = text + "\n\tRotation by Speed is enabled.";
        }

        private class Texts
        {
            public GUIContent rotation = new GUIContent("Angular Velocity", "Controls the angular velocity of each particle based on its speed.");
            public GUIContent separateAxes = new GUIContent("Separate Axes", "If enabled, you can control the angular velocity limit separately for each axis.");
            public GUIContent velocityRange = new GUIContent("Speed Range", "Remaps speed in the defined range to an angular velocity.");
            public GUIContent x = new GUIContent("X");
            public GUIContent y = new GUIContent("Y");
            public GUIContent z = new GUIContent("Z");
        }
    }
}

