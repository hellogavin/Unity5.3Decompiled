namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class CustomEditor : Attribute
    {
        internal bool m_EditorForChildClasses;
        internal Type m_InspectedType;

        public CustomEditor(Type inspectedType)
        {
            if (inspectedType == null)
            {
                Debug.LogError("Failed to load CustomEditor inspected type");
            }
            this.m_InspectedType = inspectedType;
            this.m_EditorForChildClasses = false;
        }

        public CustomEditor(Type inspectedType, bool editorForChildClasses)
        {
            if (inspectedType == null)
            {
                Debug.LogError("Failed to load CustomEditor inspected type");
            }
            this.m_InspectedType = inspectedType;
            this.m_EditorForChildClasses = editorForChildClasses;
        }

        public bool isFallback { get; set; }
    }
}

