namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UnityEditor.SceneManagement;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [EditorWindowTitle(title="Hierarchy", useTypeNameAsIconName=true)]
    internal class SceneHierarchyWindow : SearchableEditorWindow, IHasCustomMenu
    {
        [CompilerGenerated]
        private static Func<Transform, GameObject> <>f__am$cache13;
        [CompilerGenerated]
        private static Func<Transform, GameObject> <>f__am$cache14;
        [CompilerGenerated]
        private static Func<int, Scene> <>f__am$cache15;
        [CompilerGenerated]
        private static Func<Scene, bool> <>f__am$cache16;
        private const int kInvalidSceneHandle = 0;
        private bool m_AllowAlphaNumericalSort;
        [SerializeField]
        private int m_CurrenRootInstanceID;
        [SerializeField]
        private string m_CurrentSortMethod = string.Empty;
        private bool m_Debug;
        [NonSerialized]
        private bool m_DidSelectSearchResult;
        [NonSerialized]
        private bool m_FrameOnSelectionSync;
        [NonSerialized]
        private int m_LastFramedID = -1;
        [NonSerialized]
        private double m_LastUserInteractionTime;
        [SerializeField]
        private bool m_Locked;
        [NonSerialized]
        private bool m_SelectionSyncNeeded;
        private Dictionary<string, BaseHierarchySort> m_SortingObjects;
        private TreeView m_TreeView;
        private int m_TreeViewKeyboardControlID;
        [NonSerialized]
        private bool m_TreeViewReloadNeeded;
        [SerializeField]
        private TreeViewState m_TreeViewState;
        public static bool s_Debug = SessionState.GetBool("HierarchyWindowDebug", false);
        private static SceneHierarchyWindow s_LastInteractedHierarchy;
        private static List<SceneHierarchyWindow> s_SceneHierarchyWindow = new List<SceneHierarchyWindow>();
        private static Styles s_Styles;
        private const float toolbarHeight = 17f;

        private void AddCreateGameObjectItemsToMenu(GenericMenu menu, Object[] context, bool includeCreateEmptyChild, bool includeGameObjectInPath, int targetSceneHandle)
        {
            foreach (string str in Unsupported.GetSubmenus("GameObject"))
            {
                Object[] temporaryContext = context;
                if (includeCreateEmptyChild || (str.ToLower() != "GameObject/Create Empty Child".ToLower()))
                {
                    if (str.EndsWith("..."))
                    {
                        temporaryContext = null;
                    }
                    if (str.ToLower() == "GameObject/Center On Children".ToLower())
                    {
                        return;
                    }
                    string replacementMenuString = str;
                    if (!includeGameObjectInPath)
                    {
                        replacementMenuString = str.Substring(11);
                    }
                    MenuUtils.ExtractMenuItemWithPath(str, menu, replacementMenuString, temporaryContext, targetSceneHandle, new Action<string, Object[], int>(this.BeforeCreateGameObjectMenuItemWasExecuted), new Action<string, Object[], int>(this.AfterCreateGameObjectMenuItemWasExecuted));
                }
            }
        }

        public virtual void AddItemsToMenu(GenericMenu menu)
        {
            if (Unsupported.IsDeveloperBuild())
            {
                menu.AddItem(new GUIContent("DEVELOPER/Toggle DebugMode"), false, new GenericMenu.MenuFunction(SceneHierarchyWindow.ToggleDebugMode));
            }
        }

        private void AfterCreateGameObjectMenuItemWasExecuted(string menuPath, Object[] contextObjects, int userData)
        {
            EditorSceneManager.SetTargetSceneForNewGameObjects(0);
            if (this.m_Locked)
            {
                this.m_FrameOnSelectionSync = true;
            }
        }

        private void Awake()
        {
            base.m_HierarchyType = HierarchyType.GameObjects;
            if (this.m_TreeViewState != null)
            {
                this.m_TreeViewState.OnAwake();
            }
        }

        private void BeforeCreateGameObjectMenuItemWasExecuted(string menuPath, Object[] contextObjects, int userData)
        {
            int sceneHandle = userData;
            EditorSceneManager.SetTargetSceneForNewGameObjects(sceneHandle);
        }

        private void CloseSelectedScenes(bool removeScenes)
        {
            List<int> selectedScenes = this.GetSelectedScenes();
            if (EditorSceneManager.SaveModifiedScenesIfUserWantsTo(this.GetModifiedScenes(selectedScenes)))
            {
                foreach (int num in selectedScenes)
                {
                    EditorSceneManager.CloseScene(EditorSceneManager.GetSceneByHandle(num), removeScenes);
                }
                EditorApplication.RequestRepaintAllViews();
            }
        }

        private void ContextClickOutsideItems()
        {
            Event.current.Use();
            GenericMenu menu = new GenericMenu();
            this.CreateGameObjectContextClick(menu, 0);
            menu.ShowAsContext();
        }

        private void CopyGO()
        {
            Unsupported.CopyGameObjectsToPasteboard();
        }

        private void CreateGameObjectContextClick(GenericMenu menu, int contextClickedItemID)
        {
            menu.AddItem(EditorGUIUtility.TextContent("Copy"), false, new GenericMenu.MenuFunction(this.CopyGO));
            menu.AddItem(EditorGUIUtility.TextContent("Paste"), false, new GenericMenu.MenuFunction(this.PasteGO));
            menu.AddSeparator(string.Empty);
            if (!base.hasSearchFilter && (this.m_TreeViewState.selectedIDs.Count == 1))
            {
                menu.AddItem(EditorGUIUtility.TextContent("Rename"), false, new GenericMenu.MenuFunction(this.RenameGO));
            }
            else
            {
                menu.AddDisabledItem(EditorGUIUtility.TextContent("Rename"));
            }
            menu.AddItem(EditorGUIUtility.TextContent("Duplicate"), false, new GenericMenu.MenuFunction(this.DuplicateGO));
            menu.AddItem(EditorGUIUtility.TextContent("Delete"), false, new GenericMenu.MenuFunction(this.DeleteGO));
            menu.AddSeparator(string.Empty);
            bool flag = false;
            if (this.m_TreeViewState.selectedIDs.Count == 1)
            {
                GameObjectTreeViewItem item = this.treeView.FindNode(this.m_TreeViewState.selectedIDs[0]) as GameObjectTreeViewItem;
                if (item != null)
                {
                    <CreateGameObjectContextClick>c__AnonStorey3F storeyf = new <CreateGameObjectContextClick>c__AnonStorey3F {
                        prefab = PrefabUtility.GetPrefabParent(item.objectPPTR)
                    };
                    if (storeyf.prefab != null)
                    {
                        menu.AddItem(EditorGUIUtility.TextContent("Select Prefab"), false, new GenericMenu.MenuFunction(storeyf.<>m__63));
                        flag = true;
                    }
                }
            }
            if (!flag)
            {
                menu.AddDisabledItem(EditorGUIUtility.TextContent("Select Prefab"));
            }
            menu.AddSeparator(string.Empty);
            if (<>f__am$cache13 == null)
            {
                <>f__am$cache13 = t => t.gameObject;
            }
            this.AddCreateGameObjectItemsToMenu(menu, Selection.transforms.Select<Transform, GameObject>(<>f__am$cache13).ToArray<GameObject>(), false, false, 0);
            menu.ShowAsContext();
        }

        private void CreateGameObjectPopup()
        {
            Rect position = GUILayoutUtility.GetRect(s_Styles.createContent, EditorStyles.toolbarDropDown, null);
            if (Event.current.type == EventType.Repaint)
            {
                EditorStyles.toolbarDropDown.Draw(position, s_Styles.createContent, false, false, false, false);
            }
            if ((Event.current.type == EventType.MouseDown) && position.Contains(Event.current.mousePosition))
            {
                GUIUtility.hotControl = 0;
                GenericMenu menu = new GenericMenu();
                this.AddCreateGameObjectItemsToMenu(menu, null, true, false, 0);
                menu.DropDown(position);
                Event.current.Use();
            }
        }

        private void CreateMultiSceneHeaderContextClick(GenericMenu menu, int contextClickedItemID)
        {
            Scene sceneByHandle = EditorSceneManager.GetSceneByHandle(contextClickedItemID);
            if (!IsSceneHeaderInHierarchyWindow(sceneByHandle))
            {
                Debug.LogError("Context clicked item is not a scene");
            }
            else
            {
                if (sceneByHandle.isLoaded)
                {
                    menu.AddItem(EditorGUIUtility.TextContent("Set Active Scene"), false, new GenericMenu.MenuFunction2(this.SetSceneActive), contextClickedItemID);
                    menu.AddSeparator(string.Empty);
                }
                if (sceneByHandle.isLoaded)
                {
                    if (!EditorApplication.isPlaying)
                    {
                        menu.AddItem(EditorGUIUtility.TextContent("Save Scene"), false, new GenericMenu.MenuFunction2(this.SaveSelectedScenes), contextClickedItemID);
                        menu.AddItem(EditorGUIUtility.TextContent("Save Scene As"), false, new GenericMenu.MenuFunction2(this.SaveSceneAs), contextClickedItemID);
                        menu.AddItem(EditorGUIUtility.TextContent("Save All"), false, new GenericMenu.MenuFunction2(this.SaveAllScenes), contextClickedItemID);
                    }
                    else
                    {
                        menu.AddDisabledItem(EditorGUIUtility.TextContent("Save Scene"));
                        menu.AddDisabledItem(EditorGUIUtility.TextContent("Save Scene As"));
                        menu.AddDisabledItem(EditorGUIUtility.TextContent("Save All"));
                    }
                    menu.AddSeparator(string.Empty);
                }
                bool flag = EditorSceneManager.loadedSceneCount != this.GetNumLoadedScenesInSelection();
                if (sceneByHandle.isLoaded)
                {
                    if ((flag && !EditorApplication.isPlaying) && !string.IsNullOrEmpty(sceneByHandle.path))
                    {
                        menu.AddItem(EditorGUIUtility.TextContent("Unload Scene"), false, new GenericMenu.MenuFunction2(this.UnloadSelectedScenes), contextClickedItemID);
                    }
                    else
                    {
                        menu.AddDisabledItem(EditorGUIUtility.TextContent("Unload Scene"));
                    }
                }
                else if (!EditorApplication.isPlaying)
                {
                    menu.AddItem(EditorGUIUtility.TextContent("Load Scene"), false, new GenericMenu.MenuFunction2(this.LoadSelectedScenes), contextClickedItemID);
                }
                else
                {
                    menu.AddDisabledItem(EditorGUIUtility.TextContent("Load Scene"));
                }
                bool flag4 = this.GetSelectedScenes().Count == SceneManager.sceneCount;
                if ((flag && !flag4) && !EditorApplication.isPlaying)
                {
                    menu.AddItem(EditorGUIUtility.TextContent("Remove Scene"), false, new GenericMenu.MenuFunction2(this.RemoveSelectedScenes), contextClickedItemID);
                }
                else
                {
                    menu.AddDisabledItem(EditorGUIUtility.TextContent("Remove Scene"));
                }
                menu.AddSeparator(string.Empty);
                if (!string.IsNullOrEmpty(sceneByHandle.path))
                {
                    menu.AddItem(EditorGUIUtility.TextContent("Select Scene Asset"), false, new GenericMenu.MenuFunction2(this.SelectSceneAsset), contextClickedItemID);
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent("Select Scene Asset"));
                }
                if (sceneByHandle.isLoaded)
                {
                    menu.AddSeparator(string.Empty);
                    if (<>f__am$cache14 == null)
                    {
                        <>f__am$cache14 = t => t.gameObject;
                    }
                    this.AddCreateGameObjectItemsToMenu(menu, Selection.transforms.Select<Transform, GameObject>(<>f__am$cache14).ToArray<GameObject>(), false, true, sceneByHandle.handle);
                }
            }
        }

        private void DeleteGO()
        {
            Unsupported.DeleteGameObjectSelection();
        }

        private void DetectUserInteraction()
        {
            Event current = Event.current;
            if ((current.type != EventType.Layout) && (current.type != EventType.Repaint))
            {
                this.m_LastUserInteractionTime = EditorApplication.timeSinceStartup;
            }
        }

        public void DirtySortingMethods()
        {
            this.m_AllowAlphaNumericalSort = EditorPrefs.GetBool("AllowAlphaNumericHierarchy", false);
            this.SetUpSortMethodLists();
            this.treeView.SetSelection(this.treeView.GetSelection(), true);
            this.treeView.ReloadData();
        }

        private float DoSearchResultPathGUI()
        {
            if (!base.hasSearchFilter)
            {
                return 0f;
            }
            GUILayout.FlexibleSpace();
            Rect rect = EditorGUILayout.BeginVertical(EditorStyles.inspectorBig, new GUILayoutOption[0]);
            GUILayout.Label("Path:", new GUILayoutOption[0]);
            if (this.m_TreeView.HasSelection())
            {
                int instanceID = this.m_TreeView.GetSelection()[0];
                IHierarchyProperty property = new HierarchyProperty(HierarchyType.GameObjects);
                property.Find(instanceID, null);
                if (property.isValid)
                {
                    do
                    {
                        EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                        GUILayout.Label(property.icon, new GUILayoutOption[0]);
                        GUILayout.Label(property.name, new GUILayoutOption[0]);
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndHorizontal();
                    }
                    while (property.Parent());
                }
            }
            EditorGUILayout.EndVertical();
            GUILayout.Space(0f);
            return rect.height;
        }

        private void DoToolbar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
            this.CreateGameObjectPopup();
            GUILayout.Space(6f);
            if (s_Debug)
            {
                int num;
                int num2;
                this.m_TreeView.gui.GetFirstAndLastRowVisible(out num, out num2);
                GUILayout.Label(string.Format("{0} ({1}, {2})", this.m_TreeView.data.rowCount, num, num2), EditorStyles.miniLabel, new GUILayoutOption[0]);
                GUILayout.Space(6f);
            }
            GUILayout.FlexibleSpace();
            Event current = Event.current;
            if ((base.hasSearchFilterFocus && (current.type == EventType.KeyDown)) && ((current.keyCode == KeyCode.DownArrow) || (current.keyCode == KeyCode.UpArrow)))
            {
                GUIUtility.keyboardControl = this.m_TreeViewKeyboardControlID;
                if (this.treeView.IsLastClickedPartOfRows())
                {
                    this.treeView.Frame(this.treeView.state.lastClickedID, true, false);
                    this.m_DidSelectSearchResult = !string.IsNullOrEmpty(base.m_SearchFilter);
                }
                else
                {
                    this.treeView.OffsetSelection(1);
                }
                current.Use();
            }
            base.SearchFieldGUI();
            GUILayout.Space(6f);
            if (this.hasSortMethods)
            {
                if (Application.isPlaying && ((GameObjectTreeViewDataSource) this.treeView.data).isFetchAIssue)
                {
                    GUILayout.Toggle(false, s_Styles.fetchWarning, s_Styles.MiniButton, new GUILayoutOption[0]);
                }
                this.SortMethodsDropDown();
            }
            GUILayout.EndHorizontal();
        }

        private void DoTreeView(float searchPathHeight)
        {
            Rect treeViewRect = this.treeViewRect;
            treeViewRect.height -= searchPathHeight;
            this.treeView.OnGUI(treeViewRect, this.m_TreeViewKeyboardControlID);
        }

        private void DuplicateGO()
        {
            Unsupported.DuplicateGameObjectsUsingPasteboard();
        }

        private void ExecuteCommands()
        {
            Event current = Event.current;
            if ((current.type == EventType.ExecuteCommand) || (current.type == EventType.ValidateCommand))
            {
                bool flag = current.type == EventType.ExecuteCommand;
                if ((current.commandName == "Delete") || (current.commandName == "SoftDelete"))
                {
                    if (flag)
                    {
                        this.DeleteGO();
                    }
                    current.Use();
                    GUIUtility.ExitGUI();
                }
                else if (current.commandName == "Duplicate")
                {
                    if (flag)
                    {
                        this.DuplicateGO();
                    }
                    current.Use();
                    GUIUtility.ExitGUI();
                }
                else if (current.commandName == "Copy")
                {
                    if (flag)
                    {
                        this.CopyGO();
                    }
                    current.Use();
                    GUIUtility.ExitGUI();
                }
                else if (current.commandName == "Paste")
                {
                    if (flag)
                    {
                        this.PasteGO();
                    }
                    current.Use();
                    GUIUtility.ExitGUI();
                }
                else if (current.commandName == "SelectAll")
                {
                    if (flag)
                    {
                        this.SelectAll();
                    }
                    current.Use();
                    GUIUtility.ExitGUI();
                }
                else if (current.commandName == "FrameSelected")
                {
                    if (current.type == EventType.ExecuteCommand)
                    {
                        this.FrameObjectPrivate(Selection.activeInstanceID, true, true, true);
                    }
                    current.Use();
                    GUIUtility.ExitGUI();
                }
                else if (current.commandName == "Find")
                {
                    if (current.type == EventType.ExecuteCommand)
                    {
                        base.FocusSearchField();
                    }
                    current.Use();
                }
            }
        }

        public void FrameObject(int instanceID, bool ping)
        {
            this.FrameObjectPrivate(instanceID, true, ping, true);
        }

        private void FrameObjectPrivate(int instanceID, bool frame, bool ping, bool animatedFraming)
        {
            if (instanceID != 0)
            {
                if (this.m_LastFramedID != instanceID)
                {
                    this.treeView.EndPing();
                }
                this.SetSearchFilter(string.Empty, SearchableEditorWindow.SearchMode.All, true);
                this.m_LastFramedID = instanceID;
                this.treeView.Frame(instanceID, frame, ping, animatedFraming);
                this.FrameObjectPrivate(InternalEditorUtility.GetGameObjectInstanceIDFromComponent(instanceID), frame, ping, animatedFraming);
            }
        }

        public static List<SceneHierarchyWindow> GetAllSceneHierarchyWindows()
        {
            return s_SceneHierarchyWindow;
        }

        public Object[] GetCurrentVisibleObjects()
        {
            List<TreeViewItem> rows = this.m_TreeView.data.GetRows();
            Object[] objArray = new Object[rows.Count];
            for (int i = 0; i < rows.Count; i++)
            {
                objArray[i] = ((GameObjectTreeViewItem) rows[i]).objectPPTR;
            }
            return objArray;
        }

        private Scene[] GetModifiedScenes(List<int> handles)
        {
            if (<>f__am$cache15 == null)
            {
                <>f__am$cache15 = handle => EditorSceneManager.GetSceneByHandle(handle);
            }
            if (<>f__am$cache16 == null)
            {
                <>f__am$cache16 = scene => scene.isDirty;
            }
            return handles.Select<int, Scene>(<>f__am$cache15).Where<Scene>(<>f__am$cache16).ToArray<Scene>();
        }

        private string GetNameForType(Type type)
        {
            return type.Name;
        }

        private int GetNumLoadedScenesInSelection()
        {
            int num = 0;
            foreach (int num2 in this.GetSelectedScenes())
            {
                if (EditorSceneManager.GetSceneByHandle(num2).isLoaded)
                {
                    num++;
                }
            }
            return num;
        }

        private List<int> GetSelectedGameObjects()
        {
            List<int> list = new List<int>();
            foreach (int num in this.m_TreeView.GetSelection())
            {
                if (!IsSceneHeaderInHierarchyWindow(EditorSceneManager.GetSceneByHandle(num)))
                {
                    list.Add(num);
                }
            }
            return list;
        }

        private List<int> GetSelectedScenes()
        {
            List<int> list = new List<int>();
            foreach (int num in this.m_TreeView.GetSelection())
            {
                if (IsSceneHeaderInHierarchyWindow(EditorSceneManager.GetSceneByHandle(num)))
                {
                    list.Add(num);
                }
            }
            return list;
        }

        private void Init()
        {
            if (this.m_TreeViewState == null)
            {
                this.m_TreeViewState = new TreeViewState();
            }
            this.m_TreeView = new TreeView(this, this.m_TreeViewState);
            this.m_TreeView.itemDoubleClickedCallback = (Action<int>) Delegate.Combine(this.m_TreeView.itemDoubleClickedCallback, new Action<int>(this.TreeViewItemDoubleClicked));
            this.m_TreeView.selectionChangedCallback = (Action<int[]>) Delegate.Combine(this.m_TreeView.selectionChangedCallback, new Action<int[]>(this.TreeViewSelectionChanged));
            this.m_TreeView.onGUIRowCallback = (Action<int, Rect>) Delegate.Combine(this.m_TreeView.onGUIRowCallback, new Action<int, Rect>(this.OnGUIAssetCallback));
            this.m_TreeView.dragEndedCallback = (Action<int[], bool>) Delegate.Combine(this.m_TreeView.dragEndedCallback, new Action<int[], bool>(this.OnDragEndedCallback));
            this.m_TreeView.contextClickItemCallback = (Action<int>) Delegate.Combine(this.m_TreeView.contextClickItemCallback, new Action<int>(this.ItemContextClick));
            this.m_TreeView.contextClickOutsideItemsCallback = (Action) Delegate.Combine(this.m_TreeView.contextClickOutsideItemsCallback, new Action(this.ContextClickOutsideItems));
            this.m_TreeView.deselectOnUnhandledMouseDown = true;
            bool showRootNode = false;
            bool rootNodeIsCollapsable = false;
            GameObjectTreeViewDataSource data = new GameObjectTreeViewDataSource(this.m_TreeView, this.m_CurrenRootInstanceID, showRootNode, rootNodeIsCollapsable);
            GameObjectsTreeViewDragging dragging = new GameObjectsTreeViewDragging(this.m_TreeView);
            GameObjectTreeViewGUI gui = new GameObjectTreeViewGUI(this.m_TreeView, false);
            this.m_TreeView.Init(this.treeViewRect, data, gui, dragging);
            data.searchMode = (int) base.m_SearchMode;
            data.searchString = base.m_SearchFilter;
            this.m_AllowAlphaNumericalSort = EditorPrefs.GetBool("AllowAlphaNumericHierarchy", false) || InternalEditorUtility.inBatchMode;
            this.SetUpSortMethodLists();
            this.m_TreeView.ReloadData();
        }

        public static bool IsSceneHeaderInHierarchyWindow(Scene scene)
        {
            return scene.IsValid();
        }

        private bool IsTreeViewSelectionInSyncWithBackend()
        {
            return ((this.m_TreeView != null) && this.m_TreeView.state.selectedIDs.SequenceEqual<int>(Selection.instanceIDs));
        }

        private void ItemContextClick(int contextClickedItemID)
        {
            Event.current.Use();
            GenericMenu menu = new GenericMenu();
            if (IsSceneHeaderInHierarchyWindow(EditorSceneManager.GetSceneByHandle(contextClickedItemID)))
            {
                this.CreateMultiSceneHeaderContextClick(menu, contextClickedItemID);
            }
            else
            {
                this.CreateGameObjectContextClick(menu, contextClickedItemID);
            }
            menu.ShowAsContext();
        }

        private void LoadSelectedScenes(object userdata)
        {
            foreach (int num in this.GetSelectedScenes())
            {
                Scene sceneByHandle = EditorSceneManager.GetSceneByHandle(num);
                if (!sceneByHandle.isLoaded)
                {
                    EditorSceneManager.OpenScene(sceneByHandle.path, OpenSceneMode.Additive);
                }
            }
            EditorApplication.RequestRepaintAllViews();
        }

        private void OnBecameVisible()
        {
            if (SceneManager.sceneCount > 0)
            {
                this.treeViewReloadNeeded = true;
            }
        }

        public void OnDestroy()
        {
            if (s_LastInteractedHierarchy == this)
            {
                s_LastInteractedHierarchy = null;
                foreach (SceneHierarchyWindow window in s_SceneHierarchyWindow)
                {
                    if (window != this)
                    {
                        s_LastInteractedHierarchy = window;
                    }
                }
            }
        }

        public override void OnDisable()
        {
            EditorApplication.projectWindowChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.projectWindowChanged, new EditorApplication.CallbackFunction(this.ReloadData));
            EditorApplication.searchChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.searchChanged, new EditorApplication.CallbackFunction(this.SearchChanged));
            s_SceneHierarchyWindow.Remove(this);
        }

        private void OnDragEndedCallback(int[] draggedInstanceIds, bool draggedItemsFromOwnTreeView)
        {
            if ((draggedInstanceIds != null) && draggedItemsFromOwnTreeView)
            {
                this.ReloadData();
                this.treeView.SetSelection(draggedInstanceIds, true);
                this.treeView.NotifyListenersThatSelectionChanged();
                base.Repaint();
                GUIUtility.ExitGUI();
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            base.titleContent = base.GetLocalizedTitleContent();
            s_SceneHierarchyWindow.Add(this);
            EditorApplication.projectWindowChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.projectWindowChanged, new EditorApplication.CallbackFunction(this.ReloadData));
            EditorApplication.searchChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.searchChanged, new EditorApplication.CallbackFunction(this.SearchChanged));
            s_LastInteractedHierarchy = this;
        }

        private void OnEvent()
        {
            this.treeView.OnEvent();
        }

        private void OnGUI()
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            this.DetectUserInteraction();
            this.SyncIfNeeded();
            this.m_TreeViewKeyboardControlID = GUIUtility.GetControlID(FocusType.Keyboard);
            this.OnEvent();
            Rect rect = new Rect(0f, 0f, base.position.width, base.position.height);
            Event current = Event.current;
            if ((current.type == EventType.MouseDown) && rect.Contains(current.mousePosition))
            {
                this.treeView.EndPing();
                this.SetAsLastInteractedHierarchy();
            }
            this.DoToolbar();
            float searchPathHeight = this.DoSearchResultPathGUI();
            this.DoTreeView(searchPathHeight);
            this.ExecuteCommands();
        }

        private void OnGUIAssetCallback(int instanceID, Rect rect)
        {
            if (EditorApplication.hierarchyWindowItemOnGUI != null)
            {
                EditorApplication.hierarchyWindowItemOnGUI(instanceID, rect);
            }
        }

        private void OnHierarchyChange()
        {
            if (this.m_TreeView != null)
            {
                this.m_TreeView.EndNameEditing(false);
            }
            this.treeViewReloadNeeded = true;
        }

        private void OnLostFocus()
        {
            this.treeView.EndNameEditing(true);
            EditorGUI.EndEditingActiveTextField();
        }

        private void OnSelectionChange()
        {
            if (!this.IsTreeViewSelectionInSyncWithBackend())
            {
                this.selectionSyncNeeded = true;
            }
            else if (s_Debug)
            {
                Debug.Log("OnSelectionChange: Selection is already in sync so no framing will happen");
            }
        }

        private void PasteGO()
        {
            Unsupported.PasteGameObjectsFromPasteboard();
        }

        public void ReloadData()
        {
            if (this.m_TreeView == null)
            {
                this.Init();
            }
            else
            {
                this.m_TreeView.ReloadData();
            }
        }

        private void RemoveSelectedScenes(object userData)
        {
            this.CloseSelectedScenes(true);
        }

        private void RenameGO()
        {
            this.treeView.BeginNameEditing(0f);
        }

        private void SaveAllScenes(object userdata)
        {
            EditorSceneManager.SaveOpenScenes();
        }

        private void SaveSceneAs(object userdata)
        {
            int handle = (int) userdata;
            Scene sceneByHandle = EditorSceneManager.GetSceneByHandle(handle);
            if (sceneByHandle.isLoaded)
            {
                EditorSceneManager.SaveSceneAs(sceneByHandle);
            }
        }

        private void SaveSelectedScenes(object userdata)
        {
            foreach (int num in this.GetSelectedScenes())
            {
                Scene sceneByHandle = EditorSceneManager.GetSceneByHandle(num);
                if (sceneByHandle.isLoaded)
                {
                    EditorSceneManager.SaveScene(sceneByHandle);
                }
            }
        }

        public void SearchChanged()
        {
            GameObjectTreeViewDataSource data = (GameObjectTreeViewDataSource) this.treeView.data;
            if ((data.searchMode != base.searchMode) || (data.searchString != base.m_SearchFilter))
            {
                data.searchMode = (int) base.searchMode;
                data.searchString = base.m_SearchFilter;
                if (base.m_SearchFilter == string.Empty)
                {
                    this.treeView.Frame(Selection.activeInstanceID, true, false);
                }
                this.ReloadData();
            }
        }

        private void SelectAll()
        {
            int[] rowIDs = this.treeView.GetRowIDs();
            this.treeView.SetSelection(rowIDs, false);
            this.TreeViewSelectionChanged(rowIDs);
        }

        internal void SelectNext()
        {
            this.m_TreeView.OffsetSelection(1);
        }

        internal void SelectPrevious()
        {
            this.m_TreeView.OffsetSelection(-1);
        }

        private void SelectSceneAsset(object userData)
        {
            int handle = (int) userData;
            int instanceIDFromGUID = AssetDatabase.GetInstanceIDFromGUID(AssetDatabase.AssetPathToGUID(EditorSceneManager.GetSceneByHandle(handle).path));
            Selection.activeInstanceID = instanceIDFromGUID;
            EditorGUIUtility.PingObject(instanceIDFromGUID);
        }

        private void SetAsLastInteractedHierarchy()
        {
            s_LastInteractedHierarchy = this;
        }

        public void SetCurrentRootInstanceID(int instanceID)
        {
            this.m_CurrenRootInstanceID = instanceID;
            this.Init();
            GUIUtility.ExitGUI();
        }

        public void SetExpandedRecursive(int id, bool expand)
        {
            TreeViewItem item = this.treeView.data.FindItem(id);
            if (item == null)
            {
                this.ReloadData();
                item = this.treeView.data.FindItem(id);
            }
            if (item != null)
            {
                this.treeView.data.SetExpandedWithChildren(item, expand);
            }
        }

        private void SetSceneActive(object userData)
        {
            int handle = (int) userData;
            SceneManager.SetActiveScene(EditorSceneManager.GetSceneByHandle(handle));
        }

        internal override void SetSearchFilter(string searchFilter, SearchableEditorWindow.SearchMode searchMode, bool setAll)
        {
            base.SetSearchFilter(searchFilter, searchMode, setAll);
            if (this.m_DidSelectSearchResult && string.IsNullOrEmpty(searchFilter))
            {
                this.m_DidSelectSearchResult = false;
                this.FrameObjectPrivate(Selection.activeInstanceID, true, false, false);
                if (GUIUtility.keyboardControl == 0)
                {
                    GUIUtility.keyboardControl = this.m_TreeViewKeyboardControlID;
                }
            }
        }

        private void SetSortFunction(string sortTypeName)
        {
            if (!this.m_SortingObjects.ContainsKey(sortTypeName))
            {
                Debug.LogError("Invalid search type name: " + sortTypeName);
            }
            else
            {
                this.currentSortMethod = sortTypeName;
                if (this.treeView.GetSelection().Any<int>())
                {
                    this.treeView.Frame(this.treeView.GetSelection().First<int>(), true, false);
                }
                this.treeView.ReloadData();
            }
        }

        public void SetSortFunction(Type sortType)
        {
            this.SetSortFunction(this.GetNameForType(sortType));
        }

        private void SetUpSortMethodLists()
        {
            this.m_SortingObjects = new Dictionary<string, BaseHierarchySort>();
            foreach (Assembly assembly in EditorAssemblies.loadedAssemblies)
            {
                IEnumerator<BaseHierarchySort> enumerator = AssemblyHelper.FindImplementors<BaseHierarchySort>(assembly).GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        BaseHierarchySort current = enumerator.Current;
                        if ((current.GetType() != typeof(AlphabeticalSort)) || this.m_AllowAlphaNumericalSort)
                        {
                            string nameForType = this.GetNameForType(current.GetType());
                            this.m_SortingObjects.Add(nameForType, current);
                        }
                    }
                }
                finally
                {
                    if (enumerator == null)
                    {
                    }
                    enumerator.Dispose();
                }
            }
            this.currentSortMethod = this.m_CurrentSortMethod;
        }

        protected virtual void ShowButton(Rect r)
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            this.m_Locked = GUI.Toggle(r, this.m_Locked, GUIContent.none, s_Styles.lockButton);
        }

        private void SortFunctionCallback(SceneHierarchySortingWindow.InputData data)
        {
            this.SetSortFunction(data.m_TypeName);
        }

        private void SortMethodsDropDown()
        {
            if (this.hasSortMethods)
            {
                GUIContent defaultSortingContent = this.m_SortingObjects[this.currentSortMethod].content;
                if (defaultSortingContent == null)
                {
                    defaultSortingContent = s_Styles.defaultSortingContent;
                    defaultSortingContent.tooltip = this.currentSortMethod;
                }
                Rect position = GUILayoutUtility.GetRect(defaultSortingContent, EditorStyles.toolbarButton);
                if (EditorGUI.ButtonMouseDown(position, defaultSortingContent, FocusType.Passive, EditorStyles.toolbarButton))
                {
                    List<SceneHierarchySortingWindow.InputData> list = new List<SceneHierarchySortingWindow.InputData>();
                    foreach (KeyValuePair<string, BaseHierarchySort> pair in this.m_SortingObjects)
                    {
                        SceneHierarchySortingWindow.InputData item = new SceneHierarchySortingWindow.InputData {
                            m_TypeName = pair.Key,
                            m_Name = ObjectNames.NicifyVariableName(pair.Key),
                            m_Selected = pair.Key == this.m_CurrentSortMethod
                        };
                        list.Add(item);
                    }
                    if (SceneHierarchySortingWindow.ShowAtPosition(new Vector2(position.x, position.y + position.height), list, new SceneHierarchySortingWindow.OnSelectCallback(this.SortFunctionCallback)))
                    {
                        GUIUtility.ExitGUI();
                    }
                }
            }
        }

        private void SyncIfNeeded()
        {
            if (this.treeViewReloadNeeded)
            {
                this.treeViewReloadNeeded = false;
                this.ReloadData();
            }
            if (this.selectionSyncNeeded)
            {
                this.selectionSyncNeeded = false;
                bool flag = (EditorApplication.timeSinceStartup - this.m_LastUserInteractionTime) < 0.2;
                bool revealSelectionAndFrameLastSelected = (!this.m_Locked || this.m_FrameOnSelectionSync) || flag;
                bool animatedFraming = flag && revealSelectionAndFrameLastSelected;
                this.m_FrameOnSelectionSync = false;
                this.treeView.SetSelection(Selection.instanceIDs, revealSelectionAndFrameLastSelected, animatedFraming);
            }
        }

        private static void ToggleDebugMode()
        {
            s_Debug = !s_Debug;
            SessionState.SetBool("HierarchyWindowDebug", s_Debug);
        }

        private void TreeViewItemDoubleClicked(int instanceID)
        {
            Scene sceneByHandle = EditorSceneManager.GetSceneByHandle(instanceID);
            if (IsSceneHeaderInHierarchyWindow(sceneByHandle))
            {
                if (sceneByHandle.isLoaded)
                {
                    SceneManager.SetActiveScene(sceneByHandle);
                }
            }
            else
            {
                SceneView.FrameLastActiveSceneView();
            }
        }

        private void TreeViewSelectionChanged(int[] ids)
        {
            Selection.instanceIDs = ids;
            this.m_DidSelectSearchResult = !string.IsNullOrEmpty(base.m_SearchFilter);
        }

        private void UnloadSelectedScenes(object userdata)
        {
            this.CloseSelectedScenes(false);
        }

        private string currentSortMethod
        {
            get
            {
                return this.m_CurrentSortMethod;
            }
            set
            {
                this.m_CurrentSortMethod = value;
                if (!this.m_SortingObjects.ContainsKey(this.m_CurrentSortMethod))
                {
                    this.m_CurrentSortMethod = this.GetNameForType(typeof(TransformSort));
                }
                GameObjectTreeViewDataSource data = (GameObjectTreeViewDataSource) this.treeView.data;
                data.sortingState.sortingObject = this.m_SortingObjects[this.m_CurrentSortMethod];
                GameObjectsTreeViewDragging dragging = (GameObjectsTreeViewDragging) this.treeView.dragging;
                dragging.allowDragBetween = !data.sortingState.implementsCompare;
            }
        }

        internal static bool debug
        {
            get
            {
                return lastInteractedHierarchyWindow.m_Debug;
            }
            set
            {
                lastInteractedHierarchyWindow.m_Debug = value;
            }
        }

        private bool hasSortMethods
        {
            get
            {
                return (this.m_SortingObjects.Count > 1);
            }
        }

        public static SceneHierarchyWindow lastInteractedHierarchyWindow
        {
            get
            {
                return s_LastInteractedHierarchy;
            }
        }

        private bool selectionSyncNeeded
        {
            get
            {
                return this.m_SelectionSyncNeeded;
            }
            set
            {
                this.m_SelectionSyncNeeded = value;
                if (value)
                {
                    base.Repaint();
                    if (s_Debug)
                    {
                        Debug.Log("Selection sync and frameing on next event");
                    }
                }
            }
        }

        private TreeView treeView
        {
            get
            {
                if (this.m_TreeView == null)
                {
                    this.Init();
                }
                return this.m_TreeView;
            }
        }

        private Rect treeViewRect
        {
            get
            {
                return new Rect(0f, 17f, base.position.width, base.position.height - 17f);
            }
        }

        private bool treeViewReloadNeeded
        {
            get
            {
                return this.m_TreeViewReloadNeeded;
            }
            set
            {
                this.m_TreeViewReloadNeeded = value;
                if (value)
                {
                    base.Repaint();
                    if (s_Debug)
                    {
                        Debug.Log("Reload treeview on next event");
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <CreateGameObjectContextClick>c__AnonStorey3F
        {
            internal Object prefab;

            internal void <>m__63()
            {
                Selection.activeObject = this.prefab;
                EditorGUIUtility.PingObject(this.prefab.GetInstanceID());
            }
        }

        private class Styles
        {
            public GUIContent createContent = new GUIContent("Create");
            public GUIContent defaultSortingContent = new GUIContent(EditorGUIUtility.FindTexture("CustomSorting"));
            public GUIContent fetchWarning = new GUIContent(string.Empty, EditorGUIUtility.FindTexture("console.warnicon.sml"), "The current sorting method is taking a lot of time. Consider using 'Transform Sort' in playmode for better performance.");
            private const string kCustomSorting = "CustomSorting";
            private const string kWarningMessage = "The current sorting method is taking a lot of time. Consider using 'Transform Sort' in playmode for better performance.";
            private const string kWarningSymbol = "console.warnicon.sml";
            public GUIStyle lockButton = "IN LockButton";
            public GUIStyle MiniButton = "ToolbarButton";
        }
    }
}

