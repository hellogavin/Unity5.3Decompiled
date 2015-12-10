namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class AnimationCurveContextMenu
    {
        private Rect m_CurveRanges;
        private ParticleSystemCurveEditor m_ParticleSystemCurveEditor;
        private SerializedProperty m_Prop1;
        private SerializedProperty m_Prop2;
        private SerializedProperty m_Scalar;

        private AnimationCurveContextMenu(SerializedProperty prop1, SerializedProperty prop2, SerializedProperty scalar, Rect curveRanges, ParticleSystemCurveEditor owner)
        {
            this.m_Prop1 = prop1;
            this.m_Prop2 = prop2;
            this.m_Scalar = scalar;
            this.m_ParticleSystemCurveEditor = owner;
            this.m_CurveRanges = curveRanges;
        }

        private void Copy()
        {
            AnimationCurve animCurve = (this.m_Prop1 == null) ? null : this.m_Prop1.animationCurveValue;
            AnimationCurve curve2 = (this.m_Prop2 == null) ? null : this.m_Prop2.animationCurveValue;
            float scalar = (this.m_Scalar == null) ? 1f : this.m_Scalar.floatValue;
            ParticleSystemClipboard.CopyAnimationCurves(animCurve, curve2, scalar);
        }

        private void Paste()
        {
            ParticleSystemClipboard.PasteAnimationCurves(this.m_Prop1, this.m_Prop2, this.m_Scalar, this.m_CurveRanges, this.m_ParticleSystemCurveEditor);
        }

        internal static void Show(Rect position, SerializedProperty property, SerializedProperty property2, SerializedProperty scalar, Rect curveRanges, ParticleSystemCurveEditor curveEditor)
        {
            GUIContent content = new GUIContent("Copy");
            GUIContent content2 = new GUIContent("Paste");
            GenericMenu menu = new GenericMenu();
            bool flag = (property != null) && (property2 != null);
            bool flag2 = (flag && ParticleSystemClipboard.HasDoubleAnimationCurve()) || (!flag && ParticleSystemClipboard.HasSingleAnimationCurve());
            AnimationCurveContextMenu menu2 = new AnimationCurveContextMenu(property, property2, scalar, curveRanges, curveEditor);
            menu.AddItem(content, false, new GenericMenu.MenuFunction(menu2.Copy));
            if (flag2)
            {
                menu.AddItem(content2, false, new GenericMenu.MenuFunction(menu2.Paste));
            }
            else
            {
                menu.AddDisabledItem(content2);
            }
            menu.DropDown(position);
        }
    }
}

