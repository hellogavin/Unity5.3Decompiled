namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(LineRenderer))]
    internal class LineRendererInspector : RendererEditorBase
    {
        public override void OnEnable()
        {
            base.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            Editor.DrawPropertiesExcluding(base.m_SerializedObject, new string[0]);
            base.serializedObject.ApplyModifiedProperties();
        }
    }
}

