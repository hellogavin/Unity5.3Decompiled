namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomEditor(typeof(ParticleSystem))]
    internal class ParticleSystemInspector : Editor, ParticleEffectUIOwner
    {
        private GUIContent closeWindowText = new GUIContent("Close Editor");
        private GUIContent hideWindowText = new GUIContent("Hide Editor");
        private ParticleEffectUI m_ParticleEffectUI;
        private static GUIContent m_PlayBackTitle;
        private GUIContent m_PreviewTitle = new GUIContent("Particle System Curves");
        private GUIContent showWindowText = new GUIContent("Open Editor...");

        private void Clear()
        {
            if (this.m_ParticleEffectUI != null)
            {
                this.m_ParticleEffectUI.Clear();
            }
            this.m_ParticleEffectUI = null;
        }

        public override GUIContent GetPreviewTitle()
        {
            return this.m_PreviewTitle;
        }

        public override bool HasPreviewGUI()
        {
            return (this.ShouldShowInspector() && (Selection.objects.Length == 1));
        }

        private void HierarchyOrProjectWindowWasChanged()
        {
            if (this.ShouldShowInspector())
            {
                this.Init(true);
            }
        }

        private void Init(bool forceInit)
        {
            ParticleSystem target = this.target as ParticleSystem;
            if (target != null)
            {
                if (this.m_ParticleEffectUI == null)
                {
                    this.m_ParticleEffectUI = new ParticleEffectUI(this);
                    this.m_ParticleEffectUI.InitializeIfNeeded(target);
                }
                else if (forceInit)
                {
                    this.m_ParticleEffectUI.InitializeIfNeeded(target);
                }
            }
        }

        public void OnDisable()
        {
            SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc) Delegate.Remove(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneViewGUI));
            EditorApplication.projectWindowChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.projectWindowChanged, new EditorApplication.CallbackFunction(this.HierarchyOrProjectWindowWasChanged));
            EditorApplication.hierarchyWindowChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.hierarchyWindowChanged, new EditorApplication.CallbackFunction(this.HierarchyOrProjectWindowWasChanged));
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
            if (this.m_ParticleEffectUI != null)
            {
                this.m_ParticleEffectUI.Clear();
            }
        }

        public void OnEnable()
        {
            EditorApplication.hierarchyWindowChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.hierarchyWindowChanged, new EditorApplication.CallbackFunction(this.HierarchyOrProjectWindowWasChanged));
            EditorApplication.projectWindowChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.projectWindowChanged, new EditorApplication.CallbackFunction(this.HierarchyOrProjectWindowWasChanged));
            SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc) Delegate.Combine(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneViewGUI));
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins, new GUILayoutOption[0]);
            this.ShowEdiorButtonGUI();
            if (this.ShouldShowInspector())
            {
                if (this.m_ParticleEffectUI == null)
                {
                    this.Init(true);
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical(EditorStyles.inspectorFullWidthMargins, new GUILayoutOption[0]);
                this.m_ParticleEffectUI.OnGUI();
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins, new GUILayoutOption[0]);
            }
            else
            {
                this.Clear();
            }
            EditorGUILayout.EndVertical();
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (this.m_ParticleEffectUI != null)
            {
                this.m_ParticleEffectUI.GetParticleSystemCurveEditor().OnGUI(r);
            }
        }

        public override void OnPreviewSettings()
        {
        }

        public void OnSceneGUI()
        {
            if (this.ShouldShowInspector() && (this.m_ParticleEffectUI != null))
            {
                this.m_ParticleEffectUI.OnSceneGUI();
            }
        }

        public void OnSceneViewGUI(SceneView sceneView)
        {
            if (this.ShouldShowInspector())
            {
                this.Init(false);
                if (this.m_ParticleEffectUI != null)
                {
                    this.m_ParticleEffectUI.OnSceneViewGUI();
                }
            }
        }

        private bool ShouldShowInspector()
        {
            ParticleSystemWindow instance = ParticleSystemWindow.GetInstance();
            return ((instance == null) || !instance.IsVisible());
        }

        private void ShowEdiorButtonGUI()
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            GUIContent hideWindowText = null;
            ParticleSystemWindow instance = ParticleSystemWindow.GetInstance();
            if ((instance != null) && instance.IsVisible())
            {
                if (instance.GetNumTabs() > 1)
                {
                    hideWindowText = this.hideWindowText;
                }
                else
                {
                    hideWindowText = this.closeWindowText;
                }
            }
            else
            {
                hideWindowText = this.showWindowText;
            }
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(110f) };
            if (GUILayout.Button(hideWindowText, EditorStyles.miniButton, options))
            {
                if (instance != null)
                {
                    if (instance.IsVisible())
                    {
                        if (!instance.ShowNextTabIfPossible())
                        {
                            instance.Close();
                        }
                    }
                    else
                    {
                        instance.Focus();
                    }
                }
                else
                {
                    this.Clear();
                    ParticleSystemWindow.CreateWindow();
                    GUIUtility.ExitGUI();
                }
            }
            GUILayout.EndHorizontal();
        }

        private void UndoRedoPerformed()
        {
            if (this.m_ParticleEffectUI != null)
            {
                this.m_ParticleEffectUI.UndoRedoPerformed();
            }
        }

        void ParticleEffectUIOwner.Repaint()
        {
            base.Repaint();
        }

        public override bool UseDefaultMargins()
        {
            return false;
        }

        public static GUIContent playBackTitle
        {
            get
            {
                if (m_PlayBackTitle == null)
                {
                    m_PlayBackTitle = new GUIContent("Particle Effect");
                }
                return m_PlayBackTitle;
            }
        }
    }
}

