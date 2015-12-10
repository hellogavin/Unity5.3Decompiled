namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    [Serializable]
    internal class ASUpdateWindow
    {
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map14;
        private ASUpdateConflictResolveWindow asResolveWin;
        private Changeset[] changesets;
        private Constants constants;
        private string[] dropDownMenuItems = new string[] { "Compare", "Compare Binary" };
        private SplitterState horSplit;
        private Vector2 iconSize = new Vector2(16f, 16f);
        private bool isDirSelected;
        private ListViewState lv;
        private int maxNickLength;
        private string[] messageFirstLines;
        private ASMainWindow parentWin;
        private ParentViewState pv = new ParentViewState();
        private string selectedGUID = string.Empty;
        private bool showingConflicts;
        private string totalUpdates;
        private SplitterState vertSplit;

        public ASUpdateWindow(ASMainWindow parentWin, Changeset[] changesets)
        {
            float[] relativeSizes = new float[] { 50f, 50f };
            int[] minSizes = new int[] { 50, 50 };
            this.horSplit = new SplitterState(relativeSizes, minSizes, null);
            float[] singleArray2 = new float[] { 60f, 30f };
            int[] numArray2 = new int[] { 0x20, 0x20 };
            this.vertSplit = new SplitterState(singleArray2, numArray2, null);
            this.changesets = changesets;
            this.parentWin = parentWin;
            this.lv = new ListViewState(changesets.Length, 5);
            this.pv.lv = new ListViewState(0, 5);
            this.messageFirstLines = new string[changesets.Length];
            for (int i = 0; i < changesets.Length; i++)
            {
                char[] separator = new char[] { '\n' };
                this.messageFirstLines[i] = changesets[i].message.Split(separator)[0];
            }
            this.totalUpdates = changesets.Length.ToString() + ((changesets.Length != 1) ? " Updates" : " Update");
        }

        private void ContextMenuClick(object userData, string[] options, int selected)
        {
            if (selected >= 0)
            {
                string key = this.dropDownMenuItems[selected];
                if (key != null)
                {
                    int num;
                    if (<>f__switch$map14 == null)
                    {
                        Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
                        dictionary.Add("Compare", 0);
                        dictionary.Add("Compare Binary", 1);
                        <>f__switch$map14 = dictionary;
                    }
                    if (<>f__switch$map14.TryGetValue(key, out num))
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

        public bool DoGUI()
        {
            bool enabled = GUI.enabled;
            if (this.constants == null)
            {
                this.constants = new Constants();
                this.maxNickLength = 1;
                for (int i = 0; i < this.changesets.Length; i++)
                {
                    int x = (int) this.constants.serverUpdateInfo.CalcSize(new GUIContent(this.changesets[i].owner)).x;
                    if (x > this.maxNickLength)
                    {
                        this.maxNickLength = x;
                    }
                }
            }
            EditorGUIUtility.SetIconSize(this.iconSize);
            if (this.showingConflicts)
            {
                if (!this.asResolveWin.DoGUI(this))
                {
                    this.showingConflicts = false;
                }
            }
            else
            {
                this.UpdateGUI();
            }
            EditorGUIUtility.SetIconSize(Vector2.zero);
            if (!this.showingConflicts)
            {
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUI.enabled = (!this.isDirSelected && (this.selectedGUID != string.Empty)) && enabled;
                if (GUILayout.Button("Compare", this.constants.button, new GUILayoutOption[0]))
                {
                    this.DoShowDiff(false);
                    GUIUtility.ExitGUI();
                }
                GUI.enabled = enabled;
                GUILayout.FlexibleSpace();
                if (this.changesets.Length == 0)
                {
                    GUI.enabled = false;
                }
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinWidth(100f) };
                if (GUILayout.Button("Update", this.constants.bigButton, options))
                {
                    if (this.changesets.Length == 0)
                    {
                        Debug.Log("Nothing to update.");
                    }
                    else
                    {
                        this.DoUpdate(false);
                    }
                    this.parentWin.Repaint();
                    GUIUtility.ExitGUI();
                }
                if (this.changesets.Length == 0)
                {
                    GUI.enabled = enabled;
                }
                GUILayout.EndHorizontal();
                if (AssetServer.GetAssetServerError() != string.Empty)
                {
                    GUILayout.Space(10f);
                    GUILayout.Label(AssetServer.GetAssetServerError(), this.constants.errorLabel, new GUILayoutOption[0]);
                    GUILayout.Space(10f);
                }
            }
            GUILayout.Space(10f);
            return true;
        }

        private void DoSelect(int folderI, int fileI, int row)
        {
            this.pv.selectedFile = fileI;
            this.pv.selectedFolder = folderI;
            this.pv.lv.row = row;
            this.pv.lv.selectionChanged = true;
            if (fileI == -1)
            {
                if (folderI != -1)
                {
                    this.selectedGUID = this.pv.folders[folderI].guid;
                    this.isDirSelected = true;
                }
                else
                {
                    this.selectedGUID = string.Empty;
                    this.isDirSelected = false;
                }
            }
            else
            {
                this.selectedGUID = this.pv.folders[folderI].files[fileI].guid;
                this.isDirSelected = false;
            }
        }

        private void DoSelectionChange()
        {
            if (this.lv.row != -1)
            {
                string firstSelected = this.GetFirstSelected();
                if (firstSelected != string.Empty)
                {
                    this.selectedGUID = firstSelected;
                }
                if (AssetServer.IsGUIDValid(this.selectedGUID) != 0)
                {
                    int num = 0;
                    this.pv.lv.row = -1;
                    foreach (ParentViewFolder folder in this.pv.folders)
                    {
                        if (folder.guid == this.selectedGUID)
                        {
                            this.pv.lv.row = num;
                            return;
                        }
                        num++;
                        foreach (ParentViewFile file in folder.files)
                        {
                            if (file.guid == this.selectedGUID)
                            {
                                this.pv.lv.row = num;
                                return;
                            }
                            num++;
                        }
                    }
                }
                else
                {
                    this.pv.lv.row = -1;
                }
            }
        }

        private bool DoShowDiff(bool binary)
        {
            List<string> list = new List<string>();
            List<CompareInfo> list2 = new List<CompareInfo>();
            int changeset = -1;
            int num2 = -1;
            if (AssetServer.IsItemDeleted(this.selectedGUID))
            {
                changeset = -2;
            }
            else
            {
                changeset = AssetServer.GetWorkingItemChangeset(this.selectedGUID);
                changeset = AssetServer.GetServerItemChangeset(this.selectedGUID, changeset);
            }
            int serverItemChangeset = AssetServer.GetServerItemChangeset(this.selectedGUID, -1);
            num2 = (serverItemChangeset != -1) ? serverItemChangeset : -2;
            list.Add(this.selectedGUID);
            list2.Add(new CompareInfo(changeset, num2, !binary ? 0 : 1, !binary ? 1 : 0));
            if (list.Count != 0)
            {
                AssetServer.CompareFiles(list.ToArray(), list2.ToArray());
                return true;
            }
            return false;
        }

        public bool DoUpdate(bool afterResolvingConflicts)
        {
            AssetServer.RemoveMaintErrorsFromConsole();
            if (ASEditorBackend.SettingsIfNeeded())
            {
                this.showingConflicts = false;
                AssetServer.SetAfterActionFinishedCallback("ASEditorBackend", "CBReinitOnSuccess");
                AssetServer.DoUpdateOnNextTick(!afterResolvingConflicts, "ShowASConflictResolutionsWindow");
            }
            return true;
        }

        private string GetFirstSelected()
        {
            Object[] filtered = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
            return ((filtered.Length == 0) ? string.Empty : AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(filtered[0])));
        }

        public string[] GetGUIDs()
        {
            List<string> list = new List<string>();
            if (this.lv.row < 0)
            {
                return null;
            }
            for (int i = this.lv.row; i < this.lv.totalRows; i++)
            {
                for (int j = 0; j < this.changesets[i].items.Length; j++)
                {
                    if (!list.Contains(this.changesets[i].items[j].guid))
                    {
                        list.Add(this.changesets[i].items[j].guid);
                    }
                }
            }
            return list.ToArray();
        }

        public int GetSelectedRevisionNumber()
        {
            return (((this.pv.lv.row <= (this.lv.totalRows - 1)) && (this.lv.row >= 0)) ? this.changesets[this.lv.row].changeset : -1);
        }

        private bool HasFlag(ChangeFlags flags, ChangeFlags flagToCheck)
        {
            return ((flagToCheck & flags) != ChangeFlags.None);
        }

        public void OnSelectionChange()
        {
            if (this.showingConflicts)
            {
                this.asResolveWin.OnSelectionChange(this);
            }
            else
            {
                this.DoSelectionChange();
                this.parentWin.Repaint();
            }
        }

        public void Repaint()
        {
            this.parentWin.Repaint();
        }

        public void SetSelectedRevisionLine(int selIndex)
        {
            if (selIndex >= this.lv.totalRows)
            {
                this.pv.Clear();
                this.lv.row = -1;
            }
            else
            {
                this.lv.row = selIndex;
                this.pv.Clear();
                this.pv.AddAssetItems(this.changesets[selIndex]);
                this.pv.SetLineCount();
            }
            this.pv.lv.scrollPos = Vector2.zero;
            this.pv.lv.row = -1;
            this.pv.selectedFolder = -1;
            this.pv.selectedFile = -1;
            this.DoSelectionChange();
        }

        public void ShowConflictResolutions(string[] conflicting)
        {
            this.asResolveWin = new ASUpdateConflictResolveWindow(conflicting);
            this.showingConflicts = true;
        }

        public void UpdateGUI()
        {
            SplitterGUILayout.BeginHorizontalSplit(this.horSplit, new GUILayoutOption[0]);
            GUILayout.BeginVertical(this.constants.box, new GUILayoutOption[0]);
            GUILayout.Label(this.totalUpdates, this.constants.title, new GUILayoutOption[0]);
            IEnumerator enumerator = ListViewGUILayout.ListView(this.lv, GUIStyle.none, new GUILayoutOption[0]).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    ListViewElement current = (ListViewElement) enumerator.Current;
                    Rect position = current.position;
                    position.x++;
                    position.y++;
                    if (Event.current.type == EventType.Repaint)
                    {
                        if ((current.row % 2) == 0)
                        {
                            this.constants.entryEven.Draw(position, false, false, false, false);
                        }
                        else
                        {
                            this.constants.entryOdd.Draw(position, false, false, false, false);
                        }
                    }
                    GUILayout.BeginVertical((current.row != this.lv.row) ? this.constants.entryNormal : this.constants.entrySelected, new GUILayoutOption[0]);
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinWidth(50f) };
                    GUILayout.Label(this.messageFirstLines[current.row], this.constants.serverUpdateLog, options);
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.MinWidth(100f) };
                    GUILayout.Label(this.changesets[current.row].changeset.ToString() + " " + this.changesets[current.row].date, this.constants.serverUpdateInfo, optionArray2);
                    GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.Width((float) this.maxNickLength) };
                    GUILayout.Label(this.changesets[current.row].owner, this.constants.serverUpdateInfo, optionArray3);
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
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
            if (this.lv.selectionChanged)
            {
                this.SetSelectedRevisionLine(this.lv.row);
            }
            GUILayout.EndVertical();
            SplitterGUILayout.BeginVerticalSplit(this.vertSplit, new GUILayoutOption[0]);
            GUILayout.BeginVertical(this.constants.box, new GUILayoutOption[0]);
            GUILayout.Label("Changeset", this.constants.title, new GUILayoutOption[0]);
            int num = -1;
            int file = -1;
            IEnumerator enumerator2 = ListViewGUILayout.ListView(this.pv.lv, GUIStyle.none, new GUILayoutOption[0]).GetEnumerator();
            try
            {
                while (enumerator2.MoveNext())
                {
                    ChangeFlags changeFlags;
                    ListViewElement element2 = (ListViewElement) enumerator2.Current;
                    if ((num == -1) && !this.pv.IndexToFolderAndFile(element2.row, ref num, ref file))
                    {
                        return;
                    }
                    ParentViewFolder folder = this.pv.folders[num];
                    if (ListViewGUILayout.HasMouseDown(element2.position))
                    {
                        if (Event.current.clickCount == 2)
                        {
                            if (!this.isDirSelected && (this.selectedGUID != string.Empty))
                            {
                                this.DoShowDiff(false);
                                GUIUtility.ExitGUI();
                            }
                        }
                        else
                        {
                            this.pv.lv.scrollPos = ListViewShared.ListViewScrollToRow(this.pv.lv.ilvState, element2.row);
                            this.DoSelect(num, file, element2.row);
                        }
                    }
                    else if (ListViewGUILayout.HasMouseDown(element2.position, 1))
                    {
                        if (this.lv.row != element2.row)
                        {
                            this.DoSelect(num, file, element2.row);
                        }
                        if (!this.isDirSelected && (this.selectedGUID != string.Empty))
                        {
                            GUIUtility.hotControl = 0;
                            Rect rect2 = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 1f, 1f);
                            EditorUtility.DisplayCustomMenu(rect2, this.dropDownMenuItems, null, new EditorUtility.SelectMenuItemFunction(this.ContextMenuClick), null);
                            Event.current.Use();
                        }
                    }
                    if ((element2.row == this.pv.lv.row) && (Event.current.type == EventType.Repaint))
                    {
                        this.constants.entrySelected.Draw(element2.position, false, false, false, false);
                    }
                    if (file != -1)
                    {
                        Texture2D cachedIcon = AssetDatabase.GetCachedIcon(folder.name + "/" + folder.files[file].name) as Texture2D;
                        if (cachedIcon == null)
                        {
                            cachedIcon = InternalEditorUtility.GetIconForFile(folder.files[file].name);
                        }
                        GUILayout.Label(new GUIContent(folder.files[file].name, cachedIcon), this.constants.element, new GUILayoutOption[0]);
                        changeFlags = folder.files[file].changeFlags;
                    }
                    else
                    {
                        GUILayout.Label(folder.name, this.constants.header, new GUILayoutOption[0]);
                        changeFlags = folder.changeFlags;
                    }
                    GUIContent badgeNew = null;
                    if (this.HasFlag(changeFlags, ChangeFlags.Undeleted) || this.HasFlag(changeFlags, ChangeFlags.Created))
                    {
                        badgeNew = ASMainWindow.constants.badgeNew;
                    }
                    else if (this.HasFlag(changeFlags, ChangeFlags.Deleted))
                    {
                        badgeNew = ASMainWindow.constants.badgeDelete;
                    }
                    else if (this.HasFlag(changeFlags, ChangeFlags.Renamed) || this.HasFlag(changeFlags, ChangeFlags.Moved))
                    {
                        badgeNew = ASMainWindow.constants.badgeMove;
                    }
                    if ((badgeNew != null) && (Event.current.type == EventType.Repaint))
                    {
                        Rect rect3 = new Rect(((element2.position.x + element2.position.width) - badgeNew.image.width) - 5f, (element2.position.y + (element2.position.height / 2f)) - (badgeNew.image.height / 2), (float) badgeNew.image.width, (float) badgeNew.image.height);
                        EditorGUIUtility.SetIconSize(Vector2.zero);
                        GUIStyle.none.Draw(rect3, badgeNew, false, false, false, false);
                        EditorGUIUtility.SetIconSize(this.iconSize);
                    }
                    this.pv.NextFileFolder(ref num, ref file);
                }
            }
            finally
            {
                IDisposable disposable2 = enumerator2 as IDisposable;
                if (disposable2 == null)
                {
                }
                disposable2.Dispose();
            }
            if (this.pv.lv.selectionChanged && (this.selectedGUID != string.Empty))
            {
                if (this.selectedGUID != AssetServer.GetRootGUID())
                {
                    AssetServer.SetSelectionFromGUID(this.selectedGUID);
                }
                else
                {
                    AssetServer.SetSelectionFromGUID(string.Empty);
                }
            }
            if ((((GUIUtility.keyboardControl == this.pv.lv.ID) && (Event.current.type == EventType.KeyDown)) && ((Event.current.keyCode == KeyCode.Return) && !this.isDirSelected)) && (this.selectedGUID != string.Empty))
            {
                this.DoShowDiff(false);
                GUIUtility.ExitGUI();
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical(this.constants.box, new GUILayoutOption[0]);
            GUILayout.Label("Update Message", this.constants.title, new GUILayoutOption[0]);
            GUILayout.TextArea((this.lv.row < 0) ? string.Empty : this.changesets[this.lv.row].message, this.constants.wwText, new GUILayoutOption[0]);
            GUILayout.EndVertical();
            SplitterGUILayout.EndVerticalSplit();
            SplitterGUILayout.EndHorizontalSplit();
        }

        public bool CanContinue
        {
            get
            {
                return this.asResolveWin.CanContinue();
            }
        }

        public bool ShowingConflicts
        {
            get
            {
                return this.showingConflicts;
            }
        }

        internal class Constants
        {
            public GUIStyle bigButton = "LargeButton";
            public GUIStyle box = "OL Box";
            public GUIStyle button = "Button";
            public GUIStyle element = "OL elem";
            public GUIStyle entryEven = "CN EntryBackEven";
            public GUIStyle entryNormal = "ServerUpdateChangeset";
            public GUIStyle entryOdd = "CN EntryBackOdd";
            public GUIStyle entrySelected = "ServerUpdateChangesetOn";
            public GUIStyle errorLabel = "ErrorLabel";
            public GUIStyle header = "OL header";
            public GUIStyle serverChangeCount = "ServerChangeCount";
            public GUIStyle serverUpdateInfo = "ServerUpdateInfo";
            public GUIStyle serverUpdateLog = "ServerUpdateLog";
            public GUIStyle title = "OL title";
            public GUIStyle wwText = "AS TextArea";
        }
    }
}

