namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class ColorModuleUI : ModuleUI
    {
        private SerializedMinMaxGradient m_Gradient;
        private SerializedProperty m_Scale;
        private static Texts s_Texts;

        public ColorModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "ColorModule", displayName)
        {
            base.m_ToolTip = "Controls the color of each particle during its lifetime.";
        }

        protected override void Init()
        {
            if (this.m_Gradient == null)
            {
                this.m_Gradient = new SerializedMinMaxGradient(this);
                this.m_Gradient.m_AllowColor = false;
                this.m_Gradient.m_AllowRandomBetweenTwoColors = false;
            }
        }

        public override void OnInspectorGUI(ParticleSystem s)
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            base.GUIMinMaxGradient(s_Texts.color, this.m_Gradient);
        }

        private class Texts
        {
            public GUIContent color = new GUIContent("Color", "Controls the color of each particle during its lifetime.");
        }
    }
}

