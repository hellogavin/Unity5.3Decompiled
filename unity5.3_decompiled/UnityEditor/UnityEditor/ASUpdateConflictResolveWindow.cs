namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    [Serializable]
    internal class ASUpdateConflictResolveWindow
    {
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map13;
        private static string[] conflictButtonTexts = new string[] { "Skip Asset", "Discard My Changes", "Ignore Server Changes", "Merge", "Unresolved" };
        private Constants constants;
        private string[] dConflictPaths;
        private bool[] deletionConflict;
        private string[] dNamingPaths;
        private string[] downloadConflicts;
        private int downloadConflictsToResolve;
        private DownloadResolution[] downloadResolutions;
        private string[] downloadResolutionString;
        private string[] dropDownMenuItems;
        private bool enableContinueButton;
        private bool enableMergeButton;
        private Vector2 iconSize;
        private int initialSelectedLV1Item = -1;
        private int initialSelectedLV2Item = -1;
        private ListViewState lv1 = new ListViewState();
        private bool lv1HasSelection;
        private ListViewState lv2 = new ListViewState();
        private bool lv2HasSelection;
        private SplitterState lvHeaderSplit1;
        private SplitterState lvHeaderSplit2;
        private bool mySelection;
        private static string[] nameConflictButtonTexts = new string[] { "Rename Local Asset", "Rename Server Asset" };
        private string[] nameConflicts;
        private NameConflictResolution[] namingResolutions;
        private string[] namingResolutionString;
        private bool[] selectedLV1Items;
        private bool[] selectedLV2Items;
        private bool showDownloadConflicts;
        private bool showNamingConflicts;
        private bool splittersOk;

        public ASUpdateConflictResolveWindow(string[] conflicting)
        {
            float[] relativeSizes = new float[] { 20f, 80f };
            int[] minSizes = new int[] { 100, 100 };
            this.lvHeaderSplit1 = new SplitterState(relativeSizes, minSizes, null);
            float[] singleArray2 = new float[] { 20f, 80f };
            int[] numArray2 = new int[] { 100, 100 };
            this.lvHeaderSplit2 = new SplitterState(singleArray2, numArray2, null);
            this.dropDownMenuItems = new string[] { "Compare", "Compare Binary" };
            this.downloadConflicts = new string[0];
            this.nameConflicts = new string[0];
            this.dConflictPaths = new string[0];
            this.dNamingPaths = new string[0];
            this.downloadResolutions = new DownloadResolution[0];
            this.namingResolutions = new NameConflictResolution[0];
            this.enableMergeButton = true;
            this.iconSize = new Vector2(16f, 16f);
            this.downloadResolutionString = new string[] { "Unresolved", "Skip Asset", "Discard My Changes", "Ignore Server Changes", "Merge" };
            this.namingResolutionString = new string[] { "Unresolved", "Rename Local Asset", "Rename Server Asset" };
            this.downloadConflictsToResolve = 0;
            ArrayList list = new ArrayList();
            ArrayList list2 = new ArrayList();
            ArrayList list3 = new ArrayList();
            ArrayList list4 = new ArrayList();
            for (int i = 0; i < conflicting.Length; i++)
            {
                AssetStatus statusGUID = AssetServer.GetStatusGUID(conflicting[i]);
                if (statusGUID == AssetStatus.Conflict)
                {
                    list.Add(conflicting[i]);
                    DownloadResolution downloadResolution = AssetServer.GetDownloadResolution(conflicting[i]);
                    list2.Add(downloadResolution);
                    if (downloadResolution == DownloadResolution.Unresolved)
                    {
                        this.downloadConflictsToResolve++;
                    }
                }
                if ((AssetServer.GetPathNameConflict(conflicting[i]) != null) && (statusGUID != AssetStatus.ServerOnly))
                {
                    list4.Add(conflicting[i]);
                    NameConflictResolution nameConflictResolution = AssetServer.GetNameConflictResolution(conflicting[i]);
                    list3.Add(nameConflictResolution);
                    if (nameConflictResolution == NameConflictResolution.Unresolved)
                    {
                        this.downloadConflictsToResolve++;
                    }
                }
            }
            this.downloadConflicts = list.ToArray(typeof(string)) as string[];
            this.downloadResolutions = list2.ToArray(typeof(DownloadResolution)) as DownloadResolution[];
            this.namingResolutions = list3.ToArray(typeof(NameConflictResolution)) as NameConflictResolution[];
            this.nameConflicts = list4.ToArray(typeof(string)) as string[];
            this.enableContinueButton = this.downloadConflictsToResolve == 0;
            this.dConflictPaths = new string[this.downloadConflicts.Length];
            this.deletionConflict = new bool[this.downloadConflicts.Length];
            for (int j = 0; j < this.downloadConflicts.Length; j++)
            {
                if (AssetServer.HasDeletionConflict(this.downloadConflicts[j]))
                {
                    this.dConflictPaths[j] = ParentViewFolder.MakeNiceName(AssetServer.GetDeletedItemPathAndName(this.downloadConflicts[j]));
                    this.deletionConflict[j] = true;
                }
                else
                {
                    this.dConflictPaths[j] = ParentViewFolder.MakeNiceName(AssetServer.GetAssetPathName(this.downloadConflicts[j]));
                    this.deletionConflict[j] = false;
                }
            }
            this.dNamingPaths = new string[this.nameConflicts.Length];
            for (int k = 0; k < this.nameConflicts.Length; k++)
            {
                this.dNamingPaths[k] = ParentViewFolder.MakeNiceName(AssetServer.GetAssetPathName(this.nameConflicts[k]));
            }
            this.showDownloadConflicts = this.downloadConflicts.Length > 0;
            this.showNamingConflicts = this.nameConflicts.Length > 0;
            this.lv1.totalRows = this.downloadConflicts.Length;
            this.lv2.totalRows = this.nameConflicts.Length;
            this.selectedLV1Items = new bool[this.downloadConflicts.Length];
            this.selectedLV2Items = new bool[this.nameConflicts.Length];
            this.DoSelectionChange();
        }

        private bool AtLeastOneSelectedAssetCanBeMerged()
        {
            for (int i = 0; i < this.downloadConflicts.Length; i++)
            {
                if ((this.selectedLV1Items[i] && !AssetServer.AssetIsBinaryByGUID(this.downloadConflicts[i])) && !AssetServer.IsItemDeleted(this.downloadConflicts[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public bool CanContinue()
        {
            return this.enableContinueButton;
        }

        private void ContextMenuClick(object userData, string[] options, int selected)
        {
            if (selected >= 0)
            {
                string key = this.dropDownMenuItems[selected];
                if (key != null)
                {
                    int num;
                    if (<>f__switch$map13 == null)
                    {
                        Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
                        dictionary.Add("Compare", 0);
                        dictionary.Add("Compare Binary", 1);
                        <>f__switch$map13 = dictionary;
                    }
                    if (<>f__switch$map13.TryGetValue(key, out num))
                    {
                        if (num == 0)
                        {
                            this.DoShowDiff(false);
                        }
                        else if (num == 1)
                        {
                            this.DoShowDiff(true);
                        }
                    }
                }
            }
        }

        private void DoDownloadConflictsGUI()
        {
            bool enabled = GUI.enabled;
            bool shift = Event.current.shift;
            bool actionKey = EditorGUI.actionKey;
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.Label("The following assets have been changed both on the server and in the local project.\nPlease select a conflict resolution for each before continuing the update.", new GUILayoutOption[0]);
            GUILayout.Space(10f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            GUI.enabled = this.lv1HasSelection && enabled;
            if (GUILayout.Button(conflictButtonTexts[0], this.constants.ButtonLeft, new GUILayoutOption[0]))
            {
                this.ResolveSelectedDownloadConflicts(DownloadResolution.SkipAsset);
            }
            if (GUILayout.Button(conflictButtonTexts[1], this.constants.ButtonMiddle, new GUILayoutOption[0]))
            {
                this.ResolveSelectedDownloadConflicts(DownloadResolution.TrashMyChanges);
            }
            if (GUILayout.Button(conflictButtonTexts[2], this.constants.ButtonMiddle, new GUILayoutOption[0]))
            {
                this.ResolveSelectedDownloadConflicts(DownloadResolution.TrashServerChanges);
            }
            if (!this.enableMergeButton)
            {
                GUI.enabled = false;
            }
            if (GUILayout.Button(conflictButtonTexts[3], this.constants.ButtonRight, new GUILayoutOption[0]))
            {
                this.ResolveSelectedDownloadConflicts(DownloadResolution.Merge);
            }
            GUI.enabled = enabled;
            GUILayout.EndHorizontal();
            GUILayout.Space(5f);
            SplitterGUILayout.BeginHorizontalSplit(this.lvHeaderSplit1, new GUILayoutOption[0]);
            GUILayout.Box("Action", this.constants.lvHeader, new GUILayoutOption[0]);
            GUILayout.Box("Asset", this.constants.lvHeader, new GUILayoutOption[0]);
            SplitterGUILayout.EndHorizontalSplit();
            int row = this.lv1.row;
            bool flag4 = false;
            IEnumerator enumerator = ListViewGUILayout.ListView(this.lv1, this.constants.background, new GUILayoutOption[0]).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    ListViewElement current = (ListViewElement) enumerator.Current;
                    if (((GUIUtility.keyboardControl == this.lv1.ID) && (Event.current.type == EventType.KeyDown)) && actionKey)
                    {
                        Event.current.Use();
                    }
                    if (this.selectedLV1Items[current.row] && (Event.current.type == EventType.Repaint))
                    {
                        this.constants.selected.Draw(current.position, false, false, false, false);
                    }
                    if (ListViewGUILayout.HasMouseUp(current.position))
                    {
                        if (!shift && !actionKey)
                        {
                            flag4 |= ListViewGUILayout.MultiSelection(row, this.lv1.row, ref this.initialSelectedLV1Item, ref this.selectedLV1Items);
                        }
                    }
                    else if (ListViewGUILayout.HasMouseDown(current.position))
                    {
                        if ((Event.current.clickCount == 2) && !AssetServer.AssetIsDir(this.downloadConflicts[current.row]))
                        {
                            this.DoShowDiff(false);
                            GUIUtility.ExitGUI();
                        }
                        else
                        {
                            if ((!this.selectedLV1Items[current.row] || shift) || actionKey)
                            {
                                flag4 |= ListViewGUILayout.MultiSelection(row, current.row, ref this.initialSelectedLV1Item, ref this.selectedLV1Items);
                            }
                            this.lv1.row = current.row;
                        }
                    }
                    else if (ListViewGUILayout.HasMouseDown(current.position, 1))
                    {
                        if (!this.selectedLV1Items[current.row])
                        {
                            flag4 = true;
                            for (int i = 0; i < this.selectedLV1Items.Length; i++)
                            {
                                this.selectedLV1Items[i] = false;
                            }
                            this.lv1.selectionChanged = true;
                            this.selectedLV1Items[current.row] = true;
                            this.lv1.row = current.row;
                        }
                        GUIUtility.hotControl = 0;
                        Rect position = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 1f, 1f);
                        EditorUtility.DisplayCustomMenu(position, this.dropDownMenuItems, null, new EditorUtility.SelectMenuItemFunction(this.ContextMenuClick), null);
                        Event.current.Use();
                    }
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width((float) this.lvHeaderSplit1.realSizes[0]), GUILayout.Height(18f) };
                    GUILayout.Label(this.downloadResolutionString[(int) this.downloadResolutions[current.row]], options);
                    if (this.deletionConflict[current.row] && (Event.current.type == EventType.Repaint))
                    {
                        GUIContent badgeDelete = ASMainWindow.constants.badgeDelete;
                        Rect rect2 = new Rect(((current.position.x + this.lvHeaderSplit1.realSizes[0]) - badgeDelete.image.width) - 5f, (current.position.y + (current.position.height / 2f)) - (badgeDelete.image.height / 2), (float) badgeDelete.image.width, (float) badgeDelete.image.height);
                        EditorGUIUtility.SetIconSize(Vector2.zero);
                        GUIStyle.none.Draw(rect2, badgeDelete, false, false, false, false);
                        EditorGUIUtility.SetIconSize(this.iconSize);
                    }
                    GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width((float) this.lvHeaderSplit1.realSizes[1]), GUILayout.Height(18f) };
                    GUILayout.Label(new GUIContent(this.dConflictPaths[current.row], !AssetServer.AssetIsDir(this.downloadConflicts[current.row]) ? InternalEditorUtility.GetIconForFile(this.dConflictPaths[current.row]) : EditorGUIUtility.FindTexture(EditorResourcesUtility.folderIconName)), optionArray2);
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
            GUILayout.EndVertical();
            if (GUIUtility.keyboardControl == this.lv1.ID)
            {
                if ((Event.current.type == EventType.ValidateCommand) && (Event.current.commandName == "SelectAll"))
                {
                    Event.current.Use();
                }
                else if ((Event.current.type == EventType.ExecuteCommand) && (Event.current.commandName == "SelectAll"))
                {
                    for (int j = 0; j < this.selectedLV1Items.Length; j++)
                    {
                        this.selectedLV1Items[j] = true;
                    }
                    flag4 = true;
                    Event.current.Use();
                }
                if (this.lv1.selectionChanged && !actionKey)
                {
                    flag4 |= ListViewGUILayout.MultiSelection(row, this.lv1.row, ref this.initialSelectedLV1Item, ref this.selectedLV1Items);
                }
                else if (((GUIUtility.keyboardControl == this.lv1.ID) && (Event.current.type == EventType.KeyDown)) && ((Event.current.keyCode == KeyCode.Return) && !AssetServer.AssetIsDir(this.downloadConflicts[this.lv1.row])))
                {
                    this.DoShowDiff(false);
                    GUIUtility.ExitGUI();
                }
            }
            if (this.lv1.selectionChanged || flag4)
            {
                this.mySelection = true;
                AssetServer.SetSelectionFromGUIDs(this.GetSelectedGUIDs());
                this.lv1HasSelection = this.HasTrue(ref this.selectedLV1Items);
                this.enableMergeButton = this.AtLeastOneSelectedAssetCanBeMerged();
            }
        }

        public bool DoGUI(ASUpdateWindow parentWin)
        {
            if (this.constants == null)
            {
                this.constants = new Constants();
            }
            bool enabled = GUI.enabled;
            EditorGUIUtility.SetIconSize(this.iconSize);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            if (this.showDownloadConflicts)
            {
                this.DoDownloadConflictsGUI();
            }
            if (this.showNamingConflicts)
            {
                this.DoNamingConflictsGUI();
            }
            GUILayout.EndVertical();
            EditorGUIUtility.SetIconSize(Vector2.zero);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUI.enabled = this.lv1HasSelection && enabled;
            if (GUILayout.Button("Compare", this.constants.button, new GUILayoutOption[0]))
            {
                if (!this.DoShowDiff(false))
                {
                    Debug.Log("No differences found");
                }
                GUIUtility.ExitGUI();
            }
            GUI.enabled = enabled;
            GUILayout.FlexibleSpace();
            GUI.enabled = parentWin.CanContinue && enabled;
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinWidth(100f) };
            if (GUILayout.Button("Continue", this.constants.bigButton, options))
            {
                parentWin.DoUpdate(true);
                return false;
            }
            GUI.enabled = enabled;
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.MinWidth(100f) };
            if (GUILayout.Button("Cancel", this.constants.bigButton, optionArray2))
            {
                return false;
            }
            GUILayout.EndHorizontal();
            if (!this.splittersOk && (Event.current.type == EventType.Repaint))
            {
                this.splittersOk = true;
                parentWin.Repaint();
            }
            return true;
        }

        private void DoNamingConflictsGUI()
        {
            bool enabled = GUI.enabled;
            bool shift = Event.current.shift;
            bool actionKey = EditorGUI.actionKey;
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.Space(10f);
            GUILayout.Label("The following assets have the same name as an existing asset on the server.\nPlease select which one to rename before continuing the update.", new GUILayoutOption[0]);
            GUILayout.Space(10f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            GUI.enabled = this.lv2HasSelection && enabled;
            if (GUILayout.Button(nameConflictButtonTexts[0], this.constants.ButtonLeft, new GUILayoutOption[0]))
            {
                this.ResolveSelectedNamingConflicts(NameConflictResolution.RenameLocal);
            }
            if (GUILayout.Button(nameConflictButtonTexts[1], this.constants.ButtonRight, new GUILayoutOption[0]))
            {
                this.ResolveSelectedNamingConflicts(NameConflictResolution.RenameRemote);
            }
            GUI.enabled = enabled;
            GUILayout.EndHorizontal();
            GUILayout.Space(5f);
            SplitterGUILayout.BeginHorizontalSplit(this.lvHeaderSplit2, new GUILayoutOption[0]);
            GUILayout.Box("Action", this.constants.lvHeader, new GUILayoutOption[0]);
            GUILayout.Box("Asset", this.constants.lvHeader, new GUILayoutOption[0]);
            SplitterGUILayout.EndHorizontalSplit();
            int row = this.lv2.row;
            bool flag4 = false;
            IEnumerator enumerator = ListViewGUILayout.ListView(this.lv2, this.constants.background, new GUILayoutOption[0]).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    ListViewElement current = (ListViewElement) enumerator.Current;
                    if (((GUIUtility.keyboardControl == this.lv2.ID) && (Event.current.type == EventType.KeyDown)) && actionKey)
                    {
                        Event.current.Use();
                    }
                    if (this.selectedLV2Items[current.row] && (Event.current.type == EventType.Repaint))
                    {
                        this.constants.selected.Draw(current.position, false, false, false, false);
                    }
                    if (ListViewGUILayout.HasMouseUp(current.position))
                    {
                        if (!shift && !actionKey)
                        {
                            flag4 |= ListViewGUILayout.MultiSelection(row, this.lv2.row, ref this.initialSelectedLV2Item, ref this.selectedLV2Items);
                        }
                    }
                    else if (ListViewGUILayout.HasMouseDown(current.position))
                    {
                        if ((!this.selectedLV2Items[current.row] || shift) || actionKey)
                        {
                            flag4 |= ListViewGUILayout.MultiSelection(row, current.row, ref this.initialSelectedLV2Item, ref this.selectedLV2Items);
                        }
                        this.lv2.row = current.row;
                    }
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width((float) this.lvHeaderSplit2.realSizes[0]), GUILayout.Height(18f) };
                    GUILayout.Label(this.namingResolutionString[(int) this.namingResolutions[current.row]], options);
                    GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width((float) this.lvHeaderSplit2.realSizes[1]), GUILayout.Height(18f) };
                    GUILayout.Label(new GUIContent(this.dNamingPaths[current.row], !AssetServer.AssetIsDir(this.nameConflicts[current.row]) ? InternalEditorUtility.GetIconForFile(this.dNamingPaths[current.row]) : EditorGUIUtility.FindTexture(EditorResourcesUtility.folderIconName)), optionArray2);
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
            GUILayout.EndVertical();
            if (GUIUtility.keyboardControl == this.lv2.ID)
            {
                if ((Event.current.type == EventType.ValidateCommand) && (Event.current.commandName == "SelectAll"))
                {
                    Event.current.Use();
                }
                else if ((Event.current.type == EventType.ExecuteCommand) && (Event.current.commandName == "SelectAll"))
                {
                    for (int i = 0; i < this.selectedLV2Items.Length; i++)
                    {
                        this.selectedLV2Items[i] = true;
                    }
                    flag4 = true;
                    Event.current.Use();
                }
                if (this.lv2.selectionChanged && !actionKey)
                {
                    flag4 |= ListViewGUILayout.MultiSelection(row, this.lv2.row, ref this.initialSelectedLV2Item, ref this.selectedLV2Items);
                }
            }
            if (this.lv2.selectionChanged || flag4)
            {
                this.mySelection = true;
                AssetServer.SetSelectionFromGUIDs(this.GetSelectedNamingGUIDs());
                this.lv2HasSelection = this.HasTrue(ref this.selectedLV2Items);
            }
        }

        private void DoSelectionChange()
        {
            HierarchyProperty property = new HierarchyProperty(HierarchyType.Assets);
            List<string> list = new List<string>(Selection.objects.Length);
            foreach (Object obj2 in Selection.objects)
            {
                if (property.Find(obj2.GetInstanceID(), null))
                {
                    list.Add(property.guid);
                }
            }
            for (int i = 0; i < this.downloadConflicts.Length; i++)
            {
                this.selectedLV1Items[i] = list.Contains(this.downloadConflicts[i]);
            }
            for (int j = 0; j < this.nameConflicts.Length; j++)
            {
                this.selectedLV2Items[j] = list.Contains(this.nameConflicts[j]);
            }
            this.lv1HasSelection = this.HasTrue(ref this.selectedLV1Items);
            this.lv2HasSelection = this.HasTrue(ref this.selectedLV2Items);
            this.enableMergeButton = this.AtLeastOneSelectedAssetCanBeMerged();
        }

        private bool DoShowDiff(bool binary)
        {
            List<string> list = new List<string>();
            List<CompareInfo> list2 = new List<CompareInfo>();
            for (int i = 0; i < this.selectedLV1Items.Length; i++)
            {
                if (this.selectedLV1Items[i])
                {
                    int serverItemChangeset = AssetServer.GetServerItemChangeset(this.downloadConflicts[i], -1);
                    int num3 = !AssetServer.HasDeletionConflict(this.downloadConflicts[i]) ? -1 : -2;
                    list.Add(this.downloadConflicts[i]);
                    list2.Add(new CompareInfo(serverItemChangeset, num3, !binary ? 0 : 1, !binary ? 1 : 0));
                }
            }
            if (list.Count != 0)
            {
                AssetServer.CompareFiles(list.ToArray(), list2.ToArray());
                return true;
            }
            return false;
        }

        public string[] GetDownloadConflicts()
        {
            return this.downloadConflicts;
        }

        public string[] GetNameConflicts()
        {
            return this.nameConflicts;
        }

        private string[] GetSelectedGUIDs()
        {
            List<string> list = new List<string>();
            for (int i = 0; i < this.downloadConflicts.Length; i++)
            {
                if (this.selectedLV1Items[i])
                {
                    list.Add(this.downloadConflicts[i]);
                }
            }
            return list.ToArray();
        }

        private string[] GetSelectedNamingGUIDs()
        {
            List<string> list = new List<string>();
            for (int i = 0; i < this.nameConflicts.Length; i++)
            {
                if (this.selectedLV2Items[i])
                {
                    list.Add(this.nameConflicts[i]);
                }
            }
            return list.ToArray();
        }

        private bool HasTrue(ref bool[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i])
                {
                    return true;
                }
            }
            return false;
        }

        public void OnSelectionChange(ASUpdateWindow parentWin)
        {
            if (!this.mySelection)
            {
                this.DoSelectionChange();
                parentWin.Repaint();
            }
            else
            {
                this.mySelection = false;
            }
        }

        private void ResolveSelectedDownloadConflicts(DownloadResolution res)
        {
            int index = -1;
            bool flag = false;
            for (int i = 0; i < this.downloadConflicts.Length; i++)
            {
                if (this.selectedLV1Items[i])
                {
                    string guid = this.downloadConflicts[i];
                    if ((res == DownloadResolution.Merge) && (AssetServer.AssetIsBinaryByGUID(guid) || AssetServer.IsItemDeleted(guid)))
                    {
                        flag = true;
                    }
                    else
                    {
                        if (res != DownloadResolution.Unresolved)
                        {
                            if (AssetServer.GetDownloadResolution(guid) == DownloadResolution.Unresolved)
                            {
                                this.downloadConflictsToResolve--;
                            }
                        }
                        else
                        {
                            this.downloadConflictsToResolve++;
                        }
                        this.downloadResolutions[i] = res;
                        AssetServer.SetDownloadResolution(guid, res);
                        index = (index != -1) ? -2 : i;
                    }
                }
            }
            this.enableContinueButton = this.downloadConflictsToResolve == 0;
            if (index >= 0)
            {
                this.selectedLV1Items[index] = false;
                if (index < (this.selectedLV1Items.Length - 1))
                {
                    this.selectedLV1Items[index + 1] = true;
                }
            }
            this.enableMergeButton = this.AtLeastOneSelectedAssetCanBeMerged();
            if (flag)
            {
                EditorUtility.DisplayDialog("Some conflicting changes cannot be merged", "Notice that not all selected changes where selected for merging. This happened because not all of them can be merged (e.g. assets are binary or deleted).", "OK");
            }
        }

        private void ResolveSelectedNamingConflicts(NameConflictResolution res)
        {
            if (res != NameConflictResolution.Unresolved)
            {
                for (int i = 0; i < this.nameConflicts.Length; i++)
                {
                    if (this.selectedLV2Items[i])
                    {
                        string guid = this.nameConflicts[i];
                        if (AssetServer.GetNameConflictResolution(guid) == NameConflictResolution.Unresolved)
                        {
                            this.downloadConflictsToResolve--;
                        }
                        this.namingResolutions[i] = res;
                        AssetServer.SetNameConflictResolution(guid, res);
                    }
                }
                this.enableContinueButton = this.downloadConflictsToResolve == 0;
            }
        }

        private class Constants
        {
            public GUIStyle background = "OL Box";
            public GUIStyle bigButton = "LargeButton";
            public GUIStyle button = "Button";
            public GUIStyle ButtonLeft = "ButtonLeft";
            public GUIStyle ButtonMiddle = "ButtonMid";
            public GUIStyle ButtonRight = "ButtonRight";
            public GUIStyle EntryNormal = "ServerUpdateInfo";
            public GUIStyle EntrySelected = "ServerUpdateChangesetOn";
            public GUIStyle lvHeader = "OL title";
            public GUIStyle selected = "ServerUpdateChangesetOn";
        }
    }
}

