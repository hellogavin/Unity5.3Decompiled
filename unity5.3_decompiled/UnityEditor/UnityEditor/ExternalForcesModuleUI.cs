namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class ExternalForcesModuleUI : ModuleUI
    {
        private SerializedProperty m_Multiplier;
        private static Texts s_Texts;

        public ExternalForcesModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "ExternalForcesModule", displayName)
        {
            base.m_ToolTip = "Controls the wind zones that each particle is affected by.";
        }

        protected override void Init()
        {
            if (this.m_Multiplier == null)
            {
                if (s_Texts == null)
                {
                    s_Texts = new Texts();
                }
                this.m_Multiplier = base.GetProperty("multiplier");
            }
        }

        public override void OnInspectorGUI(ParticleSystem s)
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            ModuleUI.GUIFloat(s_Texts.multiplier, this.m_Multiplier);
        }

        public override void UpdateCullingSupportedString(ref string text)
        {
            text = text + "\n\tExternal Forces is enabled.";
        }

        private class Texts
        {
            public GUIContent multiplier = new GUIContent("Multiplier", "Used to scale the force applied to this particle system.");
        }
    }
}

