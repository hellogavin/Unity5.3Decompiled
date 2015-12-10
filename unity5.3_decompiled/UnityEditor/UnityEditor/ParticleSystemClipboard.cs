namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class ParticleSystemClipboard
    {
        private static AnimationCurve m_AnimationCurve1;
        private static AnimationCurve m_AnimationCurve2;
        private static float m_AnimationCurveScalar;
        private static Gradient m_Gradient1;
        private static Gradient m_Gradient2;

        private static void ClampCurve(SerializedProperty animCurveProperty, Rect curveRanges)
        {
            AnimationCurve animationCurveValue = animCurveProperty.animationCurveValue;
            Keyframe[] keys = animationCurveValue.keys;
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i].time = Mathf.Clamp(keys[i].time, curveRanges.xMin, curveRanges.xMax);
                keys[i].value = Mathf.Clamp(keys[i].value, curveRanges.yMin, curveRanges.yMax);
            }
            animationCurveValue.keys = keys;
            animCurveProperty.animationCurveValue = animationCurveValue;
        }

        public static void CopyAnimationCurves(AnimationCurve animCurve, AnimationCurve animCurve2, float scalar)
        {
            m_AnimationCurve1 = animCurve;
            m_AnimationCurve2 = animCurve2;
            m_AnimationCurveScalar = scalar;
        }

        public static void CopyGradient(Gradient gradient1, Gradient gradient2)
        {
            m_Gradient1 = gradient1;
            m_Gradient2 = gradient2;
        }

        public static bool HasDoubleAnimationCurve()
        {
            return ((m_AnimationCurve1 != null) && (m_AnimationCurve2 != null));
        }

        public static bool HasDoubleGradient()
        {
            return ((m_Gradient1 != null) && (m_Gradient2 != null));
        }

        public static bool HasSingleAnimationCurve()
        {
            return ((m_AnimationCurve1 != null) && (m_AnimationCurve2 == null));
        }

        public static bool HasSingleGradient()
        {
            return ((m_Gradient1 != null) && (m_Gradient2 == null));
        }

        public static void PasteAnimationCurves(SerializedProperty animCurveProperty, SerializedProperty animCurveProperty2, SerializedProperty scalarProperty, Rect curveRanges, ParticleSystemCurveEditor particleSystemCurveEditor)
        {
            if ((animCurveProperty != null) && (m_AnimationCurve1 != null))
            {
                animCurveProperty.animationCurveValue = m_AnimationCurve1;
                ClampCurve(animCurveProperty, curveRanges);
            }
            if ((animCurveProperty2 != null) && (m_AnimationCurve2 != null))
            {
                animCurveProperty2.animationCurveValue = m_AnimationCurve2;
                ClampCurve(animCurveProperty2, curveRanges);
            }
            if (scalarProperty != null)
            {
                scalarProperty.floatValue = m_AnimationCurveScalar;
            }
            if (particleSystemCurveEditor != null)
            {
                particleSystemCurveEditor.Refresh();
            }
        }

        public static void PasteGradient(SerializedProperty gradientProperty, SerializedProperty gradientProperty2)
        {
            if ((gradientProperty != null) && (m_Gradient1 != null))
            {
                gradientProperty.gradientValue = m_Gradient1;
            }
            if ((gradientProperty2 != null) && (m_Gradient2 != null))
            {
                gradientProperty2.gradientValue = m_Gradient2;
            }
        }
    }
}

