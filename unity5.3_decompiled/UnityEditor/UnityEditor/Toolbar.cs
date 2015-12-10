namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor.Connect;
    using UnityEditorInternal;
    using UnityEngine;

    internal class Toolbar : GUIView
    {
        [CompilerGenerated]
        private static GenericMenu.MenuFunction <>f__am$cacheD;
        [CompilerGenerated]
        private static GenericMenu.MenuFunction <>f__am$cacheE;
        [CompilerGenerated]
        private static GenericMenu.MenuFunction <>f__am$cacheF;
        public static Toolbar get = null;
        [SerializeField]
        private string m_LastLoadedLayoutName;
        private static GUIContent s_CloudIcon;
        private static GUIContent s_LayerContent;
        private static GUIContent[] s_PivotIcons;
        private static GUIContent[] s_PivotRotation;
        private static GUIContent[] s_PlayIcons;
        private static GUIContent[] s_ShownToolIcons = new GUIContent[5];
        private static GUIContent[] s_ToolIcons;
        private static GUIContent[] s_ViewToolIcons;
        private bool t1;
        private bool t2;
        private bool t3;

        public float CalcHeight()
        {
            return 30f;
        }

        private void DoLayersDropDown(Rect rect)
        {
            GUIStyle style = "DropDown";
            if (EditorGUI.ButtonMouseDown(rect, s_LayerContent, FocusType.Passive, style) && LayerVisibilityWindow.ShowAtPosition(rect))
            {
                GUIUtility.ExitGUI();
            }
        }

        private void DoLayoutDropDown(Rect rect)
        {
            if (EditorGUI.ButtonMouseDown(rect, GUIContent.Temp(lastLoadedLayoutName), FocusType.Passive, "DropDown"))
            {
                Vector2 vector = GUIUtility.GUIToScreenPoint(new Vector2(rect.x, rect.y));
                rect.x = vector.x;
                rect.y = vector.y;
                EditorUtility.Internal_DisplayPopupMenu(rect, "Window/Layouts", this, 0);
            }
        }

        private void DoPivotButtons(Rect rect)
        {
            Tools.pivotMode = (PivotMode) EditorGUI.CycleButton(new Rect(rect.x, rect.y, rect.width / 2f, rect.height), (int) Tools.pivotMode, s_PivotIcons, "ButtonLeft");
            if ((Tools.current == Tool.Scale) && (Selection.transforms.Length < 2))
            {
                GUI.enabled = false;
            }
            PivotRotation rotation = (PivotRotation) EditorGUI.CycleButton(new Rect(rect.x + (rect.width / 2f), rect.y, rect.width / 2f, rect.height), (int) Tools.pivotRotation, s_PivotRotation, "ButtonRight");
            if (Tools.pivotRotation != rotation)
            {
                Tools.pivotRotation = rotation;
                if (rotation == PivotRotation.Global)
                {
                    Tools.ResetGlobalHandleRotation();
                }
            }
            if (Tools.current == Tool.Scale)
            {
                GUI.enabled = true;
            }
            if (GUI.changed)
            {
                Tools.RepaintAllToolViews();
            }
        }

        private void DoPlayButtons(bool isOrWillEnterPlaymode)
        {
            bool isPlaying = EditorApplication.isPlaying;
            GUI.changed = false;
            int index = !isPlaying ? 0 : 4;
            if (AnimationMode.InAnimationMode())
            {
                index = 8;
            }
            Color color = GUI.color + new Color(0.01f, 0.01f, 0.01f, 0.01f);
            GUI.contentColor = new Color(1f / color.r, 1f / color.g, 1f / color.g, 1f / color.a);
            GUILayout.Toggle(isOrWillEnterPlaymode, s_PlayIcons[index], "CommandLeft", new GUILayoutOption[0]);
            GUI.backgroundColor = Color.white;
            if (GUI.changed)
            {
                TogglePlaying();
                GUIUtility.ExitGUI();
            }
            GUI.changed = false;
            bool flag2 = GUILayout.Toggle(EditorApplication.isPaused, s_PlayIcons[index + 1], "CommandMid", new GUILayoutOption[0]);
            if (GUI.changed)
            {
                EditorApplication.isPaused = flag2;
                GUIUtility.ExitGUI();
            }
            if (GUILayout.Button(s_PlayIcons[index + 2], "CommandRight", new GUILayoutOption[0]))
            {
                EditorApplication.Step();
                GUIUtility.ExitGUI();
            }
        }

        private void DoToolButtons(Rect rect)
        {
            GUI.changed = false;
            int selected = !Tools.viewToolActive ? ((int) Tools.current) : 0;
            for (int i = 1; i < 5; i++)
            {
                s_ShownToolIcons[i] = s_ToolIcons[(i - 1) + ((i != selected) ? 0 : 4)];
                s_ShownToolIcons[i].tooltip = s_ToolIcons[i - 1].tooltip;
            }
            s_ShownToolIcons[0] = s_ViewToolIcons[((int) Tools.viewTool) + ((selected != 0) ? 0 : 4)];
            selected = GUI.Toolbar(rect, selected, s_ShownToolIcons, "Command");
            if (GUI.changed)
            {
                Tools.current = (Tool) selected;
            }
        }

        private Rect GetThickArea(Rect pos)
        {
            return new Rect(pos.x, 5f, pos.width, 24f);
        }

        private Rect GetThinArea(Rect pos)
        {
            return new Rect(pos.x, 7f, pos.width, 18f);
        }

        private static void InitializeToolIcons()
        {
            if (s_ToolIcons == null)
            {
                s_ToolIcons = new GUIContent[] { EditorGUIUtility.IconContent("MoveTool", "|Move the selected objects."), EditorGUIUtility.IconContent("RotateTool", "|Rotate the selected objects."), EditorGUIUtility.IconContent("ScaleTool", "|Scale the selected objects."), EditorGUIUtility.IconContent("RectTool"), EditorGUIUtility.IconContent("MoveTool On"), EditorGUIUtility.IconContent("RotateTool On"), EditorGUIUtility.IconContent("ScaleTool On"), EditorGUIUtility.IconContent("RectTool On") };
                s_ViewToolIcons = new GUIContent[] { EditorGUIUtility.IconContent("ViewToolOrbit", "|Orbit the Scene view."), EditorGUIUtility.IconContent("ViewToolMove"), EditorGUIUtility.IconContent("ViewToolZoom"), EditorGUIUtility.IconContent("ViewToolOrbit", "|Orbit the Scene view."), EditorGUIUtility.IconContent("ViewToolOrbit On"), EditorGUIUtility.IconContent("ViewToolMove On"), EditorGUIUtility.IconContent("ViewToolZoom On"), EditorGUIUtility.IconContent("ViewToolOrbit On") };
                s_PivotIcons = new GUIContent[] { EditorGUIUtility.TextContentWithIcon("Center|The tool handle is placed at the center of the selection.", "ToolHandleCenter"), EditorGUIUtility.TextContentWithIcon("Pivot|The tool handle is placed at the active object's pivot point.", "ToolHandlePivot") };
                s_PivotRotation = new GUIContent[] { EditorGUIUtility.TextContentWithIcon("Local|Tool handles are in active object's rotation.", "ToolHandleLocal"), EditorGUIUtility.TextContentWithIcon("Global|Tool handles are in global rotation.", "ToolHandleGlobal") };
                s_LayerContent = EditorGUIUtility.TextContent("Layers|Which layers are visible in the Scene views.");
                s_PlayIcons = new GUIContent[] { EditorGUIUtility.IconContent("PlayButton"), EditorGUIUtility.IconContent("PauseButton"), EditorGUIUtility.IconContent("StepButton"), EditorGUIUtility.IconContent("PlayButtonProfile"), EditorGUIUtility.IconContent("PlayButton On"), EditorGUIUtility.IconContent("PauseButton On"), EditorGUIUtility.IconContent("StepButton On"), EditorGUIUtility.IconContent("PlayButtonProfile On"), EditorGUIUtility.IconContent("PlayButton Anim"), EditorGUIUtility.IconContent("PauseButton Anim"), EditorGUIUtility.IconContent("StepButton Anim"), EditorGUIUtility.IconContent("PlayButtonProfile Anim") };
                s_CloudIcon = EditorGUIUtility.IconContent("CloudConnect");
            }
        }

        private static void InternalWillTogglePlaymode()
        {
            InternalEditorUtility.RepaintAllViews();
        }

        public void OnDisable()
        {
            EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.modifierKeysChanged, new EditorApplication.CallbackFunction(this.Repaint));
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.OnSelectionChange));
            UnityConnect.instance.StateChanged -= new StateChangedDelegate(this.OnUnityConnectStateChanged);
        }

        public void OnEnable()
        {
            EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.modifierKeysChanged, new EditorApplication.CallbackFunction(this.Repaint));
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.OnSelectionChange));
            UnityConnect.instance.StateChanged += new StateChangedDelegate(this.OnUnityConnectStateChanged);
            get = this;
        }

        protected override bool OnFocus()
        {
            return false;
        }

        private void OnGUI()
        {
            float width = 10f;
            float num2 = 20f;
            float num3 = 32f;
            float num4 = 64f;
            float num5 = 80f;
            InitializeToolIcons();
            bool isPlayingOrWillChangePlaymode = EditorApplication.isPlayingOrWillChangePlaymode;
            if (isPlayingOrWillChangePlaymode)
            {
                GUI.color = (Color) HostView.kPlayModeDarken;
            }
            GUIStyle style = "AppToolbar";
            if (Event.current.type == EventType.Repaint)
            {
                style.Draw(new Rect(0f, 0f, base.position.width, base.position.height), false, false, false, false);
            }
            Rect pos = new Rect(0f, 0f, 0f, 0f);
            this.ReserveWidthRight(width, ref pos);
            this.ReserveWidthRight(num3 * 5f, ref pos);
            this.DoToolButtons(this.GetThickArea(pos));
            this.ReserveWidthRight(num2, ref pos);
            this.ReserveWidthRight(num4 * 2f, ref pos);
            this.DoPivotButtons(this.GetThinArea(pos));
            float num6 = 100f;
            pos = new Rect((base.position.width - num6) / 2f, 0f, 140f, 0f);
            GUILayout.BeginArea(this.GetThickArea(pos));
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            this.DoPlayButtons(isPlayingOrWillChangePlaymode);
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
            pos = new Rect(base.position.width, 0f, 0f, 0f);
            this.ReserveWidthLeft(width, ref pos);
            this.ReserveWidthLeft(num5, ref pos);
            this.DoLayoutDropDown(this.GetThinArea(pos));
            this.ReserveWidthLeft(width, ref pos);
            this.ReserveWidthLeft(num5, ref pos);
            this.DoLayersDropDown(this.GetThinArea(pos));
            this.ReserveWidthLeft(num2, ref pos);
            this.ReserveWidthLeft(num5, ref pos);
            if (EditorGUI.ButtonMouseDown(this.GetThinArea(pos), new GUIContent("Account"), FocusType.Passive, "Dropdown"))
            {
                this.ShowUserMenu(this.GetThinArea(pos));
            }
            this.ReserveWidthLeft(width, ref pos);
            this.ReserveWidthLeft(32f, ref pos);
            if (GUI.Button(this.GetThinArea(pos), s_CloudIcon, "Button"))
            {
                UnityConnectServiceCollection.instance.ShowService("Hub", true);
            }
            EditorGUI.ShowRepaints();
            Highlighter.ControlHighlightGUI(this);
        }

        private void OnSelectionChange()
        {
            Tools.OnSelectionChange();
            base.Repaint();
        }

        protected void OnUnityConnectStateChanged(ConnectInfo state)
        {
            base.Repaint();
        }

        internal static void RepaintToolbar()
        {
            if (get != null)
            {
                get.Repaint();
            }
        }

        private void ReserveWidthLeft(float width, ref Rect pos)
        {
            pos.x -= width;
            pos.width = width;
        }

        private void ReserveWidthRight(float width, ref Rect pos)
        {
            pos.x += pos.width;
            pos.width = width;
        }

        private void ShowUserMenu(Rect dropDownRect)
        {
            GenericMenu menu = new GenericMenu();
            if (!UnityConnect.instance.online)
            {
                menu.AddDisabledItem(new GUIContent("Go to account"));
                menu.AddDisabledItem(new GUIContent("Sign in..."));
                if (!Application.HasProLicense())
                {
                    menu.AddSeparator(string.Empty);
                    menu.AddDisabledItem(new GUIContent("Upgrade to Pro"));
                }
            }
            else
            {
                <ShowUserMenu>c__AnonStorey72 storey = new <ShowUserMenu>c__AnonStorey72 {
                    accountUrl = UnityConnect.instance.GetConfigurationURL(CloudConfigUrl.CloudWebauth)
                };
                if (UnityConnect.instance.loggedIn)
                {
                    menu.AddItem(new GUIContent("Go to account"), false, new GenericMenu.MenuFunction(storey.<>m__105));
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent("Go to account"));
                }
                if (UnityConnect.instance.loggedIn)
                {
                    string text = "Sign out " + UnityConnect.instance.userInfo.displayName;
                    if (<>f__am$cacheD == null)
                    {
                        <>f__am$cacheD = () => UnityConnect.instance.Logout();
                    }
                    menu.AddItem(new GUIContent(text), false, <>f__am$cacheD);
                }
                else
                {
                    if (<>f__am$cacheE == null)
                    {
                        <>f__am$cacheE = () => UnityConnect.instance.ShowLogin();
                    }
                    menu.AddItem(new GUIContent("Sign in..."), false, <>f__am$cacheE);
                }
                if (!Application.HasProLicense())
                {
                    menu.AddSeparator(string.Empty);
                    if (<>f__am$cacheF == null)
                    {
                        <>f__am$cacheF = () => Application.OpenURL("https://store.unity3d.com/");
                    }
                    menu.AddItem(new GUIContent("Upgrade to Pro"), false, <>f__am$cacheF);
                }
            }
            menu.DropDown(dropDownRect);
        }

        private static void TogglePlaying()
        {
            bool flag = !EditorApplication.isPlaying;
            EditorApplication.isPlaying = flag;
            InternalWillTogglePlaymode();
        }

        internal static string lastLoadedLayoutName
        {
            get
            {
                return (!string.IsNullOrEmpty(get.m_LastLoadedLayoutName) ? get.m_LastLoadedLayoutName : "Layout");
            }
            set
            {
                get.m_LastLoadedLayoutName = value;
                get.Repaint();
            }
        }

        [CompilerGenerated]
        private sealed class <ShowUserMenu>c__AnonStorey72
        {
            internal string accountUrl;

            internal void <>m__105()
            {
                UnityConnect.instance.OpenAuthorizedURLInWebBrowser(this.accountUrl);
            }
        }
    }
}

