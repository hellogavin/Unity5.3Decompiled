namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEditor.AnimatedValues;
    using UnityEditor.Modules;
    using UnityEditor.SceneManagement;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Events;

    [EditorWindowTitle(title="Game", useTypeNameAsIconName=true)]
    internal class GameView : EditorWindow, IHasCustomMenu
    {
        private GUIContent gizmosContent = new GUIContent("Gizmos");
        private const int kBorderSize = 5;
        private const int kToolbarHeight = 0x11;
        [SerializeField]
        private bool m_Gizmos;
        [SerializeField]
        private bool m_MaximizeOnPlay;
        private AnimBool m_ResolutionTooLargeWarning = new AnimBool(false);
        [SerializeField]
        private int[] m_SelectedSizes = new int[0];
        private Vector2 m_ShownResolution = Vector2.zero;
        private int m_SizeChangeID = -2147483648;
        [SerializeField]
        private bool m_Stats;
        [SerializeField]
        private int m_TargetDisplay;
        private GUIContent renderdocContent;
        private static List<GameView> s_GameViews = new List<GameView>();
        private static GUIStyle s_GizmoButtonStyle;
        private static GameView s_LastFocusedGameView = null;
        private static Rect s_MainGameViewRect = new Rect(0f, 0f, 640f, 480f);
        private static GUIStyle s_ResolutionWarningStyle;

        public GameView()
        {
            base.depthBufferBits = 0x20;
            base.antiAlias = -1;
            base.autoRepaintOnSceneChange = true;
            this.m_TargetDisplay = 0;
        }

        public virtual void AddItemsToMenu(GenericMenu menu)
        {
            if (RenderDoc.IsInstalled() && !RenderDoc.IsLoaded())
            {
                menu.AddItem(new GUIContent("Load RenderDoc"), false, new GenericMenu.MenuFunction(this.LoadRenderDoc));
            }
        }

        private void AllowCursorLockAndHide(bool enable)
        {
            Unsupported.SetAllowCursorLock(enable);
            Unsupported.SetAllowCursorHide(enable);
        }

        private void DelayedGameViewChanged()
        {
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.DoDelayedGameViewChanged));
        }

        private void DoDelayedGameViewChanged()
        {
            this.GameViewAspectWasChanged();
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.DoDelayedGameViewChanged));
        }

        private void DoToolbarGUI()
        {
            ScriptableSingleton<GameViewSizes>.instance.RefreshStandaloneAndWebplayerDefaultSizes();
            if (ScriptableSingleton<GameViewSizes>.instance.GetChangeID() != this.m_SizeChangeID)
            {
                this.EnsureSelectedSizeAreValid();
                this.m_SizeChangeID = ScriptableSingleton<GameViewSizes>.instance.GetChangeID();
            }
            GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
            if (this.ShouldShowMultiDisplayOption())
            {
                GUILayoutOption[] optionArray1 = new GUILayoutOption[] { GUILayout.Width(80f) };
                int num = EditorGUILayout.Popup(this.m_TargetDisplay, DisplayUtility.GetDisplayNames(), EditorStyles.toolbarPopup, optionArray1);
                EditorGUILayout.Space();
                if (num != this.m_TargetDisplay)
                {
                    this.m_TargetDisplay = num;
                    this.GameViewAspectWasChanged();
                }
            }
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(160f) };
            EditorGUILayout.GameViewSizePopup(currentSizeGroupType, this.selectedSizeIndex, new Action<int, object>(this.SelectionCallback), EditorStyles.toolbarDropDown, options);
            if (FrameDebuggerUtility.IsLocalEnabled())
            {
                GUILayout.FlexibleSpace();
                Color color = GUI.color;
                GUI.color *= AnimationMode.animatedPropertyColor;
                GUILayout.Label("Frame Debugger on", EditorStyles.miniLabel, new GUILayoutOption[0]);
                GUI.color = color;
                if (Event.current.type == EventType.Repaint)
                {
                    FrameDebuggerWindow.RepaintAll();
                }
            }
            GUILayout.FlexibleSpace();
            if (RenderDoc.IsLoaded())
            {
                EditorGUI.BeginDisabledGroup(!RenderDoc.IsSupported());
                if (GUILayout.Button(this.renderdocContent, EditorStyles.toolbarButton, new GUILayoutOption[0]))
                {
                    base.m_Parent.CaptureRenderDoc();
                    GUIUtility.ExitGUI();
                }
                EditorGUI.EndDisabledGroup();
            }
            this.m_MaximizeOnPlay = GUILayout.Toggle(this.m_MaximizeOnPlay, "Maximize on Play", EditorStyles.toolbarButton, new GUILayoutOption[0]);
            EditorUtility.audioMasterMute = GUILayout.Toggle(EditorUtility.audioMasterMute, "Mute audio", EditorStyles.toolbarButton, new GUILayoutOption[0]);
            this.m_Stats = GUILayout.Toggle(this.m_Stats, "Stats", EditorStyles.toolbarButton, new GUILayoutOption[0]);
            Rect position = GUILayoutUtility.GetRect(this.gizmosContent, s_GizmoButtonStyle);
            Rect rect2 = new Rect(position.xMax - s_GizmoButtonStyle.border.right, position.y, (float) s_GizmoButtonStyle.border.right, position.height);
            if (EditorGUI.ButtonMouseDown(rect2, GUIContent.none, FocusType.Passive, GUIStyle.none) && AnnotationWindow.ShowAtPosition(GUILayoutUtility.topLevel.GetLast(), true))
            {
                GUIUtility.ExitGUI();
            }
            this.m_Gizmos = GUI.Toggle(position, this.m_Gizmos, this.gizmosContent, s_GizmoButtonStyle);
            GUILayout.EndHorizontal();
        }

        private void EnsureSelectedSizeAreValid()
        {
            int length = Enum.GetNames(typeof(GameViewSizeGroupType)).Length;
            if (this.m_SelectedSizes.Length != length)
            {
                Array.Resize<int>(ref this.m_SelectedSizes, length);
            }
            IEnumerator enumerator = Enum.GetValues(typeof(GameViewSizeGroupType)).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    GameViewSizeGroupType current = (GameViewSizeGroupType) ((int) enumerator.Current);
                    GameViewSizeGroup group = ScriptableSingleton<GameViewSizes>.instance.GetGroup(current);
                    int index = (int) current;
                    this.m_SelectedSizes[index] = Mathf.Clamp(this.m_SelectedSizes[index], 0, group.GetTotalCount() - 1);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
        }

        private void GameViewAspectWasChanged()
        {
            base.SetInternalGameViewRect(GetConstrainedGameViewRenderRect(this.gameViewRenderRect, this.selectedSizeIndex));
            EditorApplication.SetSceneRepaintDirty();
        }

        internal Rect GetConstrainedGameViewRenderRect()
        {
            if (base.m_Parent == null)
            {
                return s_MainGameViewRect;
            }
            base.m_Pos = base.m_Parent.borderSize.Remove(base.m_Parent.position);
            return EditorGUIUtility.PixelsToPoints(GetConstrainedGameViewRenderRect(EditorGUIUtility.PointsToPixels(this.gameViewRenderRect), this.selectedSizeIndex));
        }

        internal static Rect GetConstrainedGameViewRenderRect(Rect renderRect, int sizeIndex)
        {
            bool flag;
            return GetConstrainedGameViewRenderRect(renderRect, sizeIndex, out flag);
        }

        internal static Rect GetConstrainedGameViewRenderRect(Rect renderRect, int sizeIndex, out bool fitsInsideRect)
        {
            return GameViewSizes.GetConstrainedRect(renderRect, currentSizeGroupType, sizeIndex, out fitsInsideRect);
        }

        internal static GameView GetMainGameView()
        {
            if (((s_LastFocusedGameView == null) && (s_GameViews != null)) && (s_GameViews.Count > 0))
            {
                s_LastFocusedGameView = s_GameViews[0];
            }
            return s_LastFocusedGameView;
        }

        internal static Rect GetMainGameViewRenderRect()
        {
            GameView mainGameView = GetMainGameView();
            if (mainGameView != null)
            {
                s_MainGameViewRect = mainGameView.GetConstrainedGameViewRenderRect();
            }
            return s_MainGameViewRect;
        }

        internal static Vector2 GetSizeOfMainGameView()
        {
            Rect mainGameViewRenderRect = GetMainGameViewRenderRect();
            return new Vector2(mainGameViewRenderRect.width, mainGameViewRenderRect.height);
        }

        public bool IsShowingGizmos()
        {
            return this.m_Gizmos;
        }

        private void LoadRenderDoc()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                RenderDoc.Load();
                ShaderUtil.RecreateGfxDevice();
            }
        }

        public void OnDisable()
        {
            s_GameViews.Remove(this);
            this.m_ResolutionTooLargeWarning.valueChanged.RemoveListener(new UnityAction(this.Repaint));
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.DoDelayedGameViewChanged));
        }

        public void OnEnable()
        {
            base.depthBufferBits = 0x20;
            base.titleContent = base.GetLocalizedTitleContent();
            this.EnsureSelectedSizeAreValid();
            this.renderdocContent = EditorGUIUtility.IconContent("renderdoc", "Capture|Capture the current view and open in RenderDoc");
            base.dontClearBackground = true;
            s_GameViews.Add(this);
            this.m_ResolutionTooLargeWarning.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_ResolutionTooLargeWarning.speed = 0.3f;
        }

        private void OnFocus()
        {
            this.AllowCursorLockAndHide(true);
            s_LastFocusedGameView = this;
            InternalEditorUtility.OnGameViewFocus(true);
        }

        private void OnGUI()
        {
            bool flag;
            if (s_GizmoButtonStyle == null)
            {
                s_GizmoButtonStyle = "GV Gizmo DropDown";
                s_ResolutionWarningStyle = new GUIStyle("PreOverlayLabel");
                s_ResolutionWarningStyle.alignment = TextAnchor.UpperLeft;
                s_ResolutionWarningStyle.padding = new RectOffset(6, 6, 1, 1);
            }
            this.DoToolbarGUI();
            Rect gameViewRenderRect = this.gameViewRenderRect;
            Rect rect = GetConstrainedGameViewRenderRect(EditorGUIUtility.PointsToPixels(gameViewRenderRect), this.selectedSizeIndex, out flag);
            Rect rect4 = EditorGUIUtility.PixelsToPoints(rect);
            Rect rect5 = GUIClip.Unclip(rect4);
            Rect cameraRect = EditorGUIUtility.PointsToPixels(rect5);
            base.SetInternalGameViewRect(rect5);
            EditorGUIUtility.AddCursorRect(rect4, MouseCursor.CustomCursor);
            EventType type = Event.current.type;
            if ((type == EventType.MouseDown) && gameViewRenderRect.Contains(Event.current.mousePosition))
            {
                this.AllowCursorLockAndHide(true);
            }
            else if ((type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.Escape))
            {
                Unsupported.SetAllowCursorLock(false);
            }
            switch (type)
            {
                case EventType.Layout:
                case EventType.Used:
                    break;

                case EventType.Repaint:
                {
                    bool flag2 = EditorGUIUtility.IsDisplayReferencedByCameras(this.m_TargetDisplay);
                    if ((!this.currentGameViewSize.isFreeAspectRatio || !InternalEditorUtility.HasFullscreenCamera()) || !flag2)
                    {
                        GUI.Box(gameViewRenderRect, GUIContent.none, "GameViewBackground");
                        if (!InternalEditorUtility.HasFullscreenCamera())
                        {
                            float[] singleArray1 = new float[] { 30f, (gameViewRenderRect.height / 2f) - 10f, gameViewRenderRect.height - 10f };
                            foreach (int num in singleArray1)
                            {
                                GUI.Label(new Rect((gameViewRenderRect.width / 2f) - 100f, (float) num, 300f, 20f), "Scene is missing a fullscreen camera", "WhiteLargeLabel");
                            }
                        }
                    }
                    Vector2 vector = GUIUtility.s_EditorScreenPointOffset;
                    GUIUtility.s_EditorScreenPointOffset = Vector2.zero;
                    SavedGUIState state = SavedGUIState.Create();
                    if (this.ShouldShowMultiDisplayOption())
                    {
                        EditorGUIUtility.RenderGameViewCamerasInternal(cameraRect, this.m_TargetDisplay, this.m_Gizmos, true);
                    }
                    else
                    {
                        EditorGUIUtility.RenderGameViewCamerasInternal(cameraRect, 0, this.m_Gizmos, true);
                    }
                    GL.sRGBWrite = false;
                    state.ApplyAndForget();
                    GUIUtility.s_EditorScreenPointOffset = vector;
                    break;
                }
                default:
                {
                    if (WindowLayout.s_MaximizeKey.activated && (!EditorApplication.isPlaying || EditorApplication.isPaused))
                    {
                        return;
                    }
                    bool flag3 = rect4.Contains(Event.current.mousePosition);
                    if ((Event.current.rawType == EventType.MouseDown) && !flag3)
                    {
                        return;
                    }
                    Vector2 mousePosition = Event.current.mousePosition;
                    Vector2 position = mousePosition - rect4.position;
                    position = EditorGUIUtility.PointsToPixels(position);
                    Event.current.mousePosition = position;
                    Event.current.displayIndex = this.m_TargetDisplay;
                    EditorGUIUtility.QueueGameViewInputEvent(Event.current);
                    bool flag4 = true;
                    if ((Event.current.rawType == EventType.MouseUp) && !flag3)
                    {
                        flag4 = false;
                    }
                    switch (type)
                    {
                        case EventType.ExecuteCommand:
                        case EventType.ValidateCommand:
                            flag4 = false;
                            break;
                    }
                    if (flag4)
                    {
                        Event.current.Use();
                    }
                    else
                    {
                        Event.current.mousePosition = mousePosition;
                    }
                    break;
                }
            }
            this.ShowResolutionWarning(new Rect(gameViewRenderRect.x, gameViewRenderRect.y, 200f, 20f), flag, rect.size);
            if (this.m_Stats)
            {
                GameViewGUI.GameViewStatsGUI();
            }
        }

        private void OnLostFocus()
        {
            if (!EditorApplicationLayout.IsInitializingPlaymodeLayout())
            {
                this.AllowCursorLockAndHide(false);
            }
            InternalEditorUtility.OnGameViewFocus(false);
        }

        internal override void OnResized()
        {
            this.DelayedGameViewChanged();
        }

        private void OnSelectionChange()
        {
            if (this.m_Gizmos)
            {
                base.Repaint();
            }
        }

        public void OnValidate()
        {
            this.EnsureSelectedSizeAreValid();
        }

        public static void RepaintAll()
        {
            if (s_GameViews != null)
            {
                foreach (GameView view in s_GameViews)
                {
                    view.Repaint();
                }
            }
        }

        private void SelectionCallback(int indexClicked, object objectSelected)
        {
            if (indexClicked != this.selectedSizeIndex)
            {
                this.selectedSizeIndex = indexClicked;
                base.dontClearBackground = true;
                this.GameViewAspectWasChanged();
            }
        }

        private bool ShouldShowMultiDisplayOption()
        {
            GUIContent[] displayNames = ModuleManager.GetDisplayNames(EditorUserBuildSettings.activeBuildTarget.ToString());
            return ((BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget) == BuildTargetGroup.Standalone) || (displayNames != null));
        }

        private void ShowResolutionWarning(Rect position, bool fitsInsideRect, Vector2 shownSize)
        {
            if (!fitsInsideRect && (shownSize != this.m_ShownResolution))
            {
                this.m_ShownResolution = shownSize;
                this.m_ResolutionTooLargeWarning.value = true;
            }
            if (fitsInsideRect && (this.m_ShownResolution != Vector2.zero))
            {
                this.m_ShownResolution = Vector2.zero;
                this.m_ResolutionTooLargeWarning.value = false;
            }
            this.m_ResolutionTooLargeWarning.target = !fitsInsideRect && !EditorApplication.isPlaying;
            if (this.m_ResolutionTooLargeWarning.faded > 0f)
            {
                Color color = GUI.color;
                GUI.color = new Color(1f, 1f, 1f, Mathf.Clamp01(this.m_ResolutionTooLargeWarning.faded * 2f));
                EditorGUI.DropShadowLabel(position, string.Format("Using resolution {0}x{1}", shownSize.x, shownSize.y), s_ResolutionWarningStyle);
                GUI.color = color;
            }
        }

        private GameViewSize currentGameViewSize
        {
            get
            {
                return ScriptableSingleton<GameViewSizes>.instance.currentGroup.GetGameViewSize(this.selectedSizeIndex);
            }
        }

        private static GameViewSizeGroupType currentSizeGroupType
        {
            get
            {
                return ScriptableSingleton<GameViewSizes>.instance.currentGroupType;
            }
        }

        private Rect gameViewRenderRect
        {
            get
            {
                return new Rect(0f, 17f, base.position.width, base.position.height - 17f);
            }
        }

        public bool maximizeOnPlay
        {
            get
            {
                return this.m_MaximizeOnPlay;
            }
            set
            {
                this.m_MaximizeOnPlay = value;
            }
        }

        private int selectedSizeIndex
        {
            get
            {
                return this.m_SelectedSizes[(int) currentSizeGroupType];
            }
            set
            {
                this.m_SelectedSizes[(int) currentSizeGroupType] = value;
            }
        }
    }
}

