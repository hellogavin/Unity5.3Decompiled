namespace UnityEditor
{
    using System;
    using UnityEditorInternal;
    using UnityEngine;

    internal class GradientContextMenu
    {
        private SerializedProperty m_Prop1;

        private GradientContextMenu(SerializedProperty prop1)
        {
            this.m_Prop1 = prop1;
        }

        private void Copy()
        {
            Gradient gradient = (this.m_Prop1 == null) ? null : this.m_Prop1.gradientValue;
            ParticleSystemClipboard.CopyGradient(gradient, null);
        }

        private void Paste()
        {
            ParticleSystemClipboard.PasteGradient(this.m_Prop1, null);
            GradientPreviewCache.ClearCache();
        }

        internal static void Show(SerializedProperty prop)
        {
            GUIContent content = new GUIContent("Copy");
            GUIContent content2 = new GUIContent("Paste");
            GenericMenu menu = new GenericMenu();
            menu.AddItem(content, false, new GenericMenu.MenuFunction(new GradientContextMenu(prop).Copy));
            if (ParticleSystemClipboard.HasSingleGradient())
            {
                menu.AddItem(content2, false, new GenericMenu.MenuFunction(new GradientContextMenu(prop).Paste));
            }
            else
            {
                menu.AddDisabledItem(content2);
            }
            menu.ShowAsContext();
        }
    }
}

