namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(MeshCollider))]
    internal class MeshColliderEditor : Collider3DEditorBase
    {
        private SerializedProperty m_Convex;
        private SerializedProperty m_Mesh;

        public override void OnEnable()
        {
            base.OnEnable();
            this.m_Mesh = base.serializedObject.FindProperty("m_Mesh");
            this.m_Convex = base.serializedObject.FindProperty("m_Convex");
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(this.m_Convex, Texts.convextText, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck() && !this.m_Convex.boolValue)
            {
                base.m_IsTrigger.boolValue = false;
            }
            EditorGUI.indentLevel++;
            EditorGUI.BeginDisabledGroup(!this.m_Convex.boolValue);
            EditorGUILayout.PropertyField(base.m_IsTrigger, Texts.isTriggerText, new GUILayoutOption[0]);
            EditorGUI.EndDisabledGroup();
            EditorGUI.indentLevel--;
            EditorGUILayout.PropertyField(base.m_Material, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Mesh, new GUILayoutOption[0]);
            base.serializedObject.ApplyModifiedProperties();
        }

        private static class Texts
        {
            public static GUIContent convextText = new GUIContent("Convex", "Is this collider convex?");
            public static GUIContent isTriggerText = new GUIContent("Is Trigger", "Is this collider a trigger? Triggers are only supported on convex colliders.");
        }
    }
}

