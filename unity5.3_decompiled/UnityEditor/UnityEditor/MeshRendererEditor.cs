namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomEditor(typeof(MeshRenderer)), CanEditMultipleObjects]
    internal class MeshRendererEditor : RendererEditorBase
    {
        private SerializedProperty m_CastShadows;
        private SerializedProperty m_Materials;
        private SerializedProperty m_ReceiveShadows;

        public override void OnEnable()
        {
            base.OnEnable();
            this.m_CastShadows = base.serializedObject.FindProperty("m_CastShadows");
            this.m_ReceiveShadows = base.serializedObject.FindProperty("m_ReceiveShadows");
            this.m_Materials = base.serializedObject.FindProperty("m_Materials");
            base.InitializeProbeFields();
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            bool flag = false;
            SerializedProperty property = base.serializedObject.FindProperty("m_Materials");
            if (!property.hasMultipleDifferentValues)
            {
                MeshFilter component = ((MeshRenderer) base.serializedObject.targetObject).GetComponent<MeshFilter>();
                flag = ((component != null) && (component.sharedMesh != null)) && (property.arraySize > component.sharedMesh.subMeshCount);
            }
            EditorGUILayout.PropertyField(this.m_CastShadows, true, new GUILayoutOption[0]);
            EditorGUI.BeginDisabledGroup(SceneView.IsUsingDeferredRenderingPath());
            EditorGUILayout.PropertyField(this.m_ReceiveShadows, true, new GUILayoutOption[0]);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.PropertyField(this.m_Materials, true, new GUILayoutOption[0]);
            if (!this.m_Materials.hasMultipleDifferentValues && flag)
            {
                EditorGUILayout.HelpBox("This renderer has more materials than the Mesh has submeshes. Multiple materials will be applied to the same submesh, which costs performance. Consider using multiple shader passes.", MessageType.Warning, true);
            }
            base.RenderProbeFields();
            base.serializedObject.ApplyModifiedProperties();
        }
    }
}

