namespace UnityEditor
{
    using System;
    using UnityEditorInternal;
    using UnityEngine;

    internal class SerializedMinMaxGradient
    {
        public bool m_AllowColor;
        public bool m_AllowGradient;
        public bool m_AllowRandomBetweenTwoColors;
        public bool m_AllowRandomBetweenTwoGradients;
        public SerializedProperty m_MaxColor;
        public SerializedProperty m_MaxGradient;
        public SerializedProperty m_MinColor;
        public SerializedProperty m_MinGradient;
        private SerializedProperty m_MinMaxState;

        public SerializedMinMaxGradient(SerializedModule m)
        {
            this.Init(m, "gradient");
        }

        public SerializedMinMaxGradient(SerializedModule m, string name)
        {
            this.Init(m, name);
        }

        public static Color GetGradientAsColor(SerializedProperty gradientProp)
        {
            return gradientProp.gradientValue.constantColor;
        }

        private void Init(SerializedModule m, string name)
        {
            this.m_MaxGradient = m.GetProperty(name, "maxGradient");
            this.m_MinGradient = m.GetProperty(name, "minGradient");
            this.m_MaxColor = m.GetProperty(name, "maxColor");
            this.m_MinColor = m.GetProperty(name, "minColor");
            this.m_MinMaxState = m.GetProperty(name, "minMaxState");
            this.m_AllowColor = true;
            this.m_AllowGradient = true;
            this.m_AllowRandomBetweenTwoColors = true;
            this.m_AllowRandomBetweenTwoGradients = true;
        }

        public static void SetGradientAsColor(SerializedProperty gradientProp, Color color)
        {
            gradientProp.gradientValue.constantColor = color;
            GradientPreviewCache.ClearCache();
        }

        private void SetMinMaxState(MinMaxGradientState newState)
        {
            if (newState != this.state)
            {
                this.m_MinMaxState.intValue = (int) newState;
            }
        }

        public MinMaxGradientState state
        {
            get
            {
                return (MinMaxGradientState) this.m_MinMaxState.intValue;
            }
            set
            {
                this.SetMinMaxState(value);
            }
        }
    }
}

