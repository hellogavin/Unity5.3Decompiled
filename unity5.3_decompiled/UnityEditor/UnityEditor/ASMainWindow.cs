namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    [EditorWindowTitle(title="Server", useTypeNameAsIconName=true)]
    internal class ASMainWindow : EditorWindow, IHasCustomMenu
    {
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map11;
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map12;
        internal ASServerAdminWindow asAdminWin;
        internal ASCommitWindow asCommitWin;
        internal ASConfigWindow asConfigWin;
        internal ASHistoryWindow asHistoryWin;
        internal ASUpdateWindow asUpdateWin;
        private GUIContent[] changesetContents;
        private string[] commitDropDownMenuItems = new string[] { "Commit", string.Empty, "Compare", "Compare Binary", string.Empty, "Discard" };
        private string commitMessage;
        private bool committing;
        private string connectionString = string.Empty;
        public static Constants constants;
        private string[] dropDownMenuItems = new string[] { "Connection", string.Empty, "Show History", "Discard Changes", string.Empty, "Server Administration" };
        private bool error;
        private bool focusCommitMessage;
        private Vector2 iconSize = new Vector2(16f, 16f);
        private bool isInitialUpdate;
        private const Page lastMainPage = Page.Commit;
        private int lastRevertSelectionChanged;
        private ListViewState lv = new ListViewState(0);
        private bool m_CheckedMaint;
        public SearchField m_SearchField = new SearchField();
        public ShowSearchField m_SearchToShow = ShowSearchField.HistoryList;
        public ShowSearchField m_ShowSearch;
        private int maxNickLength = 1;
        private bool mySelection;
        private bool needsSetup = true;
        private string[] pageTitles = new string[] { "Overview", "Update", "Commit", string.Empty };
        private ParentViewState pv = new ParentViewState();
        private bool pvHasSelection;
        private Page selectedPage = Page.NotInitialized;
        private bool selectionChangedWhileCommitting;
        public Changeset[] sharedChangesets;
        public AssetsItem[] sharedCommits;
        public AssetsItem[] sharedDeletedItems;
        private bool showSmallWindow;
        private bool somethingDiscardableSelected;
        private SplitterState splitter;
        private string[] unconfiguredDropDownMenuItems = new string[] { "Connection", string.Empty, "Server Administration" };
        private bool wasHidingButtons;
        private int widthToHideButtons = 0x24f;

        public ASMainWindow()
        {
            float[] relativeSizes = new float[] { 50f, 50f };
            int[] minSizes = new int[] { 80, 80 };
            this.splitter = new SplitterState(relativeSizes, minSizes, null);
            this.commitMessage = string.Empty;
            this.lastRevertSelectionChanged = -1;
            base.position = new Rect(50f, 50f, 800f, 600f);
        }

        private void ActionDiscardChanges()
        {
            if (EditorUtility.DisplayDialog("Discard all changes", "Are you sure you want to discard all local changes made in the project?", "Discard", "Cancel"))
            {
                AssetServer.RemoveMaintErrorsFromConsole();
                if (!ASEditorBackend.SettingsIfNeeded())
                {
                    Debug.Log("Asset Server connection for current project is not set up");
                    this.error = true;
                }
                else
                {
                    this.error = false;
                    AssetServer.SetAfterActionFinishedCallback("ASEditorBackend", "CBDoDiscardChanges");
                    AssetServer.DoUpdateStatusOnNextTick();
                }
            }
        }

        private void ActionRefresh()
        {
            switch (this.selectedPage)
            {
                case Page.Overview:
                    AssetServer.CheckForServerUpdates();
                    this.InitiateRefreshAssetsAndUpdateStatusWithCallback("CBInitOverviewPage");
                    break;

                case Page.Update:
                    AssetServer.CheckForServerUpdates();
                    if (this.UpdateNeedsRefresh())
                    {
                        this.InitiateUpdateStatusWithCallback("CBInitUpdatePage");
                    }
                    break;

                case Page.Commit:
                    this.asCommitWin.InitiateReinit();
                    break;

                case Page.History:
                    AssetServer.CheckForServerUpdates();
                    if (this.UpdateNeedsRefresh())
                    {
                        this.InitiateUpdateStatusWithCallback("CBInitHistoryPage");
                    }
                    break;

                default:
                    this.Reinit();
                    break;
            }
        }

        private void ActionSwitchPage(object page)
        {
            this.SwitchSelectedPage((Page) ((int) page));
        }

        public virtual void AddItemsToMenu(GenericMenu menu)
        {
            if (!this.needsSetup)
            {
                menu.AddItem(new GUIContent("Refresh"), false, new GenericMenu.MenuFunction(this.ActionRefresh));
                menu.AddSeparator(string.Empty);
            }
            menu.AddItem(new GUIContent("Connection"), false, new GenericMenu.MenuFunction2(this.ActionSwitchPage), Page.ServerConfig);
            menu.AddSeparator(string.Empty);
            if (!this.needsSetup)
            {
                menu.AddItem(new GUIContent("Show History"), false, new GenericMenu.MenuFunction2(this.ActionSwitchPage), Page.History);
                menu.AddItem(new GUIContent("Discard Changes"), false, new GenericMenu.MenuFunction(this.ActionDiscardChanges));
                menu.AddSeparator(string.Empty);
            }
            menu.AddItem(new GUIContent("Server Administration"), false, new GenericMenu.MenuFunction2(this.ActionSwitchPage), Page.Admin);
        }

        private void Awake()
        {
            this.pv.lv = new ListViewState(0);
            this.isInitialUpdate = true;
        }

        private void CancelCommit()
        {
            this.committing = false;
            if (this.selectionChangedWhileCommitting)
            {
                this.DoSelectionChange();
            }
        }

        private void CommitContextMenuClick(object userData, string[] options, int selected)
        {
            if (selected >= 0)
            {
                string key = this.commitDropDownMenuItems[selected];
                if (key != null)
                {
                    int num;
                    if (<>f__switch$map12 == null)
                    {
                        Dictionary<string, int> dictionary = new Dictionary<string, int>(4);
                        dictionary.Add("Commit", 0);
                        dictionary.Add("Compare", 1);
                        dictionary.Add("Compare Binary", 2);
                        dictionary.Add("Discard", 3);
                        <>f__switch$map12 = dictionary;
                    }
                    if (<>f__switch$map12.TryGetValue(key, out num))
                    {
                        switch (num)
                        {
                            case 0:
                                this.StartCommitting();
                                break;

                            case 1:
                                ASCommitWindow.DoShowDiff(ASCommitWindow.GetParentViewSelectedItems(this.pv, false, false), false);
                                break;

                            case 2:
                                ASCommitWindow.DoShowDiff(ASCommitWindow.GetParentViewSelectedItems(this.pv, false, false), true);
                                break;

                            case 3:
                                this.DoMyRevert(false);
                                break;
                        }
                    }
                }
            }
        }

        internal void CommitFinished(bool actionResult)
        {
            if (actionResult)
            {
                AssetServer.ClearCommitPersistentData();
                this.InitOverviewPage(true);
            }
            else
            {
                base.Repaint();
            }
        }

        public void CommitItemsChanged()
        {
            this.InitCommits();
            this.DisplayedItemsChanged();
            if (this.selectedPage == Page.Commit)
            {
                this.asCommitWin.Update();
            }
            base.Repaint();
        }

        public bool CommitNeedsRefresh()
        {
            return (((this.sharedCommits == null) || (this.sharedDeletedItems == null)) || AssetServer.GetRefreshCommit());
        }

        private void ContextMenuClick(object userData, string[] options, int selected)
        {
            if (selected >= 0)
            {
                string key = this.dropDownMenuItems[selected];
                if (key != null)
                {
                    int num;
                    if (<>f__switch$map11 == null)
                    {
                        Dictionary<string, int> dictionary = new Dictionary<string, int>(4);
                        dictionary.Add("Connection", 0);
                        dictionary.Add("Show History", 1);
                        dictionary.Add("Discard Changes", 2);
                        dictionary.Add("Server Administration", 3);
                        <>f__switch$map11 = dictionary;
                    }
                    if (<>f__switch$map11.TryGetValue(key, out num))
                    {
                        switch (num)
                        {
                            case 0:
                                this.ActionSwitchPage(Page.ServerConfig);
                                break;

                            case 1:
                                this.ActionSwitchPage(Page.History);
                                break;

                            case 2:
                                this.ActionDiscardChanges();
                                break;

                            case 3:
                                this.ActionSwitchPage(Page.Admin);
                                break;
                        }
                    }
                }
            }
        }

        public void DisplayedItemsChanged()
        {
            float[] relativeSizes = new float[2];
            bool flag = (this.sharedChangesets != null) && (this.sharedChangesets.Length != 0);
            bool flag2 = this.pv.lv.totalRows != 0;
            if ((flag && flag2) || (!flag && !flag2))
            {
                relativeSizes[0] = relativeSizes[1] = 0.5f;
            }
            else
            {
                relativeSizes[0] = !flag ? ((float) 0) : ((float) 1);
                relativeSizes[1] = !flag2 ? ((float) 0) : ((float) 1);
            }
            int[] minSizes = new int[] { 80, 80 };
            this.splitter = new SplitterState(relativeSizes, minSizes, null);
            this.DoSelectionChange();
        }

        private void DoCommit()
        {
            if ((this.commitMessage == string.Empty) && !EditorUtility.DisplayDialog("Commit without description", "Are you sure you want to commit with empty commit description message?", "Commit", "Cancel"))
            {
                GUIUtility.ExitGUI();
            }
            bool refreshCommit = AssetServer.GetRefreshCommit();
            ASCommitWindow window = new ASCommitWindow(this, ASCommitWindow.GetParentViewSelectedItems(this.pv, true, false).ToArray());
            window.InitiateReinit();
            if ((refreshCommit || window.lastTransferMovedDependencies) && ((!refreshCommit && !EditorUtility.DisplayDialog("Committing with dependencies", "Assets selected for committing have dependencies that will also be committed. Press Details to view full changeset", "Commit", "Details")) || refreshCommit))
            {
                this.committing = false;
                this.selectedPage = Page.Commit;
                window.description = this.commitMessage;
                if (refreshCommit)
                {
                    window.showReinitedWarning = 1;
                }
                this.asCommitWin = window;
                base.Repaint();
                GUIUtility.ExitGUI();
            }
            else
            {
                string[] itemsToCommit = window.GetItemsToCommit();
                AssetServer.SetAfterActionFinishedCallback("ASEditorBackend", "CBOverviewsCommitFinished");
                AssetServer.DoCommitOnNextTick(this.commitMessage, itemsToCommit);
                AssetServer.SetLastCommitMessage(this.commitMessage);
                window.AddToCommitMessageHistory(this.commitMessage);
                this.committing = false;
                GUIUtility.ExitGUI();
            }
        }

        private void DoCommitParentView()
        {
            bool shift = Event.current.shift;
            bool actionKey = EditorGUI.actionKey;
            int row = this.pv.lv.row;
            int num2 = -1;
            int file = -1;
            bool flag3 = false;
            IEnumerator enumerator = ListViewGUILayout.ListView(this.pv.lv, constants.background, new GUILayoutOption[0]).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    ChangeFlags changeFlags;
                    ListViewElement current = (ListViewElement) enumerator.Current;
                    if (((GUIUtility.keyboardControl == this.pv.lv.ID) && (Event.current.type == EventType.KeyDown)) && actionKey)
                    {
                        Event.current.Use();
                    }
                    if ((num2 == -1) && !this.pv.IndexToFolderAndFile(current.row, ref num2, ref file))
                    {
                        goto Label_0533;
                    }
                    ParentViewFolder folder = this.pv.folders[num2];
                    if (this.pv.selectedItems[current.row] && (Event.current.type == EventType.Repaint))
                    {
                        constants.entrySelected.Draw(current.position, false, false, false, false);
                    }
                    if (!this.committing)
                    {
                        if (ListViewGUILayout.HasMouseUp(current.position))
                        {
                            if (!shift && !actionKey)
                            {
                                flag3 |= ListViewGUILayout.MultiSelection(row, this.pv.lv.row, ref this.pv.initialSelectedItem, ref this.pv.selectedItems);
                            }
                        }
                        else if (ListViewGUILayout.HasMouseDown(current.position))
                        {
                            if (Event.current.clickCount == 2)
                            {
                                ASCommitWindow.DoShowDiff(ASCommitWindow.GetParentViewSelectedItems(this.pv, false, false), false);
                                GUIUtility.ExitGUI();
                            }
                            else
                            {
                                if ((!this.pv.selectedItems[current.row] || shift) || actionKey)
                                {
                                    flag3 |= ListViewGUILayout.MultiSelection(row, current.row, ref this.pv.initialSelectedItem, ref this.pv.selectedItems);
                                }
                                this.pv.selectedFile = file;
                                this.pv.selectedFolder = num2;
                                this.pv.lv.row = current.row;
                            }
                        }
                        else if (ListViewGUILayout.HasMouseDown(current.position, 1))
                        {
                            if (!this.pv.selectedItems[current.row])
                            {
                                flag3 = true;
                                this.pv.ClearSelection();
                                this.pv.selectedItems[current.row] = true;
                                this.pv.selectedFile = file;
                                this.pv.selectedFolder = num2;
                                this.pv.lv.row = current.row;
                            }
                            GUIUtility.hotControl = 0;
                            Rect position = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 1f, 1f);
                            EditorUtility.DisplayCustomMenu(position, this.commitDropDownMenuItems, null, new EditorUtility.SelectMenuItemFunction(this.CommitContextMenuClick), null);
                            Event.current.Use();
                        }
                    }
                    if (file != -1)
                    {
                        Texture2D cachedIcon = AssetDatabase.GetCachedIcon(folder.name + "/" + folder.files[file].name) as Texture2D;
                        if (cachedIcon == null)
                        {
                            cachedIcon = InternalEditorUtility.GetIconForFile(folder.files[file].name);
                        }
                        GUILayout.Label(new GUIContent(folder.files[file].name, cachedIcon), constants.element, new GUILayoutOption[0]);
                        changeFlags = folder.files[file].changeFlags;
                    }
                    else
                    {
                        GUILayout.Label(folder.name, constants.header, new GUILayoutOption[0]);
                        changeFlags = folder.changeFlags;
                    }
                    GUIContent badgeNew = null;
                    if (this.HasFlag(changeFlags, ChangeFlags.Undeleted) || this.HasFlag(changeFlags, ChangeFlags.Created))
                    {
                        badgeNew = constants.badgeNew;
                    }
                    else if (this.HasFlag(changeFlags, ChangeFlags.Deleted))
                    {
                        badgeNew = constants.badgeDelete;
                    }
                    else if (this.HasFlag(changeFlags, ChangeFlags.Renamed) || this.HasFlag(changeFlags, ChangeFlags.Moved))
                    {
                        badgeNew = constants.badgeMove;
                    }
                    if ((badgeNew != null) && (Event.current.type == EventType.Repaint))
                    {
                        Rect rect2 = new Rect(((current.position.x + current.position.width) - badgeNew.image.width) - 5f, (current.position.y + (current.position.height / 2f)) - (badgeNew.image.height / 2), (float) badgeNew.image.width, (float) badgeNew.image.height);
                        EditorGUIUtility.SetIconSize(Vector2.zero);
                        GUIStyle.none.Draw(rect2, badgeNew, false, false, false, false);
                        EditorGUIUtility.SetIconSize(this.iconSize);
                    }
                    this.pv.NextFileFolder(ref num2, ref file);
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
        Label_0533:
            if (!this.committing)
            {
                if (GUIUtility.keyboardControl == this.pv.lv.ID)
                {
                    if ((Event.current.type == EventType.ValidateCommand) && (Event.current.commandName == "SelectAll"))
                    {
                        Event.current.Use();
                    }
                    else if ((Event.current.type == EventType.ExecuteCommand) && (Event.current.commandName == "SelectAll"))
                    {
                        for (int i = 0; i < this.pv.selectedItems.Length; i++)
                        {
                            this.pv.selectedItems[i] = true;
                        }
                        flag3 = true;
                        Event.current.Use();
                    }
                }
                if (((GUIUtility.keyboardControl == this.pv.lv.ID) && !actionKey) && this.pv.lv.selectionChanged)
                {
                    flag3 |= ListViewGUILayout.MultiSelection(row, this.pv.lv.row, ref this.pv.initialSelectedItem, ref this.pv.selectedItems);
                    this.pv.IndexToFolderAndFile(this.pv.lv.row, ref this.pv.selectedFolder, ref this.pv.selectedFile);
                }
                if (this.pv.lv.selectionChanged || flag3)
                {
                    this.MySelectionToGlobalSelection();
                }
            }
        }

        public void DoDiscardChanges(bool lastActionsResult)
        {
            List<string> list = new List<string>();
            if (false)
            {
                list.AddRange(AssetServer.CollectDeepSelection());
            }
            else
            {
                list.AddRange(AssetServer.GetAllRootGUIDs());
                list.AddRange(AssetServer.CollectAllChildren(AssetServer.GetRootGUID(), AssetServer.GetAllRootGUIDs()));
            }
            if (list.Count == 0)
            {
                list.AddRange(AssetServer.GetAllRootGUIDs());
            }
            AssetServer.SetAfterActionFinishedCallback("ASEditorBackend", "CBReinitOnSuccess");
            AssetServer.DoUpdateWithoutConflictResolutionOnNextTick(list.ToArray());
        }

        private void DoMyRevert(bool afterMarkingDependencies)
        {
            if (!afterMarkingDependencies)
            {
                List<string> list = ASCommitWindow.GetParentViewSelectedItems(this.pv, true, false);
                if (ASCommitWindow.MarkAllFolderDependenciesForDiscarding(this.pv, null))
                {
                    this.lastRevertSelectionChanged = 2;
                    this.MySelectionToGlobalSelection();
                }
                else
                {
                    this.lastRevertSelectionChanged = -1;
                }
                List<string> list2 = ASCommitWindow.GetParentViewSelectedItems(this.pv, true, false);
                if (list.Count != list2.Count)
                {
                    this.lastRevertSelectionChanged = 2;
                }
            }
            if (afterMarkingDependencies || (this.lastRevertSelectionChanged == -1))
            {
                ASCommitWindow.DoRevert(ASCommitWindow.GetParentViewSelectedItems(this.pv, true, true), "CBInitOverviewPage");
            }
        }

        private void DoSearchToggle(ShowSearchField field)
        {
            if (this.selectedPage == Page.History)
            {
                if (this.m_SearchField.DoGUI())
                {
                    this.asHistoryWin.FilterItems(false);
                }
                GUILayout.Space(10f);
            }
        }

        private void DoSelectedPageGUI()
        {
            switch (this.selectedPage)
            {
                case Page.Overview:
                    this.OverviewPageGUI();
                    break;

                case Page.Update:
                    if ((this.asUpdateWin != null) && (this.asUpdateWin != null))
                    {
                        this.asUpdateWin.DoGUI();
                    }
                    break;

                case Page.Commit:
                    if ((this.asCommitWin != null) && (this.asCommitWin != null))
                    {
                        this.asCommitWin.DoGUI();
                    }
                    break;

                case Page.History:
                    if ((this.asHistoryWin != null) && !this.asHistoryWin.DoGUI(base.m_Parent.hasFocus))
                    {
                        this.SwitchSelectedPage(Page.Overview);
                        GUIUtility.ExitGUI();
                    }
                    break;

                case Page.ServerConfig:
                    if ((this.asConfigWin != null) && !this.asConfigWin.DoGUI())
                    {
                        this.SwitchSelectedPage(Page.Overview);
                        GUIUtility.ExitGUI();
                    }
                    break;

                case Page.Admin:
                    if ((this.asAdminWin != null) && !this.asAdminWin.DoGUI())
                    {
                        this.SwitchSelectedPage(Page.Overview);
                        GUIUtility.ExitGUI();
                    }
                    break;
            }
        }

        private void DoSelectionChange()
        {
            if (this.committing)
            {
                this.selectionChangedWhileCommitting = true;
            }
            else
            {
                HierarchyProperty property = new HierarchyProperty(HierarchyType.Assets);
                List<string> guids = new List<string>(Selection.objects.Length);
                foreach (Object obj2 in Selection.objects)
                {
                    if (property.Find(obj2.GetInstanceID(), null))
                    {
                        guids.Add(property.guid);
                    }
                }
                this.pvHasSelection = ASCommitWindow.MarkSelected(this.pv, guids);
            }
        }

        private void GetUpdates()
        {
            AssetServer.ClearAssetServerError();
            this.sharedChangesets = AssetServer.GetNewItems();
            Array.Reverse(this.sharedChangesets);
            this.changesetContents = null;
            this.maxNickLength = 1;
            AssetServer.ClearRefreshUpdate();
            if (AssetServer.GetAssetServerError() != string.Empty)
            {
                this.sharedChangesets = null;
            }
        }

        private bool HasFlag(ChangeFlags flags, ChangeFlags flagToCheck)
        {
            return ((flagToCheck & flags) != ChangeFlags.None);
        }

        private void InitCommits()
        {
            if (this.CommitNeedsRefresh())
            {
                if (AssetServer.GetAssetServerError() == string.Empty)
                {
                    this.sharedCommits = ASCommitWindow.GetCommits();
                    this.sharedDeletedItems = AssetServer.GetLocalDeletedItems();
                }
                else
                {
                    this.sharedCommits = new AssetsItem[0];
                    this.sharedDeletedItems = new AssetsItem[0];
                }
            }
            this.pv.Clear();
            this.pv.AddAssetItems(this.sharedCommits);
            this.pv.AddAssetItems(this.sharedDeletedItems);
            this.pv.SetLineCount();
            this.pv.selectedItems = new bool[this.pv.lv.totalRows];
            this.pv.initialSelectedItem = -1;
            AssetServer.ClearRefreshCommit();
        }

        public void InitHistoryPage(bool lastActionsResult)
        {
            if (!lastActionsResult)
            {
                this.Reinit();
            }
            else
            {
                this.asHistoryWin = new ASHistoryWindow(this);
                if (this.asHistoryWin == null)
                {
                    this.Reinit();
                }
            }
        }

        private void InitiateRefreshAssetsAndUpdateStatusWithCallback(string callbackName)
        {
            if (!ASEditorBackend.SettingsIfNeeded())
            {
                Debug.Log("Asset Server connection for current project is not set up");
                this.error = true;
            }
            else
            {
                this.error = false;
                AssetServer.SetAfterActionFinishedCallback("ASEditorBackend", callbackName);
                AssetServer.DoRefreshAssetsAndUpdateStatusOnNextTick();
            }
        }

        private void InitiateRefreshAssetsWithCallback(string callbackName)
        {
            if (!ASEditorBackend.SettingsIfNeeded())
            {
                Debug.Log("Asset Server connection for current project is not set up");
                this.error = true;
            }
            else
            {
                this.error = false;
                AssetServer.SetAfterActionFinishedCallback("ASEditorBackend", callbackName);
                AssetServer.DoRefreshAssetsOnNextTick();
            }
        }

        private void InitiateUpdateStatusWithCallback(string callbackName)
        {
            if (!ASEditorBackend.SettingsIfNeeded())
            {
                Debug.Log("Asset Server connection for current project is not set up");
                this.error = true;
            }
            else
            {
                this.error = false;
                AssetServer.SetAfterActionFinishedCallback("ASEditorBackend", callbackName);
                AssetServer.DoUpdateStatusOnNextTick();
            }
        }

        public void InitOverviewPage(bool lastActionsResult)
        {
            if (!lastActionsResult)
            {
                this.needsSetup = true;
                this.sharedChangesets = null;
                this.sharedCommits = null;
                this.sharedDeletedItems = null;
            }
            else
            {
                PListConfig config = new PListConfig("Library/ServerPreferences.plist");
                string[] textArray1 = new string[] { config["Maint UserName"], " @ ", config["Maint Server"], " : ", config["Maint project name"] };
                this.connectionString = string.Concat(textArray1);
                if (this.UpdateNeedsRefresh())
                {
                    this.GetUpdates();
                }
                this.needsSetup = (this.sharedChangesets == null) || AssetServer.HasConnectionError();
                this.InitCommits();
                this.DisplayedItemsChanged();
            }
        }

        public void InitUpdatePage(bool lastActionsResult)
        {
            if (!lastActionsResult)
            {
                this.Reinit();
            }
            else
            {
                if (this.UpdateNeedsRefresh())
                {
                    this.GetUpdates();
                }
                if (this.sharedChangesets == null)
                {
                    this.Reinit();
                }
                else
                {
                    this.asUpdateWin = new ASUpdateWindow(this, this.sharedChangesets);
                    this.asUpdateWin.SetSelectedRevisionLine(0);
                }
            }
        }

        private bool IsLastOne(int f, int fl, ParentViewState st)
        {
            return (((st.folders.Length - 1) == f) && ((st.folders[f].files.Length - 1) == fl));
        }

        public void LogError(string errorStr)
        {
            Debug.LogError(errorStr);
            AssetServer.SetAssetServerError(errorStr, false);
            this.error = true;
        }

        private void MySelectionToGlobalSelection()
        {
            this.mySelection = true;
            List<string> list = ASCommitWindow.GetParentViewSelectedItems(this.pv, true, false);
            list.Remove(AssetServer.GetRootGUID());
            if (list.Count > 0)
            {
                AssetServer.SetSelectionFromGUID(list[0]);
            }
            this.pvHasSelection = this.pv.HasTrue();
            this.somethingDiscardableSelected = ASCommitWindow.SomethingDiscardableSelected(this.pv);
        }

        private void NotifyClosingCommit()
        {
            if (this.asCommitWin != null)
            {
                this.asCommitWin.OnClose();
            }
        }

        private void OnDestroy()
        {
            this.sharedCommits = null;
            this.sharedDeletedItems = null;
            this.sharedChangesets = null;
            this.changesetContents = null;
            if (this.selectedPage == Page.Commit)
            {
                this.NotifyClosingCommit();
            }
        }

        private void OnEnable()
        {
            base.titleContent = base.GetLocalizedTitleContent();
        }

        private void OnGUI()
        {
            if ((EditorSettings.externalVersionControl != ExternalVersionControl.Disabled) && (EditorSettings.externalVersionControl != ExternalVersionControl.AssetServer))
            {
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.FlexibleSpace();
                GUILayout.Label("Asset Server is disabled when external version control is used. Go to 'Edit -> Project Settings -> Editor' to re-enable it.", new GUILayoutOption[0]);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
            }
            else
            {
                if (constants == null)
                {
                    constants = new Constants();
                }
                if (!this.m_CheckedMaint && (Event.current.type != EventType.Layout))
                {
                    if (!InternalEditorUtility.HasTeamLicense())
                    {
                        base.Close();
                        GUIUtility.ExitGUI();
                    }
                    this.m_CheckedMaint = true;
                }
                if ((this.maxNickLength == 1) && (this.sharedChangesets != null))
                {
                    for (int i = 0; i < this.sharedChangesets.Length; i++)
                    {
                        int x = (int) constants.serverUpdateInfo.CalcSize(new GUIContent(this.sharedChangesets[i].owner)).x;
                        if (x > this.maxNickLength)
                        {
                            this.maxNickLength = x;
                        }
                    }
                    this.changesetContents = new GUIContent[this.sharedChangesets.Length];
                    ParentViewState st = new ParentViewState();
                    for (int j = 0; j < this.changesetContents.Length; j++)
                    {
                        int num4 = 15;
                        Changeset assets = this.sharedChangesets[j];
                        char[] separator = new char[] { '\n' };
                        string str = assets.message.Split(separator)[0];
                        str = (str.Length >= 0x2d) ? (str.Substring(0, 0x2a) + "...") : str;
                        string tooltip = string.Format("[{0} {1}] {2}", assets.date, assets.owner, str);
                        num4--;
                        st.Clear();
                        st.AddAssetItems(assets);
                        for (int k = 0; k < st.folders.Length; k++)
                        {
                            if ((--num4 == 0) && !this.IsLastOne(k, 0, st))
                            {
                                tooltip = tooltip + "\n(and more...)";
                                break;
                            }
                            tooltip = tooltip + "\n" + st.folders[k].name;
                            for (int m = 0; m < st.folders[k].files.Length; m++)
                            {
                                if ((--num4 == 0) && !this.IsLastOne(k, m, st))
                                {
                                    tooltip = tooltip + "\n(and more...)";
                                    break;
                                }
                                tooltip = tooltip + "\n\t" + st.folders[k].files[m].name;
                            }
                            if (num4 == 0)
                            {
                                break;
                            }
                        }
                        char[] chArray2 = new char[] { '\n' };
                        this.changesetContents[j] = new GUIContent(this.sharedChangesets[j].message.Split(chArray2)[0], tooltip);
                    }
                    if (this.maxNickLength == 1)
                    {
                        this.maxNickLength = 0;
                    }
                }
                if (AssetServer.IsControllerBusy() != 0)
                {
                    base.Repaint();
                }
                else
                {
                    if (this.isInitialUpdate)
                    {
                        this.isInitialUpdate = false;
                        this.SwitchSelectedPage(Page.Overview);
                    }
                    if ((Event.current.type == EventType.ExecuteCommand) && (Event.current.commandName == "Find"))
                    {
                        this.SetShownSearchField(this.m_SearchToShow);
                        Event.current.Use();
                    }
                    GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
                    int num7 = -1;
                    bool enabled = GUI.enabled;
                    if (this.ToolbarToggle(this.selectedPage == Page.Overview, this.pageTitles[0], EditorStyles.toolbarButton))
                    {
                        num7 = 0;
                    }
                    GUI.enabled = ((!this.needsSetup && (this.sharedChangesets != null)) && (this.sharedChangesets.Length != 0)) && enabled;
                    if (this.ToolbarToggle(this.selectedPage == Page.Update, this.pageTitles[1], EditorStyles.toolbarButton))
                    {
                        num7 = 1;
                    }
                    GUI.enabled = (!this.needsSetup && (this.pv.lv.totalRows != 0)) && enabled;
                    if (this.selectedPage > Page.Commit)
                    {
                        if (this.ToolbarToggle(this.selectedPage == Page.Commit, this.pageTitles[2], EditorStyles.toolbarButton))
                        {
                            num7 = 2;
                        }
                        GUI.enabled = enabled;
                        if (this.ToolbarToggle(this.selectedPage > Page.Commit, this.pageTitles[3], EditorStyles.toolbarButton))
                        {
                            num7 = 3;
                        }
                    }
                    else
                    {
                        if (this.ToolbarToggle(this.selectedPage == Page.Commit, this.pageTitles[2], EditorStyles.toolbarButton))
                        {
                            num7 = 2;
                        }
                        GUI.enabled = enabled;
                    }
                    if ((num7 != -1) && (num7 != this.selectedPage))
                    {
                        if (this.selectedPage == Page.Commit)
                        {
                            this.NotifyClosingCommit();
                        }
                        if (num7 <= 2)
                        {
                            this.SwitchSelectedPage((Page) num7);
                            GUIUtility.ExitGUI();
                        }
                    }
                    GUILayout.FlexibleSpace();
                    if (this.selectedPage == Page.History)
                    {
                        this.DoSearchToggle(ShowSearchField.HistoryList);
                    }
                    if (!this.needsSetup)
                    {
                        switch (this.selectedPage)
                        {
                            case Page.Overview:
                            case Page.Update:
                            case Page.History:
                                if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, new GUILayoutOption[0]))
                                {
                                    this.ActionRefresh();
                                    GUIUtility.ExitGUI();
                                }
                                break;
                        }
                    }
                    GUILayout.EndHorizontal();
                    EditorGUIUtility.SetIconSize(this.iconSize);
                    this.DoSelectedPageGUI();
                    EditorGUIUtility.SetIconSize(Vector2.zero);
                    if (Event.current.type == EventType.ContextClick)
                    {
                        GUIUtility.hotControl = 0;
                        Rect position = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 1f, 1f);
                        EditorUtility.DisplayCustomMenu(position, !this.needsSetup ? this.dropDownMenuItems : this.unconfiguredDropDownMenuItems, null, new EditorUtility.SelectMenuItemFunction(this.ContextMenuClick), null);
                        Event.current.Use();
                    }
                }
            }
        }

        private void OnSelectionChange()
        {
            switch (this.selectedPage)
            {
                case Page.Overview:
                    if (this.mySelection)
                    {
                        this.mySelection = false;
                        break;
                    }
                    this.DoSelectionChange();
                    base.Repaint();
                    break;

                case Page.Update:
                    this.asUpdateWin.OnSelectionChange();
                    return;

                case Page.Commit:
                    this.asCommitWin.OnSelectionChange();
                    return;

                case Page.History:
                    this.asHistoryWin.OnSelectionChange();
                    return;

                default:
                    return;
            }
            this.somethingDiscardableSelected = ASCommitWindow.SomethingDiscardableSelected(this.pv);
        }

        private void OtherServerCommands()
        {
            GUILayout.BeginVertical(constants.groupBox, new GUILayoutOption[0]);
            GUILayout.Label("Asset Server Actions", constants.title, new GUILayoutOption[0]);
            GUILayout.BeginVertical(constants.contentBox, new GUILayoutOption[0]);
            if (this.WordWrappedLabelButton("Browse the complete history of the project", "Show History"))
            {
                this.SwitchSelectedPage(Page.History);
                GUIUtility.ExitGUI();
            }
            GUILayout.Space(5f);
            if (this.WordWrappedLabelButton("Discard all local changes you made to the project", "Discard Changes"))
            {
                this.ActionDiscardChanges();
            }
            GUILayout.EndVertical();
            GUILayout.EndVertical();
        }

        private void OverviewPageGUI()
        {
            bool enabled = GUI.enabled;
            this.showSmallWindow = base.position.width <= this.widthToHideButtons;
            if (Event.current.type == EventType.Layout)
            {
                this.wasHidingButtons = this.showSmallWindow;
            }
            else if (this.showSmallWindow != this.wasHidingButtons)
            {
                GUIUtility.ExitGUI();
            }
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (!this.showSmallWindow)
            {
                GUILayout.BeginVertical(new GUILayoutOption[0]);
                this.ShortServerInfo();
                if (this.needsSetup)
                {
                    GUI.enabled = false;
                }
                this.OtherServerCommands();
                GUI.enabled = enabled;
                this.ServerAdministration();
                GUI.enabled = !this.needsSetup && enabled;
                GUILayout.EndVertical();
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width((base.position.width - 30f) / 2f) };
                GUILayout.BeginHorizontal(options);
            }
            else
            {
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            }
            GUI.enabled = !this.needsSetup && enabled;
            SplitterGUILayout.BeginVerticalSplit(this.splitter, new GUILayoutOption[0]);
            this.ShortUpdateList();
            this.ShortCommitList();
            SplitterGUILayout.EndVerticalSplit();
            GUILayout.EndHorizontal();
            GUILayout.EndHorizontal();
            GUI.enabled = enabled;
        }

        internal void Reinit()
        {
            this.SwitchSelectedPage(Page.Overview);
            base.Repaint();
        }

        public void RevertProject(int toRevision, Changeset[] changesets)
        {
            AssetServer.SetStickyChangeset(toRevision);
            this.asUpdateWin = new ASUpdateWindow(this, changesets);
            this.asUpdateWin.SetSelectedRevisionLine(0);
            this.asUpdateWin.DoUpdate(false);
            this.selectedPage = Page.Update;
        }

        private bool RightButton(string title)
        {
            return this.RightButton(title, constants.smallButton);
        }

        private bool RightButton(string title, GUIStyle style)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            bool flag = GUILayout.Button(title, style, new GUILayoutOption[0]);
            GUILayout.EndHorizontal();
            return flag;
        }

        private void SelectedPageChanged()
        {
            AssetServer.ClearAssetServerError();
            if (this.committing)
            {
                this.CancelCommit();
            }
            switch (this.selectedPage)
            {
                case Page.Overview:
                    if (!ASEditorBackend.SettingsAreValid())
                    {
                        this.connectionString = "Asset Server connection for current project is not set up";
                        this.sharedChangesets = new Changeset[0];
                        this.changesetContents = new GUIContent[0];
                        this.needsSetup = true;
                        break;
                    }
                    AssetServer.CheckForServerUpdates();
                    if (!this.UpdateNeedsRefresh())
                    {
                        this.InitOverviewPage(true);
                        break;
                    }
                    this.InitiateUpdateStatusWithCallback("CBInitOverviewPage");
                    break;

                case Page.Update:
                    this.InitUpdatePage(true);
                    break;

                case Page.Commit:
                    this.asCommitWin = new ASCommitWindow(this, !this.pvHasSelection ? null : ASCommitWindow.GetParentViewSelectedItems(this.pv, true, false).ToArray());
                    this.asCommitWin.InitiateReinit();
                    break;

                case Page.History:
                    this.pageTitles[3] = "History";
                    this.InitHistoryPage(true);
                    break;

                case Page.ServerConfig:
                    this.pageTitles[3] = "Connection";
                    this.asConfigWin = new ASConfigWindow(this);
                    break;

                case Page.Admin:
                    this.pageTitles[3] = "Administration";
                    this.asAdminWin = new ASServerAdminWindow(this);
                    if (!this.error)
                    {
                        break;
                    }
                    return;
            }
        }

        private void ServerAdministration()
        {
            GUILayout.BeginVertical(constants.groupBox, new GUILayoutOption[0]);
            GUILayout.Label("Asset Server Administration", constants.title, new GUILayoutOption[0]);
            GUILayout.BeginVertical(constants.contentBox, new GUILayoutOption[0]);
            if (this.WordWrappedLabelButton("Create and administer Asset Server projects", "Administration"))
            {
                this.SwitchSelectedPage(Page.Admin);
                GUIUtility.ExitGUI();
            }
            GUILayout.EndVertical();
            GUILayout.EndVertical();
        }

        private void SetShownSearchField(ShowSearchField newShow)
        {
            EditorGUI.FocusTextInControl("SearchFilter");
            this.m_SearchField.Show = false;
            this.m_ShowSearch = newShow;
            this.m_SearchField.Show = true;
            this.asHistoryWin.FilterItems(false);
        }

        private void ShortCommitList()
        {
            bool enabled = GUI.enabled;
            GUILayout.BeginVertical(!this.showSmallWindow ? constants.groupBox : constants.groupBoxNoMargin, new GUILayoutOption[0]);
            GUILayout.Label("Local Changes", constants.title, new GUILayoutOption[0]);
            if (this.pv.lv.totalRows == 0)
            {
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandHeight(true) };
                GUILayout.BeginVertical(constants.contentBox, options);
                GUILayout.Label("Nothing to commit", new GUILayoutOption[0]);
                GUILayout.EndVertical();
            }
            else
            {
                this.DoCommitParentView();
                GUILayout.BeginHorizontal(constants.contentBox, new GUILayoutOption[0]);
                Event current = Event.current;
                if (!this.committing)
                {
                    GUI.enabled = this.pvHasSelection && enabled;
                    if (GUILayout.Button("Compare", constants.smallButton, new GUILayoutOption[0]))
                    {
                        ASCommitWindow.DoShowDiff(ASCommitWindow.GetParentViewSelectedItems(this.pv, false, false), false);
                        GUIUtility.ExitGUI();
                    }
                    bool flag2 = GUI.enabled;
                    if (!this.somethingDiscardableSelected)
                    {
                        GUI.enabled = false;
                    }
                    if (GUILayout.Button("Discard", constants.smallButton, new GUILayoutOption[0]))
                    {
                        this.DoMyRevert(false);
                        GUIUtility.ExitGUI();
                    }
                    GUI.enabled = flag2;
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Commit...", constants.smallButton, new GUILayoutOption[0]) || ((this.pvHasSelection && (current.type == EventType.KeyDown)) && (current.keyCode == KeyCode.Return)))
                    {
                        this.StartCommitting();
                        current.Use();
                    }
                    if ((current.type == EventType.KeyDown) && ((current.character == '\n') || (current.character == '\x0003')))
                    {
                        current.Use();
                    }
                    GUI.enabled = enabled;
                    if (GUILayout.Button("Details", constants.smallButton, new GUILayoutOption[0]))
                    {
                        this.SwitchSelectedPage(Page.Commit);
                        base.Repaint();
                        GUIUtility.ExitGUI();
                    }
                }
                else
                {
                    if (current.type == EventType.KeyDown)
                    {
                        KeyCode keyCode = current.keyCode;
                        if (keyCode != KeyCode.Return)
                        {
                            if (keyCode != KeyCode.Escape)
                            {
                                if ((current.character == '\n') || (current.character == '\x0003'))
                                {
                                    current.Use();
                                }
                            }
                            else
                            {
                                this.CancelCommit();
                                current.Use();
                            }
                        }
                        else
                        {
                            this.DoCommit();
                            current.Use();
                        }
                    }
                    GUI.SetNextControlName("commitMessage");
                    this.commitMessage = EditorGUILayout.TextField(this.commitMessage, new GUILayoutOption[0]);
                    GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(60f) };
                    if (GUILayout.Button("Commit", constants.smallButton, optionArray2))
                    {
                        this.DoCommit();
                    }
                    GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.Width(60f) };
                    if (GUILayout.Button("Cancel", constants.smallButton, optionArray3))
                    {
                        this.CancelCommit();
                    }
                    if (this.focusCommitMessage)
                    {
                        EditorGUI.FocusTextInControl("commitMessage");
                        this.focusCommitMessage = false;
                        base.Repaint();
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            if (this.lastRevertSelectionChanged == 0)
            {
                this.lastRevertSelectionChanged = -1;
                if (ASCommitWindow.ShowDiscardWarning())
                {
                    this.DoMyRevert(true);
                }
            }
            if (this.lastRevertSelectionChanged > 0)
            {
                this.lastRevertSelectionChanged--;
                base.Repaint();
            }
        }

        private void ShortServerInfo()
        {
            GUILayout.BeginVertical(constants.groupBox, new GUILayoutOption[0]);
            GUILayout.Label("Current Project", constants.title, new GUILayoutOption[0]);
            GUILayout.BeginVertical(constants.contentBox, new GUILayoutOption[0]);
            if (this.WordWrappedLabelButton(this.connectionString, "Connection"))
            {
                this.SwitchSelectedPage(Page.ServerConfig);
            }
            if (AssetServer.GetAssetServerError() != string.Empty)
            {
                GUILayout.Space(10f);
                GUILayout.Label(AssetServer.GetAssetServerError(), constants.errorLabel, new GUILayoutOption[0]);
            }
            GUILayout.EndVertical();
            GUILayout.EndVertical();
        }

        private void ShortUpdateList()
        {
            GUILayout.BeginVertical(!this.showSmallWindow ? constants.groupBox : constants.groupBoxNoMargin, new GUILayoutOption[0]);
            GUILayout.Label("Updates on Server", constants.title, new GUILayoutOption[0]);
            if (this.sharedChangesets == null)
            {
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandHeight(true) };
                GUILayout.BeginVertical(constants.contentBox, options);
                GUILayout.Label("Could not retrieve changes", new GUILayoutOption[0]);
                GUILayout.EndVertical();
            }
            else if (this.sharedChangesets.Length == 0)
            {
                GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandHeight(true) };
                GUILayout.BeginVertical(constants.contentBox, optionArray2);
                GUILayout.Label("You are up to date", new GUILayoutOption[0]);
                GUILayout.EndVertical();
            }
            else
            {
                this.lv.totalRows = this.sharedChangesets.Length;
                int num = (int) constants.entryNormal.CalcHeight(new GUIContent("X"), 100f);
                constants.serverUpdateLog.alignment = TextAnchor.MiddleLeft;
                constants.serverUpdateInfo.alignment = TextAnchor.MiddleLeft;
                IEnumerator enumerator = ListViewGUILayout.ListView(this.lv, constants.background, new GUILayoutOption[0]).GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        ListViewElement current = (ListViewElement) enumerator.Current;
                        GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.MinHeight((float) num) };
                        Rect rect2 = GUILayoutUtility.GetRect(GUIClip.visibleRect.width, (float) num, optionArray3);
                        Rect position = rect2;
                        position.x++;
                        position.y++;
                        if ((current.row % 2) == 0)
                        {
                            if (Event.current.type == EventType.Repaint)
                            {
                                constants.entryEven.Draw(position, false, false, false, false);
                            }
                            position.y += rect2.height;
                            if (Event.current.type == EventType.Repaint)
                            {
                                constants.entryOdd.Draw(position, false, false, false, false);
                            }
                        }
                        position = rect2;
                        position.width -= this.maxNickLength + 0x19;
                        position.x += 10f;
                        GUI.Button(position, this.changesetContents[current.row], constants.serverUpdateLog);
                        position = rect2;
                        position.x += (position.width - this.maxNickLength) - 5f;
                        GUI.Label(position, this.sharedChangesets[current.row].owner, constants.serverUpdateInfo);
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
                constants.serverUpdateLog.alignment = TextAnchor.UpperLeft;
                constants.serverUpdateInfo.alignment = TextAnchor.UpperLeft;
                GUILayout.BeginHorizontal(constants.contentBox, new GUILayoutOption[0]);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Update", constants.smallButton, new GUILayoutOption[0]))
                {
                    this.selectedPage = Page.Update;
                    this.InitUpdatePage(true);
                    this.asUpdateWin.DoUpdate(false);
                }
                if (GUILayout.Button("Details", constants.smallButton, new GUILayoutOption[0]))
                {
                    this.SwitchSelectedPage(Page.Update);
                    base.Repaint();
                    GUIUtility.ExitGUI();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        public void ShowConflictResolutions(string[] conflicting)
        {
            if (this.asUpdateWin == null)
            {
                this.LogError("Found unexpected conflicts. Please use Bug Reporter to report a bug.");
            }
            else
            {
                this.asUpdateWin.ShowConflictResolutions(conflicting);
            }
        }

        public void ShowHistory()
        {
            this.SwitchSelectedPage(Page.Overview);
            this.isInitialUpdate = false;
            this.SwitchSelectedPage(Page.History);
        }

        private void StartCommitting()
        {
            this.committing = true;
            this.commitMessage = string.Empty;
            this.selectionChangedWhileCommitting = false;
            this.focusCommitMessage = true;
        }

        private void SwitchSelectedPage(Page page)
        {
            Page selectedPage = this.selectedPage;
            this.selectedPage = page;
            this.SelectedPageChanged();
            if (this.error)
            {
                this.selectedPage = selectedPage;
                this.error = false;
            }
        }

        private bool ToolbarToggle(bool pressed, string title, GUIStyle style)
        {
            bool changed = GUI.changed;
            GUI.changed = false;
            GUILayout.Toggle(pressed, title, style, new GUILayoutOption[0]);
            if (GUI.changed)
            {
                return true;
            }
            GUI.changed |= changed;
            return false;
        }

        public bool UpdateNeedsRefresh()
        {
            return ((this.sharedChangesets == null) || AssetServer.GetRefreshUpdate());
        }

        private bool WordWrappedLabelButton(string label, string buttonText)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label(label, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(110f) };
            bool flag = GUILayout.Button(buttonText, options);
            GUILayout.EndHorizontal();
            return flag;
        }

        public bool Error
        {
            get
            {
                return this.error;
            }
        }

        public bool NeedsSetup
        {
            get
            {
                return this.needsSetup;
            }
            set
            {
                this.needsSetup = value;
            }
        }

        internal class Constants
        {
            public GUIStyle background = "OL Box";
            public GUIContent badgeDelete = EditorGUIUtility.IconContent("AS Badge Delete");
            public GUIContent badgeMove = EditorGUIUtility.IconContent("AS Badge Move");
            public GUIContent badgeNew = EditorGUIUtility.IconContent("AS Badge New");
            public GUIStyle bigButton = "LargeButton";
            public GUIStyle button = "Button";
            public GUIStyle columnHeader = "OL Title";
            public GUIStyle contentBox = "GroupBox";
            public GUIStyle dropDown = "MiniPullDown";
            public GUIStyle element = "OL elem";
            public GUIStyle entryEven = "CN EntryBackEven";
            public GUIStyle entryNormal = "ServerUpdateChangeset";
            public GUIStyle entryOdd = "CN EntryBackOdd";
            public GUIStyle entrySelected = "ServerUpdateChangesetOn";
            public GUIStyle errorLabel = "ErrorLabel";
            public GUIStyle groupBox = new GUIStyle();
            public GUIStyle groupBoxNoMargin = new GUIStyle();
            public GUIStyle header = "OL header";
            public GUIStyle largeButton = "ButtonMid";
            public GUIStyle miniButton = "MiniButton";
            public GUIStyle serverUpdateInfo = "ServerUpdateInfo";
            public GUIStyle serverUpdateLog = "ServerUpdateLog";
            public GUIStyle smallButton = "Button";
            public GUIStyle title = "OL Title";
            public GUIStyle toggle = "Toggle";
            public Vector2 toggleSize;

            public Constants()
            {
                this.groupBox.margin = new RectOffset(10, 10, 10, 10);
                this.contentBox = new GUIStyle(this.contentBox);
                this.contentBox.margin = new RectOffset(0, 0, 0, 0);
                this.contentBox.overflow = new RectOffset(0, 1, 0, 1);
                this.contentBox.padding = new RectOffset(8, 8, 7, 7);
                this.title = new GUIStyle(this.title);
                int num = this.contentBox.padding.left + 2;
                this.title.padding.right = num;
                this.title.padding.left = num;
                this.background = new GUIStyle(this.background);
                this.background.padding.top = 1;
            }
        }

        internal enum Page
        {
            Admin = 5,
            Commit = 2,
            History = 3,
            NotInitialized = -1,
            Overview = 0,
            ServerConfig = 4,
            Update = 1
        }

        [Serializable]
        public class SearchField
        {
            private string m_FilterText = string.Empty;
            private bool m_Show;

            public bool DoGUI()
            {
                GUI.SetNextControlName("SearchFilter");
                string str = EditorGUILayout.ToolbarSearchField(this.m_FilterText, new GUILayoutOption[0]);
                if (this.m_FilterText != str)
                {
                    this.m_FilterText = str;
                    return true;
                }
                return false;
            }

            public string FilterText
            {
                get
                {
                    return this.m_FilterText;
                }
            }

            public bool Show
            {
                get
                {
                    return this.m_Show;
                }
                set
                {
                    this.m_Show = value;
                }
            }
        }

        public enum ShowSearchField
        {
            None,
            ProjectView,
            HistoryList
        }
    }
}

