namespace UnityEditor
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Internal;

    public class EditorWindow : ScriptableObject
    {
        private const double kWarningFadeoutTime = 1.0;
        private const double kWarningFadeoutWait = 4.0;
        [SerializeField, HideInInspector]
        private int m_AntiAlias;
        [SerializeField, HideInInspector]
        private bool m_AutoRepaintOnSceneChange;
        [SerializeField, HideInInspector]
        private int m_DepthBufferBits;
        private bool m_DontClearBackground;
        internal float m_FadeoutTime;
        private Rect m_GameViewRect;
        [HideInInspector, SerializeField]
        private Vector2 m_MaxSize = new Vector2(4000f, 4000f);
        [SerializeField, HideInInspector]
        private Vector2 m_MinSize = new Vector2(100f, 100f);
        internal GUIContent m_Notification;
        private Vector2 m_NotificationSize;
        [NonSerialized]
        internal HostView m_Parent;
        [HideInInspector, SerializeField]
        internal Rect m_Pos = new Rect(0f, 0f, 320f, 240f);
        [HideInInspector, SerializeField]
        internal GUIContent m_TitleContent;
        private bool m_WantsMouseMove;

        public EditorWindow()
        {
            this.titleContent.text = base.GetType().ToString();
            base.hideFlags = HideFlags.DontSave;
        }

        [ContextMenu("Add Game")]
        internal void AddGameTab()
        {
        }

        [ContextMenu("Add Scene")]
        internal void AddSceneTab()
        {
        }

        public void BeginWindows()
        {
            EditorGUIInternal.BeginWindowsForward(1, base.GetInstanceID());
        }

        internal void CheckForWindowRepaint()
        {
            double timeSinceStartup = EditorApplication.timeSinceStartup;
            if (timeSinceStartup >= this.m_FadeoutTime)
            {
                if (timeSinceStartup > (this.m_FadeoutTime + 1.0))
                {
                    this.RemoveNotification();
                }
                else
                {
                    this.Repaint();
                }
            }
        }

        public void Close()
        {
            if (WindowLayout.IsMaximized(this))
            {
                WindowLayout.Unmaximize(this);
            }
            DockArea parent = this.m_Parent as DockArea;
            if (parent != null)
            {
                parent.RemoveTab(this, true);
            }
            else
            {
                this.m_Parent.window.Close();
            }
            Object.DestroyImmediate(this, true);
        }

        internal static void CreateNewWindowForEditorWindow(EditorWindow window, bool loadPosition, bool showImmediately)
        {
            CreateNewWindowForEditorWindow(window, new Vector2(window.position.x, window.position.y), loadPosition, showImmediately);
        }

        internal static void CreateNewWindowForEditorWindow(EditorWindow window, Vector2 screenPosition, bool loadPosition, bool showImmediately)
        {
            ContainerWindow window2 = ScriptableObject.CreateInstance<ContainerWindow>();
            SplitView view = ScriptableObject.CreateInstance<SplitView>();
            window2.mainView = view;
            DockArea child = ScriptableObject.CreateInstance<DockArea>();
            view.AddChild(child);
            child.AddTab(window);
            Rect rect = window.m_Parent.borderSize.Add(new Rect(screenPosition.x, screenPosition.y, window.position.width, window.position.height));
            window2.position = rect;
            view.position = new Rect(0f, 0f, rect.width, rect.height);
            window.MakeParentsSettingsMatchMe();
            window2.Show(ShowMode.NormalWindow, loadPosition, showImmediately);
            window2.OnResize();
        }

        internal void DrawNotification()
        {
            Vector2 notificationSize = this.m_NotificationSize;
            float num = this.position.width - EditorStyles.notificationText.margin.horizontal;
            float num2 = (this.position.height - EditorStyles.notificationText.margin.vertical) - 20f;
            if (num < this.m_NotificationSize.x)
            {
                float num3 = num / this.m_NotificationSize.x;
                notificationSize.x *= num3;
                notificationSize.y = EditorStyles.notificationText.CalcHeight(this.m_Notification, notificationSize.x);
            }
            if (notificationSize.y > num2)
            {
                notificationSize.y = num2;
            }
            Rect position = new Rect((this.position.width - notificationSize.x) * 0.5f, 20f + (((this.position.height - 20f) - notificationSize.y) * 0.7f), notificationSize.x, notificationSize.y);
            double timeSinceStartup = EditorApplication.timeSinceStartup;
            if (timeSinceStartup > this.m_FadeoutTime)
            {
                GUI.color = new Color(1f, 1f, 1f, 1f - ((float) ((timeSinceStartup - this.m_FadeoutTime) / 1.0)));
            }
            GUI.Label(position, GUIContent.none, EditorStyles.notificationBackground);
            EditorGUI.DoDropShadowLabel(position, this.m_Notification, EditorStyles.notificationText, 0.3f);
        }

        public void EndWindows()
        {
            GUI.EndWindows();
        }

        public void Focus()
        {
            if (this.m_Parent != null)
            {
                this.ShowTab();
                this.m_Parent.Focus();
            }
        }

        public static void FocusWindowIfItsOpen<T>() where T: EditorWindow
        {
            FocusWindowIfItsOpen(typeof(T));
        }

        public static void FocusWindowIfItsOpen(Type t)
        {
            Object[] objArray = Resources.FindObjectsOfTypeAll(t);
            EditorWindow window = (objArray.Length <= 0) ? null : (objArray[0] as EditorWindow);
            if (window != null)
            {
                window.Focus();
            }
        }

        internal Rect GetCurrentGameViewRect()
        {
            return this.m_GameViewRect;
        }

        private static EditorWindowTitleAttribute GetEditorWindowTitleAttribute(Type t)
        {
            foreach (object obj2 in t.GetCustomAttributes(true))
            {
                Attribute attribute = (Attribute) obj2;
                if (attribute.TypeId == typeof(EditorWindowTitleAttribute))
                {
                    return (EditorWindowTitleAttribute) obj2;
                }
            }
            return null;
        }

        internal GUIContent GetLocalizedTitleContent()
        {
            return GetLocalizedTitleContentFromType(base.GetType());
        }

        internal static GUIContent GetLocalizedTitleContentFromType(Type t)
        {
            EditorWindowTitleAttribute editorWindowTitleAttribute = GetEditorWindowTitleAttribute(t);
            if (editorWindowTitleAttribute == null)
            {
                return new GUIContent(t.ToString());
            }
            string icon = string.Empty;
            if (!string.IsNullOrEmpty(editorWindowTitleAttribute.icon))
            {
                icon = editorWindowTitleAttribute.icon;
            }
            else if (editorWindowTitleAttribute.useTypeNameAsIconName)
            {
                icon = t.ToString();
            }
            if (!string.IsNullOrEmpty(icon))
            {
                return EditorGUIUtility.TextContentWithIcon(editorWindowTitleAttribute.title, icon);
            }
            return EditorGUIUtility.TextContent(editorWindowTitleAttribute.title);
        }

        internal int GetNumTabs()
        {
            DockArea parent = this.m_Parent as DockArea;
            if (parent != null)
            {
                return parent.m_Panes.Count;
            }
            return 0;
        }

        public static T GetWindow<T>() where T: EditorWindow
        {
            return GetWindow<T>(false, null, true);
        }

        public static T GetWindow<T>(bool utility) where T: EditorWindow
        {
            return GetWindow<T>(utility, null, true);
        }

        public static T GetWindow<T>(string title) where T: EditorWindow
        {
            return GetWindow<T>(title, true);
        }

        [ExcludeFromDocs]
        public static EditorWindow GetWindow(Type t)
        {
            bool focus = true;
            string title = null;
            bool utility = false;
            return GetWindow(t, utility, title, focus);
        }

        public static T GetWindow<T>(params Type[] desiredDockNextTo) where T: EditorWindow
        {
            return GetWindow<T>(null, true, desiredDockNextTo);
        }

        public static T GetWindow<T>(bool utility, string title) where T: EditorWindow
        {
            return GetWindow<T>(utility, title, true);
        }

        public static T GetWindow<T>(string title, bool focus) where T: EditorWindow
        {
            return GetWindow<T>(false, title, focus);
        }

        public static T GetWindow<T>(string title, params Type[] desiredDockNextTo) where T: EditorWindow
        {
            return GetWindow<T>(title, true, desiredDockNextTo);
        }

        [ExcludeFromDocs]
        public static EditorWindow GetWindow(Type t, bool utility)
        {
            bool focus = true;
            string title = null;
            return GetWindow(t, utility, title, focus);
        }

        public static T GetWindow<T>(bool utility, string title, bool focus) where T: EditorWindow
        {
            return (GetWindow(typeof(T), utility, title, focus) as T);
        }

        public static T GetWindow<T>(string title, bool focus, params Type[] desiredDockNextTo) where T: EditorWindow
        {
            T[] localArray = Resources.FindObjectsOfTypeAll(typeof(T)) as T[];
            T pane = (localArray.Length <= 0) ? null : localArray[0];
            if (pane != null)
            {
                if (focus)
                {
                    pane.Focus();
                }
                return pane;
            }
            pane = ScriptableObject.CreateInstance<T>();
            if (title != null)
            {
                pane.titleContent = new GUIContent(title);
            }
            <GetWindow>c__AnonStorey11<T> storey = new <GetWindow>c__AnonStorey11<T>();
            Type[] typeArray = desiredDockNextTo;
            for (int i = 0; i < typeArray.Length; i++)
            {
                storey.desired = typeArray[i];
                foreach (ContainerWindow window in ContainerWindow.windows)
                {
                    foreach (View view in window.mainView.allChildren)
                    {
                        DockArea area = view as DockArea;
                        if ((area != null) && area.m_Panes.Any<EditorWindow>(new Func<EditorWindow, bool>(storey.<>m__13)))
                        {
                            area.AddTab(pane);
                            return pane;
                        }
                    }
                }
            }
            pane.Show();
            return pane;
        }

        [ExcludeFromDocs]
        public static EditorWindow GetWindow(Type t, bool utility, string title)
        {
            bool focus = true;
            return GetWindow(t, utility, title, focus);
        }

        public static EditorWindow GetWindow(Type t, [DefaultValue("false")] bool utility, [DefaultValue("null")] string title, [DefaultValue("true")] bool focus)
        {
            return GetWindowPrivate(t, utility, title, focus);
        }

        internal static T GetWindowDontShow<T>() where T: EditorWindow
        {
            Object[] objArray = Resources.FindObjectsOfTypeAll(typeof(T));
            return ((objArray.Length <= 0) ? ScriptableObject.CreateInstance<T>() : ((T) objArray[0]));
        }

        private static EditorWindow GetWindowPrivate(Type t, bool utility, string title, bool focus)
        {
            Object[] objArray = Resources.FindObjectsOfTypeAll(t);
            EditorWindow window = (objArray.Length <= 0) ? null : ((EditorWindow) objArray[0]);
            if (window == null)
            {
                window = ScriptableObject.CreateInstance(t) as EditorWindow;
                if (title != null)
                {
                    window.titleContent = new GUIContent(title);
                }
                if (utility)
                {
                    window.ShowUtility();
                    return window;
                }
                window.Show();
                return window;
            }
            if (focus)
            {
                window.Show();
                window.Focus();
            }
            return window;
        }

        public static T GetWindowWithRect<T>(Rect rect) where T: EditorWindow
        {
            return GetWindowWithRect<T>(rect, false, null, true);
        }

        [ExcludeFromDocs]
        public static EditorWindow GetWindowWithRect(Type t, Rect rect)
        {
            string title = null;
            bool utility = false;
            return GetWindowWithRect(t, rect, utility, title);
        }

        public static T GetWindowWithRect<T>(Rect rect, bool utility) where T: EditorWindow
        {
            return GetWindowWithRect<T>(rect, utility, null, true);
        }

        [ExcludeFromDocs]
        public static EditorWindow GetWindowWithRect(Type t, Rect rect, bool utility)
        {
            string title = null;
            return GetWindowWithRect(t, rect, utility, title);
        }

        public static T GetWindowWithRect<T>(Rect rect, bool utility, string title) where T: EditorWindow
        {
            return GetWindowWithRect<T>(rect, utility, title, true);
        }

        public static EditorWindow GetWindowWithRect(Type t, Rect rect, [DefaultValue("false")] bool utility, [DefaultValue("null")] string title)
        {
            return GetWindowWithRectPrivate(t, rect, utility, title);
        }

        public static T GetWindowWithRect<T>(Rect rect, bool utility, string title, bool focus) where T: EditorWindow
        {
            T local;
            Object[] objArray = Resources.FindObjectsOfTypeAll(typeof(T));
            if (objArray.Length > 0)
            {
                local = (T) objArray[0];
                if (focus)
                {
                    local.Focus();
                }
                return local;
            }
            local = ScriptableObject.CreateInstance<T>();
            local.minSize = new Vector2(rect.width, rect.height);
            local.maxSize = new Vector2(rect.width, rect.height);
            local.position = rect;
            if (title != null)
            {
                local.titleContent = new GUIContent(title);
            }
            if (utility)
            {
                local.ShowUtility();
                return local;
            }
            local.Show();
            return local;
        }

        private static EditorWindow GetWindowWithRectPrivate(Type t, Rect rect, bool utility, string title)
        {
            Object[] objArray = Resources.FindObjectsOfTypeAll(t);
            EditorWindow window = (objArray.Length <= 0) ? null : ((EditorWindow) objArray[0]);
            if (window == null)
            {
                window = ScriptableObject.CreateInstance(t) as EditorWindow;
                window.minSize = new Vector2(rect.width, rect.height);
                window.maxSize = new Vector2(rect.width, rect.height);
                window.position = rect;
                if (title != null)
                {
                    window.titleContent = new GUIContent(title);
                }
                if (utility)
                {
                    window.ShowUtility();
                    return window;
                }
                window.Show();
                return window;
            }
            window.Focus();
            return window;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void MakeModal(ContainerWindow win);
        internal void MakeParentsSettingsMatchMe()
        {
            if ((this.m_Parent != null) && (this.m_Parent.actualView == this))
            {
                this.m_Parent.SetTitle(base.GetType().FullName);
                this.m_Parent.autoRepaintOnSceneChange = this.m_AutoRepaintOnSceneChange;
                bool flag = (this.m_Parent.antiAlias != this.m_AntiAlias) || (this.m_Parent.depthBufferBits != this.m_DepthBufferBits);
                this.m_Parent.antiAlias = this.m_AntiAlias;
                this.m_Parent.depthBufferBits = this.m_DepthBufferBits;
                this.m_Parent.SetInternalGameViewRect(this.m_GameViewRect);
                this.m_Parent.wantsMouseMove = this.m_WantsMouseMove;
                Vector2 vector = new Vector2((float) (this.m_Parent.borderSize.left + this.m_Parent.borderSize.right), (float) (this.m_Parent.borderSize.top + this.m_Parent.borderSize.bottom));
                this.m_Parent.SetMinMaxSizes(this.minSize + vector, this.maxSize + vector);
                if (flag)
                {
                    this.m_Parent.RecreateContext();
                }
            }
        }

        internal virtual void OnResized()
        {
        }

        internal void RemoveFromDockArea()
        {
            DockArea parent = this.m_Parent as DockArea;
            if (parent != null)
            {
                parent.RemoveTab(this, true);
            }
        }

        public void RemoveNotification()
        {
            if (this.m_FadeoutTime != 0f)
            {
                EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.CheckForWindowRepaint));
                this.m_Notification = null;
                this.m_FadeoutTime = 0f;
            }
        }

        public void Repaint()
        {
            if ((this.m_Parent != null) && (this.m_Parent.actualView == this))
            {
                this.m_Parent.Repaint();
            }
        }

        internal void RepaintImmediately()
        {
            if ((this.m_Parent != null) && (this.m_Parent.actualView == this))
            {
                this.m_Parent.RepaintImmediately();
            }
        }

        public bool SendEvent(Event e)
        {
            return this.m_Parent.SendEvent(e);
        }

        internal void SetInternalGameViewRect(Rect rect)
        {
            this.m_GameViewRect = rect;
            this.m_Parent.SetInternalGameViewRect(this.m_GameViewRect);
        }

        public void Show()
        {
            this.Show(false);
        }

        public void Show(bool immediateDisplay)
        {
            if (this.m_Parent == null)
            {
                CreateNewWindowForEditorWindow(this, true, immediateDisplay);
            }
        }

        public void ShowAsDropDown(Rect buttonRect, Vector2 windowSize)
        {
            this.ShowAsDropDown(buttonRect, windowSize, null);
        }

        internal void ShowAsDropDown(Rect buttonRect, Vector2 windowSize, PopupLocationHelper.PopupLocation[] locationPriorityOrder)
        {
            this.position = this.ShowAsDropDownFitToScreen(buttonRect, windowSize, locationPriorityOrder);
            this.ShowWithMode(ShowMode.PopupMenu);
            this.position = this.ShowAsDropDownFitToScreen(buttonRect, windowSize, locationPriorityOrder);
            this.minSize = new Vector2(this.position.width, this.position.height);
            this.maxSize = new Vector2(this.position.width, this.position.height);
            if (focusedWindow != this)
            {
                this.Focus();
            }
            this.m_Parent.AddToAuxWindowList();
            this.m_Parent.window.m_DontSaveToLayout = true;
        }

        internal Rect ShowAsDropDownFitToScreen(Rect buttonRect, Vector2 windowSize, PopupLocationHelper.PopupLocation[] locationPriorityOrder)
        {
            if (this.m_Parent == null)
            {
                return new Rect(buttonRect.x, buttonRect.yMax, windowSize.x, windowSize.y);
            }
            return this.m_Parent.window.GetDropDownRect(buttonRect, windowSize, windowSize, locationPriorityOrder);
        }

        public void ShowAuxWindow()
        {
            this.ShowWithMode(ShowMode.AuxWindow);
            this.Focus();
            this.m_Parent.AddToAuxWindowList();
        }

        internal void ShowModal()
        {
            this.ShowWithMode(ShowMode.AuxWindow);
            this.MakeModal(this.m_Parent.window);
        }

        internal bool ShowNextTabIfPossible()
        {
            DockArea parent = this.m_Parent as DockArea;
            if (parent != null)
            {
                int num = (parent.m_Panes.IndexOf(this) + 1) % parent.m_Panes.Count;
                if (parent.selected != num)
                {
                    parent.selected = num;
                    parent.Repaint();
                    return true;
                }
            }
            return false;
        }

        public void ShowNotification(GUIContent notification)
        {
            this.m_Notification = new GUIContent(notification);
            EditorStyles.notificationText.CalcMinMaxWidth(this.m_Notification, out this.m_NotificationSize.y, out this.m_NotificationSize.x);
            this.m_NotificationSize.y = EditorStyles.notificationText.CalcHeight(this.m_Notification, this.m_NotificationSize.x);
            if (this.m_FadeoutTime == 0f)
            {
                EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.CheckForWindowRepaint));
            }
            this.m_FadeoutTime = (float) (EditorApplication.timeSinceStartup + 4.0);
        }

        public void ShowPopup()
        {
            if (this.m_Parent == null)
            {
                ContainerWindow window = ScriptableObject.CreateInstance<ContainerWindow>();
                window.title = this.titleContent.text;
                HostView view = ScriptableObject.CreateInstance<HostView>();
                view.actualView = this;
                Rect rect = this.m_Parent.borderSize.Add(new Rect(this.position.x, this.position.y, this.position.width, this.position.height));
                window.position = rect;
                window.mainView = view;
                this.MakeParentsSettingsMatchMe();
                window.ShowPopup();
            }
        }

        public void ShowTab()
        {
            DockArea parent = this.m_Parent as DockArea;
            if (parent != null)
            {
                int index = parent.m_Panes.IndexOf(this);
                if (parent.selected != index)
                {
                    parent.selected = index;
                }
            }
            this.Repaint();
        }

        public void ShowUtility()
        {
            this.ShowWithMode(ShowMode.Utility);
        }

        internal void ShowWithMode(ShowMode mode)
        {
            if (this.m_Parent == null)
            {
                SavedGUIState state = SavedGUIState.Create();
                ContainerWindow window = ScriptableObject.CreateInstance<ContainerWindow>();
                window.title = this.titleContent.text;
                HostView view = ScriptableObject.CreateInstance<HostView>();
                view.actualView = this;
                Rect rect = this.m_Parent.borderSize.Add(new Rect(this.position.x, this.position.y, this.position.width, this.position.height));
                window.position = rect;
                window.mainView = view;
                this.MakeParentsSettingsMatchMe();
                window.Show(mode, true, false);
                state.ApplyAndForget();
            }
        }

        public int antiAlias
        {
            get
            {
                return this.m_AntiAlias;
            }
            set
            {
                this.m_AntiAlias = value;
            }
        }

        public bool autoRepaintOnSceneChange
        {
            get
            {
                return this.m_AutoRepaintOnSceneChange;
            }
            set
            {
                this.m_AutoRepaintOnSceneChange = value;
                this.MakeParentsSettingsMatchMe();
            }
        }

        public int depthBufferBits
        {
            get
            {
                return this.m_DepthBufferBits;
            }
            set
            {
                this.m_DepthBufferBits = value;
            }
        }

        internal bool docked
        {
            get
            {
                return (((this.m_Parent != null) && (this.m_Parent.window != null)) && !this.m_Parent.window.IsNotDocked());
            }
        }

        internal bool dontClearBackground
        {
            get
            {
                return this.m_DontClearBackground;
            }
            set
            {
                this.m_DontClearBackground = value;
                if ((this.m_Parent != null) && (this.m_Parent.actualView == this))
                {
                    this.m_Parent.backgroundValid = false;
                }
            }
        }

        public static EditorWindow focusedWindow
        {
            get
            {
                HostView focusedView = GUIView.focusedView as HostView;
                if (focusedView != null)
                {
                    return focusedView.actualView;
                }
                return null;
            }
        }

        internal bool hasFocus
        {
            get
            {
                return ((this.m_Parent != null) && (this.m_Parent.actualView == this));
            }
        }

        public bool maximized
        {
            get
            {
                return WindowLayout.IsMaximized(this);
            }
            set
            {
                bool flag = WindowLayout.IsMaximized(this);
                if (value != flag)
                {
                    if (value)
                    {
                        WindowLayout.Maximize(this);
                    }
                    else
                    {
                        WindowLayout.Unmaximize(this);
                    }
                }
            }
        }

        public Vector2 maxSize
        {
            get
            {
                return this.m_MaxSize;
            }
            set
            {
                this.m_MaxSize = value;
                this.MakeParentsSettingsMatchMe();
            }
        }

        public Vector2 minSize
        {
            get
            {
                return this.m_MinSize;
            }
            set
            {
                this.m_MinSize = value;
                this.MakeParentsSettingsMatchMe();
            }
        }

        public static EditorWindow mouseOverWindow
        {
            get
            {
                HostView mouseOverView = GUIView.mouseOverView as HostView;
                if (mouseOverView != null)
                {
                    return mouseOverView.actualView;
                }
                return null;
            }
        }

        public Rect position
        {
            get
            {
                return this.m_Pos;
            }
            set
            {
                this.m_Pos = value;
                if (this.m_Parent != null)
                {
                    DockArea parent = this.m_Parent as DockArea;
                    if (parent == null)
                    {
                        this.m_Parent.window.position = value;
                    }
                    else if ((parent == null) || (((parent.parent != null) && (parent.m_Panes.Count == 1)) && (parent.parent.parent == null)))
                    {
                        parent.window.position = parent.borderSize.Add(value);
                    }
                    else
                    {
                        parent.RemoveTab(this);
                        CreateNewWindowForEditorWindow(this, true, true);
                    }
                }
            }
        }

        [Obsolete("Use titleContent instead (it supports setting a title icon as well).")]
        public string title
        {
            get
            {
                return this.titleContent.text;
            }
            set
            {
                this.titleContent = EditorGUIUtility.TextContent(value);
            }
        }

        public GUIContent titleContent
        {
            get
            {
                if (this.m_TitleContent == null)
                {
                }
                return (this.m_TitleContent = new GUIContent());
            }
            set
            {
                this.m_TitleContent = value;
                if (((this.m_TitleContent != null) && (this.m_Parent != null)) && ((this.m_Parent.window != null) && (this.m_Parent.window.mainView == this.m_Parent)))
                {
                    this.m_Parent.window.title = this.m_TitleContent.text;
                }
            }
        }

        public bool wantsMouseMove
        {
            get
            {
                return this.m_WantsMouseMove;
            }
            set
            {
                this.m_WantsMouseMove = value;
                this.MakeParentsSettingsMatchMe();
            }
        }

        [CompilerGenerated]
        private sealed class <GetWindow>c__AnonStorey11<T> where T: EditorWindow
        {
            internal Type desired;

            internal bool <>m__13(EditorWindow pane)
            {
                return (pane.GetType() == this.desired);
            }
        }
    }
}

