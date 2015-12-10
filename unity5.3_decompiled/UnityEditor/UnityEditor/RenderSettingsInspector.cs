namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomEditor(typeof(RenderSettings))]
    internal class RenderSettingsInspector : Editor
    {
        private Editor m_FogEditor;
        private Editor m_LightingEditor;
        private Editor m_OtherRenderingEditor;

        public virtual void OnEnable()
        {
            this.m_LightingEditor = null;
            this.m_FogEditor = null;
            this.m_OtherRenderingEditor = null;
        }

        public override void OnInspectorGUI()
        {
            this.lightingEditor.OnInspectorGUI();
            this.fogEditor.OnInspectorGUI();
            this.otherRenderingEditor.OnInspectorGUI();
        }

        private Editor fogEditor
        {
            get
            {
                if (this.m_FogEditor == null)
                {
                }
                return (this.m_FogEditor = Editor.CreateEditor(this.target, typeof(FogEditor)));
            }
        }

        private Editor lightingEditor
        {
            get
            {
                if (this.m_LightingEditor == null)
                {
                }
                return (this.m_LightingEditor = Editor.CreateEditor(this.target, typeof(LightingEditor)));
            }
        }

        private Editor otherRenderingEditor
        {
            get
            {
                if (this.m_OtherRenderingEditor == null)
                {
                }
                return (this.m_OtherRenderingEditor = Editor.CreateEditor(this.target, typeof(OtherRenderingEditor)));
            }
        }
    }
}

