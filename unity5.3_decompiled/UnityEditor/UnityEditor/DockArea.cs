namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEditorInternal;
    using UnityEngine;

    internal class DockArea : HostView, IDropArea
    {
        private const float kBottomBorders = 2f;
        internal const float kDockHeight = 39f;
        private const float kSideBorders = 2f;
        internal const float kTabHeight = 17f;
        private const float kWindowButtonsWidth = 40f;
        [SerializeField]
        internal int m_LastSelected;
        [SerializeField]
        internal List<EditorWindow> m_Panes = new List<EditorWindow>();
        [SerializeField]
        internal int m_Selected;
        private static int s_DragMode;
        private static EditorWindow s_DragPane;
        private static DropInfo s_DropInfo;
        internal static View s_IgnoreDockingForView;
        internal static DockArea s_OriginalDragSource;
        private static int s_PlaceholderPos;
        private static Vector2 s_StartDragPosition;
        [NonSerialized]
        internal GUIStyle tabStyle;

        public DockArea()
        {
            if ((this.m_Panes != null) && (this.m_Panes.Count != 0))
            {
                Debug.LogError("m_Panes is filled in DockArea constructor.");
            }
        }

        protected override void AddDefaultItemsToMenu(GenericMenu menu, EditorWindow view)
        {
            if (menu.GetItemCount() != 0)
            {
                menu.AddSeparator(string.Empty);
            }
            if (base.parent.window.showMode == ShowMode.MainWindow)
            {
                menu.AddItem(EditorGUIUtility.TextContent("Maximize"), !(base.parent is SplitView), new GenericMenu.MenuFunction2(this.Maximize), view);
            }
            else
            {
                menu.AddDisabledItem(EditorGUIUtility.TextContent("Maximize"));
            }
            menu.AddItem(EditorGUIUtility.TextContent("Close Tab"), false, new GenericMenu.MenuFunction2(this.Close), view);
            menu.AddSeparator(string.Empty);
            Type[] paneTypes = base.GetPaneTypes();
            GUIContent content = EditorGUIUtility.TextContent("Add Tab");
            foreach (Type type in paneTypes)
            {
                if (type != null)
                {
                    GUIContent content2;
                    content2 = new GUIContent(EditorWindow.GetLocalizedTitleContentFromType(type)) {
                        text = content.text + "/" + content2.text
                    };
                    menu.AddItem(content2, false, new GenericMenu.MenuFunction2(this.AddTabToHere), type);
                }
            }
        }

        public void AddTab(EditorWindow pane)
        {
            this.AddTab(this.m_Panes.Count, pane);
        }

        public void AddTab(int idx, EditorWindow pane)
        {
            base.DeregisterSelectedPane(true);
            this.m_Panes.Insert(idx, pane);
            base.m_ActualView = pane;
            this.m_Panes[idx] = pane;
            this.m_Selected = idx;
            base.RegisterSelectedPane();
            base.Repaint();
        }

        private void AddTabToHere(object userData)
        {
            EditorWindow pane = (EditorWindow) ScriptableObject.CreateInstance((Type) userData);
            this.AddTab(pane);
        }

        public static void BeginOffsetArea(Rect screenRect, GUIContent content, GUIStyle style)
        {
            GUILayoutGroup group = EditorGUILayoutUtilityInternal.BeginLayoutArea(style, typeof(GUILayoutGroup));
            if (Event.current.type == EventType.Layout)
            {
                group.resetCoords = false;
                group.minWidth = group.maxWidth = screenRect.width + 1f;
                group.minHeight = group.maxHeight = screenRect.height + 2f;
                group.rect = Rect.MinMaxRect(-1f, -1f, group.rect.xMax, group.rect.yMax - 10f);
            }
            GUI.BeginGroup(screenRect, content, style);
        }

        private static void CheckDragWindowExists()
        {
            if ((s_DragMode == 1) && (PaneDragTab.get.m_Window == null))
            {
                s_OriginalDragSource.RemoveTab(s_DragPane);
                Object.DestroyImmediate(s_DragPane);
                PaneDragTab.get.Close();
                GUIUtility.hotControl = 0;
                ResetDragVars();
            }
        }

        private void Close(object userData)
        {
            ((EditorWindow) userData).Close();
        }

        public DropInfo DragOver(EditorWindow window, Vector2 mouseScreenPosition)
        {
            Rect screenPosition = base.screenPosition;
            screenPosition.height = 39f;
            if (!screenPosition.Contains(mouseScreenPosition))
            {
                return null;
            }
            if (base.background == null)
            {
                base.background = "hostview";
            }
            Rect rect2 = base.background.margin.Remove(base.screenPosition);
            Vector2 mousePos = mouseScreenPosition - new Vector2(rect2.x, rect2.y);
            Rect tabRect = this.tabRect;
            int tabAtMousePos = this.GetTabAtMousePos(mousePos, tabRect);
            float tabWidth = this.GetTabWidth(tabRect.width);
            if (s_PlaceholderPos != tabAtMousePos)
            {
                base.Repaint();
                s_PlaceholderPos = tabAtMousePos;
            }
            return new DropInfo(this) { type = DropInfo.Type.Tab, rect = new Rect((mousePos.x - (tabWidth * 0.25f)) + rect2.x, tabRect.y + rect2.y, tabWidth, tabRect.height) };
        }

        private void DragTab(Rect pos, GUIStyle tabStyle)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            float tabWidth = this.GetTabWidth(pos.width);
            Event current = Event.current;
            if ((s_DragMode != 0) && (GUIUtility.hotControl == 0))
            {
                PaneDragTab.get.Close();
                ResetDragVars();
            }
            EventType typeForControl = current.GetTypeForControl(controlID);
            switch (typeForControl)
            {
                case EventType.MouseDown:
                    if (pos.Contains(current.mousePosition) && (GUIUtility.hotControl == 0))
                    {
                        int tabAtMousePos = this.GetTabAtMousePos(current.mousePosition, pos);
                        if (tabAtMousePos < this.m_Panes.Count)
                        {
                            switch (current.button)
                            {
                                case 0:
                                    if (tabAtMousePos != this.selected)
                                    {
                                        this.selected = tabAtMousePos;
                                    }
                                    GUIUtility.hotControl = controlID;
                                    s_StartDragPosition = current.mousePosition;
                                    s_DragMode = 0;
                                    current.Use();
                                    break;

                                case 2:
                                    this.m_Panes[tabAtMousePos].Close();
                                    current.Use();
                                    break;
                            }
                        }
                    }
                    goto Label_06B9;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        Vector2 vector3 = GUIUtility.GUIToScreenPoint(current.mousePosition);
                        if (s_DragMode != 0)
                        {
                            s_DragMode = 0;
                            PaneDragTab.get.Close();
                            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(DockArea.CheckDragWindowExists));
                            if ((s_DropInfo == null) || (s_DropInfo.dropArea == null))
                            {
                                EditorWindow pane = s_DragPane;
                                ResetDragVars();
                                this.RemoveTab(pane);
                                Rect position = pane.position;
                                position.x = vector3.x - (position.width * 0.5f);
                                position.y = vector3.y - (position.height * 0.5f);
                                if (Application.platform == RuntimePlatform.WindowsEditor)
                                {
                                    position.y = Mathf.Max(InternalEditorUtility.GetBoundsOfDesktopAtPoint(vector3).y, position.y);
                                }
                                EditorWindow.CreateNewWindowForEditorWindow(pane, false, false);
                                pane.position = pane.m_Parent.window.FitWindowRectToScreen(position, true, true);
                                GUIUtility.hotControl = 0;
                                GUIUtility.ExitGUI();
                            }
                            else
                            {
                                s_DropInfo.dropArea.PerformDrop(s_DragPane, s_DropInfo, vector3);
                            }
                            ResetDragVars();
                        }
                        GUIUtility.hotControl = 0;
                        current.Use();
                    }
                    goto Label_06B9;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        Vector2 vector = current.mousePosition - s_StartDragPosition;
                        current.Use();
                        Rect screenPosition = base.screenPosition;
                        if ((s_DragMode == 0) && (vector.sqrMagnitude > 99f))
                        {
                            s_DragMode = 1;
                            s_PlaceholderPos = this.selected;
                            s_DragPane = this.m_Panes[this.selected];
                            if (this.m_Panes.Count != 1)
                            {
                                s_IgnoreDockingForView = null;
                            }
                            else
                            {
                                s_IgnoreDockingForView = this;
                            }
                            s_OriginalDragSource = this;
                            PaneDragTab.get.content = s_DragPane.titleContent;
                            base.Internal_SetAsActiveWindow();
                            PaneDragTab.get.GrabThumbnail();
                            PaneDragTab.get.Show(new Rect((pos.x + screenPosition.x) + (tabWidth * this.selected), pos.y + screenPosition.y, tabWidth, pos.height), GUIUtility.GUIToScreenPoint(current.mousePosition));
                            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(DockArea.CheckDragWindowExists));
                            GUIUtility.ExitGUI();
                        }
                        if (s_DragMode == 1)
                        {
                            DropInfo di = null;
                            ContainerWindow[] windows = ContainerWindow.windows;
                            Vector2 screenPos = GUIUtility.GUIToScreenPoint(current.mousePosition);
                            ContainerWindow inFrontOf = null;
                            foreach (ContainerWindow window2 in windows)
                            {
                                foreach (View view in window2.mainView.allChildren)
                                {
                                    IDropArea area = view as IDropArea;
                                    if (area != null)
                                    {
                                        di = area.DragOver(s_DragPane, screenPos);
                                    }
                                    if (di != null)
                                    {
                                        break;
                                    }
                                }
                                if (di != null)
                                {
                                    inFrontOf = window2;
                                    break;
                                }
                            }
                            if (di == null)
                            {
                                di = new DropInfo(null);
                            }
                            if (di.type != DropInfo.Type.Tab)
                            {
                                s_PlaceholderPos = -1;
                            }
                            s_DropInfo = di;
                            if (PaneDragTab.get.m_Window != null)
                            {
                                PaneDragTab.get.SetDropInfo(di, screenPos, inFrontOf);
                            }
                        }
                    }
                    goto Label_06B9;

                case EventType.Repaint:
                {
                    float xMin = pos.xMin;
                    int num8 = 0;
                    if (base.actualView == null)
                    {
                        Rect rect5 = new Rect(xMin, pos.yMin, tabWidth, pos.height);
                        float x = Mathf.Round(rect5.x);
                        Rect rect6 = new Rect(x, rect5.y, Mathf.Round(rect5.x + rect5.width) - x, rect5.height);
                        tabStyle.Draw(rect6, "Failed to load", false, false, true, false);
                    }
                    else
                    {
                        for (int i = 0; i < this.m_Panes.Count; i++)
                        {
                            if (s_DragPane != this.m_Panes[i])
                            {
                                if (((s_DropInfo != null) && object.ReferenceEquals(s_DropInfo.dropArea, this)) && (s_PlaceholderPos == num8))
                                {
                                    xMin += tabWidth;
                                }
                                Rect rect3 = new Rect(xMin, pos.yMin, tabWidth, pos.height);
                                float num10 = Mathf.Round(rect3.x);
                                Rect rect4 = new Rect(num10, rect3.y, Mathf.Round(rect3.x + rect3.width) - num10, rect3.height);
                                tabStyle.Draw(rect4, this.m_Panes[i].titleContent, false, false, i == this.selected, base.hasFocus);
                                xMin += tabWidth;
                                num8++;
                            }
                        }
                    }
                    goto Label_06B9;
                }
            }
            if ((typeForControl == EventType.ContextClick) && (pos.Contains(current.mousePosition) && (GUIUtility.hotControl == 0)))
            {
                int num4 = this.GetTabAtMousePos(current.mousePosition, pos);
                if (num4 < this.m_Panes.Count)
                {
                    base.PopupGenericMenu(this.m_Panes[num4], new Rect(current.mousePosition.x, current.mousePosition.y, 0f, 0f));
                }
            }
        Label_06B9:
            this.selected = Mathf.Clamp(this.selected, 0, this.m_Panes.Count - 1);
        }

        public static void EndOffsetArea()
        {
            if (Event.current.type != EventType.Used)
            {
                GUILayoutUtility.EndLayoutGroup();
                GUI.EndGroup();
            }
        }

        protected override RectOffset GetBorderSize()
        {
            if (base.window != null)
            {
                int num = 0;
                base.m_BorderSize.bottom = num;
                base.m_BorderSize.top = num;
                base.m_BorderSize.right = num;
                base.m_BorderSize.left = num;
                Rect windowPosition = base.windowPosition;
                if (windowPosition.xMin != 0f)
                {
                    base.m_BorderSize.left += 2;
                }
                if (windowPosition.xMax != base.window.position.width)
                {
                    base.m_BorderSize.right += 2;
                }
                base.m_BorderSize.top = 0x11;
                bool flag = base.windowPosition.y == 0f;
                bool flag2 = windowPosition.yMax == base.window.position.height;
                base.m_BorderSize.bottom = 4;
                if (flag2)
                {
                    base.m_BorderSize.bottom -= 2;
                }
                if (flag)
                {
                    base.m_BorderSize.bottom += 3;
                }
            }
            return base.m_BorderSize;
        }

        private int GetTabAtMousePos(Vector2 mousePos, Rect position)
        {
            return (int) Mathf.Min((float) ((mousePos.x - position.xMin) / this.GetTabWidth(position.width)), (float) 100f);
        }

        private float GetTabWidth(float width)
        {
            int count = this.m_Panes.Count;
            if ((s_DropInfo != null) && object.ReferenceEquals(s_DropInfo.dropArea, this))
            {
                count++;
            }
            if (this.m_Panes.IndexOf(s_DragPane) != -1)
            {
                count--;
            }
            return Mathf.Min((float) (width / ((float) count)), (float) 100f);
        }

        internal override void Initialize(ContainerWindow win)
        {
            base.Initialize(win);
            this.RemoveNullWindows();
            foreach (EditorWindow window in this.m_Panes)
            {
                window.m_Parent = this;
            }
        }

        private void KillIfEmpty()
        {
            if (this.m_Panes.Count == 0)
            {
                if (base.parent == null)
                {
                    base.window.InternalCloseWindow();
                }
                else
                {
                    SplitView parent = base.parent as SplitView;
                    ICleanuppable cleanuppable = base.parent as ICleanuppable;
                    parent.RemoveChildNice(this);
                    Object.DestroyImmediate(this, true);
                    if (cleanuppable != null)
                    {
                        cleanuppable.Cleanup();
                    }
                }
            }
        }

        private void Maximize(object userData)
        {
            EditorWindow win = (EditorWindow) userData;
            WindowLayout.Maximize(win);
        }

        public void OnDestroy()
        {
            if (base.hasFocus)
            {
                base.Invoke("OnLostFocus");
            }
            base.actualView = null;
            foreach (EditorWindow window in this.m_Panes)
            {
                Object.DestroyImmediate(window, true);
            }
            base.OnDestroy();
        }

        public void OnEnable()
        {
            if ((this.m_Panes != null) && (this.m_Panes.Count > this.m_Selected))
            {
                base.actualView = this.m_Panes[this.m_Selected];
            }
            base.OnEnable();
        }

        public void OnGUI()
        {
            base.ClearBackground();
            EditorGUIUtility.ResetGUIState();
            SplitView parent = base.parent as SplitView;
            if ((Event.current.type == EventType.Repaint) && (parent != null))
            {
                View child = this;
                while (parent != null)
                {
                    int controlID = parent.controlID;
                    if ((controlID == GUIUtility.hotControl) || (GUIUtility.hotControl == 0))
                    {
                        int num2 = parent.IndexOfChild(child);
                        if (parent.vertical)
                        {
                            if (num2 != 0)
                            {
                                EditorGUIUtility.AddCursorRect(new Rect(0f, 0f, base.position.width, 5f), MouseCursor.SplitResizeUpDown, controlID);
                            }
                            if (num2 != (parent.children.Length - 1))
                            {
                                EditorGUIUtility.AddCursorRect(new Rect(0f, base.position.height - 5f, base.position.width, 5f), MouseCursor.SplitResizeUpDown, controlID);
                            }
                        }
                        else
                        {
                            if (num2 != 0)
                            {
                                EditorGUIUtility.AddCursorRect(new Rect(0f, 0f, 5f, base.position.height), MouseCursor.SplitResizeLeftRight, controlID);
                            }
                            if (num2 != (parent.children.Length - 1))
                            {
                                EditorGUIUtility.AddCursorRect(new Rect(base.position.width - 5f, 0f, 5f, base.position.height), MouseCursor.SplitResizeLeftRight, controlID);
                            }
                        }
                    }
                    child = parent;
                    parent = parent.parent as SplitView;
                }
                parent = base.parent as SplitView;
            }
            bool flag = false;
            if (base.window.mainView.GetType() != typeof(MainWindow))
            {
                flag = true;
                if (base.windowPosition.y == 0f)
                {
                    base.background = "dockareaStandalone";
                }
                else
                {
                    base.background = "dockarea";
                }
            }
            else
            {
                base.background = "dockarea";
            }
            if (parent != null)
            {
                Event event2;
                event2 = new Event(Event.current) {
                    mousePosition = event2.mousePosition + new Vector2(base.position.x, base.position.y)
                };
                parent.SplitGUI(event2);
                if (event2.type == EventType.Used)
                {
                    Event.current.Use();
                }
            }
            GUIStyle style = "dockareaoverlay";
            Rect position = base.background.margin.Remove(new Rect(0f, 0f, base.position.width, base.position.height));
            position.x = base.background.margin.left;
            position.y = base.background.margin.top;
            Rect windowPosition = base.windowPosition;
            float num3 = 2f;
            if (windowPosition.x == 0f)
            {
                position.x -= num3;
                position.width += num3;
            }
            if (windowPosition.xMax == base.window.position.width)
            {
                position.width += num3;
            }
            if (windowPosition.yMax == base.window.position.height)
            {
                position.height += !flag ? 2f : 2f;
            }
            GUI.Box(position, GUIContent.none, base.background);
            if (this.tabStyle == null)
            {
                this.tabStyle = "dragtab";
            }
            this.DragTab(new Rect(position.x + 1f, position.y, position.width - 40f, 17f), this.tabStyle);
            this.tabStyle = "dragtab";
            base.ShowGenericMenu();
            base.DoWindowDecorationStart();
            if (this.m_Panes.Count > 0)
            {
                if (this.m_Panes[this.selected] is GameView)
                {
                    GUI.Box(position, GUIContent.none, style);
                }
                BeginOffsetArea(new Rect(position.x + 2f, position.y + 17f, position.width - 4f, (position.height - 17f) - 2f), GUIContent.none, "TabWindowBackground");
                Vector2 vector = GUIUtility.GUIToScreenPoint(Vector2.zero);
                Rect rect3 = base.borderSize.Remove(base.position);
                rect3.x = vector.x;
                rect3.y = vector.y;
                this.m_Panes[this.selected].m_Pos = rect3;
                EditorGUIUtility.ResetGUIState();
                try
                {
                    base.Invoke("OnGUI");
                }
                catch (TargetInvocationException exception)
                {
                    throw exception.InnerException;
                }
                EditorGUIUtility.ResetGUIState();
                if (((base.actualView != null) && (base.actualView.m_FadeoutTime != 0f)) && ((Event.current != null) && (Event.current.type == EventType.Repaint)))
                {
                    base.actualView.DrawNotification();
                }
                EndOffsetArea();
            }
            base.DoWindowDecorationEnd();
            GUI.Box(position, GUIContent.none, style);
            EditorGUI.ShowRepaints();
            Highlighter.ControlHighlightGUI(this);
        }

        public bool PerformDrop(EditorWindow w, DropInfo info, Vector2 screenPos)
        {
            s_OriginalDragSource.RemoveTab(w, s_OriginalDragSource != this);
            int idx = (s_PlaceholderPos <= this.m_Panes.Count) ? s_PlaceholderPos : this.m_Panes.Count;
            this.AddTab(idx, w);
            this.selected = idx;
            return true;
        }

        private void RemoveNullWindows()
        {
            List<EditorWindow> list = new List<EditorWindow>();
            foreach (EditorWindow window in this.m_Panes)
            {
                if (window != null)
                {
                    list.Add(window);
                }
            }
            this.m_Panes = list;
        }

        public void RemoveTab(EditorWindow pane)
        {
            this.RemoveTab(pane, true);
        }

        public void RemoveTab(EditorWindow pane, bool killIfEmpty)
        {
            if (base.m_ActualView == pane)
            {
                base.DeregisterSelectedPane(true);
            }
            int index = this.m_Panes.IndexOf(pane);
            if (index == -1)
            {
                Debug.LogError("Unable to remove Pane - it's not IN the window");
            }
            else
            {
                this.m_Panes.Remove(pane);
                if (index == this.m_Selected)
                {
                    if (this.m_LastSelected >= (this.m_Panes.Count - 1))
                    {
                        this.m_LastSelected = this.m_Panes.Count - 1;
                    }
                    this.m_Selected = this.m_LastSelected;
                    if (this.m_Selected > -1)
                    {
                        base.m_ActualView = this.m_Panes[this.m_Selected];
                    }
                }
                else if (index < this.m_Selected)
                {
                    this.m_Selected--;
                }
                base.Repaint();
                pane.m_Parent = null;
                if (killIfEmpty)
                {
                    this.KillIfEmpty();
                }
                base.RegisterSelectedPane();
            }
        }

        private static void ResetDragVars()
        {
            s_DragPane = null;
            s_DropInfo = null;
            s_PlaceholderPos = -1;
            s_DragMode = 0;
            s_OriginalDragSource = null;
        }

        public int selected
        {
            get
            {
                return this.m_Selected;
            }
            set
            {
                if (this.m_Selected != value)
                {
                    this.m_Selected = value;
                    if ((this.m_Selected >= 0) && (this.m_Selected < this.m_Panes.Count))
                    {
                        base.actualView = this.m_Panes[this.m_Selected];
                    }
                }
            }
        }

        private Rect tabRect
        {
            get
            {
                return new Rect(0f, 0f, base.position.width, 17f);
            }
        }
    }
}

