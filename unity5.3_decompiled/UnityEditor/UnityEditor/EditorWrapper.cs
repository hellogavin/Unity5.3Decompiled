namespace UnityEditor
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class EditorWrapper : IDisposable
    {
        private Editor editor;
        public VoidDelegate OnSceneDrag;

        private EditorWrapper()
        {
        }

        private void DefaultOnSceneDrag(SceneView sceneView)
        {
        }

        public void Dispose()
        {
            if (this.editor != null)
            {
                this.OnSceneDrag = null;
                Object.DestroyImmediate(this.editor);
                this.editor = null;
            }
            GC.SuppressFinalize(this);
        }

        ~EditorWrapper()
        {
            Debug.LogError("Failed to dispose EditorWrapper.");
        }

        public string GetInfoString()
        {
            return this.editor.GetInfoString();
        }

        public bool HasPreviewGUI()
        {
            return this.editor.HasPreviewGUI();
        }

        private bool Init(Object obj, EditorFeatures requirements)
        {
            this.editor = Editor.CreateEditor(obj);
            if (this.editor == null)
            {
                return false;
            }
            if (((requirements & EditorFeatures.PreviewGUI) > EditorFeatures.None) && !this.editor.HasPreviewGUI())
            {
                return false;
            }
            MethodInfo method = this.editor.GetType().GetMethod("OnSceneDrag", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (method != null)
            {
                this.OnSceneDrag = (VoidDelegate) Delegate.CreateDelegate(typeof(VoidDelegate), this.editor, method);
            }
            else
            {
                if ((requirements & EditorFeatures.OnSceneDrag) > EditorFeatures.None)
                {
                    return false;
                }
                this.OnSceneDrag = new VoidDelegate(this.DefaultOnSceneDrag);
            }
            return true;
        }

        public static EditorWrapper Make(Object obj, EditorFeatures requirements)
        {
            EditorWrapper wrapper = new EditorWrapper();
            if (wrapper.Init(obj, requirements))
            {
                return wrapper;
            }
            wrapper.Dispose();
            return null;
        }

        internal void OnAssetStoreInspectorGUI()
        {
            if (this.editor != null)
            {
                this.editor.OnAssetStoreInspectorGUI();
            }
        }

        public void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            if (this.editor != null)
            {
                this.editor.OnInteractivePreviewGUI(r, background);
            }
        }

        public void OnPreviewGUI(Rect position, GUIStyle background)
        {
            this.editor.OnPreviewGUI(position, background);
        }

        public void OnPreviewSettings()
        {
            this.editor.OnPreviewSettings();
        }

        public string name
        {
            get
            {
                return this.editor.target.name;
            }
        }

        public delegate void VoidDelegate(SceneView sceneView);
    }
}

