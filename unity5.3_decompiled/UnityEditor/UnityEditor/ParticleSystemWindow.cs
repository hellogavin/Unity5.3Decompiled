namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class ParticleSystemWindow : EditorWindow, ParticleEffectUIOwner
    {
        private bool m_IsVisible;
        private ParticleEffectUI m_ParticleEffectUI;
        private ParticleSystem m_Target;
        private static GUIContent[] s_Icons;
        private static ParticleSystemWindow s_Instance;
        private static Texts s_Texts;

        private ParticleSystemWindow()
        {
        }

        private void Awake()
        {
        }

        private void Clear()
        {
            this.m_Target = null;
            if (this.m_ParticleEffectUI != null)
            {
                this.m_ParticleEffectUI.Clear();
                this.m_ParticleEffectUI = null;
            }
        }

        public static void CreateWindow()
        {
            s_Instance = EditorWindow.GetWindow<ParticleSystemWindow>();
            s_Instance.titleContent = EditorGUIUtility.TextContent("Particle Effect");
            s_Instance.minSize = ParticleEffectUI.GetMinSize();
        }

        private void DoToolbarGUI()
        {
            GUILayout.BeginHorizontal("Toolbar", new GUILayoutOption[0]);
            EditorGUI.BeginDisabledGroup(this.m_ParticleEffectUI == null);
            EditorGUI.BeginDisabledGroup(ParticleSystemEditorUtils.editorUpdateAll);
            if (!EditorApplication.isPlaying)
            {
                bool flag = false;
                if (this.m_ParticleEffectUI != null)
                {
                    flag = this.m_ParticleEffectUI.IsPlaying();
                }
                GUILayoutOption[] optionArray1 = new GUILayoutOption[] { GUILayout.Width(65f) };
                if (GUILayout.Button(!flag ? ParticleEffectUI.texts.play : ParticleEffectUI.texts.pause, "ToolbarButton", optionArray1))
                {
                    if (this.m_ParticleEffectUI != null)
                    {
                        if (flag)
                        {
                            this.m_ParticleEffectUI.Pause();
                        }
                        else
                        {
                            this.m_ParticleEffectUI.Play();
                        }
                    }
                    base.Repaint();
                }
                if (GUILayout.Button(ParticleEffectUI.texts.stop, "ToolbarButton", new GUILayoutOption[0]) && (this.m_ParticleEffectUI != null))
                {
                    this.m_ParticleEffectUI.Stop();
                }
            }
            else
            {
                GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(65f) };
                if (GUILayout.Button(ParticleEffectUI.texts.play, "ToolbarButton", optionArray2) && (this.m_ParticleEffectUI != null))
                {
                    this.m_ParticleEffectUI.Stop();
                    this.m_ParticleEffectUI.Play();
                }
                if (GUILayout.Button(ParticleEffectUI.texts.stop, "ToolbarButton", new GUILayoutOption[0]) && (this.m_ParticleEffectUI != null))
                {
                    this.m_ParticleEffectUI.Stop();
                }
            }
            GUILayout.FlexibleSpace();
            bool flag2 = (this.m_ParticleEffectUI != null) && this.m_ParticleEffectUI.IsShowOnlySelectedMode();
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(100f) };
            bool enable = GUILayout.Toggle(flag2, !flag2 ? "Show: All" : "Show: Selected", ParticleSystemStyles.Get().toolbarButtonLeftAlignText, options);
            if ((enable != flag2) && (this.m_ParticleEffectUI != null))
            {
                this.m_ParticleEffectUI.SetShowOnlySelectedMode(enable);
            }
            ParticleSystemEditorUtils.editorResimulation = GUILayout.Toggle(ParticleSystemEditorUtils.editorResimulation, ParticleEffectUI.texts.resimulation, "ToolbarButton", new GUILayoutOption[0]);
            ParticleEffectUI.m_ShowWireframe = GUILayout.Toggle(ParticleEffectUI.m_ShowWireframe, ParticleEffectUI.texts.wireframe, "ToolbarButton", new GUILayoutOption[0]);
            if (GUILayout.Button(!ParticleEffectUI.m_VerticalLayout ? s_Icons[1] : s_Icons[0], "ToolbarButton", new GUILayoutOption[0]))
            {
                ParticleEffectUI.m_VerticalLayout = !ParticleEffectUI.m_VerticalLayout;
                EditorPrefs.SetBool("ShurikenVerticalLayout", ParticleEffectUI.m_VerticalLayout);
            }
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.Space(3f);
            bool flag4 = ParticleSystemEditorUtils.lockedParticleSystem != null;
            bool flag5 = GUILayout.Toggle(flag4, s_Texts.lockParticleSystem, "IN LockButton", new GUILayoutOption[0]);
            if (((flag4 != flag5) && (this.m_ParticleEffectUI != null)) && (this.m_Target != null))
            {
                if (flag5)
                {
                    ParticleSystemEditorUtils.lockedParticleSystem = this.m_Target;
                }
                else
                {
                    ParticleSystemEditorUtils.lockedParticleSystem = null;
                }
            }
            GUILayout.EndVertical();
            EditorGUI.EndDisabledGroup();
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();
        }

        internal static ParticleSystemWindow GetInstance()
        {
            return s_Instance;
        }

        private void InitEffectUI()
        {
            if (this.m_IsVisible)
            {
                ParticleSystem lockedParticleSystem = ParticleSystemEditorUtils.lockedParticleSystem;
                if ((lockedParticleSystem == null) && (Selection.activeGameObject != null))
                {
                    lockedParticleSystem = Selection.activeGameObject.GetComponent<ParticleSystem>();
                }
                this.m_Target = lockedParticleSystem;
                if (this.m_Target != null)
                {
                    if (this.m_ParticleEffectUI == null)
                    {
                        this.m_ParticleEffectUI = new ParticleEffectUI(this);
                    }
                    if (this.m_ParticleEffectUI.InitializeIfNeeded(this.m_Target))
                    {
                        base.Repaint();
                    }
                }
                if ((this.m_Target == null) && (this.m_ParticleEffectUI != null))
                {
                    this.Clear();
                    base.Repaint();
                    SceneView.RepaintAll();
                    GameView.RepaintAll();
                }
            }
        }

        internal bool IsVisible()
        {
            return this.m_IsVisible;
        }

        private void OnBecameInvisible()
        {
            this.m_IsVisible = false;
            ParticleSystemEditorUtils.editorUpdateAll = false;
            this.Clear();
            SceneView.RepaintAll();
            InspectorWindow.RepaintAllInspectors();
        }

        private void OnBecameVisible()
        {
            if (!this.m_IsVisible)
            {
                this.m_IsVisible = true;
                this.InitEffectUI();
                SceneView.RepaintAll();
                InspectorWindow.RepaintAllInspectors();
            }
        }

        private void OnDidOpenScene()
        {
            base.Repaint();
        }

        private void OnDisable()
        {
            ParticleSystemEditorUtils.editorUpdateAll = false;
            SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc) Delegate.Remove(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneViewGUI));
            EditorApplication.projectWindowChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.projectWindowChanged, new EditorApplication.CallbackFunction(this.OnHierarchyOrProjectWindowWasChanged));
            EditorApplication.hierarchyWindowChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.hierarchyWindowChanged, new EditorApplication.CallbackFunction(this.OnHierarchyOrProjectWindowWasChanged));
            EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.playmodeStateChanged, new EditorApplication.CallbackFunction(this.OnPlayModeStateChanged));
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
            this.Clear();
            if (s_Instance == this)
            {
                s_Instance = null;
            }
        }

        private void OnEnable()
        {
            s_Instance = this;
            this.m_Target = null;
            ParticleEffectUI.m_VerticalLayout = EditorPrefs.GetBool("ShurikenVerticalLayout", false);
            EditorApplication.hierarchyWindowChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.hierarchyWindowChanged, new EditorApplication.CallbackFunction(this.OnHierarchyOrProjectWindowWasChanged));
            EditorApplication.projectWindowChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.projectWindowChanged, new EditorApplication.CallbackFunction(this.OnHierarchyOrProjectWindowWasChanged));
            SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc) Delegate.Combine(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneViewGUI));
            EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.playmodeStateChanged, new EditorApplication.CallbackFunction(this.OnPlayModeStateChanged));
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
            base.autoRepaintOnSceneChange = false;
        }

        private void OnGUI()
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            if (s_Icons == null)
            {
                s_Icons = new GUIContent[] { EditorGUIUtility.IconContent("HorizontalSplit"), EditorGUIUtility.IconContent("VerticalSplit") };
            }
            if ((this.m_Target == null) && ((Selection.activeGameObject != null) || (ParticleSystemEditorUtils.lockedParticleSystem != null)))
            {
                this.InitEffectUI();
            }
            this.DoToolbarGUI();
            EditorGUI.BeginDisabledGroup(ParticleSystemEditorUtils.editorUpdateAll);
            if ((this.m_Target != null) && (this.m_ParticleEffectUI != null))
            {
                this.m_ParticleEffectUI.OnGUI();
            }
            EditorGUI.EndDisabledGroup();
        }

        private void OnHierarchyOrProjectWindowWasChanged()
        {
            this.InitEffectUI();
        }

        private void OnPlayModeStateChanged()
        {
            base.Repaint();
        }

        public void OnSceneViewGUI(SceneView sceneView)
        {
            if ((this.m_IsVisible && !ParticleSystemEditorUtils.editorUpdateAll) && (this.m_ParticleEffectUI != null))
            {
                this.m_ParticleEffectUI.OnSceneGUI();
                this.m_ParticleEffectUI.OnSceneViewGUI();
            }
        }

        private void OnSelectionChange()
        {
            this.InitEffectUI();
            base.Repaint();
        }

        private void UndoRedoPerformed()
        {
            if (this.m_ParticleEffectUI != null)
            {
                this.m_ParticleEffectUI.UndoRedoPerformed();
            }
            base.Repaint();
        }

        void ParticleEffectUIOwner.Repaint()
        {
            base.Repaint();
        }

        private class Texts
        {
            public GUIContent lockParticleSystem = new GUIContent(string.Empty, "Lock the current selected Particle System");
            public GUIContent previewAll = new GUIContent("Simulate All", "Simulate all particle systems that have Play On Awake set");
        }
    }
}

