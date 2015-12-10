namespace UnityEditor
{
    using System;
    using UnityEngine;

    public abstract class MaterialPropertyDrawer
    {
        protected MaterialPropertyDrawer()
        {
        }

        public virtual void Apply(MaterialProperty prop)
        {
        }

        public virtual float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return 16f;
        }

        public virtual void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
        {
            EditorGUI.LabelField(position, new GUIContent(label), EditorGUIUtility.TempContent("No GUI Implemented"));
        }
    }
}

