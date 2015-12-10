namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEditorInternal;
    using UnityEngine;

    internal class WindowLayout
    {
        private const string kMaximizeRestoreFile = "CurrentMaximizeLayout.dwlt";
        internal static PrefKey s_MaximizeKey = new PrefKey("Window/Maximize View", "# ");

        public static void AddSplitViewAndChildrenRecurse(View splitview, ArrayList list)
        {
            list.Add(splitview);
            DockArea area = splitview as DockArea;
            if (area != null)
            {
                list.AddRange(area.m_Panes);
            }
            HostView view = splitview as DockArea;
            if (view != null)
            {
                list.Add(area.actualView);
            }
            foreach (View view2 in splitview.children)
            {
                AddSplitViewAndChildrenRecurse(view2, list);
            }
        }

        internal static void CheckWindowConsistency()
        {
            foreach (EditorWindow window in Resources.FindObjectsOfTypeAll(typeof(EditorWindow)))
            {
                if (window.m_Parent == null)
                {
                    Debug.LogError("Invalid editor window " + window.GetType());
                }
            }
        }

        public static void CloseWindows()
        {
            try
            {
                TooltipView.Close();
            }
            catch (Exception)
            {
            }
            foreach (ContainerWindow window in Resources.FindObjectsOfTypeAll(typeof(ContainerWindow)))
            {
                try
                {
                    window.Close();
                }
                catch (Exception)
                {
                }
            }
            Object[] objArray3 = Resources.FindObjectsOfTypeAll(typeof(EditorWindow));
            if (objArray3.Length != 0)
            {
                string str = string.Empty;
                foreach (EditorWindow window2 in objArray3)
                {
                    str = str + "\n" + window2.GetType().Name;
                    Object.DestroyImmediate(window2, true);
                }
                Debug.LogError("Failed to destroy editor windows: #" + objArray3.Length + str);
            }
            Object[] objArray5 = Resources.FindObjectsOfTypeAll(typeof(View));
            if (objArray5.Length != 0)
            {
                string str2 = string.Empty;
                foreach (View view in objArray5)
                {
                    str2 = str2 + "\n" + view.GetType().Name;
                    Object.DestroyImmediate(view, true);
                }
                Debug.LogError("Failed to destroy views: #" + objArray5.Length + str2);
            }
        }

        public static void DeleteGUI()
        {
            Rect screenPosition = FindMainWindow().screenPosition;
            EditorWindow.GetWindowWithRect<DeleteWindowLayout>(new Rect(screenPosition.xMax - 180f, screenPosition.y + 20f, 200f, 150f), true, "Delete Window Layout").m_Parent.window.m_DontSaveToLayout = true;
        }

        public static void EnsureMainWindowHasBeenLoaded()
        {
            if (Resources.FindObjectsOfTypeAll(typeof(MainWindow)).Length == 0)
            {
                MainWindow.MakeMain();
            }
        }

        internal static EditorWindow FindEditorWindowOfType(Type type)
        {
            Object[] objArray = Resources.FindObjectsOfTypeAll(type);
            if (objArray.Length > 0)
            {
                return (objArray[0] as EditorWindow);
            }
            return null;
        }

        [DebuggerHidden]
        private static IEnumerable<T> FindEditorWindowsOfType<T>() where T: class
        {
            return new <FindEditorWindowsOfType>c__Iterator6<T> { $PC = -2 };
        }

        internal static void FindFirstGameViewAndSetToMaximizeOnPlay()
        {
            GameView view = (GameView) FindEditorWindowOfType(typeof(GameView));
            if (view != null)
            {
                view.maximizeOnPlay = true;
            }
        }

        internal static MainWindow FindMainWindow()
        {
            Object[] objArray = Resources.FindObjectsOfTypeAll(typeof(MainWindow));
            if (objArray.Length == 0)
            {
                Debug.LogError("No Main Window found!");
                return null;
            }
            return (objArray[0] as MainWindow);
        }

        internal static EditorWindow GetMaximizedWindow()
        {
            Object[] objArray = Resources.FindObjectsOfTypeAll(typeof(MaximizedHostView));
            if (objArray.Length != 0)
            {
                MaximizedHostView view = objArray[0] as MaximizedHostView;
                if (view.actualView != null)
                {
                    return view.actualView;
                }
            }
            return null;
        }

        internal static bool IsMaximized(EditorWindow window)
        {
            return (window.m_Parent is MaximizedHostView);
        }

        private static void LoadDefaultLayout()
        {
            InternalEditorUtility.LoadDefaultLayout();
        }

        public static bool LoadWindowLayout(string path, bool newProjectLayoutWasCreated)
        {
            Rect position = new Rect();
            foreach (ContainerWindow window in Resources.FindObjectsOfTypeAll(typeof(ContainerWindow)))
            {
                if (window.showMode == ShowMode.MainWindow)
                {
                    position = window.position;
                }
            }
            try
            {
                ContainerWindow.SetFreezeDisplay(true);
                CloseWindows();
                Object[] objArray3 = InternalEditorUtility.LoadSerializedFileAndForget(path);
                ContainerWindow window2 = null;
                ContainerWindow window3 = null;
                foreach (Object obj2 in objArray3)
                {
                    ContainerWindow window4 = obj2 as ContainerWindow;
                    if ((window4 != null) && (window4.showMode == ShowMode.MainWindow))
                    {
                        window3 = window4;
                        if (position.width != 0.0)
                        {
                            window2 = window4;
                            window2.position = position;
                        }
                    }
                }
                int num3 = 0;
                foreach (Object obj3 in objArray3)
                {
                    if (obj3 == null)
                    {
                        Debug.LogError("Error while reading window layout: window #" + num3 + " is null");
                        throw new Exception();
                    }
                    if (obj3.GetType() == null)
                    {
                        Debug.LogError(string.Concat(new object[] { "Error while reading window layout: window #", num3, " type is null, instanceID=", obj3.GetInstanceID() }));
                        throw new Exception();
                    }
                    if (newProjectLayoutWasCreated)
                    {
                        MethodInfo method = obj3.GetType().GetMethod("OnNewProjectLayoutWasCreated", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                        if (method != null)
                        {
                            method.Invoke(obj3, null);
                        }
                    }
                    num3++;
                }
                if (window2 != null)
                {
                    window2.position = position;
                    window2.OnResize();
                }
                if (window3 == null)
                {
                    Debug.LogError("Error while reading window layout: no main window found");
                    throw new Exception();
                }
                window3.Show(window3.showMode, true, true);
                foreach (Object obj4 in objArray3)
                {
                    EditorWindow window5 = obj4 as EditorWindow;
                    if (window5 != null)
                    {
                        window5.minSize = window5.minSize;
                    }
                    ContainerWindow window6 = obj4 as ContainerWindow;
                    if ((window6 != null) && (window6 != window3))
                    {
                        window6.Show(window6.showMode, true, true);
                    }
                }
                GameView maximizedWindow = GetMaximizedWindow() as GameView;
                if ((maximizedWindow != null) && maximizedWindow.maximizeOnPlay)
                {
                    Unmaximize(maximizedWindow);
                }
            }
            catch (Exception exception)
            {
                Debug.LogError("Failed to load window layout: " + exception);
                switch (EditorUtility.DisplayDialogComplex("Failed to load window layout", "This can happen if layout contains custom windows and there are compile errors in the project.", "Load Default Layout", "Quit", "Revert Factory Settings"))
                {
                    case 0:
                        LoadDefaultLayout();
                        break;

                    case 1:
                        EditorApplication.Exit(0);
                        break;

                    case 2:
                        RevertFactorySettings();
                        break;
                }
                return false;
            }
            finally
            {
                ContainerWindow.SetFreezeDisplay(false);
                if (Path.GetExtension(path) == ".wlt")
                {
                    Toolbar.lastLoadedLayoutName = Path.GetFileNameWithoutExtension(path);
                }
                else
                {
                    Toolbar.lastLoadedLayoutName = null;
                }
            }
            return true;
        }

        public static void Maximize(EditorWindow win)
        {
            View rootSplit = MaximizePrepare(win);
            if (rootSplit != null)
            {
                MaximizePresent(win, rootSplit);
            }
        }

        internal static void MaximizeKeyHandler()
        {
            if ((s_MaximizeKey.activated || (Event.current.type == EditorGUIUtility.magnifyGestureEventType)) && (GUIUtility.hotControl == 0))
            {
                EventType type = Event.current.type;
                Event.current.Use();
                EditorWindow mouseOverWindow = EditorWindow.mouseOverWindow;
                if ((mouseOverWindow != null) && !(mouseOverWindow is PreviewWindow))
                {
                    if (type == EditorGUIUtility.magnifyGestureEventType)
                    {
                        if (Event.current.delta.x < -0.05)
                        {
                            if (IsMaximized(mouseOverWindow))
                            {
                                Unmaximize(mouseOverWindow);
                            }
                        }
                        else if ((Event.current.delta.x > 0.05) && !IsMaximized(mouseOverWindow))
                        {
                            Maximize(mouseOverWindow);
                        }
                    }
                    else if (IsMaximized(mouseOverWindow))
                    {
                        Unmaximize(mouseOverWindow);
                    }
                    else
                    {
                        Maximize(mouseOverWindow);
                    }
                }
            }
        }

        public static View MaximizePrepare(EditorWindow win)
        {
            View parent = win.m_Parent.parent;
            View splitview = parent;
            while ((parent != null) && (parent is SplitView))
            {
                splitview = parent;
                parent = parent.parent;
            }
            DockArea area = win.m_Parent as DockArea;
            if (area == null)
            {
                return null;
            }
            if (parent == null)
            {
                return null;
            }
            MainWindow window = splitview.parent as MainWindow;
            if (window == null)
            {
                return null;
            }
            if (win.m_Parent.window == null)
            {
                return null;
            }
            int index = area.m_Panes.IndexOf(win);
            if (index == -1)
            {
                return null;
            }
            area.selected = index;
            SaveSplitViewAndChildren(splitview, win, Path.Combine(layoutsProjectPath, "CurrentMaximizeLayout.dwlt"));
            area.m_Panes[index] = null;
            MaximizedHostView child = ScriptableObject.CreateInstance<MaximizedHostView>();
            int idx = parent.IndexOfChild(splitview);
            Rect position = splitview.position;
            parent.RemoveChild(splitview);
            parent.AddChild(child, idx);
            child.position = position;
            child.actualView = win;
            return splitview;
        }

        public static void MaximizePresent(EditorWindow win, View rootSplit)
        {
            ContainerWindow.SetFreezeDisplay(true);
            Object.DestroyImmediate(rootSplit, true);
            win.Focus();
            CheckWindowConsistency();
            win.m_Parent.window.DisplayAllViews();
            win.m_Parent.MakeVistaDWMHappyDance();
            ContainerWindow.SetFreezeDisplay(false);
        }

        private static void RevertFactorySettings()
        {
            InternalEditorUtility.RevertFactoryLayoutSettings(true);
        }

        internal static void SaveCurrentFocusedWindowInSameDock(EditorWindow windowToBeFocused)
        {
            if ((windowToBeFocused.m_Parent != null) && (windowToBeFocused.m_Parent is DockArea))
            {
                DockArea parent = windowToBeFocused.m_Parent as DockArea;
                EditorWindow actualView = parent.actualView;
                if (actualView != null)
                {
                    WindowFocusState.instance.m_LastWindowTypeInSameDock = actualView.GetType().ToString();
                }
            }
        }

        public static void SaveGUI()
        {
            Rect screenPosition = FindMainWindow().screenPosition;
            EditorWindow.GetWindowWithRect<UnityEditor.SaveWindowLayout>(new Rect(screenPosition.xMax - 180f, screenPosition.y + 20f, 200f, 48f), true, "Save Window Layout").m_Parent.window.m_DontSaveToLayout = true;
        }

        public static void SaveSplitViewAndChildren(View splitview, EditorWindow win, string path)
        {
            ArrayList list = new ArrayList();
            AddSplitViewAndChildrenRecurse(splitview, list);
            list.Remove(splitview);
            list.Remove(win);
            list.Insert(0, splitview);
            list.Insert(1, win);
            InternalEditorUtility.SaveToSerializedFileAndForget(list.ToArray(typeof(Object)) as Object[], path, false);
        }

        public static void SaveWindowLayout(string path)
        {
            TooltipView.Close();
            ArrayList list = new ArrayList();
            Object[] objArray = Resources.FindObjectsOfTypeAll(typeof(EditorWindow));
            Object[] objArray2 = Resources.FindObjectsOfTypeAll(typeof(ContainerWindow));
            Object[] objArray3 = Resources.FindObjectsOfTypeAll(typeof(View));
            foreach (ContainerWindow window in objArray2)
            {
                if (!window.m_DontSaveToLayout)
                {
                    list.Add(window);
                }
            }
            foreach (View view in objArray3)
            {
                if ((view.window == null) || !view.window.m_DontSaveToLayout)
                {
                    list.Add(view);
                }
            }
            foreach (EditorWindow window2 in objArray)
            {
                if (((window2.m_Parent == null) || (window2.m_Parent.window == null)) || !window2.m_Parent.window.m_DontSaveToLayout)
                {
                    list.Add(window2);
                }
            }
            InternalEditorUtility.SaveToSerializedFileAndForget(list.ToArray(typeof(Object)) as Object[], path, false);
        }

        internal static EditorWindow ShowAppropriateViewOnEnterExitPlaymode(bool entering)
        {
            GameView view;
            if (WindowFocusState.instance.m_CurrentlyInPlayMode == entering)
            {
                return null;
            }
            WindowFocusState.instance.m_CurrentlyInPlayMode = entering;
            EditorWindow window = null;
            EditorWindow maximizedWindow = GetMaximizedWindow();
            if (entering)
            {
                WindowFocusState.instance.m_WasMaximizedBeforePlay = maximizedWindow != null;
                if (maximizedWindow != null)
                {
                    return maximizedWindow;
                }
            }
            else if (WindowFocusState.instance.m_WasMaximizedBeforePlay)
            {
                return maximizedWindow;
            }
            if (maximizedWindow != null)
            {
                Unmaximize(maximizedWindow);
            }
            window = TryFocusAppropriateWindow(entering);
            if (window != null)
            {
                return window;
            }
            if (!entering)
            {
                return window;
            }
            EditorWindow window3 = FindEditorWindowOfType(typeof(SceneView));
            if ((window3 != null) && (window3.m_Parent is DockArea))
            {
                DockArea parent = window3.m_Parent as DockArea;
                if (parent != null)
                {
                    WindowFocusState.instance.m_LastWindowTypeInSameDock = window3.GetType().ToString();
                    view = ScriptableObject.CreateInstance<GameView>();
                    parent.AddTab(view);
                    return view;
                }
            }
            view = ScriptableObject.CreateInstance<GameView>();
            view.Show(true);
            view.Focus();
            return view;
        }

        private static void ShowWindowImmediate(EditorWindow win)
        {
            win.Show(true);
        }

        internal static EditorWindow TryFocusAppropriateWindow(bool enteringPlaymode)
        {
            if (enteringPlaymode)
            {
                GameView windowToBeFocused = (GameView) FindEditorWindowOfType(typeof(GameView));
                if (windowToBeFocused != null)
                {
                    SaveCurrentFocusedWindowInSameDock(windowToBeFocused);
                    windowToBeFocused.Focus();
                }
                return windowToBeFocused;
            }
            EditorWindow window = TryGetLastFocusedWindowInSameDock();
            if (window != null)
            {
                window.ShowTab();
            }
            return window;
        }

        internal static EditorWindow TryGetLastFocusedWindowInSameDock()
        {
            Type type = null;
            string lastWindowTypeInSameDock = WindowFocusState.instance.m_LastWindowTypeInSameDock;
            if (lastWindowTypeInSameDock != string.Empty)
            {
                type = Type.GetType(lastWindowTypeInSameDock);
            }
            GameView view = FindEditorWindowOfType(typeof(GameView)) as GameView;
            if (((type != null) && (view != null)) && ((view.m_Parent != null) && (view.m_Parent is DockArea)))
            {
                object[] objArray = Resources.FindObjectsOfTypeAll(type);
                DockArea parent = view.m_Parent as DockArea;
                for (int i = 0; i < objArray.Length; i++)
                {
                    EditorWindow window = objArray[i] as EditorWindow;
                    if ((window != null) && (window.m_Parent == parent))
                    {
                        return window;
                    }
                }
            }
            return null;
        }

        public static void Unmaximize(EditorWindow win)
        {
            HostView parent = win.m_Parent;
            if (parent == null)
            {
                Debug.LogError("Host view was not found");
                RevertFactorySettings();
            }
            else
            {
                Object[] objArray = InternalEditorUtility.LoadSerializedFileAndForget(Path.Combine(layoutsProjectPath, "CurrentMaximizeLayout.dwlt"));
                if (objArray.Length < 2)
                {
                    Debug.Log("Maximized serialized file backup not found");
                    RevertFactorySettings();
                }
                else
                {
                    SplitView child = objArray[0] as SplitView;
                    EditorWindow item = objArray[1] as EditorWindow;
                    if (child == null)
                    {
                        Debug.Log("Maximization failed because the root split view was not found");
                        RevertFactorySettings();
                    }
                    else
                    {
                        ContainerWindow window = win.m_Parent.window;
                        if (window == null)
                        {
                            Debug.Log("Maximization failed because the root split view has no container window");
                            RevertFactorySettings();
                        }
                        else
                        {
                            try
                            {
                                ContainerWindow.SetFreezeDisplay(true);
                                if (parent.parent == null)
                                {
                                    throw new Exception();
                                }
                                int idx = parent.parent.IndexOfChild(parent);
                                Rect position = parent.position;
                                View view3 = parent.parent;
                                view3.RemoveChild(idx);
                                view3.AddChild(child, idx);
                                child.position = position;
                                DockArea area = item.m_Parent as DockArea;
                                int index = area.m_Panes.IndexOf(item);
                                parent.actualView = null;
                                win.m_Parent = null;
                                area.AddTab(index, win);
                                area.RemoveTab(item);
                                Object.DestroyImmediate(item);
                                foreach (Object obj2 in objArray)
                                {
                                    EditorWindow window3 = obj2 as EditorWindow;
                                    if (window3 != null)
                                    {
                                        window3.MakeParentsSettingsMatchMe();
                                    }
                                }
                                view3.Initialize(view3.window);
                                view3.position = view3.position;
                                child.Reflow();
                                Object.DestroyImmediate(parent);
                                win.Focus();
                                window.DisplayAllViews();
                                win.m_Parent.MakeVistaDWMHappyDance();
                            }
                            catch (Exception exception)
                            {
                                Debug.Log("Maximization failed: " + exception);
                                RevertFactorySettings();
                            }
                            try
                            {
                                if (((Application.platform == RuntimePlatform.OSXEditor) && SystemInfo.operatingSystem.Contains("10.7")) && SystemInfo.graphicsDeviceVendor.Contains("ATI"))
                                {
                                    foreach (GUIView view4 in Resources.FindObjectsOfTypeAll(typeof(GUIView)))
                                    {
                                        view4.Repaint();
                                    }
                                }
                            }
                            finally
                            {
                                ContainerWindow.SetFreezeDisplay(false);
                            }
                        }
                    }
                }
            }
        }

        internal static string layoutsPreferencesPath
        {
            get
            {
                return (InternalEditorUtility.unityPreferencesFolder + "/Layouts");
            }
        }

        internal static string layoutsProjectPath
        {
            get
            {
                return (Directory.GetCurrentDirectory() + "/Library");
            }
        }

        [CompilerGenerated]
        private sealed class <FindEditorWindowsOfType>c__Iterator6<T> : IDisposable, IEnumerator, IEnumerable, IEnumerable<T>, IEnumerator<T> where T: class
        {
            internal T $current;
            internal int $PC;
            internal Object[] <$s_1061>__0;
            internal int <$s_1062>__1;
            internal Object <obj>__2;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<$s_1061>__0 = Resources.FindObjectsOfTypeAll(typeof(T));
                        this.<$s_1062>__1 = 0;
                        goto Label_009A;

                    case 1:
                        break;

                    default:
                        goto Label_00B4;
                }
            Label_008C:
                this.<$s_1062>__1++;
            Label_009A:
                if (this.<$s_1062>__1 < this.<$s_1061>__0.Length)
                {
                    this.<obj>__2 = this.<$s_1061>__0[this.<$s_1062>__1];
                    if (this.<obj>__2 is T)
                    {
                        this.$current = this.<obj>__2 as T;
                        this.$PC = 1;
                        return true;
                    }
                    goto Label_008C;
                }
                this.$PC = -1;
            Label_00B4:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new WindowLayout.<FindEditorWindowsOfType>c__Iterator6<T>();
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<T>.GetEnumerator();
            }

            T IEnumerator<T>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }
    }
}

