namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.ProjectWindowCallback;
    using UnityEditorInternal;
    using UnityEngine;

    [EditorWindowTitle(title="Project", icon="Project")]
    internal class ProjectBrowser : EditorWindow, IHasCustomMenu
    {
        [CompilerGenerated]
        private static Func<PopupList.ListElement, bool> <>f__am$cache31;
        [CompilerGenerated]
        private static Func<PopupList.ListElement, string> <>f__am$cache32;
        [CompilerGenerated]
        private static Func<PopupList.ListElement, bool> <>f__am$cache33;
        [CompilerGenerated]
        private static Func<PopupList.ListElement, string> <>f__am$cache34;
        private const float k_BottomBarHeight = 17f;
        private float k_MinDirectoriesAreaWidth = 110f;
        private const float k_MinHeight = 250f;
        private const float k_MinWidthOneColumn = 230f;
        private const float k_MinWidthTwoColumns = 230f;
        private const float k_ResizerWidth = 5f;
        private const float k_SliderWidth = 55f;
        private PopupList.InputData m_AssetLabels;
        private TreeView m_AssetTree;
        [SerializeField]
        private TreeViewState m_AssetTreeState;
        [NonSerialized]
        private Rect m_BottomBarRect;
        private bool m_BreadCrumbLastFolderHasSubFolders;
        private List<KeyValuePair<GUIContent, string>> m_BreadCrumbs = new List<KeyValuePair<GUIContent, string>>();
        private int m_CurrentNumItems;
        private bool m_DidSelectSearchResult;
        [SerializeField]
        private float m_DirectoriesAreaWidth = 115f;
        private bool m_EnableOldAssetTree = true;
        private bool m_FocusSearchField;
        private TreeView m_FolderTree;
        [SerializeField]
        private TreeViewState m_FolderTreeState;
        private bool m_GrabKeyboardFocusForListArea;
        private bool m_InternalSelectionChange;
        [SerializeField]
        private bool m_IsLocked;
        private bool m_ItemSelectedByRightClickThisEvent;
        [SerializeField]
        private string[] m_LastFolders = new string[0];
        [SerializeField]
        private float m_LastFoldersGridSize = -1f;
        [NonSerialized]
        private int m_LastFramedID = -1;
        private float m_LastListWidth;
        private SearchFilter.SearchArea m_LastLocalAssetsSearchArea;
        [SerializeField]
        private string m_LastProjectPath;
        private ObjectListArea m_ListArea;
        [NonSerialized]
        private Rect m_ListAreaRect;
        [SerializeField]
        private ObjectListAreaState m_ListAreaState;
        [NonSerialized]
        private Rect m_ListHeaderRect;
        private int m_ListKeyboardControlID;
        private PopupList.InputData m_ObjectTypes;
        [NonSerialized]
        public GUIContent m_SearchAllAssets = new GUIContent("Assets");
        private ExposablePopupMenu m_SearchAreaMenu;
        [NonSerialized]
        private float m_SearchAreaMenuOffset = -1f;
        [NonSerialized]
        public GUIContent m_SearchAssetStore = new GUIContent("Asset Store");
        [NonSerialized]
        private string m_SearchFieldText = string.Empty;
        [SerializeField]
        private SearchFilter m_SearchFilter;
        [NonSerialized]
        public GUIContent m_SearchInFolders = new GUIContent(string.Empty);
        private string m_SelectedPath;
        private List<GUIContent> m_SelectedPathSplitted = new List<GUIContent>();
        [SerializeField]
        private int m_StartGridSize = 0x40;
        private float m_ToolbarHeight;
        private int m_TreeViewKeyboardControlID;
        [NonSerialized]
        private Rect m_TreeViewRect;
        private bool m_UseTreeViewSelectionInsteadOfMainSelection;
        [SerializeField]
        private ViewMode m_ViewMode = ViewMode.TwoColumns;
        private static int s_HashForSearchField = "ProjectBrowserSearchField".GetHashCode();
        public static ProjectBrowser s_LastInteractedProjectBrowser;
        private static List<ProjectBrowser> s_ProjectBrowsers = new List<ProjectBrowser>();
        private static Styles s_Styles;

        private ProjectBrowser()
        {
        }

        public virtual void AddItemsToMenu(GenericMenu menu)
        {
            if (this.m_EnableOldAssetTree)
            {
                GUIContent content = new GUIContent("One Column Layout");
                GUIContent content2 = new GUIContent("Two Column Layout");
                menu.AddItem(content, this.m_ViewMode == ViewMode.OneColumn, new GenericMenu.MenuFunction(this.SetOneColumn));
                if (base.position.width >= 230f)
                {
                    menu.AddItem(content2, this.m_ViewMode == ViewMode.TwoColumns, new GenericMenu.MenuFunction(this.SetTwoColumns));
                }
                else
                {
                    menu.AddDisabledItem(content2);
                }
                if (Unsupported.IsDeveloperBuild())
                {
                    menu.AddItem(new GUIContent("DEVELOPER/Open TreeView Test Window..."), false, new GenericMenu.MenuFunction(this.OpenTreeViewTestWindow));
                    menu.AddItem(new GUIContent("DEVELOPER/Use TreeView Expansion Animation"), EditorPrefs.GetBool("TreeViewExpansionAnimation", false), new GenericMenu.MenuFunction(this.ToggleExpansionAnimationPreference));
                }
            }
        }

        public void AssetLabelListCallback(PopupList.ListElement element)
        {
            if (!Event.current.control)
            {
                foreach (PopupList.ListElement element2 in this.m_AssetLabels.m_ListElements)
                {
                    if (element2 != element)
                    {
                        element2.selected = false;
                    }
                }
            }
            element.selected = !element.selected;
            if (<>f__am$cache33 == null)
            {
                <>f__am$cache33 = item => item.selected;
            }
            if (<>f__am$cache34 == null)
            {
                <>f__am$cache34 = item => item.text;
            }
            this.m_SearchFilter.assetLabels = this.m_AssetLabels.m_ListElements.Where<PopupList.ListElement>(<>f__am$cache33).Select<PopupList.ListElement, string>(<>f__am$cache34).ToArray<string>();
            this.m_SearchFieldText = this.m_SearchFilter.FilterToSearchFieldString();
            this.TopBarSearchSettingsChanged();
            base.Repaint();
        }

        private void AssetLabelsDropDown()
        {
            Rect position = GUILayoutUtility.GetRect(s_Styles.m_FilterByLabel, EditorStyles.toolbarButton);
            if (EditorGUI.ButtonMouseDown(position, s_Styles.m_FilterByLabel, FocusType.Passive, EditorStyles.toolbarButton))
            {
                PopupWindow.Show(position, new PopupList(this.m_AssetLabels));
            }
        }

        private void AssetStoreSearchEndedCallback()
        {
            this.InitSearchMenu();
        }

        private void AssetTreeDragEnded(int[] draggedInstanceIds, bool draggedItemsFromOwnTreeView)
        {
            if ((draggedInstanceIds != null) && draggedItemsFromOwnTreeView)
            {
                this.m_AssetTree.SetSelection(draggedInstanceIds, true);
                this.m_AssetTree.NotifyListenersThatSelectionChanged();
                base.Repaint();
                GUIUtility.ExitGUI();
            }
        }

        private void AssetTreeItemDoubleClickedCallback(int instanceID)
        {
            OpenAssetSelection(Selection.instanceIDs);
        }

        private void AssetTreeKeyboardInputCallback()
        {
            if (Event.current.type == EventType.KeyDown)
            {
                switch (Event.current.keyCode)
                {
                    case KeyCode.KeypadEnter:
                    case KeyCode.Return:
                        if (Application.platform == RuntimePlatform.WindowsEditor)
                        {
                            Event.current.Use();
                            OpenAssetSelection(Selection.instanceIDs);
                        }
                        return;

                    case KeyCode.DownArrow:
                        if ((Application.platform == RuntimePlatform.OSXEditor) && Event.current.command)
                        {
                            Event.current.Use();
                            OpenAssetSelection(Selection.instanceIDs);
                        }
                        return;
                }
            }
        }

        private void AssetTreeSelectionCallback(int[] selectedTreeViewInstanceIDs)
        {
            this.SetAsLastInteractedProjectBrowser();
            Selection.activeObject = null;
            if (selectedTreeViewInstanceIDs.Length > 0)
            {
                Selection.instanceIDs = selectedTreeViewInstanceIDs;
            }
            this.RefreshSelectedPath();
            this.SetSearchFoldersFromCurrentSelection();
            this.RefreshSearchText();
        }

        private void AssetTreeViewContextClick(int clickedItemID)
        {
            Event current = Event.current;
            EditorUtility.DisplayPopupMenu(new Rect(current.mousePosition.x, current.mousePosition.y, 0f, 0f), "Assets/", null);
            current.Use();
        }

        private void AssetTreeViewContextClickOutsideItems()
        {
            Event current = Event.current;
            if (this.m_AssetTree.GetSelection().Length > 0)
            {
                int[] selectedIDs = new int[0];
                this.m_AssetTree.SetSelection(selectedIDs, false);
                this.AssetTreeSelectionCallback(selectedIDs);
            }
            EditorUtility.DisplayPopupMenu(new Rect(current.mousePosition.x, current.mousePosition.y, 0f, 0f), "Assets/", null);
            current.Use();
        }

        private void Awake()
        {
            if (this.m_ListAreaState != null)
            {
                this.m_ListAreaState.OnAwake();
            }
            if (this.m_FolderTreeState != null)
            {
                this.m_FolderTreeState.OnAwake();
                this.m_FolderTreeState.expandedIDs = new List<int>(InternalEditorUtility.expandedProjectWindowItems);
            }
            if (this.m_AssetTreeState != null)
            {
                this.m_AssetTreeState.OnAwake();
                this.m_AssetTreeState.expandedIDs = new List<int>(InternalEditorUtility.expandedProjectWindowItems);
            }
            if (this.m_SearchFilter != null)
            {
                this.EnsureValidFolders();
            }
        }

        internal void BeginPreimportedNameEditing(int instanceID, EndNameEditAction endAction, string pathName, Texture2D icon, string resourceFile)
        {
            if (!this.Initialized())
            {
                this.Init();
            }
            this.EndRenaming();
            bool isCreatingNewFolder = endAction is DoCreateFolder;
            if (this.m_ViewMode == ViewMode.TwoColumns)
            {
                if (this.m_SearchFilter.GetState() != SearchFilter.State.FolderBrowsing)
                {
                    this.SelectAssetsFolder();
                }
                pathName = this.ValidateCreateNewAssetPath(pathName);
                if (this.m_ListAreaState.m_CreateAssetUtility.BeginNewAssetCreation(instanceID, endAction, pathName, icon, resourceFile))
                {
                    this.ShowFolderContents(AssetDatabase.GetMainAssetInstanceID(this.m_ListAreaState.m_CreateAssetUtility.folder), true);
                    this.m_ListArea.BeginNamingNewAsset(this.m_ListAreaState.m_CreateAssetUtility.originalName, instanceID, isCreatingNewFolder);
                }
            }
            else if (this.m_ViewMode == ViewMode.OneColumn)
            {
                if (this.m_SearchFilter.IsSearching())
                {
                    this.ClearSearch();
                }
                AssetsTreeViewGUI gui = this.m_AssetTree.gui as AssetsTreeViewGUI;
                if (gui != null)
                {
                    gui.BeginCreateNewAsset(instanceID, endAction, pathName, icon, resourceFile);
                }
                else
                {
                    Debug.LogError("Not valid defaultTreeViewGUI!");
                }
            }
        }

        private void BottomBar()
        {
            if (this.m_BottomBarRect.height != 0f)
            {
                Rect bottomBarRect = this.m_BottomBarRect;
                GUI.Label(bottomBarRect, GUIContent.none, s_Styles.bottomBarBg);
                Rect r = new Rect(((bottomBarRect.x + bottomBarRect.width) - 55f) - 16f, (bottomBarRect.y + bottomBarRect.height) - 17f, 55f, 17f);
                this.IconSizeSlider(r);
                EditorGUIUtility.SetIconSize(new Vector2(16f, 16f));
                bottomBarRect.width -= 4f;
                bottomBarRect.x += 2f;
                bottomBarRect.height = 17f;
                for (int i = this.m_SelectedPathSplitted.Count - 1; i >= 0; i--)
                {
                    if (i == 0)
                    {
                        bottomBarRect.width = (bottomBarRect.width - 55f) - 14f;
                    }
                    GUI.Label(bottomBarRect, this.m_SelectedPathSplitted[i], s_Styles.selectedPathLabel);
                    bottomBarRect.y += 17f;
                }
                EditorGUIUtility.SetIconSize(new Vector2(0f, 0f));
            }
        }

        private void BreadCrumbBar()
        {
            if ((this.m_ListHeaderRect.height > 0f) && (this.m_SearchFilter.folders.Length != 0))
            {
                Event current = Event.current;
                if ((current.type == EventType.MouseDown) && this.m_ListHeaderRect.Contains(current.mousePosition))
                {
                    GUIUtility.keyboardControl = this.m_ListKeyboardControlID;
                    base.Repaint();
                }
                if (this.m_BreadCrumbs.Count == 0)
                {
                    string path = this.m_SearchFilter.folders[0];
                    char[] separator = new char[] { '/' };
                    string[] strArray = path.Split(separator);
                    string str2 = string.Empty;
                    foreach (string str3 in strArray)
                    {
                        if (!string.IsNullOrEmpty(str2))
                        {
                            str2 = str2 + "/";
                        }
                        str2 = str2 + str3;
                        this.m_BreadCrumbs.Add(new KeyValuePair<GUIContent, string>(new GUIContent(str3), str2));
                    }
                    this.m_BreadCrumbLastFolderHasSubFolders = AssetDatabase.GetSubFolders(path).Length > 0;
                }
                GUI.Label(this.m_ListHeaderRect, GUIContent.none, s_Styles.topBarBg);
                Rect listHeaderRect = this.m_ListHeaderRect;
                listHeaderRect.y++;
                listHeaderRect.x += 4f;
                if (this.m_SearchFilter.folders.Length == 1)
                {
                    for (int i = 0; i < this.m_BreadCrumbs.Count; i++)
                    {
                        bool flag = i == (this.m_BreadCrumbs.Count - 1);
                        GUIStyle style = !flag ? EditorStyles.label : EditorStyles.boldLabel;
                        KeyValuePair<GUIContent, string> pair = this.m_BreadCrumbs[i];
                        GUIContent key = pair.Key;
                        KeyValuePair<GUIContent, string> pair2 = this.m_BreadCrumbs[i];
                        string assetPath = pair2.Value;
                        Vector2 vector = style.CalcSize(key);
                        listHeaderRect.width = vector.x;
                        if (GUI.Button(listHeaderRect, key, style))
                        {
                            this.ShowFolderContents(AssetDatabase.GetMainAssetInstanceID(assetPath), false);
                        }
                        listHeaderRect.x += vector.x + 3f;
                        if (!flag || this.m_BreadCrumbLastFolderHasSubFolders)
                        {
                            Rect position = new Rect(listHeaderRect.x, listHeaderRect.y + 2f, 13f, 13f);
                            if (EditorGUI.ButtonMouseDown(position, GUIContent.none, FocusType.Passive, s_Styles.foldout))
                            {
                                string currentSubFolder = string.Empty;
                                if (!flag)
                                {
                                    KeyValuePair<GUIContent, string> pair3 = this.m_BreadCrumbs[i + 1];
                                    currentSubFolder = pair3.Value;
                                }
                                BreadCrumbListMenu.Show(assetPath, currentSubFolder, position, this);
                            }
                        }
                        listHeaderRect.x += 11f;
                    }
                }
                else if (this.m_SearchFilter.folders.Length > 1)
                {
                    GUI.Label(listHeaderRect, GUIContent.Temp("Showing multiple folders..."), EditorStyles.miniLabel);
                }
            }
        }

        private void ButtonSaveFilter()
        {
            EditorGUI.BeginDisabledGroup(!this.m_SearchFilter.IsSearching());
            if (GUILayout.Button(s_Styles.m_SaveFilterContent, EditorStyles.toolbarButton, new GUILayoutOption[0]))
            {
                ProjectBrowserColumnOneTreeViewGUI gui = this.m_FolderTree.gui as ProjectBrowserColumnOneTreeViewGUI;
                if (gui != null)
                {
                    bool flag = true;
                    int[] selection = this.m_FolderTree.GetSelection();
                    if (selection.Length == 1)
                    {
                        int instanceID = selection[0];
                        bool flag2 = SavedSearchFilters.GetRootInstanceID() == instanceID;
                        if (SavedSearchFilters.IsSavedFilter(instanceID) && !flag2)
                        {
                            flag = false;
                            string title = "Overwrite Filter?";
                            string message = "Do you want to overwrite '" + SavedSearchFilters.GetName(instanceID) + "' or create a new filter?";
                            switch (EditorUtility.DisplayDialogComplex(title, message, "Overwrite", "Create", "Cancel"))
                            {
                                case 0:
                                    SavedSearchFilters.UpdateExistingSavedFilter(instanceID, this.m_SearchFilter, this.listAreaGridSize);
                                    break;

                                case 1:
                                    flag = true;
                                    break;
                            }
                        }
                    }
                    if (flag)
                    {
                        base.Focus();
                        gui.BeginCreateSavedFilter(this.m_SearchFilter);
                    }
                }
            }
            EditorGUI.EndDisabledGroup();
        }

        private void CalculateRects()
        {
            float bottomBarHeight = this.GetBottomBarHeight();
            float listHeaderHeight = this.GetListHeaderHeight();
            if (this.m_ViewMode == ViewMode.OneColumn)
            {
                this.m_ListAreaRect = new Rect(0f, this.m_ToolbarHeight + listHeaderHeight, base.position.width, ((base.position.height - this.m_ToolbarHeight) - listHeaderHeight) - bottomBarHeight);
                this.m_TreeViewRect = new Rect(0f, this.m_ToolbarHeight, base.position.width, (base.position.height - this.m_ToolbarHeight) - bottomBarHeight);
                this.m_BottomBarRect = new Rect(0f, base.position.height - bottomBarHeight, base.position.width, bottomBarHeight);
                this.m_ListHeaderRect = new Rect(0f, this.m_ToolbarHeight, base.position.width, listHeaderHeight);
            }
            else
            {
                float width = base.position.width - this.m_DirectoriesAreaWidth;
                this.m_ListAreaRect = new Rect(this.m_DirectoriesAreaWidth, this.m_ToolbarHeight + listHeaderHeight, width, ((base.position.height - this.m_ToolbarHeight) - listHeaderHeight) - bottomBarHeight);
                this.m_TreeViewRect = new Rect(0f, this.m_ToolbarHeight, this.m_DirectoriesAreaWidth, base.position.height - this.m_ToolbarHeight);
                this.m_BottomBarRect = new Rect(this.m_DirectoriesAreaWidth, base.position.height - bottomBarHeight, width, bottomBarHeight);
                this.m_ListHeaderRect = new Rect(this.m_ListAreaRect.x, this.m_ToolbarHeight, this.m_ListAreaRect.width, listHeaderHeight);
            }
        }

        private void ClearSearch()
        {
            this.m_SearchFilter.ClearSearch();
            this.m_SearchFieldText = string.Empty;
            this.m_AssetLabels.DeselectAll();
            this.m_ObjectTypes.DeselectAll();
            this.m_DidSelectSearchResult = false;
        }

        private void CreateDropdown()
        {
            Rect position = GUILayoutUtility.GetRect(s_Styles.m_CreateDropdownContent, EditorStyles.toolbarDropDown);
            if (EditorGUI.ButtonMouseDown(position, s_Styles.m_CreateDropdownContent, FocusType.Passive, EditorStyles.toolbarDropDown))
            {
                GUIUtility.hotControl = 0;
                EditorUtility.DisplayPopupMenu(position, "Assets/Create", null);
            }
        }

        private void DefaultSetup()
        {
            if (this.m_LastProjectPath != Directory.GetCurrentDirectory())
            {
                this.m_SearchFilter = new SearchFilter();
                this.m_LastFolders = new string[0];
                this.SyncFilterGUI();
                if (Selection.activeInstanceID != 0)
                {
                    this.FrameObjectPrivate(Selection.activeInstanceID, !this.m_IsLocked, false);
                }
                if ((this.m_ViewMode == ViewMode.TwoColumns) && !this.IsShowingFolderContents())
                {
                    this.SelectAssetsFolder();
                }
            }
            this.m_LastProjectPath = Directory.GetCurrentDirectory();
        }

        private static void DeleteFilter(int filterInstanceID)
        {
            if (SavedSearchFilters.GetRootInstanceID() == filterInstanceID)
            {
                string title = "Cannot Delete";
                EditorUtility.DisplayDialog(title, "Deleting the 'Filters' root is not allowed", "Ok");
            }
            else
            {
                string str2 = "Delete selected favorite?";
                if (EditorUtility.DisplayDialog(str2, "You cannot undo this action.", "Delete", "Cancel"))
                {
                    SavedSearchFilters.RemoveSavedFilter(filterInstanceID);
                }
            }
        }

        internal static void DeleteSelectedAssets(bool askIfSure)
        {
            List<int> list;
            int[] treeViewFolderSelection = GetTreeViewFolderSelection();
            if (treeViewFolderSelection.Length > 0)
            {
                list = new List<int>(treeViewFolderSelection);
            }
            else
            {
                list = new List<int>(Selection.instanceIDs);
            }
            if (list.Count != 0)
            {
                if (list.IndexOf(ProjectBrowserColumnOneTreeViewDataSource.GetAssetsFolderInstanceID()) >= 0)
                {
                    string title = "Cannot Delete";
                    EditorUtility.DisplayDialog(title, "Deleting the 'Assets' folder is not allowed", "Ok");
                }
                else
                {
                    List<string> mainPaths = GetMainPaths(list);
                    if (mainPaths.Count != 0)
                    {
                        if (askIfSure)
                        {
                            string str2 = "Delete selected asset";
                            if (mainPaths.Count > 1)
                            {
                                str2 = str2 + "s";
                            }
                            str2 = str2 + "?";
                            int num = 3;
                            string message = string.Empty;
                            for (int i = 0; (i < mainPaths.Count) && (i < num); i++)
                            {
                                message = message + "   " + mainPaths[i] + "\n";
                            }
                            if (mainPaths.Count > num)
                            {
                                message = message + "   ...\n";
                            }
                            message = message + "\nYou cannot undo this action.";
                            if (!EditorUtility.DisplayDialog(str2, message, "Delete", "Cancel"))
                            {
                                return;
                            }
                        }
                        AssetDatabase.StartAssetEditing();
                        foreach (string str4 in mainPaths)
                        {
                            AssetDatabase.MoveAssetToTrash(str4);
                        }
                        AssetDatabase.StopAssetEditing();
                        Selection.instanceIDs = new int[0];
                    }
                }
            }
        }

        private float DrawLocalAssetHeader(Rect r)
        {
            return 0f;
        }

        internal static int[] DuplicateFolders(int[] instanceIDs)
        {
            AssetDatabase.Refresh();
            List<string> list = new List<string>();
            bool flag = false;
            int assetsFolderInstanceID = ProjectBrowserColumnOneTreeViewDataSource.GetAssetsFolderInstanceID();
            foreach (int num2 in instanceIDs)
            {
                if (num2 != assetsFolderInstanceID)
                {
                    string assetPath = AssetDatabase.GetAssetPath(InternalEditorUtility.GetObjectFromInstanceID(num2));
                    string newPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
                    if (newPath.Length != 0)
                    {
                        flag |= !AssetDatabase.CopyAsset(assetPath, newPath);
                    }
                    else
                    {
                        flag |= true;
                    }
                    if (!flag)
                    {
                        list.Add(newPath);
                    }
                }
            }
            AssetDatabase.Refresh();
            int[] numArray2 = new int[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                numArray2[i] = AssetDatabase.LoadMainAssetAtPath(list[i]).GetInstanceID();
            }
            return numArray2;
        }

        private void EndPing()
        {
            if (this.m_ViewMode == ViewMode.OneColumn)
            {
                this.m_AssetTree.EndPing();
            }
            else
            {
                this.m_FolderTree.EndPing();
                this.m_ListArea.EndPing();
            }
        }

        private void EndRenaming()
        {
            if (this.m_AssetTree != null)
            {
                this.m_AssetTree.EndNameEditing(true);
            }
            if (this.m_FolderTree != null)
            {
                this.m_FolderTree.EndNameEditing(true);
            }
            if (this.m_ListArea != null)
            {
                this.m_ListArea.EndRename(true);
            }
        }

        private void EnsureValidFolders()
        {
            HashSet<string> source = new HashSet<string>();
            foreach (string str in this.m_SearchFilter.folders)
            {
                if (AssetDatabase.IsValidFolder(str))
                {
                    source.Add(str);
                }
                else
                {
                    string containingFolder = str;
                    for (int i = 0; i < 30; i++)
                    {
                        if (string.IsNullOrEmpty(containingFolder))
                        {
                            break;
                        }
                        containingFolder = ProjectWindowUtil.GetContainingFolder(containingFolder);
                        if (!string.IsNullOrEmpty(containingFolder) && AssetDatabase.IsValidFolder(containingFolder))
                        {
                            source.Add(containingFolder);
                            break;
                        }
                    }
                }
            }
            this.m_SearchFilter.folders = source.ToArray<string>();
        }

        private void FolderTreeDragEnded(int[] draggedInstanceIds, bool draggedItemsFromOwnTreeView)
        {
        }

        private void FolderTreeSelectionCallback(int[] selectedTreeViewInstanceIDs)
        {
            this.SetAsLastInteractedProjectBrowser();
            int instanceID = 0;
            if (selectedTreeViewInstanceIDs.Length > 0)
            {
                instanceID = selectedTreeViewInstanceIDs[0];
            }
            bool folderWasSelected = false;
            if (instanceID != 0)
            {
                switch (GetItemType(instanceID))
                {
                    case ItemType.Asset:
                        this.SetFoldersInSearchFilter(selectedTreeViewInstanceIDs);
                        folderWasSelected = true;
                        break;

                    case ItemType.SavedFilter:
                    {
                        SearchFilter filter = SavedSearchFilters.GetFilter(instanceID);
                        if (this.ValidateFilter(instanceID, filter))
                        {
                            this.m_SearchFilter = filter;
                            this.EnsureValidFolders();
                            float previewSize = SavedSearchFilters.GetPreviewSize(instanceID);
                            if (previewSize > 0f)
                            {
                                this.m_ListArea.gridSize = Mathf.Clamp((int) previewSize, this.m_ListArea.minGridSize, this.m_ListArea.maxGridSize);
                            }
                            this.SyncFilterGUI();
                        }
                        break;
                    }
                }
            }
            this.FolderTreeSelectionChanged(folderWasSelected);
        }

        private void FolderTreeSelectionChanged(bool folderWasSelected)
        {
            if (folderWasSelected)
            {
                switch (this.GetSearchViewState())
                {
                    case SearchViewState.AllAssets:
                    case SearchViewState.AssetStore:
                    {
                        string[] folders = this.m_SearchFilter.folders;
                        this.ClearSearch();
                        this.m_SearchFilter.folders = folders;
                        this.m_SearchFilter.searchArea = this.m_LastLocalAssetsSearchArea;
                        break;
                    }
                }
                this.m_LastFolders = this.m_SearchFilter.folders;
            }
            this.RefreshSearchText();
            this.InitListArea();
        }

        private void FolderTreeViewContextClick(int clickedItemID)
        {
            Event current = Event.current;
            if (SavedSearchFilters.IsSavedFilter(clickedItemID))
            {
                if (clickedItemID != SavedSearchFilters.GetRootInstanceID())
                {
                    SavedFiltersContextMenu.Show(clickedItemID);
                }
            }
            else
            {
                EditorUtility.DisplayPopupMenu(new Rect(current.mousePosition.x, current.mousePosition.y, 0f, 0f), "Assets/", null);
            }
            current.Use();
        }

        public void FrameObject(int instanceID, bool ping)
        {
            this.FrameObjectPrivate(instanceID, !this.m_IsLocked, ping);
            if (s_LastInteractedProjectBrowser == this)
            {
                this.m_GrabKeyboardFocusForListArea = true;
            }
        }

        private void FrameObjectInTwoColumnMode(int instanceID, bool frame, bool ping)
        {
            int id = 0;
            string assetPath = AssetDatabase.GetAssetPath(instanceID);
            if (!string.IsNullOrEmpty(assetPath))
            {
                string containingFolder = ProjectWindowUtil.GetContainingFolder(assetPath);
                if (!string.IsNullOrEmpty(containingFolder))
                {
                    id = AssetDatabase.GetMainAssetInstanceID(containingFolder);
                }
                if (id == 0)
                {
                    id = ProjectBrowserColumnOneTreeViewDataSource.GetAssetsFolderInstanceID();
                }
            }
            if (id != 0)
            {
                this.m_FolderTree.Frame(id, frame, ping);
                if (frame)
                {
                    this.ShowFolderContents(id, true);
                }
                this.m_ListArea.Frame(instanceID, frame, ping);
            }
        }

        private void FrameObjectPrivate(int instanceID, bool frame, bool ping)
        {
            if ((instanceID != 0) && (this.m_ListArea != null))
            {
                if (this.m_LastFramedID != instanceID)
                {
                    this.EndPing();
                }
                this.m_LastFramedID = instanceID;
                this.ClearSearch();
                if (this.m_ViewMode == ViewMode.TwoColumns)
                {
                    this.FrameObjectInTwoColumnMode(instanceID, frame, ping);
                }
                else if (this.m_ViewMode == ViewMode.OneColumn)
                {
                    this.m_AssetTree.Frame(instanceID, frame, ping);
                }
            }
        }

        internal string GetActiveFolderPath()
        {
            if (((this.m_ViewMode == ViewMode.TwoColumns) && (this.m_SearchFilter.GetState() == SearchFilter.State.FolderBrowsing)) && (this.m_SearchFilter.folders.Length > 0))
            {
                return this.m_SearchFilter.folders[0];
            }
            return "Assets";
        }

        public static List<ProjectBrowser> GetAllProjectBrowsers()
        {
            return s_ProjectBrowsers;
        }

        private string GetAnalyticsSizeLabel(float size)
        {
            if (size > 600f)
            {
                return "Larger than 600 pix";
            }
            if (size < 240f)
            {
                return "Less than 240 pix";
            }
            return "240 - 600 pix";
        }

        private float GetBottomBarHeight()
        {
            if (this.m_SelectedPathSplitted.Count == 0)
            {
                this.RefreshSplittedSelectedPath();
            }
            if ((this.m_ViewMode == ViewMode.OneColumn) && !this.m_SearchFilter.IsSearching())
            {
                return 0f;
            }
            return (17f * this.m_SelectedPathSplitted.Count);
        }

        private static int[] GetFolderInstanceIDs(string[] folders)
        {
            int[] numArray = new int[folders.Length];
            for (int i = 0; i < folders.Length; i++)
            {
                numArray[i] = AssetDatabase.GetMainAssetInstanceID(folders[i]);
            }
            return numArray;
        }

        private static string[] GetFolderPathsFromInstanceIDs(int[] instanceIDs)
        {
            List<string> list = new List<string>();
            foreach (int num in instanceIDs)
            {
                string assetPath = AssetDatabase.GetAssetPath(num);
                if (!string.IsNullOrEmpty(assetPath))
                {
                    list.Add(assetPath);
                }
            }
            return list.ToArray();
        }

        internal IHierarchyProperty GetHierarchyPropertyUsingFilter(string textFilter)
        {
            FilteredHierarchy filteredHierarchy = new FilteredHierarchy(HierarchyType.Assets) {
                searchFilter = SearchFilter.CreateSearchFilterFromString(textFilter)
            };
            return FilteredHierarchyProperty.CreateHierarchyPropertyForFilter(filteredHierarchy);
        }

        internal static ItemType GetItemType(int instanceID)
        {
            if (SavedSearchFilters.IsSavedFilter(instanceID))
            {
                return ItemType.SavedFilter;
            }
            return ItemType.Asset;
        }

        private float GetListHeaderHeight()
        {
            return ((this.m_SearchFilter.GetState() != SearchFilter.State.EmptySearchFilter) ? 18f : 0f);
        }

        private static List<string> GetMainPaths(List<int> instanceIDs)
        {
            List<string> list = new List<string>();
            foreach (int num in instanceIDs)
            {
                if (AssetDatabase.IsMainAsset(num))
                {
                    string assetPath = AssetDatabase.GetAssetPath(num);
                    list.Add(assetPath);
                }
            }
            return list;
        }

        private static int GetParentInstanceID(int objectInstanceID)
        {
            string assetPath = AssetDatabase.GetAssetPath(objectInstanceID);
            int length = assetPath.LastIndexOf("/");
            if (length >= 0)
            {
                Object obj2 = AssetDatabase.LoadAssetAtPath(assetPath.Substring(0, length), typeof(Object));
                if (obj2 != null)
                {
                    return obj2.GetInstanceID();
                }
            }
            else
            {
                Debug.LogError("Invalid path: " + assetPath);
            }
            return -1;
        }

        private int GetProjectBrowserDebugID()
        {
            for (int i = 0; i < s_ProjectBrowsers.Count; i++)
            {
                if (s_ProjectBrowsers[i] == this)
                {
                    return i;
                }
            }
            return -1;
        }

        private SearchViewState GetSearchViewState()
        {
            switch (this.m_SearchFilter.GetState())
            {
                case SearchFilter.State.SearchingInAllAssets:
                    return SearchViewState.AllAssets;

                case SearchFilter.State.SearchingInFolders:
                    return SearchViewState.SubFolders;

                case SearchFilter.State.SearchingInAssetStore:
                    return SearchViewState.AssetStore;
            }
            return SearchViewState.NotSearching;
        }

        private bool GetShouldShowFoldersFirst()
        {
            return (Application.platform != RuntimePlatform.OSXEditor);
        }

        private static int[] GetTreeViewFolderSelection()
        {
            ProjectBrowser browser = s_LastInteractedProjectBrowser;
            if (((browser != null) && browser.useTreeViewSelectionInsteadOfMainSelection) && (browser.m_FolderTree != null))
            {
                return s_LastInteractedProjectBrowser.m_FolderTree.GetSelection();
            }
            return new int[0];
        }

        private string[] GetTypesDisplayNames()
        {
            return new string[] { "AnimationClip", "AudioClip", "AudioMixer", "Font", "GUISkin", "Material", "Mesh", "Model", "PhysicMaterial", "Prefab", "Scene", "Script", "Shader", "Sprite", "Texture" };
        }

        private bool HandleCommandEvents()
        {
            EventType type = Event.current.type;
            switch (type)
            {
                case EventType.ExecuteCommand:
                case EventType.ValidateCommand:
                {
                    bool flag = type == EventType.ExecuteCommand;
                    if ((Event.current.commandName == "Delete") || (Event.current.commandName == "SoftDelete"))
                    {
                        Event.current.Use();
                        if (flag)
                        {
                            bool askIfSure = Event.current.commandName == "SoftDelete";
                            DeleteSelectedAssets(askIfSure);
                            if (askIfSure)
                            {
                                base.Focus();
                            }
                        }
                        GUIUtility.ExitGUI();
                        break;
                    }
                    if (Event.current.commandName == "Duplicate")
                    {
                        if (flag)
                        {
                            Event.current.Use();
                            ProjectWindowUtil.DuplicateSelectedAssets();
                            GUIUtility.ExitGUI();
                        }
                        else if (Selection.GetFiltered(typeof(Object), SelectionMode.Assets).Length != 0)
                        {
                            Event.current.Use();
                        }
                        break;
                    }
                    if (Event.current.commandName == "FocusProjectWindow")
                    {
                        if (flag)
                        {
                            this.FrameObjectPrivate(Selection.activeInstanceID, true, false);
                            Event.current.Use();
                            base.Focus();
                            GUIUtility.ExitGUI();
                        }
                        else
                        {
                            Event.current.Use();
                        }
                        break;
                    }
                    if (Event.current.commandName == "SelectAll")
                    {
                        if (flag)
                        {
                            this.SelectAll();
                        }
                        Event.current.Use();
                        break;
                    }
                    if (Event.current.commandName == "FrameSelected")
                    {
                        if (flag)
                        {
                            this.FrameObjectPrivate(Selection.activeInstanceID, true, false);
                            Event.current.Use();
                            GUIUtility.ExitGUI();
                        }
                        Event.current.Use();
                    }
                    else if (Event.current.commandName == "Find")
                    {
                        if (flag)
                        {
                            this.m_FocusSearchField = true;
                        }
                        Event.current.Use();
                    }
                    break;
                }
            }
            return false;
        }

        private bool HandleCommandEventsForTreeView()
        {
            EventType type = Event.current.type;
            if ((type == EventType.ExecuteCommand) || (type == EventType.ValidateCommand))
            {
                bool flag = type == EventType.ExecuteCommand;
                int[] selection = this.m_FolderTree.GetSelection();
                if (selection.Length == 0)
                {
                    return false;
                }
                ItemType itemType = GetItemType(selection[0]);
                if (!(Event.current.commandName == "Delete") && !(Event.current.commandName == "SoftDelete"))
                {
                    if (Event.current.commandName == "Duplicate")
                    {
                        if (flag)
                        {
                            if ((itemType != ItemType.SavedFilter) && (itemType == ItemType.Asset))
                            {
                                Event.current.Use();
                                int[] selectedIDs = DuplicateFolders(selection);
                                this.m_FolderTree.SetSelection(selectedIDs, true);
                                GUIUtility.ExitGUI();
                            }
                        }
                        else
                        {
                            Event.current.Use();
                        }
                    }
                }
                else
                {
                    Event.current.Use();
                    if (flag)
                    {
                        switch (itemType)
                        {
                            case ItemType.SavedFilter:
                                DeleteFilter(selection[0]);
                                break;

                            case ItemType.Asset:
                            {
                                bool askIfSure = Event.current.commandName == "SoftDelete";
                                DeleteSelectedAssets(askIfSure);
                                if (askIfSure)
                                {
                                    base.Focus();
                                }
                                break;
                            }
                        }
                    }
                    GUIUtility.ExitGUI();
                }
            }
            return false;
        }

        private void HandleContextClickInListArea(Rect listRect)
        {
            Event current = Event.current;
            switch (current.type)
            {
                case EventType.MouseDown:
                    if ((((this.m_ViewMode == ViewMode.TwoColumns) && (this.m_SearchFilter.GetState() == SearchFilter.State.FolderBrowsing)) && ((current.button == 1) && !this.m_ItemSelectedByRightClickThisEvent)) && ((this.m_SearchFilter.folders.Length > 0) && listRect.Contains(current.mousePosition)))
                    {
                        this.m_InternalSelectionChange = true;
                        Selection.instanceIDs = GetFolderInstanceIDs(this.m_SearchFilter.folders);
                    }
                    break;

                case EventType.ContextClick:
                    if (listRect.Contains(current.mousePosition))
                    {
                        GUIUtility.hotControl = 0;
                        if (AssetStoreAssetSelection.GetFirstAsset() != null)
                        {
                            AssetStoreItemContextMenu.Show();
                        }
                        else
                        {
                            EditorUtility.DisplayPopupMenu(new Rect(current.mousePosition.x, current.mousePosition.y, 0f, 0f), "Assets/", null);
                        }
                        current.Use();
                    }
                    break;
            }
        }

        private void IconSizeSlider(Rect r)
        {
            EditorGUI.BeginChangeCheck();
            int num = (int) GUI.HorizontalSlider(r, (float) this.m_ListArea.gridSize, (float) this.m_ListArea.minGridSize, (float) this.m_ListArea.maxGridSize);
            if (EditorGUI.EndChangeCheck())
            {
                AssetStorePreviewManager.AbortSize(this.m_ListArea.gridSize);
                this.m_ListArea.gridSize = num;
            }
        }

        public void Init()
        {
            if (!this.Initialized())
            {
                this.m_FocusSearchField = false;
                if (this.m_SearchFilter == null)
                {
                    this.m_DirectoriesAreaWidth = Mathf.Min((float) (base.position.width / 2f), (float) 200f);
                }
                if (this.m_SearchFilter == null)
                {
                    this.m_SearchFilter = new SearchFilter();
                }
                this.m_SearchFieldText = this.m_SearchFilter.FilterToSearchFieldString();
                this.CalculateRects();
                this.RefreshSelectedPath();
                this.SetupDroplists();
                if (this.m_ListAreaState == null)
                {
                    this.m_ListAreaState = new ObjectListAreaState();
                }
                this.m_ListAreaState.m_RenameOverlay.isRenamingFilename = true;
                this.m_ListArea = new ObjectListArea(this.m_ListAreaState, this, false);
                this.m_ListArea.allowDeselection = true;
                this.m_ListArea.allowDragging = true;
                this.m_ListArea.allowFocusRendering = true;
                this.m_ListArea.allowMultiSelect = true;
                this.m_ListArea.allowRenaming = true;
                this.m_ListArea.allowBuiltinResources = false;
                this.m_ListArea.allowUserRenderingHook = true;
                this.m_ListArea.allowFindNextShortcut = true;
                this.m_ListArea.foldersFirst = this.GetShouldShowFoldersFirst();
                this.m_ListArea.repaintCallback = (Action) Delegate.Combine(this.m_ListArea.repaintCallback, new Action(this.Repaint));
                this.m_ListArea.itemSelectedCallback = (Action<bool>) Delegate.Combine(this.m_ListArea.itemSelectedCallback, new Action<bool>(this.ListAreaItemSelectedCallback));
                this.m_ListArea.keyboardCallback = (Action) Delegate.Combine(this.m_ListArea.keyboardCallback, new Action(this.ListAreaKeyboardCallback));
                this.m_ListArea.gotKeyboardFocus = (Action) Delegate.Combine(this.m_ListArea.gotKeyboardFocus, new Action(this.ListGotKeyboardFocus));
                this.m_ListArea.drawLocalAssetHeader = (Func<Rect, float>) Delegate.Combine(this.m_ListArea.drawLocalAssetHeader, new Func<Rect, float>(this.DrawLocalAssetHeader));
                this.m_ListArea.assetStoreSearchEnded = (Action) Delegate.Combine(this.m_ListArea.assetStoreSearchEnded, new Action(this.AssetStoreSearchEndedCallback));
                this.m_ListArea.gridSize = this.m_StartGridSize;
                this.m_StartGridSize = Mathf.Clamp(this.m_StartGridSize, this.m_ListArea.minGridSize, this.m_ListArea.maxGridSize);
                this.m_LastFoldersGridSize = Mathf.Min(this.m_LastFoldersGridSize, (float) this.m_ListArea.maxGridSize);
                this.InitListArea();
                this.SyncFilterGUI();
                if (this.m_FolderTreeState == null)
                {
                    this.m_FolderTreeState = new TreeViewState();
                }
                this.m_FolderTreeState.renameOverlay.isRenamingFilename = true;
                if (this.m_AssetTreeState == null)
                {
                    this.m_AssetTreeState = new TreeViewState();
                }
                this.m_AssetTreeState.renameOverlay.isRenamingFilename = true;
                this.InitViewMode(this.m_ViewMode);
                this.m_SearchAreaMenu = new ExposablePopupMenu();
                this.RefreshSearchText();
                this.DefaultSetup();
            }
        }

        public bool Initialized()
        {
            return (this.m_ListArea != null);
        }

        private void InitListArea()
        {
            this.ShowAndHideFolderTreeSelectionAsNeeded();
            this.m_ListArea.Init(this.m_ListAreaRect, HierarchyType.Assets, this.m_SearchFilter, false);
            this.m_ListArea.InitSelection(Selection.instanceIDs);
        }

        private void InitSearchMenu()
        {
            SearchViewState searchViewState = this.GetSearchViewState();
            if (searchViewState != SearchViewState.NotSearching)
            {
                List<ExposablePopupMenu.ItemData> items = new List<ExposablePopupMenu.ItemData>();
                GUIStyle style = "ExposablePopupItem";
                GUIStyle style2 = "ExposablePopupItem";
                bool enabled = this.m_SearchFilter.folders.Length > 0;
                this.m_SearchAssetStore.text = this.m_ListArea.GetAssetStoreButtonText();
                bool on = searchViewState == SearchViewState.AllAssets;
                items.Add(new ExposablePopupMenu.ItemData(this.m_SearchAllAssets, !on ? style2 : style, on, true, 1));
                on = searchViewState == SearchViewState.SubFolders;
                items.Add(new ExposablePopupMenu.ItemData(this.m_SearchInFolders, !on ? style2 : style, on, enabled, 2));
                on = searchViewState == SearchViewState.AssetStore;
                items.Add(new ExposablePopupMenu.ItemData(this.m_SearchAssetStore, !on ? style2 : style, on, true, 3));
                GUIContent searchAllAssets = this.m_SearchAllAssets;
                switch (searchViewState)
                {
                    case SearchViewState.NotSearching:
                        searchAllAssets = this.m_SearchAssetStore;
                        break;

                    case SearchViewState.AllAssets:
                        searchAllAssets = this.m_SearchAllAssets;
                        break;

                    case SearchViewState.SubFolders:
                        searchAllAssets = this.m_SearchInFolders;
                        break;

                    case SearchViewState.AssetStore:
                        searchAllAssets = this.m_SearchAssetStore;
                        break;

                    default:
                        Debug.LogError("Unhandled enum");
                        break;
                }
                ExposablePopupMenu.PopupButtonData popupButtonData = new ExposablePopupMenu.PopupButtonData(searchAllAssets, s_Styles.exposablePopup);
                this.m_SearchAreaMenu.Init(items, 10f, 450f, popupButtonData, new Action<ExposablePopupMenu.ItemData>(this.SearchButtonClickedCallback));
            }
        }

        private void InitViewMode(ViewMode viewMode)
        {
            this.m_ViewMode = viewMode;
            this.m_FolderTree = null;
            this.m_AssetTree = null;
            this.useTreeViewSelectionInsteadOfMainSelection = false;
            if (this.m_ViewMode == ViewMode.OneColumn)
            {
                this.m_AssetTree = new TreeView(this, this.m_AssetTreeState);
                this.m_AssetTree.deselectOnUnhandledMouseDown = true;
                this.m_AssetTree.selectionChangedCallback = (Action<int[]>) Delegate.Combine(this.m_AssetTree.selectionChangedCallback, new Action<int[]>(this.AssetTreeSelectionCallback));
                this.m_AssetTree.keyboardInputCallback = (Action) Delegate.Combine(this.m_AssetTree.keyboardInputCallback, new Action(this.AssetTreeKeyboardInputCallback));
                this.m_AssetTree.contextClickItemCallback = (Action<int>) Delegate.Combine(this.m_AssetTree.contextClickItemCallback, new Action<int>(this.AssetTreeViewContextClick));
                this.m_AssetTree.contextClickOutsideItemsCallback = (Action) Delegate.Combine(this.m_AssetTree.contextClickOutsideItemsCallback, new Action(this.AssetTreeViewContextClickOutsideItems));
                this.m_AssetTree.itemDoubleClickedCallback = (Action<int>) Delegate.Combine(this.m_AssetTree.itemDoubleClickedCallback, new Action<int>(this.AssetTreeItemDoubleClickedCallback));
                this.m_AssetTree.onGUIRowCallback = (Action<int, Rect>) Delegate.Combine(this.m_AssetTree.onGUIRowCallback, new Action<int, Rect>(this.OnGUIAssetCallback));
                this.m_AssetTree.dragEndedCallback = (Action<int[], bool>) Delegate.Combine(this.m_AssetTree.dragEndedCallback, new Action<int[], bool>(this.AssetTreeDragEnded));
                string guid = AssetDatabase.AssetPathToGUID("Assets");
                AssetsTreeViewDataSource data = new AssetsTreeViewDataSource(this.m_AssetTree, AssetDatabase.GetInstanceIDFromGUID(guid), false, false) {
                    foldersFirst = this.GetShouldShowFoldersFirst()
                };
                this.m_AssetTree.Init(this.m_TreeViewRect, data, new AssetsTreeViewGUI(this.m_AssetTree), new AssetsTreeViewDragging(this.m_AssetTree));
                this.m_AssetTree.ReloadData();
            }
            else if (this.m_ViewMode == ViewMode.TwoColumns)
            {
                this.m_FolderTree = new TreeView(this, this.m_FolderTreeState);
                this.m_FolderTree.deselectOnUnhandledMouseDown = false;
                this.m_FolderTree.selectionChangedCallback = (Action<int[]>) Delegate.Combine(this.m_FolderTree.selectionChangedCallback, new Action<int[]>(this.FolderTreeSelectionCallback));
                this.m_FolderTree.contextClickItemCallback = (Action<int>) Delegate.Combine(this.m_FolderTree.contextClickItemCallback, new Action<int>(this.FolderTreeViewContextClick));
                this.m_FolderTree.onGUIRowCallback = (Action<int, Rect>) Delegate.Combine(this.m_FolderTree.onGUIRowCallback, new Action<int, Rect>(this.OnGUIAssetCallback));
                this.m_FolderTree.dragEndedCallback = (Action<int[], bool>) Delegate.Combine(this.m_FolderTree.dragEndedCallback, new Action<int[], bool>(this.FolderTreeDragEnded));
                this.m_FolderTree.Init(this.m_TreeViewRect, new ProjectBrowserColumnOneTreeViewDataSource(this.m_FolderTree), new ProjectBrowserColumnOneTreeViewGUI(this.m_FolderTree), new ProjectBrowserColumnOneTreeViewDragging(this.m_FolderTree));
                this.m_FolderTree.ReloadData();
            }
            float x = (this.m_ViewMode != ViewMode.OneColumn) ? 230f : 230f;
            base.minSize = new Vector2(x, 250f);
            base.maxSize = new Vector2(10000f, 10000f);
        }

        private bool IsShowingFolder(int folderInstanceID)
        {
            string assetPath = AssetDatabase.GetAssetPath(folderInstanceID);
            return new List<string>(this.m_SearchFilter.folders).Contains(assetPath);
        }

        private bool IsShowingFolderContents()
        {
            return (this.m_SearchFilter.folders.Length > 0);
        }

        private void ListAreaItemSelectedCallback(bool doubleClicked)
        {
            this.SetAsLastInteractedProjectBrowser();
            Selection.activeObject = null;
            int[] selection = this.m_ListArea.GetSelection();
            if (selection.Length > 0)
            {
                Selection.instanceIDs = selection;
                this.m_SearchFilter.searchArea = this.m_LastLocalAssetsSearchArea;
                this.m_InternalSelectionChange = true;
            }
            else if (AssetStoreAssetSelection.Count > 0)
            {
                Selection.activeObject = AssetStoreAssetInspector.Instance;
            }
            this.m_FocusSearchField = false;
            if ((Event.current.button == 1) && (Event.current.type == EventType.MouseDown))
            {
                this.m_ItemSelectedByRightClickThisEvent = true;
            }
            this.RefreshSelectedPath();
            this.m_DidSelectSearchResult = this.m_SearchFilter.IsSearching();
            if (doubleClicked)
            {
                this.OpenListAreaSelection();
            }
        }

        private void ListAreaKeyboardCallback()
        {
            if (Event.current.type == EventType.KeyDown)
            {
                KeyCode keyCode = Event.current.keyCode;
                switch (keyCode)
                {
                    case KeyCode.KeypadEnter:
                        break;

                    case KeyCode.UpArrow:
                        if (((Application.platform == RuntimePlatform.OSXEditor) && Event.current.command) && (this.m_ViewMode == ViewMode.TwoColumns))
                        {
                            this.ShowParentFolderOfCurrentlySelected();
                            Event.current.Use();
                        }
                        return;

                    case KeyCode.DownArrow:
                        if ((Application.platform == RuntimePlatform.OSXEditor) && Event.current.command)
                        {
                            Event.current.Use();
                            this.OpenListAreaSelection();
                        }
                        return;

                    default:
                        switch (keyCode)
                        {
                            case KeyCode.Backspace:
                                if ((Application.platform == RuntimePlatform.WindowsEditor) && (this.m_ViewMode == ViewMode.TwoColumns))
                                {
                                    this.ShowParentFolderOfCurrentlySelected();
                                    Event.current.Use();
                                }
                                return;

                            case KeyCode.Return:
                                break;

                            case KeyCode.F2:
                                if ((Application.platform == RuntimePlatform.WindowsEditor) && this.m_ListArea.BeginRename(0f))
                                {
                                    Event.current.Use();
                                }
                                return;

                            default:
                                return;
                        }
                        break;
                }
                if (Application.platform == RuntimePlatform.OSXEditor)
                {
                    if (this.m_ListArea.BeginRename(0f))
                    {
                        Event.current.Use();
                    }
                }
                else
                {
                    Event.current.Use();
                    this.OpenListAreaSelection();
                }
            }
        }

        private void ListGotKeyboardFocus()
        {
        }

        private void OnAssetBundleNameChanged()
        {
            if (this.m_ListArea != null)
            {
                this.InitListArea();
            }
        }

        private void OnAssetLabelsChanged()
        {
            if (this.Initialized())
            {
                this.SetupAssetLabelList();
                if (this.m_SearchFilter.IsSearching())
                {
                    this.InitListArea();
                }
            }
        }

        private void OnDestroy()
        {
            if (this.m_ListArea != null)
            {
                this.m_ListArea.OnDestroy();
            }
            if (this == s_LastInteractedProjectBrowser)
            {
                s_LastInteractedProjectBrowser = null;
            }
        }

        private void OnDisable()
        {
            EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.playmodeStateChanged, new EditorApplication.CallbackFunction(this.OnPlayModeStateChanged));
            EditorApplication.projectWindowChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.projectWindowChanged, new EditorApplication.CallbackFunction(this.OnProjectChanged));
            EditorApplication.assetLabelsChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.assetLabelsChanged, new EditorApplication.CallbackFunction(this.OnAssetLabelsChanged));
            EditorApplication.assetBundleNameChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.assetBundleNameChanged, new EditorApplication.CallbackFunction(this.OnAssetBundleNameChanged));
            s_ProjectBrowsers.Remove(this);
        }

        private void OnEnable()
        {
            base.titleContent = base.GetLocalizedTitleContent();
            s_ProjectBrowsers.Add(this);
            EditorApplication.projectWindowChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.projectWindowChanged, new EditorApplication.CallbackFunction(this.OnProjectChanged));
            EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.playmodeStateChanged, new EditorApplication.CallbackFunction(this.OnPlayModeStateChanged));
            EditorApplication.assetLabelsChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.assetLabelsChanged, new EditorApplication.CallbackFunction(this.OnAssetLabelsChanged));
            EditorApplication.assetBundleNameChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.assetBundleNameChanged, new EditorApplication.CallbackFunction(this.OnAssetBundleNameChanged));
            s_LastInteractedProjectBrowser = this;
        }

        private void OnEvent()
        {
            if (this.m_AssetTree != null)
            {
                this.m_AssetTree.OnEvent();
            }
            if (this.m_FolderTree != null)
            {
                this.m_FolderTree.OnEvent();
            }
            if (this.m_ListArea != null)
            {
                this.m_ListArea.OnEvent();
            }
        }

        private void OnGotFocus()
        {
        }

        private void OnGUI()
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            if (!this.Initialized())
            {
                this.Init();
            }
            this.m_ListKeyboardControlID = GUIUtility.GetControlID(FocusType.Keyboard);
            this.m_TreeViewKeyboardControlID = GUIUtility.GetControlID(FocusType.Keyboard);
            this.OnEvent();
            this.m_ToolbarHeight = EditorStyles.toolbar.fixedHeight;
            this.m_ItemSelectedByRightClickThisEvent = false;
            this.ResizeHandling(base.position.width, base.position.height - this.m_ToolbarHeight);
            this.CalculateRects();
            Event current = Event.current;
            Rect position = new Rect(0f, 0f, base.position.width, base.position.height);
            if ((current.type == EventType.MouseDown) && position.Contains(current.mousePosition))
            {
                this.EndPing();
                this.SetAsLastInteractedProjectBrowser();
            }
            if (this.m_GrabKeyboardFocusForListArea)
            {
                this.m_GrabKeyboardFocusForListArea = false;
                GUIUtility.keyboardControl = this.m_ListKeyboardControlID;
            }
            GUI.BeginGroup(position, GUIContent.none);
            this.TopToolbar();
            this.BottomBar();
            if (this.m_ViewMode == ViewMode.OneColumn)
            {
                if (this.m_SearchFilter.IsSearching())
                {
                    this.SearchAreaBar();
                    if (GUIUtility.keyboardControl == this.m_TreeViewKeyboardControlID)
                    {
                        GUIUtility.keyboardControl = this.m_ListKeyboardControlID;
                    }
                    this.m_ListArea.OnGUI(this.m_ListAreaRect, this.m_ListKeyboardControlID);
                }
                else
                {
                    if (GUIUtility.keyboardControl == this.m_ListKeyboardControlID)
                    {
                        GUIUtility.keyboardControl = this.m_TreeViewKeyboardControlID;
                    }
                    this.m_AssetTree.OnGUI(this.m_TreeViewRect, this.m_TreeViewKeyboardControlID);
                }
            }
            else
            {
                if (this.m_SearchFilter.IsSearching())
                {
                    this.SearchAreaBar();
                }
                else
                {
                    this.BreadCrumbBar();
                }
                this.m_FolderTree.OnGUI(this.m_TreeViewRect, this.m_TreeViewKeyboardControlID);
                EditorGUIUtility.DrawHorizontalSplitter(new Rect(this.m_ListAreaRect.x, this.m_ToolbarHeight, 1f, this.m_TreeViewRect.height));
                this.m_ListArea.OnGUI(this.m_ListAreaRect, this.m_ListKeyboardControlID);
                if ((this.m_SearchFilter.GetState() == SearchFilter.State.FolderBrowsing) && (this.m_ListArea.numItemsDisplayed == 0))
                {
                    Vector2 vector = EditorStyles.label.CalcSize(s_Styles.m_EmptyFolderText);
                    Rect rect2 = new Rect((this.m_ListAreaRect.x + 2f) + Mathf.Max((float) 0f, (float) ((this.m_ListAreaRect.width - vector.x) * 0.5f)), this.m_ListAreaRect.y + 10f, vector.x, 20f);
                    EditorGUI.BeginDisabledGroup(true);
                    GUI.Label(rect2, s_Styles.m_EmptyFolderText, EditorStyles.label);
                    EditorGUI.EndDisabledGroup();
                }
            }
            this.HandleContextClickInListArea(this.m_ListAreaRect);
            if (this.m_ListArea.gridSize != this.m_StartGridSize)
            {
                this.m_StartGridSize = this.m_ListArea.gridSize;
                if (this.m_SearchFilter.GetState() == SearchFilter.State.FolderBrowsing)
                {
                    this.m_LastFoldersGridSize = this.m_ListArea.gridSize;
                }
            }
            GUI.EndGroup();
            if (this.m_ViewMode == ViewMode.TwoColumns)
            {
                this.useTreeViewSelectionInsteadOfMainSelection = GUIUtility.keyboardControl == this.m_TreeViewKeyboardControlID;
            }
            if ((this.m_ViewMode == ViewMode.TwoColumns) && (GUIUtility.keyboardControl == this.m_TreeViewKeyboardControlID))
            {
                this.HandleCommandEventsForTreeView();
            }
            this.HandleCommandEvents();
        }

        private void OnGUIAssetCallback(int instanceID, Rect rect)
        {
            if (EditorApplication.projectWindowItemOnGUI != null)
            {
                string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(instanceID));
                EditorApplication.projectWindowItemOnGUI(guid, rect);
            }
        }

        private void OnInspectorUpdate()
        {
            if (this.m_ListArea != null)
            {
                this.m_ListArea.OnInspectorUpdate();
            }
        }

        private void OnLostFocus()
        {
            this.EndRenaming();
            EditorGUI.EndEditingActiveTextField();
        }

        private void OnPlayModeStateChanged()
        {
            this.EndRenaming();
        }

        private void OnProjectChanged()
        {
            if (this.m_AssetTree != null)
            {
                this.m_AssetTree.ReloadData();
                this.SetSearchFoldersFromCurrentSelection();
            }
            if (this.m_FolderTree != null)
            {
                this.m_FolderTree.ReloadData();
                this.SetSearchFolderFromFolderTreeSelection();
            }
            this.EnsureValidFolders();
            if (this.m_ListArea != null)
            {
                this.InitListArea();
            }
            this.RefreshSelectedPath();
            this.m_BreadCrumbs.Clear();
        }

        private void OnSelectionChange()
        {
            if (this.m_ListArea != null)
            {
                this.m_ListArea.InitSelection(Selection.instanceIDs);
                if (this.m_ViewMode == ViewMode.OneColumn)
                {
                    bool revealSelectionAndFrameLastSelected = !this.m_IsLocked;
                    this.m_AssetTree.SetSelection(Selection.instanceIDs, revealSelectionAndFrameLastSelected);
                }
                else if (((this.m_ViewMode == ViewMode.TwoColumns) && !this.m_InternalSelectionChange) && (!this.m_IsLocked && (Selection.instanceIDs.Length > 0)))
                {
                    int instanceID = Selection.instanceIDs[Selection.instanceIDs.Length - 1];
                    if (this.m_SearchFilter.IsSearching())
                    {
                        this.m_ListArea.Frame(instanceID, true, false);
                    }
                    else
                    {
                        this.FrameObjectInTwoColumnMode(instanceID, true, false);
                    }
                }
                this.m_InternalSelectionChange = false;
                if ((Selection.activeObject != null) && (Selection.activeObject.GetType() != typeof(AssetStoreAssetInspector)))
                {
                    this.m_ListArea.selectedAssetStoreAsset = false;
                    AssetStoreAssetSelection.Clear();
                }
                this.RefreshSelectedPath();
                base.Repaint();
            }
        }

        private static void OpenAssetSelection(int[] selectedInstanceIDs)
        {
            foreach (int num in selectedInstanceIDs)
            {
                if (AssetDatabase.Contains(num))
                {
                    AssetDatabase.OpenAsset(num);
                }
            }
            GUIUtility.ExitGUI();
        }

        private void OpenListAreaSelection()
        {
            int[] selection = this.m_ListArea.GetSelection();
            int length = selection.Length;
            if (length > 0)
            {
                int num2 = 0;
                foreach (int num3 in selection)
                {
                    if (ProjectWindowUtil.IsFolder(num3))
                    {
                        num2++;
                    }
                }
                if (num2 == length)
                {
                    if (this.m_ViewMode == ViewMode.TwoColumns)
                    {
                        this.SetFolderSelection(selection, false);
                    }
                    else if (this.m_ViewMode == ViewMode.OneColumn)
                    {
                        this.ClearSearch();
                        this.m_AssetTree.Frame(selection[0], true, false);
                    }
                    base.Repaint();
                    GUIUtility.ExitGUI();
                }
                else
                {
                    OpenAssetSelection(selection);
                    base.Repaint();
                    GUIUtility.ExitGUI();
                }
            }
        }

        private void OpenTreeViewTestWindow()
        {
            EditorWindow.GetWindow<TreeViewTestWindow>();
        }

        private void RefreshSearchText()
        {
            if (this.m_SearchFilter.folders.Length > 0)
            {
                string[] baseFolders = ProjectWindowUtil.GetBaseFolders(this.m_SearchFilter.folders);
                string str = string.Empty;
                string str2 = string.Empty;
                int num = 3;
                for (int i = 0; (i < baseFolders.Length) && (i < num); i++)
                {
                    if (i > 0)
                    {
                        str = str + ", ";
                    }
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(baseFolders[i]);
                    str = str + "'" + fileNameWithoutExtension + "'";
                    if ((i == 0) && (fileNameWithoutExtension != "Assets"))
                    {
                        str2 = baseFolders[i];
                    }
                }
                if (baseFolders.Length > num)
                {
                    str = str + " +";
                }
                this.m_SearchInFolders.text = str;
                this.m_SearchInFolders.tooltip = str2;
            }
            else
            {
                this.m_SearchInFolders.text = "Selected folder";
                this.m_SearchInFolders.tooltip = string.Empty;
            }
            this.m_BreadCrumbs.Clear();
            this.InitSearchMenu();
        }

        private void RefreshSelectedPath()
        {
            if (Selection.activeObject != null)
            {
                this.m_SelectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            }
            else
            {
                this.m_SelectedPath = string.Empty;
            }
            this.m_SelectedPathSplitted.Clear();
        }

        private void RefreshSplittedSelectedPath()
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            this.m_SelectedPathSplitted.Clear();
            if (string.IsNullOrEmpty(this.m_SelectedPath))
            {
                this.m_SelectedPathSplitted.Add(new GUIContent());
            }
            else
            {
                string selectedPath = this.m_SelectedPath;
                if (this.m_SelectedPath.StartsWith("assets/", StringComparison.CurrentCultureIgnoreCase))
                {
                    selectedPath = this.m_SelectedPath.Substring("assets/".Length);
                }
                if (this.m_SearchFilter.GetState() == SearchFilter.State.FolderBrowsing)
                {
                    this.m_SelectedPathSplitted.Add(new GUIContent(Path.GetFileName(this.m_SelectedPath), AssetDatabase.GetCachedIcon(this.m_SelectedPath)));
                }
                else
                {
                    float num = ((base.position.width - this.m_DirectoriesAreaWidth) - 55f) - 16f;
                    if ((s_Styles.selectedPathLabel.CalcSize(GUIContent.Temp(selectedPath)).x + 25f) > num)
                    {
                        char[] separator = new char[] { '/' };
                        string[] strArray = selectedPath.Split(separator);
                        string path = "Assets/";
                        for (int i = 0; i < strArray.Length; i++)
                        {
                            path = path + strArray[i];
                            Texture cachedIcon = AssetDatabase.GetCachedIcon(path);
                            this.m_SelectedPathSplitted.Add(new GUIContent(strArray[i], cachedIcon));
                            path = path + "/";
                        }
                    }
                    else
                    {
                        this.m_SelectedPathSplitted.Add(new GUIContent(selectedPath, AssetDatabase.GetCachedIcon(this.m_SelectedPath)));
                    }
                }
            }
        }

        private void ResizeHandling(float width, float height)
        {
            if (this.m_ViewMode != ViewMode.OneColumn)
            {
                Rect dragRect = new Rect(this.m_DirectoriesAreaWidth, this.m_ToolbarHeight, 5f, height);
                dragRect = EditorGUIUtility.HandleHorizontalSplitter(dragRect, base.position.width, this.k_MinDirectoriesAreaWidth, 230f - this.k_MinDirectoriesAreaWidth);
                this.m_DirectoriesAreaWidth = dragRect.x;
                float num = base.position.width - this.m_DirectoriesAreaWidth;
                if (num != this.m_LastListWidth)
                {
                    this.RefreshSplittedSelectedPath();
                }
                this.m_LastListWidth = num;
            }
        }

        private void SearchAreaBar()
        {
            GUI.Label(this.m_ListHeaderRect, GUIContent.none, s_Styles.topBarBg);
            Rect listHeaderRect = this.m_ListHeaderRect;
            listHeaderRect.x += 5f;
            listHeaderRect.width -= 10f;
            listHeaderRect.y++;
            GUIStyle boldLabel = EditorStyles.boldLabel;
            GUI.Label(listHeaderRect, s_Styles.m_SearchIn, boldLabel);
            if (this.m_SearchAreaMenuOffset < 0f)
            {
                this.m_SearchAreaMenuOffset = boldLabel.CalcSize(s_Styles.m_SearchIn).x;
            }
            listHeaderRect.x += this.m_SearchAreaMenuOffset + 7f;
            listHeaderRect.width -= this.m_SearchAreaMenuOffset + 7f;
            listHeaderRect.width = this.m_SearchAreaMenu.OnGUI(listHeaderRect);
        }

        private void SearchButtonClickedCallback(ExposablePopupMenu.ItemData itemClicked)
        {
            if (!itemClicked.m_On)
            {
                this.SetSearchViewState((SearchViewState) ((int) itemClicked.m_UserData));
                if ((this.m_SearchFilter.searchArea == SearchFilter.SearchArea.AllAssets) || (this.m_SearchFilter.searchArea == SearchFilter.SearchArea.SelectedFolders))
                {
                    this.m_LastLocalAssetsSearchArea = this.m_SearchFilter.searchArea;
                }
            }
        }

        private void SearchField()
        {
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinWidth(65f), GUILayout.MaxWidth(300f) };
            Rect position = GUILayoutUtility.GetRect(0f, EditorGUILayout.kLabelFloatMaxW * 1.5f, 16f, 16f, EditorStyles.toolbarSearchField, options);
            int id = GUIUtility.GetControlID(s_HashForSearchField, FocusType.Passive, position);
            if (this.m_FocusSearchField)
            {
                GUIUtility.keyboardControl = id;
                EditorGUIUtility.editingTextField = true;
                if (Event.current.type == EventType.Repaint)
                {
                    this.m_FocusSearchField = false;
                }
            }
            Event current = Event.current;
            if (((current.type == EventType.KeyDown) && ((current.keyCode == KeyCode.DownArrow) || (current.keyCode == KeyCode.UpArrow))) && (GUIUtility.keyboardControl == id))
            {
                if (!this.m_ListArea.IsLastClickedItemVisible())
                {
                    this.m_ListArea.SelectFirst();
                }
                GUIUtility.keyboardControl = this.m_ListKeyboardControlID;
                current.Use();
            }
            string str = EditorGUI.ToolbarSearchField(id, position, this.m_SearchFieldText, false);
            if ((str != this.m_SearchFieldText) || this.m_FocusSearchField)
            {
                this.m_SearchFieldText = str;
                this.m_SearchFilter.SearchFieldStringToFilter(this.m_SearchFieldText);
                this.SyncFilterGUI();
                this.TopBarSearchSettingsChanged();
                base.Repaint();
            }
        }

        private void SelectAll()
        {
            if (this.m_ViewMode == ViewMode.OneColumn)
            {
                if (this.m_SearchFilter.IsSearching())
                {
                    this.m_ListArea.SelectAll();
                }
                else
                {
                    int[] rowIDs = this.m_AssetTree.GetRowIDs();
                    this.m_AssetTree.SetSelection(rowIDs, false);
                    this.AssetTreeSelectionCallback(rowIDs);
                }
            }
            else if (this.m_ViewMode == ViewMode.TwoColumns)
            {
                this.m_ListArea.SelectAll();
            }
            else
            {
                Debug.LogError("Missing implementation for ViewMode " + this.m_ViewMode);
            }
        }

        private void SelectAssetsFolder()
        {
            this.ShowFolderContents(ProjectBrowserColumnOneTreeViewDataSource.GetAssetsFolderInstanceID(), true);
        }

        private void SetAsLastInteractedProjectBrowser()
        {
            s_LastInteractedProjectBrowser = this;
        }

        private void SetFolderSelection(int[] selectedInstanceIDs, bool revealSelectionAndFrameLastSelected)
        {
            this.m_FolderTree.SetSelection(selectedInstanceIDs, revealSelectionAndFrameLastSelected);
            this.SetFoldersInSearchFilter(selectedInstanceIDs);
            this.FolderTreeSelectionChanged(true);
        }

        private void SetFoldersInSearchFilter(int[] selectedInstanceIDs)
        {
            this.m_SearchFilter.folders = GetFolderPathsFromInstanceIDs(selectedInstanceIDs);
            this.EnsureValidFolders();
            if ((selectedInstanceIDs.Length > 0) && (this.m_LastFoldersGridSize > 0f))
            {
                this.m_ListArea.gridSize = (int) this.m_LastFoldersGridSize;
            }
        }

        private void SetOneColumn()
        {
            this.SetViewMode(ViewMode.OneColumn);
        }

        public void SetSearch(string searchString)
        {
            this.SetSearch(SearchFilter.CreateSearchFilterFromString(searchString));
        }

        public void SetSearch(SearchFilter searchFilter)
        {
            this.m_SearchFilter = searchFilter;
            this.m_SearchFieldText = searchFilter.FilterToSearchFieldString();
            this.TopBarSearchSettingsChanged();
        }

        private void SetSearchFolderFromFolderTreeSelection()
        {
            if (this.m_FolderTree != null)
            {
                this.m_SearchFilter.folders = GetFolderPathsFromInstanceIDs(this.m_FolderTree.GetSelection());
            }
        }

        private void SetSearchFoldersFromCurrentSelection()
        {
            HashSet<string> source = new HashSet<string>();
            foreach (int num in Selection.instanceIDs)
            {
                if (AssetDatabase.Contains(num))
                {
                    string assetPath = AssetDatabase.GetAssetPath(num);
                    if (AssetDatabase.IsValidFolder(assetPath))
                    {
                        source.Add(assetPath);
                    }
                    else
                    {
                        string containingFolder = ProjectWindowUtil.GetContainingFolder(assetPath);
                        if (!string.IsNullOrEmpty(containingFolder))
                        {
                            source.Add(containingFolder);
                        }
                    }
                }
            }
            this.m_SearchFilter.folders = ProjectWindowUtil.GetBaseFolders(source.ToArray<string>());
        }

        private void SetSearchViewState(SearchViewState state)
        {
            switch (state)
            {
                case SearchViewState.NotSearching:
                    Debug.LogError("Invalid search mode as setter");
                    break;

                case SearchViewState.AllAssets:
                    this.m_SearchFilter.searchArea = SearchFilter.SearchArea.AllAssets;
                    break;

                case SearchViewState.SubFolders:
                    this.m_SearchFilter.searchArea = SearchFilter.SearchArea.SelectedFolders;
                    break;

                case SearchViewState.AssetStore:
                    this.m_SearchFilter.searchArea = SearchFilter.SearchArea.AssetStore;
                    break;
            }
            this.InitSearchMenu();
            this.InitListArea();
        }

        private void SetTwoColumns()
        {
            this.SetViewMode(ViewMode.TwoColumns);
        }

        private void SetupAssetLabelList()
        {
            Dictionary<string, float> allLabels = AssetDatabase.GetAllLabels();
            this.m_AssetLabels = new PopupList.InputData();
            this.m_AssetLabels.m_CloseOnSelection = false;
            this.m_AssetLabels.m_AllowCustom = true;
            this.m_AssetLabels.m_OnSelectCallback = new PopupList.OnSelectCallback(this.AssetLabelListCallback);
            this.m_AssetLabels.m_MaxCount = 15;
            this.m_AssetLabels.m_SortAlphabetically = true;
            foreach (KeyValuePair<string, float> pair in allLabels)
            {
                PopupList.ListElement element = this.m_AssetLabels.NewOrMatchingElement(pair.Key);
                if (element.filterScore < pair.Value)
                {
                    element.filterScore = pair.Value;
                }
            }
        }

        private void SetupDroplists()
        {
            this.SetupAssetLabelList();
            this.m_ObjectTypes = new PopupList.InputData();
            this.m_ObjectTypes.m_CloseOnSelection = false;
            this.m_ObjectTypes.m_AllowCustom = false;
            this.m_ObjectTypes.m_OnSelectCallback = new PopupList.OnSelectCallback(this.TypeListCallback);
            this.m_ObjectTypes.m_SortAlphabetically = false;
            this.m_ObjectTypes.m_MaxCount = 0;
            string[] typesDisplayNames = this.GetTypesDisplayNames();
            for (int i = 0; i < typesDisplayNames.Length; i++)
            {
                PopupList.ListElement element = this.m_ObjectTypes.NewOrMatchingElement(typesDisplayNames[i]);
                if (i == 0)
                {
                    element.selected = true;
                }
            }
        }

        private void SetViewMode(ViewMode newViewMode)
        {
            if (this.m_ViewMode != newViewMode)
            {
                this.EndRenaming();
                this.InitViewMode((this.m_ViewMode != ViewMode.OneColumn) ? ViewMode.OneColumn : ViewMode.TwoColumns);
                if (Selection.activeInstanceID != 0)
                {
                    this.FrameObjectPrivate(Selection.activeInstanceID, !this.m_IsLocked, false);
                }
                base.RepaintImmediately();
            }
        }

        private void ShowAndHideFolderTreeSelectionAsNeeded()
        {
            if ((this.m_ViewMode == ViewMode.TwoColumns) && (this.m_FolderTree != null))
            {
                bool flag = false;
                int[] selection = this.m_FolderTree.GetSelection();
                if (selection.Length > 0)
                {
                    flag = GetItemType(selection[0]) == ItemType.SavedFilter;
                }
                switch (this.GetSearchViewState())
                {
                    case SearchViewState.NotSearching:
                    case SearchViewState.SubFolders:
                        if (!flag)
                        {
                            this.m_FolderTree.SetSelection(GetFolderInstanceIDs(this.m_SearchFilter.folders), true);
                        }
                        break;

                    case SearchViewState.AllAssets:
                    case SearchViewState.AssetStore:
                        if (!flag)
                        {
                            this.m_FolderTree.SetSelection(new int[0], false);
                        }
                        break;
                }
            }
        }

        public static void ShowAssetStoreHitsWhileSearchingLocalAssetsChanged()
        {
            foreach (ProjectBrowser browser in s_ProjectBrowsers)
            {
                browser.m_ListArea.ShowAssetStoreHitCountWhileSearchingLocalAssetsChanged();
                browser.InitSearchMenu();
            }
        }

        protected virtual void ShowButton(Rect r)
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            this.m_IsLocked = GUI.Toggle(r, this.m_IsLocked, GUIContent.none, s_Styles.lockButton);
        }

        private void ShowFolderContents(int folderInstanceID, bool revealAndFrameInFolderTree)
        {
            if (this.m_ViewMode != ViewMode.TwoColumns)
            {
                Debug.LogError("ShowFolderContents should only be called in two column mode");
            }
            if (folderInstanceID != 0)
            {
                string assetPath = AssetDatabase.GetAssetPath(folderInstanceID);
                this.m_SearchFilter.ClearSearch();
                this.m_SearchFilter.folders = new string[] { assetPath };
                int[] selectedIDs = new int[] { folderInstanceID };
                this.m_FolderTree.SetSelection(selectedIDs, revealAndFrameInFolderTree);
                this.FolderTreeSelectionChanged(true);
            }
        }

        internal void ShowObjectsInList(int[] instanceIDs)
        {
            if (this.m_ViewMode == ViewMode.TwoColumns)
            {
                this.m_ListArea.ShowObjectsInList(instanceIDs);
                this.m_FolderTree.SetSelection(new int[0], false);
            }
            else if (this.m_ViewMode == ViewMode.OneColumn)
            {
                foreach (int num in Selection.instanceIDs)
                {
                    this.m_AssetTree.Frame(num, true, false);
                }
            }
        }

        private void ShowParentFolderOfCurrentlySelected()
        {
            if (this.IsShowingFolderContents())
            {
                int[] selection = this.m_FolderTree.GetSelection();
                if (selection.Length == 1)
                {
                    TreeViewItem item = this.m_FolderTree.FindNode(selection[0]);
                    if (((item != null) && (item.parent != null)) && (item.id != ProjectBrowserColumnOneTreeViewDataSource.GetAssetsFolderInstanceID()))
                    {
                        int[] selectedInstanceIDs = new int[] { item.parent.id };
                        this.SetFolderSelection(selectedInstanceIDs, true);
                        this.m_ListArea.Frame(item.id, true, false);
                        Selection.activeInstanceID = item.id;
                    }
                }
            }
        }

        private static void ShowSelectedObjectsInLastInteractedProjectBrowser()
        {
            if (s_LastInteractedProjectBrowser != null)
            {
                int[] instanceIDs = Selection.instanceIDs;
                s_LastInteractedProjectBrowser.ShowObjectsInList(instanceIDs);
            }
        }

        private void SyncFilterGUI()
        {
            List<string> list = new List<string>(this.m_SearchFilter.assetLabels);
            foreach (PopupList.ListElement element in this.m_AssetLabels.m_ListElements)
            {
                element.selected = list.Contains(element.text);
            }
            List<string> list2 = new List<string>(this.m_SearchFilter.classNames);
            foreach (PopupList.ListElement element2 in this.m_ObjectTypes.m_ListElements)
            {
                element2.selected = list2.Contains(element2.text);
            }
            this.m_SearchFieldText = this.m_SearchFilter.FilterToSearchFieldString();
        }

        private void ToggleExpansionAnimationPreference()
        {
            bool @bool = EditorPrefs.GetBool("TreeViewExpansionAnimation", false);
            EditorPrefs.SetBool("TreeViewExpansionAnimation", !@bool);
            InternalEditorUtility.RequestScriptReload();
        }

        private void TopBarSearchSettingsChanged()
        {
            if (!this.m_SearchFilter.IsSearching())
            {
                if (this.m_DidSelectSearchResult)
                {
                    this.m_DidSelectSearchResult = false;
                    this.FrameObjectPrivate(Selection.activeInstanceID, true, false);
                    if (GUIUtility.keyboardControl == 0)
                    {
                        if (this.m_ViewMode == ViewMode.OneColumn)
                        {
                            GUIUtility.keyboardControl = this.m_TreeViewKeyboardControlID;
                        }
                        else if (this.m_ViewMode == ViewMode.TwoColumns)
                        {
                            GUIUtility.keyboardControl = this.m_ListKeyboardControlID;
                        }
                    }
                }
                else if (((this.m_ViewMode == ViewMode.TwoColumns) && (GUIUtility.keyboardControl == 0)) && ((this.m_LastFolders != null) && (this.m_LastFolders.Length > 0)))
                {
                    this.m_SearchFilter.folders = this.m_LastFolders;
                    this.SetFolderSelection(GetFolderInstanceIDs(this.m_LastFolders), true);
                }
            }
            else
            {
                this.InitSearchMenu();
            }
            this.InitListArea();
        }

        private void TopToolbar()
        {
            GUILayout.BeginArea(new Rect(0f, 0f, base.position.width, this.m_ToolbarHeight));
            GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
            float num = base.position.width - this.m_DirectoriesAreaWidth;
            float pixels = 4f;
            if (num >= 500f)
            {
                pixels = 10f;
            }
            this.CreateDropdown();
            GUILayout.FlexibleSpace();
            GUILayout.Space(pixels * 2f);
            this.SearchField();
            GUILayout.Space(pixels);
            this.TypeDropDown();
            this.AssetLabelsDropDown();
            if (this.m_ViewMode == ViewMode.TwoColumns)
            {
                this.ButtonSaveFilter();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void TypeDropDown()
        {
            if (EditorGUI.ButtonMouseDown(GUILayoutUtility.GetRect(s_Styles.m_FilterByType, EditorStyles.toolbarButton), s_Styles.m_FilterByType, FocusType.Passive, EditorStyles.toolbarButton))
            {
                PopupWindow.Show(GUILayoutUtility.topLevel.GetLast(), new PopupList(this.m_ObjectTypes));
            }
        }

        public void TypeListCallback(PopupList.ListElement element)
        {
            if (!Event.current.control)
            {
                foreach (PopupList.ListElement element2 in this.m_ObjectTypes.m_ListElements)
                {
                    if (element2 != element)
                    {
                        element2.selected = false;
                    }
                }
            }
            element.selected = !element.selected;
            if (<>f__am$cache31 == null)
            {
                <>f__am$cache31 = item => item.selected;
            }
            if (<>f__am$cache32 == null)
            {
                <>f__am$cache32 = item => item.text;
            }
            string[] strArray = this.m_ObjectTypes.m_ListElements.Where<PopupList.ListElement>(<>f__am$cache31).Select<PopupList.ListElement, string>(<>f__am$cache32).ToArray<string>();
            for (int i = 0; i < strArray.Length; i++)
            {
                strArray[i] = strArray[i];
            }
            this.m_SearchFilter.classNames = strArray;
            this.m_SearchFieldText = this.m_SearchFilter.FilterToSearchFieldString();
            this.TopBarSearchSettingsChanged();
            base.Repaint();
        }

        private string ValidateCreateNewAssetPath(string pathName)
        {
            if ((((this.m_ViewMode == ViewMode.TwoColumns) && (this.m_SearchFilter.GetState() == SearchFilter.State.FolderBrowsing)) && ((this.m_SearchFilter.folders.Length > 0) && !pathName.StartsWith("assets/", StringComparison.CurrentCultureIgnoreCase))) && (Selection.GetFiltered(typeof(Object), SelectionMode.Assets).Length == 0))
            {
                pathName = Path.Combine(this.m_SearchFilter.folders[0], pathName);
                pathName = pathName.Replace(@"\", "/");
            }
            return pathName;
        }

        private bool ValidateFilter(int savedFilterID, SearchFilter filter)
        {
            if (filter == null)
            {
                return false;
            }
            switch (filter.GetState())
            {
                case SearchFilter.State.FolderBrowsing:
                case SearchFilter.State.SearchingInFolders:
                    foreach (string str in filter.folders)
                    {
                        if (AssetDatabase.GetMainAssetInstanceID(str) == 0)
                        {
                            if (EditorUtility.DisplayDialog("Folder not found", "The folder '" + str + "' might have been deleted or belong to another project. Do you want to delete the favorite?", "Delete", "Cancel"))
                            {
                                SavedSearchFilters.RemoveSavedFilter(savedFilterID);
                            }
                            return false;
                        }
                    }
                    break;
            }
            return true;
        }

        public float listAreaGridSize
        {
            get
            {
                return (float) this.m_ListArea.gridSize;
            }
        }

        private bool useTreeViewSelectionInsteadOfMainSelection
        {
            get
            {
                return this.m_UseTreeViewSelectionInsteadOfMainSelection;
            }
            set
            {
                this.m_UseTreeViewSelectionInsteadOfMainSelection = value;
            }
        }

        internal class AssetStoreItemContextMenu
        {
            private AssetStoreItemContextMenu()
            {
            }

            private void OpenAssetStoreWindow()
            {
                AssetStoreAsset firstAsset = AssetStoreAssetSelection.GetFirstAsset();
                if (firstAsset != null)
                {
                    AssetStoreAssetInspector.OpenItemInAssetStore(firstAsset);
                }
            }

            internal static void Show()
            {
                GenericMenu menu = new GenericMenu();
                GUIContent content = new GUIContent("Show in Asset Store window");
                AssetStoreAsset firstAsset = AssetStoreAssetSelection.GetFirstAsset();
                if ((firstAsset != null) && (firstAsset.id != 0))
                {
                    menu.AddItem(content, false, new GenericMenu.MenuFunction(new ProjectBrowser.AssetStoreItemContextMenu().OpenAssetStoreWindow));
                }
                else
                {
                    menu.AddDisabledItem(content);
                }
                menu.ShowAsContext();
            }
        }

        internal class BreadCrumbListMenu
        {
            private static ProjectBrowser m_Caller;
            private string m_SubFolder;

            private BreadCrumbListMenu(string subFolder)
            {
                this.m_SubFolder = subFolder;
            }

            private void SelectSubFolder()
            {
                int mainAssetInstanceID = AssetDatabase.GetMainAssetInstanceID(this.m_SubFolder);
                if (mainAssetInstanceID != 0)
                {
                    m_Caller.ShowFolderContents(mainAssetInstanceID, false);
                }
            }

            internal static void Show(string folder, string currentSubFolder, Rect activatorRect, ProjectBrowser caller)
            {
                m_Caller = caller;
                string[] subFolders = AssetDatabase.GetSubFolders(folder);
                GenericMenu menu = new GenericMenu();
                if (subFolders.Length >= 0)
                {
                    currentSubFolder = Path.GetFileName(currentSubFolder);
                    foreach (string str in subFolders)
                    {
                        string fileName = Path.GetFileName(str);
                        menu.AddItem(new GUIContent(fileName), fileName == currentSubFolder, new GenericMenu.MenuFunction(new ProjectBrowser.BreadCrumbListMenu(str).SelectSubFolder));
                        menu.ShowAsContext();
                    }
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent("No sub folders..."));
                }
                menu.DropDown(activatorRect);
            }
        }

        internal enum ItemType
        {
            Asset,
            SavedFilter
        }

        internal class SavedFiltersContextMenu
        {
            private int m_SavedFilterInstanceID;

            private SavedFiltersContextMenu(int savedFilterInstanceID)
            {
                this.m_SavedFilterInstanceID = savedFilterInstanceID;
            }

            private void Delete()
            {
                ProjectBrowser.DeleteFilter(this.m_SavedFilterInstanceID);
            }

            internal static void Show(int savedFilterInstanceID)
            {
                GUIContent content = new GUIContent("Delete");
                GenericMenu menu = new GenericMenu();
                menu.AddItem(content, false, new GenericMenu.MenuFunction(new ProjectBrowser.SavedFiltersContextMenu(savedFilterInstanceID).Delete));
                menu.ShowAsContext();
            }
        }

        public enum SearchViewState
        {
            NotSearching,
            AllAssets,
            SubFolders,
            AssetStore
        }

        private class Styles
        {
            public GUIStyle background = "ObjectPickerBackground";
            public GUIStyle bottomBarBg = "ProjectBrowserBottomBarBg";
            public GUIStyle bottomResize = "WindowBottomResize";
            public GUIStyle exposablePopup = GetStyle("ExposablePopupMenu");
            public GUIStyle exposablePopupItem = GetStyle("ExposablePopupItem");
            public GUIStyle foldout = "AC RightArrow";
            public GUIStyle largeStatus = "ObjectPickerLargeStatus";
            public GUIStyle lockButton = "IN LockButton";
            public GUIContent m_CreateDropdownContent = new GUIContent("Create");
            public GUIContent m_EmptyFolderText = new GUIContent("This folder is empty");
            public GUIContent m_FilterByLabel = new GUIContent(EditorGUIUtility.FindTexture("FilterByLabel"), "Search by Label");
            public GUIContent m_FilterByType = new GUIContent(EditorGUIUtility.FindTexture("FilterByType"), "Search by Type");
            public GUIContent m_SaveFilterContent = new GUIContent(EditorGUIUtility.FindTexture("Favorite"), "Save search");
            public GUIContent m_SearchIn = new GUIContent("Search:");
            public GUIContent m_ShowChildAssetsContent = new GUIContent(string.Empty, EditorGUIUtility.FindTexture("UnityEditor.HierarchyWindow"), "Toggle visibility of child assets in folders");
            public GUIStyle previewBackground = "PopupCurveSwatchBackground";
            public GUIStyle previewTextureBackground = "ObjectPickerPreviewBackground";
            public GUIStyle selectedPathLabel = "Label";
            public GUIStyle smallStatus = "ObjectPickerSmallStatus";
            public GUIStyle tab = "ObjectPickerTab";
            public GUIStyle toolbarBack = "ObjectPickerToolbar";
            public GUIStyle topBarBg = "ProjectBrowserTopBarBg";

            private static GUIStyle GetStyle(string styleName)
            {
                return styleName;
            }
        }

        private enum ViewMode
        {
            OneColumn,
            TwoColumns
        }
    }
}

