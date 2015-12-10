namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class SizeModuleUI : ModuleUI
    {
        private SerializedMinMaxCurve m_Curve;
        private GUIContent m_SizeText;

        public SizeModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "SizeModule", displayName)
        {
            this.m_SizeText = new GUIContent("Size", "Controls the size of each particle during its lifetime.");
            base.m_ToolTip = "Controls the size of each particle during its lifetime.";
        }

        protected override void Init()
        {
            if (this.m_Curve == null)
            {
                this.m_Curve = new SerializedMinMaxCurve(this, this.m_SizeText);
                this.m_Curve.m_AllowConstant = false;
            }
        }

        public override void OnInspectorGUI(ParticleSystem s)
        {
            ModuleUI.GUIMinMaxCurve(this.m_SizeText, this.m_Curve);
        }
    }
}

