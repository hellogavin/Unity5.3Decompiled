namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    [Serializable]
    internal class ASCommitWindow
    {
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$mapF;
        private string[] commitMessageList;
        private static Constants constants;
        internal string description;
        private string dragTitle = string.Empty;
        private string[] dropDownMenuItems = new string[] { string.Empty, string.Empty, "Compare", "Compare Binary", string.Empty, "Discard" };
        private string[] guidsToTransferToTheRightSide;
        private SplitterState horSplit;
        private Vector2 iconSize = new Vector2(16f, 16f);
        private bool initialUpdate;
        internal int lastRevertSelectionChanged;
        internal bool lastTransferMovedDependencies;
        private const int listLenghts = 20;
        private bool mySelection;
        private ASMainWindow parentWin;
        private bool pv1hasSelection;
        private ParentViewState pv1state = new ParentViewState();
        private bool pv2hasSelection;
        private ParentViewState pv2state = new ParentViewState();
        private bool resetKeyboardControl;
        private static List<string> s_AssetGuids;
        private static string s_Callback;
        private Vector2 scrollPos;
        internal int showReinitedWarning;
        private bool somethingDiscardableSelected;
        private string totalChanges;
        private SplitterState vertSplit;
        private bool wasHidingButtons;
        private const int widthToHideButtons = 0x1b0;

        public ASCommitWindow(ASMainWindow parentWin, string[] guidsToTransfer)
        {
            float[] relativeSizes = new float[] { 50f, 50f };
            int[] minSizes = new int[] { 50, 50 };
            this.horSplit = new SplitterState(relativeSizes, minSizes, null);
            float[] singleArray2 = new float[] { 60f, 30f };
            int[] numArray2 = new int[] { 0x20, 0x40 };
            this.vertSplit = new SplitterState(singleArray2, numArray2, null);
            this.description = string.Empty;
            this.scrollPos = Vector2.zero;
            this.lastRevertSelectionChanged = -1;
            this.showReinitedWarning = -1;
            this.guidsToTransferToTheRightSide = guidsToTransfer;
            this.parentWin = parentWin;
            this.initialUpdate = true;
        }

        private void AddFolderToRemove(ref List<string> guidsOfFoldersToRemove, string guid)
        {
            if (!guidsOfFoldersToRemove.Contains(guid))
            {
                guidsOfFoldersToRemove.Add(guid);
            }
        }

        internal void AddToCommitMessageHistory(string description)
        {
            if (description.Trim() != string.Empty)
            {
                if (ArrayUtility.Contains<string>(this.commitMessageList, description))
                {
                    ArrayUtility.Remove<string>(ref this.commitMessageList, description);
                }
                ArrayUtility.Insert<string>(ref this.commitMessageList, 0, description);
                InternalEditorUtility.SaveEditorSettingsList("ASCommitMsgs", this.commitMessageList, 20);
            }
        }

        private static bool AllFolderWouldBeMovedAnyway(ParentViewState pvState, string guid)
        {
            int num = 0;
            for (int i = 0; i < pvState.folders.Length; i++)
            {
                if (pvState.folders[i].guid == guid)
                {
                    bool flag = true;
                    bool flag2 = true;
                    bool flag3 = pvState.selectedItems[num++];
                    for (int j = 0; j < pvState.folders[i].files.Length; j++)
                    {
                        if (pvState.selectedItems[num++])
                        {
                            flag = false;
                        }
                        else
                        {
                            flag2 = false;
                        }
                    }
                    return (flag3 && (flag2 || flag));
                }
                num += 1 + pvState.folders[i].files.Length;
            }
            return false;
        }

        private static bool AnyOfTheParentsIsSelected(ref ParentViewState pvState, string guid)
        {
            string itemGUID = guid;
            while (AssetServer.IsGUIDValid(itemGUID = AssetServer.GetParentGUID(itemGUID, -1)) != 0)
            {
                if (AllFolderWouldBeMovedAnyway(pvState, itemGUID))
                {
                    return true;
                }
            }
            return false;
        }

        internal void AssetItemsToParentViews()
        {
            this.pv1state.Clear();
            this.pv2state.Clear();
            this.pv1state.AddAssetItems(this.parentWin.sharedCommits);
            this.pv1state.AddAssetItems(this.parentWin.sharedDeletedItems);
            this.pv1state.lv = new ListViewState(0);
            this.pv2state.lv = new ListViewState(0);
            this.pv1state.SetLineCount();
            this.pv2state.SetLineCount();
            if (this.pv1state.lv.totalRows == 0)
            {
                this.parentWin.Reinit();
            }
            else
            {
                this.pv1state.selectedItems = new bool[this.pv1state.lv.totalRows];
                this.pv2state.selectedItems = new bool[this.pv1state.lv.totalRows];
                int num = 0;
                for (int i = 0; i < this.parentWin.sharedCommits.Length; i++)
                {
                    if (this.parentWin.sharedCommits[i].assetIsDir != 0)
                    {
                        num++;
                    }
                }
                for (int j = 0; j < this.parentWin.sharedDeletedItems.Length; j++)
                {
                    if (this.parentWin.sharedDeletedItems[j].assetIsDir != 0)
                    {
                        num++;
                    }
                }
                this.totalChanges = (((this.pv1state.lv.totalRows - this.pv1state.GetFoldersCount()) + num)).ToString() + " Local Changes";
                this.GetPersistedData();
            }
        }

        internal bool CanCommit()
        {
            return (this.pv2state.folders.Length != 0);
        }

        internal void CommitFinished(bool actionResult)
        {
            if (actionResult)
            {
                AssetServer.ClearCommitPersistentData();
                this.parentWin.Reinit();
            }
            else
            {
                this.parentWin.Repaint();
            }
        }

        private void ContextMenuClick(object userData, string[] options, int selected)
        {
            if (selected >= 0)
            {
                string key = this.dropDownMenuItems[selected];
                if (key != null)
                {
                    int num;
                    if (<>f__switch$mapF == null)
                    {
                        Dictionary<string, int> dictionary = new Dictionary<string, int>(5);
                        dictionary.Add("Compare", 0);
                        dictionary.Add("Compare Binary", 1);
                        dictionary.Add("Discard", 2);
                        dictionary.Add(">>>", 3);
                        dictionary.Add("<<<", 4);
                        <>f__switch$mapF = dictionary;
                    }
                    if (<>f__switch$mapF.TryGetValue(key, out num))
                    {
                        switch (num)
                        {
                            case 0:
                                this.DoShowMyDiff(false);
                                break;

                            case 1:
                                this.DoShowMyDiff(true);
                                break;

                            case 2:
                                this.DoMyRevert(false);
                                break;

                            case 3:
                                this.DoTransferAll(this.pv1state, this.pv2state, this.pv1state.selectedFolder, this.pv1state.selectedFile);
                                break;

                            case 4:
                                this.DoTransferAll(this.pv2state, this.pv1state, this.pv2state.selectedFolder, this.pv2state.selectedFile);
                                break;
                        }
                    }
                }
            }
        }

        internal void DoCommit()
        {
            if (AssetServer.GetRefreshCommit())
            {
                this.SetPersistedData();
                this.InitiateReinit();
                this.showReinitedWarning = 2;
                this.parentWin.Repaint();
                GUIUtility.ExitGUI();
            }
            if ((this.description != string.Empty) || EditorUtility.DisplayDialog("Commit without description", "Are you sure you want to commit with empty commit description message?", "Commit", "Cancel"))
            {
                string[] itemsToCommit = this.GetItemsToCommit();
                this.SetPersistedData();
                AssetServer.SetAfterActionFinishedCallback("ASEditorBackend", "CBCommitFinished");
                AssetServer.DoCommitOnNextTick(this.description, itemsToCommit);
                GUIUtility.ExitGUI();
            }
        }

        internal bool DoGUI()
        {
            bool enabled = GUI.enabled;
            if (constants == null)
            {
                constants = new Constants();
            }
            if (this.resetKeyboardControl)
            {
                this.resetKeyboardControl = false;
                GUIUtility.keyboardControl = 0;
            }
            bool flag2 = this.parentWin.position.width <= 432f;
            if (Event.current.type == EventType.Layout)
            {
                this.wasHidingButtons = flag2;
            }
            else if (flag2 != this.wasHidingButtons)
            {
                GUIUtility.ExitGUI();
            }
            SplitterGUILayout.BeginHorizontalSplit(this.horSplit, new GUILayoutOption[0]);
            GUILayout.BeginVertical(constants.box, new GUILayoutOption[0]);
            GUILayout.Label(this.totalChanges, constants.title, new GUILayoutOption[0]);
            if (this.ParentViewGUI(this.pv1state, this.pv2state, ref this.pv1hasSelection))
            {
                GUILayout.EndVertical();
                SplitterGUILayout.BeginVerticalSplit(this.vertSplit, new GUILayoutOption[0]);
                GUILayout.BeginVertical(constants.box, new GUILayoutOption[0]);
                GUILayout.Label("Changeset", constants.title, new GUILayoutOption[0]);
                if (!this.ParentViewGUI(this.pv2state, this.pv1state, ref this.pv2hasSelection))
                {
                    return true;
                }
                GUILayout.EndVertical();
                GUILayout.BeginVertical(new GUILayoutOption[0]);
                GUILayout.Label("Commit Message", constants.title, new GUILayoutOption[0]);
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                if (this.commitMessageList.Length > 0)
                {
                    GUIContent content = new GUIContent("Recent");
                    Rect position = GUILayoutUtility.GetRect(content, constants.dropDown, null);
                    if (GUI.Button(position, content, constants.dropDown))
                    {
                        GUIUtility.hotControl = 0;
                        string[] strArray = new string[this.commitMessageList.Length];
                        for (int i = 0; i < strArray.Length; i++)
                        {
                            strArray[i] = (this.commitMessageList[i].Length <= 200) ? this.commitMessageList[i] : (this.commitMessageList[i].Substring(0, 200) + " ... ");
                        }
                        EditorUtility.DisplayCustomMenu(position, strArray, null, new EditorUtility.SelectMenuItemFunction(this.MenuClick), null);
                        Event.current.Use();
                    }
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(constants.box, new GUILayoutOption[0]);
                this.scrollPos = EditorGUILayout.BeginVerticalScrollView(this.scrollPos, new GUILayoutOption[0]);
                this.description = EditorGUILayout.TextArea(this.description, constants.wwText, new GUILayoutOption[0]);
                EditorGUILayout.EndScrollView();
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                SplitterGUILayout.EndVerticalSplit();
                SplitterGUILayout.EndHorizontalSplit();
                if (!flag2)
                {
                    GUILayout.Label("Please drag files you want to commit to Changeset and fill in commit description", new GUILayoutOption[0]);
                }
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                if (!this.pv1hasSelection && !this.pv2hasSelection)
                {
                    GUI.enabled = false;
                }
                if (!flag2 && GUILayout.Button("Compare", constants.button, new GUILayoutOption[0]))
                {
                    this.DoShowMyDiff(false);
                    GUIUtility.ExitGUI();
                }
                bool flag3 = GUI.enabled;
                if (!this.somethingDiscardableSelected)
                {
                    GUI.enabled = false;
                }
                if (GUILayout.Button(!flag2 ? "Discard Selected Changes" : "Discard", constants.button, new GUILayoutOption[0]))
                {
                    this.DoMyRevert(false);
                    GUIUtility.ExitGUI();
                }
                GUI.enabled = flag3;
                GUILayout.FlexibleSpace();
                GUI.enabled = this.pv1hasSelection && enabled;
                if (GUILayout.Button(!flag2 ? ">>>" : ">", constants.button, new GUILayoutOption[0]))
                {
                    this.DoTransferAll(this.pv1state, this.pv2state, this.pv1state.selectedFolder, this.pv1state.selectedFile);
                }
                GUI.enabled = this.pv2hasSelection && enabled;
                if (GUILayout.Button(!flag2 ? "<<<" : "<", constants.button, new GUILayoutOption[0]))
                {
                    this.DoTransferAll(this.pv2state, this.pv1state, this.pv2state.selectedFolder, this.pv2state.selectedFile);
                }
                GUI.enabled = (this.pv1state.lv.totalRows != 0) && enabled;
                if (GUILayout.Button("Add All", constants.button, new GUILayoutOption[0]))
                {
                    int num2 = 0;
                    while (num2 < this.pv1state.selectedItems.Length)
                    {
                        this.pv1state.selectedItems[num2++] = true;
                    }
                    this.DoTransferAll(this.pv1state, this.pv2state, this.pv1state.selectedFolder, this.pv1state.selectedFile);
                }
                GUI.enabled = (this.pv2state.lv.totalRows != 0) && enabled;
                if (GUILayout.Button("Remove All", constants.button, new GUILayoutOption[0]))
                {
                    int num3 = 0;
                    while (num3 < this.pv2state.selectedItems.Length)
                    {
                        this.pv2state.selectedItems[num3++] = true;
                    }
                    this.DoTransferAll(this.pv2state, this.pv1state, this.pv2state.selectedFolder, this.pv2state.selectedFile);
                }
                GUI.enabled = enabled;
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.FlexibleSpace();
                if (!this.CanCommit())
                {
                    GUI.enabled = false;
                }
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinWidth(100f) };
                if (GUILayout.Button("Commit", constants.bigButton, options))
                {
                    this.DoCommit();
                }
                GUI.enabled = enabled;
                if (((Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.KeypadEnter)) && ((Application.platform == RuntimePlatform.OSXEditor) && this.CanCommit()))
                {
                    this.DoCommit();
                }
                GUILayout.EndHorizontal();
                if (AssetServer.GetAssetServerError() != string.Empty)
                {
                    GUILayout.Space(10f);
                    GUILayout.Label(AssetServer.GetAssetServerError(), constants.errorLabel, new GUILayoutOption[0]);
                    GUILayout.Space(10f);
                }
                GUILayout.Space(10f);
                if (this.lastRevertSelectionChanged == 0)
                {
                    this.lastRevertSelectionChanged = -1;
                    if (ShowDiscardWarning())
                    {
                        this.DoMyRevert(true);
                    }
                }
                if (this.lastRevertSelectionChanged > 0)
                {
                    if (Event.current.type == EventType.Repaint)
                    {
                        this.lastRevertSelectionChanged--;
                    }
                    this.parentWin.Repaint();
                }
                if (this.showReinitedWarning == 0)
                {
                    EditorUtility.DisplayDialog("Commits updated", "Commits had to be updated to reflect latest changes", "OK", string.Empty);
                    this.showReinitedWarning = -1;
                }
                if (this.showReinitedWarning > 0)
                {
                    if (Event.current.type == EventType.Repaint)
                    {
                        this.showReinitedWarning--;
                    }
                    this.parentWin.Repaint();
                }
            }
            return true;
        }

        private void DoMyRevert(bool afterMarkingDependencies)
        {
            if (!afterMarkingDependencies)
            {
                bool flag;
                List<string> selectedItems = this.GetSelectedItems();
                if (this.pv2hasSelection)
                {
                    flag = MarkAllFolderDependenciesForDiscarding(this.pv2state, this.pv1state);
                }
                else
                {
                    flag = MarkAllFolderDependenciesForDiscarding(this.pv1state, this.pv2state);
                }
                if (flag)
                {
                    this.MySelectionToGlobalSelection();
                }
                List<string> list2 = this.GetSelectedItems();
                if (selectedItems.Count != list2.Count)
                {
                    flag = true;
                }
                this.lastRevertSelectionChanged = !flag ? -1 : 1;
            }
            if (afterMarkingDependencies || (this.lastRevertSelectionChanged == -1))
            {
                this.SetPersistedData();
                DoRevert(GetParentViewSelectedItems(!this.pv2hasSelection ? this.pv1state : this.pv2state, true, true), "CBReinitCommitWindow");
            }
        }

        internal static void DoRevert(List<string> assetGuids, string callback)
        {
            if (assetGuids.Count != 0)
            {
                s_AssetGuids = assetGuids;
                s_Callback = callback;
                AssetServer.SetAfterActionFinishedCallback("ASCommitWindow", "DoRevertAfterDialog");
                AssetServer.ShowDialogOnNextTick("Discard changes", "Are you really sure you want to discard selected changes?", "Discard", "Cancel");
            }
        }

        internal static void DoRevertAfterDialog(bool result)
        {
            if (result)
            {
                AssetServer.SetAfterActionFinishedCallback("ASEditorBackend", s_Callback);
                AssetServer.DoUpdateWithoutConflictResolutionOnNextTick(s_AssetGuids.ToArray());
            }
        }

        private void DoSelectionChange()
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
            if (this.pv1hasSelection)
            {
                this.pv1hasSelection = MarkSelected(this.pv1state, guids);
            }
            if (!this.pv1hasSelection)
            {
                if (this.pv2hasSelection)
                {
                    this.pv2hasSelection = MarkSelected(this.pv2state, guids);
                }
                if (!this.pv2hasSelection)
                {
                    this.pv1hasSelection = MarkSelected(this.pv1state, guids);
                    if (!this.pv1hasSelection)
                    {
                        this.pv2hasSelection = MarkSelected(this.pv2state, guids);
                    }
                }
            }
        }

        internal static bool DoShowDiff(List<string> selectedAssets, bool binary)
        {
            List<string> list = new List<string>();
            List<CompareInfo> list2 = new List<CompareInfo>();
            for (int i = 0; i < selectedAssets.Count; i++)
            {
                int num2 = -1;
                int workingItemChangeset = AssetServer.GetWorkingItemChangeset(selectedAssets[i]);
                workingItemChangeset = AssetServer.GetServerItemChangeset(selectedAssets[i], workingItemChangeset);
                if (AssetServer.IsItemDeleted(selectedAssets[i]))
                {
                    num2 = -2;
                }
                if (workingItemChangeset == -1)
                {
                    workingItemChangeset = -2;
                }
                list.Add(selectedAssets[i]);
                list2.Add(new CompareInfo(workingItemChangeset, num2, !binary ? 0 : 1, !binary ? 1 : 0));
            }
            if (list.Count != 0)
            {
                AssetServer.CompareFiles(list.ToArray(), list2.ToArray());
                return true;
            }
            return false;
        }

        private bool DoShowMyDiff(bool binary)
        {
            return DoShowDiff(GetParentViewSelectedItems(!this.pv2hasSelection ? this.pv1state : this.pv2state, false, false), binary);
        }

        private bool DoTransfer(ref ParentViewFolder[] foldersFrom, ref ParentViewFolder[] foldersTo, int folder, int file, ref List<string> guidsOfFoldersToRemove, bool leftToRight)
        {
            ParentViewFolder item = foldersFrom[folder];
            ParentViewFolder folder3 = null;
            string name = item.name;
            bool flag = false;
            bool flag2 = false;
            if (file == -1)
            {
                this.AddFolderToRemove(ref guidsOfFoldersToRemove, foldersFrom[folder].guid);
                int index = ParentViewState.IndexOf(foldersTo, name);
                if (index != -1)
                {
                    folder3 = foldersTo[index];
                    ArrayUtility.AddRange<ParentViewFile>(ref folder3.files, item.files);
                }
                else
                {
                    ArrayUtility.Add<ParentViewFolder>(ref foldersTo, item);
                    flag2 = true;
                    if (!HasFlag(item.changeFlags, ChangeFlags.Deleted))
                    {
                        flag = this.TransferDependentParentFolders(ref guidsOfFoldersToRemove, item.guid, leftToRight);
                    }
                    else
                    {
                        flag = this.TransferDeletedDependentParentFolders(ref guidsOfFoldersToRemove, item.guid, leftToRight);
                    }
                }
            }
            else
            {
                int num2 = ParentViewState.IndexOf(foldersTo, name);
                if (num2 == -1)
                {
                    if (HasFlag(item.files[file].changeFlags, ChangeFlags.Deleted) && HasFlag(item.changeFlags, ChangeFlags.Deleted))
                    {
                        ArrayUtility.Add<ParentViewFolder>(ref foldersTo, item);
                        this.AddFolderToRemove(ref guidsOfFoldersToRemove, item.guid);
                        num2 = foldersTo.Length - 1;
                        if (!AllFolderWouldBeMovedAnyway(!leftToRight ? this.pv2state : this.pv1state, item.guid))
                        {
                            flag = true;
                        }
                        flag |= this.TransferDeletedDependentParentFolders(ref guidsOfFoldersToRemove, item.guid, leftToRight);
                    }
                    else
                    {
                        ArrayUtility.Add<ParentViewFolder>(ref foldersTo, item.CloneWithoutFiles());
                        num2 = foldersTo.Length - 1;
                        flag = this.TransferDependentParentFolders(ref guidsOfFoldersToRemove, item.guid, leftToRight);
                    }
                    flag2 = true;
                }
                folder3 = foldersTo[num2];
                ArrayUtility.Add<ParentViewFile>(ref folder3.files, item.files[file]);
                ArrayUtility.RemoveAt<ParentViewFile>(ref item.files, file);
                if (item.files.Length == 0)
                {
                    this.AddFolderToRemove(ref guidsOfFoldersToRemove, foldersFrom[folder].guid);
                }
            }
            if (folder3 != null)
            {
                Array.Sort<ParentViewFile>(folder3.files, new Comparison<ParentViewFile>(ParentViewState.CompareViewFile));
            }
            if (flag2)
            {
                Array.Sort<ParentViewFolder>(foldersTo, new Comparison<ParentViewFolder>(ParentViewState.CompareViewFolder));
            }
            return flag;
        }

        private void DoTransferAll(ParentViewState pvState, ParentViewState anotherPvState, int selFolder, int selFile)
        {
            List<string> guidsOfFoldersToRemove = new List<string>();
            bool flag = this.MarkDependantFiles(pvState);
            int index = pvState.lv.totalRows - 1;
            for (int i = pvState.folders.Length - 1; i >= 0; i--)
            {
                ParentViewFolder folder = pvState.folders[i];
                bool flag2 = false;
                for (int j = folder.files.Length - 1; j >= -1; j--)
                {
                    if (!guidsOfFoldersToRemove.Contains(folder.guid) && pvState.selectedItems[index])
                    {
                        if ((j != -1) || !flag2)
                        {
                            flag |= this.DoTransfer(ref pvState.folders, ref anotherPvState.folders, i, j, ref guidsOfFoldersToRemove, pvState == this.pv1state);
                        }
                        flag2 = true;
                    }
                    index--;
                }
            }
            for (index = pvState.folders.Length - 1; index >= 0; index--)
            {
                if (guidsOfFoldersToRemove.Contains(pvState.folders[index].guid))
                {
                    guidsOfFoldersToRemove.Remove(pvState.folders[index].guid);
                    ArrayUtility.RemoveAt<ParentViewFolder>(ref pvState.folders, index);
                }
            }
            this.pv1state.SetLineCount();
            this.pv2state.SetLineCount();
            this.pv1state.ClearSelection();
            this.pv2state.ClearSelection();
            pvState.selectedFile = -1;
            pvState.selectedFolder = -1;
            AssetServer.SetSelectionFromGUID(string.Empty);
            this.lastTransferMovedDependencies = flag;
        }

        private static int FolderIndexToTotalIndex(ParentViewFolder[] folders, int folderIndex)
        {
            int num = 0;
            for (int i = 0; i < folderIndex; i++)
            {
                num += folders[i].files.Length + 1;
            }
            return num;
        }

        private static int FolderSelectionIndexFromGUID(ParentViewFolder[] folders, string guid)
        {
            int num = 0;
            for (int i = 0; i < folders.Length; i++)
            {
                if (guid == folders[i].guid)
                {
                    return num;
                }
                num += 1 + folders[i].files.Length;
            }
            return -1;
        }

        internal static AssetsItem[] GetCommits()
        {
            return AssetServer.GetChangedAssetsItems();
        }

        internal string[] GetItemsToCommit()
        {
            List<string> list = new List<string>();
            for (int i = 0; i < this.pv2state.folders.Length; i++)
            {
                ParentViewFolder folder = this.pv2state.folders[i];
                if (AssetServer.IsGUIDValid(folder.guid) != 0)
                {
                    list.Add(folder.guid);
                }
                for (int j = 0; j < folder.files.Length; j++)
                {
                    if (AssetServer.IsGUIDValid(folder.files[j].guid) != 0)
                    {
                        list.Add(folder.files[j].guid);
                    }
                }
            }
            return list.ToArray();
        }

        internal static List<string> GetParentViewSelectedItems(ParentViewState state, bool includeFolders, bool excludeUndiscardableOnes)
        {
            List<string> list = new List<string>();
            int index = 0;
            for (int i = 0; i < state.folders.Length; i++)
            {
                ParentViewFolder folder = state.folders[i];
                bool flag = true;
                bool flag2 = true;
                int num3 = index++;
                int count = list.Count;
                for (int j = 0; j < folder.files.Length; j++)
                {
                    if (state.selectedItems[index])
                    {
                        if (!excludeUndiscardableOnes || IsDiscardableAsset(folder.files[j].guid, folder.files[j].changeFlags))
                        {
                            list.Add(folder.files[j].guid);
                            flag = false;
                        }
                    }
                    else
                    {
                        flag2 = false;
                    }
                    index++;
                }
                if (((includeFolders && state.selectedItems[num3]) && (flag || flag2)) && ((AssetServer.IsGUIDValid(folder.guid) != 0) && (count <= list.Count)))
                {
                    list.Insert(count, folder.guid);
                }
            }
            return list;
        }

        private void GetPersistedData()
        {
            string[] guidsToTransferToTheRightSide;
            this.description = AssetServer.GetLastCommitMessage();
            if ((this.guidsToTransferToTheRightSide != null) && (this.guidsToTransferToTheRightSide.Length != 0))
            {
                guidsToTransferToTheRightSide = this.guidsToTransferToTheRightSide;
                this.guidsToTransferToTheRightSide = null;
            }
            else
            {
                guidsToTransferToTheRightSide = AssetServer.GetCommitSelectionGUIDs();
            }
            int index = 0;
            foreach (ParentViewFolder folder in this.pv1state.folders)
            {
                this.pv1state.selectedItems[index++] = guidsToTransferToTheRightSide.Contains(folder.guid) && (AssetServer.IsGUIDValid(folder.guid) != 0);
                foreach (ParentViewFile file in folder.files)
                {
                    this.pv1state.selectedItems[index++] = guidsToTransferToTheRightSide.Contains(file.guid) && (AssetServer.IsGUIDValid(file.guid) != 0);
                }
            }
            this.DoTransferAll(this.pv1state, this.pv2state, this.pv1state.selectedFolder, this.pv1state.selectedFile);
            this.commitMessageList = InternalEditorUtility.GetEditorSettingsList("ASCommitMsgs", 20);
            for (index = 0; index < this.commitMessageList.Length; index++)
            {
                this.commitMessageList[index] = this.commitMessageList[index].Replace('/', '?').Replace('%', '?');
            }
        }

        private List<string> GetSelectedItems()
        {
            this.pv1hasSelection = this.pv1state.HasTrue();
            this.pv2hasSelection = this.pv2state.HasTrue();
            List<string> list = GetParentViewSelectedItems(!this.pv2hasSelection ? this.pv1state : this.pv2state, true, false);
            list.Remove(AssetServer.GetRootGUID());
            return list;
        }

        private static bool HasFlag(ChangeFlags flags, ChangeFlags flagToCheck)
        {
            return ((flagToCheck & flags) != ChangeFlags.None);
        }

        private static int IndexOfFolderWithGUID(ParentViewFolder[] folders, string guid)
        {
            for (int i = 0; i < folders.Length; i++)
            {
                if (folders[i].guid == guid)
                {
                    return i;
                }
            }
            return -1;
        }

        internal void InitiateReinit()
        {
            if (this.parentWin.CommitNeedsRefresh())
            {
                if (!this.initialUpdate)
                {
                    this.SetPersistedData();
                }
                else
                {
                    this.initialUpdate = false;
                }
                this.Reinit(true);
            }
            else if (this.initialUpdate)
            {
                this.AssetItemsToParentViews();
                this.initialUpdate = false;
            }
            else
            {
                this.SetPersistedData();
                AssetServer.SetAfterActionFinishedCallback("ASEditorBackend", "CBReinitCommitWindow");
                AssetServer.DoRefreshAssetsOnNextTick();
            }
        }

        internal static bool IsDiscardableAsset(string guid, ChangeFlags changeFlags)
        {
            return ((AssetServer.IsConstantGUID(guid) == 0) || (!HasFlag(changeFlags, ChangeFlags.Created) && !HasFlag(changeFlags, ChangeFlags.Undeleted)));
        }

        public static bool MarkAllFolderDependenciesForDiscarding(ParentViewState pvState, ParentViewState anotherPvState)
        {
            bool flag = false;
            bool flag2 = false;
            int index = 0;
            List<string> list = new List<string>();
            for (int i = 0; i < pvState.folders.Length; i++)
            {
                ParentViewFolder folder = pvState.folders[i];
                if (HasFlag(folder.changeFlags, ChangeFlags.Deleted))
                {
                    bool flag3 = false;
                    for (int j = 1; j <= folder.files.Length; j++)
                    {
                        if (pvState.selectedItems[index + j])
                        {
                            flag3 = true;
                            pvState.selectedItems[index] = true;
                            list.Add(folder.guid);
                            break;
                        }
                    }
                    if (pvState.selectedItems[index] || flag3)
                    {
                        string guid = folder.guid;
                        while (AssetServer.IsGUIDValid(guid = AssetServer.GetParentGUID(guid, -1)) != 0)
                        {
                            int folderIndex = IndexOfFolderWithGUID(pvState.folders, guid);
                            if (folderIndex == -1)
                            {
                                break;
                            }
                            folderIndex = FolderIndexToTotalIndex(pvState.folders, folderIndex);
                            if (!pvState.selectedItems[folderIndex] && HasFlag(pvState.folders[folderIndex].changeFlags, ChangeFlags.Deleted))
                            {
                                pvState.selectedItems[folderIndex] = true;
                                list.Add(guid);
                                flag = true;
                            }
                        }
                    }
                }
                else if (!AllFolderWouldBeMovedAnyway(pvState, folder.guid))
                {
                    if (AnyOfTheParentsIsSelected(ref pvState, folder.guid))
                    {
                        pvState.selectedItems[index] = true;
                        list.Add(folder.guid);
                        for (int k = 1; k <= folder.files.Length; k++)
                        {
                            pvState.selectedItems[index + k] = true;
                        }
                        flag = true;
                    }
                }
                else
                {
                    for (int m = 1; m <= folder.files.Length; m++)
                    {
                        if (!pvState.selectedItems[index + m])
                        {
                            pvState.selectedItems[index + m] = true;
                        }
                    }
                    list.Add(folder.guid);
                }
                index += 1 + pvState.folders[i].files.Length;
            }
            if (anotherPvState != null)
            {
                for (int n = 0; n < anotherPvState.folders.Length; n++)
                {
                    ParentViewFolder folder2 = anotherPvState.folders[n];
                    if (AnyOfTheParentsIsSelected(ref pvState, folder2.guid))
                    {
                        list.Add(folder2.guid);
                    }
                }
                for (int num8 = anotherPvState.folders.Length - 1; num8 >= 0; num8--)
                {
                    if (!list.Contains(anotherPvState.folders[num8].guid))
                    {
                        continue;
                    }
                    ParentViewFolder folder3 = anotherPvState.folders[num8];
                    int num9 = FolderSelectionIndexFromGUID(pvState.folders, folder3.guid);
                    if (num9 != -1)
                    {
                        ParentViewFolder folder4 = pvState.folders[num9];
                        int length = ((pvState.lv.totalRows - num9) - 1) - folder4.files.Length;
                        int sourceIndex = (num9 + 1) + folder4.files.Length;
                        Array.Copy(pvState.selectedItems, sourceIndex, pvState.selectedItems, sourceIndex + folder3.files.Length, length);
                        ArrayUtility.AddRange<ParentViewFile>(ref folder4.files, folder3.files);
                        for (int num12 = 1; num12 <= folder4.files.Length; num12++)
                        {
                            pvState.selectedItems[num9 + num12] = true;
                        }
                        Array.Sort<ParentViewFile>(folder4.files, new Comparison<ParentViewFile>(ParentViewState.CompareViewFile));
                    }
                    else
                    {
                        num9 = 0;
                        for (int num13 = 0; num13 < pvState.folders.Length; num13++)
                        {
                            if (ParentViewState.CompareViewFolder(pvState.folders[num9], folder3) > 0)
                            {
                                break;
                            }
                            num9 += 1 + pvState.folders[num13].files.Length;
                        }
                        int num14 = pvState.lv.totalRows - num9;
                        int num15 = num9;
                        Array.Copy(pvState.selectedItems, num15, pvState.selectedItems, (num15 + 1) + folder3.files.Length, num14);
                        ArrayUtility.Add<ParentViewFolder>(ref pvState.folders, folder3);
                        for (int num16 = 0; num16 <= folder3.files.Length; num16++)
                        {
                            pvState.selectedItems[num9 + num16] = true;
                        }
                        flag2 = true;
                    }
                    ArrayUtility.RemoveAt<ParentViewFolder>(ref anotherPvState.folders, num8);
                    flag = true;
                }
                anotherPvState.SetLineCount();
            }
            pvState.SetLineCount();
            if (flag2)
            {
                Array.Sort<ParentViewFolder>(pvState.folders, new Comparison<ParentViewFolder>(ParentViewState.CompareViewFolder));
            }
            return flag;
        }

        private bool MarkDependantFiles(ParentViewState pvState)
        {
            string[] strArray = new string[0];
            bool flag = false;
            if (pvState == this.pv1state)
            {
                strArray = AssetServer.CollectAllDependencies(GetParentViewSelectedItems(this.pv1state, false, false).ToArray());
                if (strArray.Length == 0)
                {
                    return flag;
                }
                int index = 1;
                int num2 = 0;
                while (index < pvState.lv.totalRows)
                {
                    int num3 = 0;
                    while (num3 < pvState.folders[num2].files.Length)
                    {
                        if (!pvState.selectedItems[index])
                        {
                            for (int i = 0; i < strArray.Length; i++)
                            {
                                if (strArray[i] == pvState.folders[num2].files[num3].guid)
                                {
                                    pvState.selectedItems[index] = true;
                                    flag = true;
                                    break;
                                }
                            }
                        }
                        num3++;
                        index++;
                    }
                    num2++;
                    index++;
                }
            }
            return flag;
        }

        internal static bool MarkSelected(ParentViewState activeState, List<string> guids)
        {
            int num = 0;
            bool flag = false;
            foreach (ParentViewFolder folder in activeState.folders)
            {
                bool flag2 = guids.Contains(folder.guid);
                activeState.selectedItems[num++] = flag2;
                flag |= flag2;
                foreach (ParentViewFile file in folder.files)
                {
                    flag2 = guids.Contains(file.guid);
                    activeState.selectedItems[num++] = flag2;
                    flag |= flag2;
                }
            }
            return flag;
        }

        private void MenuClick(object userData, string[] options, int selected)
        {
            if (selected >= 0)
            {
                this.description = this.commitMessageList[selected];
                this.resetKeyboardControl = true;
                this.parentWin.Repaint();
            }
        }

        private void MySelectionToGlobalSelection()
        {
            this.mySelection = true;
            this.somethingDiscardableSelected = SomethingDiscardableSelected(!this.pv2hasSelection ? this.pv1state : this.pv2state);
            List<string> selectedItems = this.GetSelectedItems();
            if (selectedItems.Count > 0)
            {
                AssetServer.SetSelectionFromGUID(selectedItems[0]);
            }
        }

        internal void OnClose()
        {
            this.SetPersistedData();
        }

        internal void OnSelectionChange()
        {
            if (!this.mySelection)
            {
                this.DoSelectionChange();
                this.parentWin.Repaint();
            }
            else
            {
                this.mySelection = false;
            }
            this.somethingDiscardableSelected = SomethingDiscardableSelected(!this.pv2hasSelection ? this.pv1state : this.pv2state);
        }

        private bool ParentViewGUI(ParentViewState pvState, ParentViewState anotherPvState, ref bool hasSelection)
        {
            bool flag = false;
            EditorGUIUtility.SetIconSize(this.iconSize);
            ListViewState lv = pvState.lv;
            bool shift = Event.current.shift;
            bool actionKey = EditorGUI.actionKey;
            int row = lv.row;
            int num2 = -1;
            int file = -1;
            bool flag4 = false;
            IEnumerator enumerator = ListViewGUILayout.ListView(lv, ListViewOptions.wantsToAcceptCustomDrag | ListViewOptions.wantsToStartCustomDrag, this.dragTitle, GUIStyle.none, new GUILayoutOption[0]).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    ChangeFlags changeFlags;
                    ListViewElement current = (ListViewElement) enumerator.Current;
                    if ((num2 == -1) && !pvState.IndexToFolderAndFile(current.row, ref num2, ref file))
                    {
                        flag = true;
                        goto Label_04D8;
                    }
                    if (((GUIUtility.keyboardControl == lv.ID) && (Event.current.type == EventType.KeyDown)) && actionKey)
                    {
                        Event.current.Use();
                    }
                    ParentViewFolder folder = pvState.folders[num2];
                    if (pvState.selectedItems[current.row] && (Event.current.type == EventType.Repaint))
                    {
                        constants.entrySelected.Draw(current.position, false, false, false, false);
                    }
                    if (ListViewGUILayout.HasMouseUp(current.position))
                    {
                        if (!shift && !actionKey)
                        {
                            flag4 |= ListViewGUILayout.MultiSelection(row, pvState.lv.row, ref pvState.initialSelectedItem, ref pvState.selectedItems);
                        }
                    }
                    else if (ListViewGUILayout.HasMouseDown(current.position))
                    {
                        if (Event.current.clickCount == 2)
                        {
                            this.DoShowMyDiff(false);
                            GUIUtility.ExitGUI();
                        }
                        else
                        {
                            if ((!pvState.selectedItems[current.row] || shift) || actionKey)
                            {
                                flag4 |= ListViewGUILayout.MultiSelection(row, current.row, ref pvState.initialSelectedItem, ref pvState.selectedItems);
                            }
                            pvState.selectedFile = file;
                            pvState.selectedFolder = num2;
                            lv.row = current.row;
                        }
                    }
                    else if (ListViewGUILayout.HasMouseDown(current.position, 1))
                    {
                        if (!pvState.selectedItems[current.row])
                        {
                            flag4 = true;
                            pvState.ClearSelection();
                            pvState.selectedItems[current.row] = true;
                            pvState.selectedFile = file;
                            pvState.selectedFolder = num2;
                            lv.row = current.row;
                        }
                        this.dropDownMenuItems[0] = (pvState != this.pv1state) ? "<<<" : ">>>";
                        GUIUtility.hotControl = 0;
                        Rect position = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 1f, 1f);
                        EditorUtility.DisplayCustomMenu(position, this.dropDownMenuItems, null, new EditorUtility.SelectMenuItemFunction(this.ContextMenuClick), null);
                        Event.current.Use();
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
                    if (HasFlag(changeFlags, ChangeFlags.Undeleted) || HasFlag(changeFlags, ChangeFlags.Created))
                    {
                        badgeNew = ASMainWindow.constants.badgeNew;
                    }
                    else if (HasFlag(changeFlags, ChangeFlags.Deleted))
                    {
                        badgeNew = ASMainWindow.constants.badgeDelete;
                    }
                    else if (HasFlag(changeFlags, ChangeFlags.Renamed) || HasFlag(changeFlags, ChangeFlags.Moved))
                    {
                        badgeNew = ASMainWindow.constants.badgeMove;
                    }
                    if ((badgeNew != null) && (Event.current.type == EventType.Repaint))
                    {
                        Rect rect2 = new Rect(((current.position.x + current.position.width) - badgeNew.image.width) - 5f, (current.position.y + (current.position.height / 2f)) - (badgeNew.image.height / 2), (float) badgeNew.image.width, (float) badgeNew.image.height);
                        EditorGUIUtility.SetIconSize(Vector2.zero);
                        GUIStyle.none.Draw(rect2, badgeNew, false, false, false, false);
                        EditorGUIUtility.SetIconSize(this.iconSize);
                    }
                    pvState.NextFileFolder(ref num2, ref file);
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
        Label_04D8:
            if (!flag)
            {
                if (GUIUtility.keyboardControl == lv.ID)
                {
                    if ((Event.current.type == EventType.ValidateCommand) && (Event.current.commandName == "SelectAll"))
                    {
                        Event.current.Use();
                    }
                    else if ((Event.current.type == EventType.ExecuteCommand) && (Event.current.commandName == "SelectAll"))
                    {
                        for (int i = 0; i < pvState.selectedItems.Length; i++)
                        {
                            pvState.selectedItems[i] = true;
                        }
                        flag4 = true;
                        Event.current.Use();
                    }
                }
                if ((lv.customDraggedFromID != 0) && (lv.customDraggedFromID == anotherPvState.lv.ID))
                {
                    this.DoTransferAll(anotherPvState, pvState, pvState.selectedFolder, pvState.selectedFile);
                }
                if ((GUIUtility.keyboardControl == lv.ID) && !actionKey)
                {
                    if (lv.selectionChanged)
                    {
                        flag4 |= ListViewGUILayout.MultiSelection(row, lv.row, ref pvState.initialSelectedItem, ref pvState.selectedItems);
                        if (!pvState.IndexToFolderAndFile(lv.row, ref pvState.selectedFolder, ref pvState.selectedFile))
                        {
                            flag = true;
                        }
                    }
                    else if (((pvState.selectedFolder != -1) && (Event.current.type == EventType.KeyDown)) && ((GUIUtility.keyboardControl == lv.ID) && (Event.current.keyCode == KeyCode.Return)))
                    {
                        this.DoTransferAll(pvState, anotherPvState, pvState.selectedFolder, pvState.selectedFile);
                        ListViewGUILayout.MultiSelection(row, lv.row, ref pvState.initialSelectedItem, ref pvState.selectedItems);
                        pvState.IndexToFolderAndFile(lv.row, ref pvState.selectedFolder, ref pvState.selectedFile);
                        Event.current.Use();
                        flag = true;
                    }
                }
                if (lv.selectionChanged || flag4)
                {
                    if (pvState.IndexToFolderAndFile(lv.row, ref num2, ref file))
                    {
                        this.dragTitle = (file != -1) ? pvState.folders[num2].files[file].name : pvState.folders[num2].name;
                    }
                    anotherPvState.ClearSelection();
                    anotherPvState.lv.row = -1;
                    anotherPvState.selectedFile = -1;
                    anotherPvState.selectedFolder = -1;
                    this.MySelectionToGlobalSelection();
                }
            }
            EditorGUIUtility.SetIconSize(Vector2.zero);
            return !flag;
        }

        internal void Reinit(bool lastActionsResult)
        {
            this.parentWin.sharedCommits = GetCommits();
            this.parentWin.sharedDeletedItems = AssetServer.GetLocalDeletedItems();
            AssetServer.ClearRefreshCommit();
            this.AssetItemsToParentViews();
        }

        private void SetPersistedData()
        {
            AssetServer.SetLastCommitMessage(this.description);
            this.AddToCommitMessageHistory(this.description);
            List<string> list = new List<string>();
            foreach (ParentViewFolder folder in this.pv2state.folders)
            {
                if (AssetServer.IsGUIDValid(folder.guid) != 0)
                {
                    list.Add(folder.guid);
                }
                foreach (ParentViewFile file in folder.files)
                {
                    if (AssetServer.IsGUIDValid(file.guid) != 0)
                    {
                        list.Add(file.guid);
                    }
                }
            }
            AssetServer.SetCommitSelectionGUIDs(list.ToArray());
        }

        internal static bool ShowDiscardWarning()
        {
            return EditorUtility.DisplayDialog("Discard changes", "More items will be discarded then initially selected. Dependencies of selected items where all marked in commit window. Please review.", "Discard", "Cancel");
        }

        public static bool SomethingDiscardableSelected(ParentViewState st)
        {
            int num = 0;
            foreach (ParentViewFolder folder in st.folders)
            {
                if (st.selectedItems[num++])
                {
                    return true;
                }
                foreach (ParentViewFile file in folder.files)
                {
                    if (st.selectedItems[num++] && IsDiscardableAsset(file.guid, file.changeFlags))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool TransferDeletedDependentParentFolders(ref List<string> guidsOfFoldersToRemove, string guid, bool leftToRight)
        {
            bool flag = false;
            ParentViewState pvState = !leftToRight ? this.pv2state : this.pv1state;
            ParentViewState state2 = !leftToRight ? this.pv1state : this.pv2state;
            if (leftToRight)
            {
                for (int i = 0; i < pvState.folders.Length; i++)
                {
                    ParentViewFolder item = pvState.folders[i];
                    if ((AssetServer.GetParentGUID(item.guid, -1) == guid) && !AllFolderWouldBeMovedAnyway(pvState, item.guid))
                    {
                        if (!HasFlag(item.changeFlags, ChangeFlags.Deleted))
                        {
                            Debug.LogError("Folder of nested deleted folders marked as not deleted (" + item.name + ")");
                            return false;
                        }
                        for (int j = 0; j < item.files.Length; j++)
                        {
                            if (!HasFlag(item.files[j].changeFlags, ChangeFlags.Deleted))
                            {
                                Debug.LogError("File of nested deleted folder is marked as not deleted (" + item.files[j].name + ")");
                                return false;
                            }
                        }
                        flag |= this.TransferDeletedDependentParentFolders(ref guidsOfFoldersToRemove, item.guid, leftToRight);
                        if (IndexOfFolderWithGUID(state2.folders, item.guid) == -1)
                        {
                            ArrayUtility.Add<ParentViewFolder>(ref state2.folders, item);
                        }
                        this.AddFolderToRemove(ref guidsOfFoldersToRemove, item.guid);
                        flag = true;
                    }
                }
                return flag;
            }
            while (AssetServer.IsGUIDValid(guid = AssetServer.GetParentGUID(guid, -1)) != 0)
            {
                int index = IndexOfFolderWithGUID(this.pv2state.folders, guid);
                if (index == -1)
                {
                    return flag;
                }
                if (HasFlag(this.pv2state.folders[index].changeFlags, ChangeFlags.Deleted))
                {
                    ArrayUtility.Add<ParentViewFolder>(ref this.pv1state.folders, this.pv2state.folders[index]);
                    flag = true;
                    this.AddFolderToRemove(ref guidsOfFoldersToRemove, this.pv2state.folders[index].guid);
                }
            }
            return flag;
        }

        private bool TransferDependentParentFolders(ref List<string> guidsOfFoldersToRemove, string guid, bool leftToRight)
        {
            bool flag = false;
            if (leftToRight)
            {
                while (AssetServer.IsGUIDValid(guid = AssetServer.GetParentGUID(guid, -1)) != 0)
                {
                    if (!AllFolderWouldBeMovedAnyway(!leftToRight ? this.pv2state : this.pv1state, guid))
                    {
                        int index = IndexOfFolderWithGUID(this.pv1state.folders, guid);
                        int num2 = IndexOfFolderWithGUID(this.pv2state.folders, guid);
                        if (((index != -1) || (num2 != -1)) && ((index != -1) && (num2 == -1)))
                        {
                            ChangeFlags flags = this.pv1state.folders[index].changeFlags;
                            if ((HasFlag(flags, ChangeFlags.Undeleted) || HasFlag(flags, ChangeFlags.Created)) || HasFlag(flags, ChangeFlags.Moved))
                            {
                                ArrayUtility.Add<ParentViewFolder>(ref this.pv2state.folders, this.pv1state.folders[index].CloneWithoutFiles());
                                flag = true;
                                if (this.pv1state.folders[index].files.Length == 0)
                                {
                                    this.AddFolderToRemove(ref guidsOfFoldersToRemove, this.pv1state.folders[index].guid);
                                }
                            }
                        }
                    }
                }
                return flag;
            }
            ChangeFlags changeFlags = this.pv1state.folders[IndexOfFolderWithGUID(this.pv1state.folders, guid)].changeFlags;
            if ((!HasFlag(changeFlags, ChangeFlags.Undeleted) && !HasFlag(changeFlags, ChangeFlags.Created)) && !HasFlag(changeFlags, ChangeFlags.Moved))
            {
                return false;
            }
            for (int i = 0; i < this.pv2state.folders.Length; i++)
            {
                string itemGUID = this.pv2state.folders[i].guid;
                if (AssetServer.GetParentGUID(itemGUID, -1) == guid)
                {
                    int num4 = IndexOfFolderWithGUID(this.pv1state.folders, itemGUID);
                    if (num4 != -1)
                    {
                        ArrayUtility.AddRange<ParentViewFile>(ref this.pv1state.folders[num4].files, this.pv2state.folders[i].files);
                    }
                    else
                    {
                        ArrayUtility.Add<ParentViewFolder>(ref this.pv1state.folders, this.pv2state.folders[i]);
                    }
                    this.AddFolderToRemove(ref guidsOfFoldersToRemove, itemGUID);
                    this.TransferDependentParentFolders(ref guidsOfFoldersToRemove, itemGUID, leftToRight);
                    flag = true;
                }
            }
            return flag;
        }

        internal void Update()
        {
            this.SetPersistedData();
            this.AssetItemsToParentViews();
            this.GetPersistedData();
        }

        private class Constants
        {
            public GUIStyle bigButton = "LargeButton";
            public GUIStyle box = "OL Box";
            public GUIStyle button = "Button";
            public GUIStyle dropDown = "MiniPullDown";
            public GUIStyle element = "OL elem";
            public GUIStyle entrySelected = "ServerUpdateChangesetOn";
            public GUIStyle errorLabel = "ErrorLabel";
            public GUIStyle header = "OL header";
            public GUIStyle serverChangeCount = "ServerChangeCount";
            public GUIStyle serverUpdateInfo = "ServerUpdateInfo";
            public GUIStyle title = "OL title";
            public GUIStyle wwText = "AS TextArea";
        }
    }
}

