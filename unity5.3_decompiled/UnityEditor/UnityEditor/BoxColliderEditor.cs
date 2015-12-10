namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(BoxCollider))]
    internal class BoxColliderEditor : Collider3DEditorBase
    {
        private readonly BoxEditor m_BoxEditor = new BoxEditor(true, s_BoxHash);
        private SerializedProperty m_Center;
        private SerializedProperty m_Size;
        private static readonly int s_BoxHash = "BoxColliderEditor".GetHashCode();

        public override void OnDisable()
        {
            base.OnDisable();
            this.m_BoxEditor.OnDisable();
        }

        public override void OnEnable()
        {
            base.OnEnable();
            this.m_Center = base.serializedObject.FindProperty("m_Center");
            this.m_Size = base.serializedObject.FindProperty("m_Size");
            this.m_BoxEditor.OnEnable();
            this.m_BoxEditor.SetAlwaysDisplayHandles(true);
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            base.InspectorEditButtonGUI();
            EditorGUILayout.PropertyField(base.m_IsTrigger, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(base.m_Material, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Center, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Size, new GUILayoutOption[0]);
            base.serializedObject.ApplyModifiedProperties();
        }

        public void OnSceneGUI()
        {
            if (base.editingCollider)
            {
                BoxCollider target = (BoxCollider) this.target;
                Vector3 center = target.center;
                Vector3 size = target.size;
                Color color = Handles.s_ColliderHandleColor;
                if (!target.enabled)
                {
                    color = Handles.s_ColliderHandleColorDisabled;
                }
                if (this.m_BoxEditor.OnSceneGUI(target.transform, color, ref center, ref size))
                {
                    Undo.RecordObject(target, "Modify Box Collider");
                    target.center = center;
                    target.size = size;
                }
            }
        }
    }
}

