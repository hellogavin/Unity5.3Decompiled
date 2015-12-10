namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomEditor(typeof(OcclusionPortal))]
    internal class OcclusionPortalInspector : Editor
    {
        private readonly BoxEditor m_BoxEditor = new BoxEditor(true, s_BoxHash);
        private SerializedProperty m_Center;
        private SerializedObject m_Object;
        private SerializedProperty m_Size;
        private static readonly int s_BoxHash = "BoxColliderEditor".GetHashCode();

        public void OnEnable()
        {
            this.m_Object = new SerializedObject(base.targets);
            this.m_Center = this.m_Object.FindProperty("m_Center");
            this.m_Size = this.m_Object.FindProperty("m_Size");
            this.m_BoxEditor.OnEnable();
            this.m_BoxEditor.SetAlwaysDisplayHandles(true);
        }

        private void OnSceneGUI()
        {
            OcclusionPortal target = this.target as OcclusionPortal;
            Vector3 center = this.m_Center.vector3Value;
            Vector3 size = this.m_Size.vector3Value;
            Color color = Handles.s_ColliderHandleColor;
            if (this.m_BoxEditor.OnSceneGUI(target.transform, color, ref center, ref size))
            {
                this.m_Center.vector3Value = center;
                this.m_Size.vector3Value = size;
                this.m_Object.ApplyModifiedProperties();
            }
        }
    }
}

