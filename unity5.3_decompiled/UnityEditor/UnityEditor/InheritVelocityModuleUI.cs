namespace UnityEditor
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class InheritVelocityModuleUI : ModuleUI
    {
        [CompilerGenerated]
        private static Func<Rigidbody, bool> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<Rigidbody2D, bool> <>f__am$cache4;
        private SerializedMinMaxCurve m_Curve;
        private SerializedProperty m_Mode;
        private static Texts s_Texts;

        public InheritVelocityModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "InheritVelocityModule", displayName)
        {
            base.m_ToolTip = "Controls the velocity inherited from the emitter, for each particle.";
        }

        private bool CanInheritVelocity(ParticleSystem s)
        {
            Rigidbody[] componentsInParent = s.GetComponentsInParent<Rigidbody>();
            Rigidbody2D[] source = s.GetComponentsInParent<Rigidbody2D>();
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = o => !o.isKinematic && (o.interpolation == RigidbodyInterpolation.None);
            }
            if (!componentsInParent.Any<Rigidbody>(<>f__am$cache3))
            {
                if (<>f__am$cache4 == null)
                {
                    <>f__am$cache4 = o => !o.isKinematic && (o.interpolation == RigidbodyInterpolation2D.None);
                }
                if (!source.Any<Rigidbody2D>(<>f__am$cache4))
                {
                    return true;
                }
            }
            return false;
        }

        protected override void Init()
        {
            if (this.m_Curve == null)
            {
                if (s_Texts == null)
                {
                    s_Texts = new Texts();
                }
                this.m_Mode = base.GetProperty("m_Mode");
                this.m_Curve = new SerializedMinMaxCurve(this, GUIContent.none, "m_Curve");
            }
        }

        public override void OnInspectorGUI(ParticleSystem s)
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            ModuleUI.GUIPopup(s_Texts.mode, this.m_Mode, s_Texts.modes);
            ModuleUI.GUIMinMaxCurve(GUIContent.none, this.m_Curve);
            if ((this.m_Curve.scalar.floatValue != 0f) && !this.CanInheritVelocity(s))
            {
                EditorGUILayout.HelpBox(EditorGUIUtility.TextContent("Inherit velocity requires interpolation enabled on the rigidbody to function correctly.").text, MessageType.Warning, true);
            }
        }

        public override void UpdateCullingSupportedString(ref string text)
        {
            this.Init();
            if (!this.m_Curve.SupportsProcedural())
            {
                text = text + "\n\tInherited velocity curves use too many keys.";
            }
        }

        private enum Modes
        {
            Initial,
            Current
        }

        private class Texts
        {
            public GUIContent mode = new GUIContent("Mode", "Specifies whether the emitter velocity is inherited as a one-shot when a particle is born, always using the current emitter velocity, or using the emitter velocity when the particle was born.");
            public string[] modes = new string[] { "Initial", "Current" };
        }
    }
}

