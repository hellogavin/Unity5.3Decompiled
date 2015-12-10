namespace UnityEditor
{
    using System;
    using System.Reflection;
    using UnityEngine;

    public abstract class PropertyDrawer : GUIDrawer
    {
        internal PropertyAttribute m_Attribute;
        internal FieldInfo m_FieldInfo;

        protected PropertyDrawer()
        {
        }

        public virtual float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 16f;
        }

        internal float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            ScriptAttributeUtility.s_DrawerStack.Push(this);
            float propertyHeight = this.GetPropertyHeight(property, label);
            ScriptAttributeUtility.s_DrawerStack.Pop();
            return propertyHeight;
        }

        public virtual void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.DefaultPropertyField(position, property, label);
            EditorGUI.LabelField(position, label, EditorGUIUtility.TempContent("No GUI Implemented"));
        }

        internal void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            ScriptAttributeUtility.s_DrawerStack.Push(this);
            this.OnGUI(position, property, label);
            ScriptAttributeUtility.s_DrawerStack.Pop();
        }

        public PropertyAttribute attribute
        {
            get
            {
                return this.m_Attribute;
            }
        }

        public FieldInfo fieldInfo
        {
            get
            {
                return this.m_FieldInfo;
            }
        }
    }
}

