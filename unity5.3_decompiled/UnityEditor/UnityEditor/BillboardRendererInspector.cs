namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [CustomEditor(typeof(BillboardRenderer)), CanEditMultipleObjects]
    internal class BillboardRendererInspector : RendererEditorBase
    {
        private string[] m_ExcludedProperties;

        public override void OnEnable()
        {
            base.OnEnable();
            base.InitializeProbeFields();
            List<string> list = new List<string>();
            string[] collection = new string[] { "m_Materials", "m_LightmapParameters" };
            list.AddRange(collection);
            list.AddRange(RendererEditorBase.Probes.GetFieldsStringArray());
            this.m_ExcludedProperties = list.ToArray();
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            Editor.DrawPropertiesExcluding(base.serializedObject, this.m_ExcludedProperties);
            base.RenderProbeFields();
            base.serializedObject.ApplyModifiedProperties();
        }
    }
}

