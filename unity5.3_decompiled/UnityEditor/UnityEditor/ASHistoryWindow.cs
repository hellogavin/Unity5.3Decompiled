namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEditorInternal;
    using UnityEngine;

    [Serializable]
    internal class ASHistoryWindow
    {
        private static GUIContent emptyGUIContent = new GUIContent();
        private const int kFirst = -999999;
        private const int kLast = 0xf423f;
        private const int kUncollapsedItemsCount = 5;
        private int m_AssetSelectionIndex;
        private bool m_BinaryDiff;
        private string m_ChangeLogSelectionAssetName;
        private string m_ChangeLogSelectionGUID;
        private int m_ChangeLogSelectionRev;
        private Changeset[] m_Changesets;
        private int m_ChangesetSelectionIndex;
        private GUIContent[] m_DropDownChangesetMenuItems;
        private GUIContent[] m_DropDownMenuItems;
        private ASHistoryFileView m_FileViewWin;
        private bool m_FolderSelected;
        private GUIHistoryListItem[] m_GUIItems;
        private int m_HistoryControlID;
        private SplitterState m_HorSplit;
        private bool m_InRevisionSelectMode;
        private bool m_NextSelectionMine;
        private EditorWindow m_ParentWindow;
        private int m_Rev1ForCustomDiff;
        private int m_RowHeight;
        private Vector2 m_ScrollPos;
        private int m_ScrollViewHeight;
        private string m_SelectedGUID;
        private string m_SelectedPath;
        private bool m_SplittersOk;
        private int m_TotalHeight;
        private static int ms_HistoryControlHash = "HistoryControl".GetHashCode();
        private static Vector2 ms_IconSize = new Vector2(16f, 16f);
        private static Constants ms_Style = null;

        public ASHistoryWindow(EditorWindow parent)
        {
            float[] relativeSizes = new float[] { 30f, 70f };
            int[] minSizes = new int[] { 60, 100 };
            this.m_HorSplit = new SplitterState(relativeSizes, minSizes, null);
            this.m_ScrollPos = Vector2.zero;
            this.m_RowHeight = 0x10;
            this.m_HistoryControlID = -1;
            this.m_ChangesetSelectionIndex = -1;
            this.m_AssetSelectionIndex = -1;
            this.m_ChangeLogSelectionRev = -1;
            this.m_Rev1ForCustomDiff = -1;
            this.m_ChangeLogSelectionGUID = string.Empty;
            this.m_ChangeLogSelectionAssetName = string.Empty;
            this.m_SelectedPath = string.Empty;
            this.m_SelectedGUID = string.Empty;
            this.m_DropDownMenuItems = new GUIContent[] { EditorGUIUtility.TextContent("Show History"), emptyGUIContent, EditorGUIUtility.TextContent("Compare to Local"), EditorGUIUtility.TextContent("Compare Binary to Local"), emptyGUIContent, EditorGUIUtility.TextContent("Compare to Another Revision"), EditorGUIUtility.TextContent("Compare Binary to Another Revision"), emptyGUIContent, EditorGUIUtility.TextContent("Download This File") };
            this.m_DropDownChangesetMenuItems = new GUIContent[] { EditorGUIUtility.TextContent("Revert Entire Project to This Changeset") };
            this.m_FileViewWin = new ASHistoryFileView();
            this.m_ParentWindow = parent;
            ASEditorBackend.SettingsIfNeeded();
            if (Selection.objects.Length != 0)
            {
                this.m_FileViewWin.SelType = ASHistoryFileView.SelectionType.Items;
            }
        }

        private void CancelShowCustomDiff()
        {
            this.m_InRevisionSelectMode = false;
        }

        private void ChangesetContextMenuClick(object userData, string[] options, int selected)
        {
            if ((selected >= 0) && (selected == 0))
            {
                this.DoRevertProject();
            }
        }

        private int CheckParentViewInFilterAndMarkBoldItems(GUIHistoryListItem item, string text)
        {
            ParentViewState assets = item.assets;
            int num = -1;
            int index = 0;
            for (int i = 0; i < assets.folders.Length; i++)
            {
                ParentViewFolder folder = assets.folders[i];
                if (folder.name.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    item.boldAssets[index] = true;
                    if (num == -1)
                    {
                        num = index;
                    }
                }
                index++;
                for (int j = 0; j < folder.files.Length; j++)
                {
                    if (folder.files[j].name.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) != -1)
                    {
                        item.boldAssets[index] = true;
                        if (num == -1)
                        {
                            num = index;
                        }
                    }
                    index++;
                }
            }
            return num;
        }

        private void ClearItemSelection()
        {
            this.m_ChangeLogSelectionGUID = string.Empty;
            this.m_ChangeLogSelectionAssetName = string.Empty;
            this.m_FolderSelected = false;
            this.m_AssetSelectionIndex = -1;
        }

        private void ClearLV()
        {
            this.m_Changesets = new Changeset[0];
            this.m_TotalHeight = 5;
        }

        private void ContextMenuClick(object userData, string[] options, int selected)
        {
            if (selected >= 0)
            {
                switch (selected)
                {
                    case 0:
                        this.ShowAssetsHistory();
                        break;

                    case 2:
                        this.DoShowDiff(false, this.ChangeLogSelectionRev, -1);
                        break;

                    case 3:
                        this.DoShowDiff(true, this.ChangeLogSelectionRev, -1);
                        break;

                    case 5:
                        this.DoShowCustomDiff(false);
                        break;

                    case 6:
                        this.DoShowCustomDiff(true);
                        break;

                    case 8:
                        this.DownloadFile();
                        break;
                }
            }
        }

        public bool DoGUI(bool hasFocus)
        {
            bool enabled = GUI.enabled;
            if (ms_Style == null)
            {
                ms_Style = new Constants();
                ms_Style.entryEven = new GUIStyle(ms_Style.entryEven);
                ms_Style.entryEven.padding.left = 3;
                ms_Style.entryOdd = new GUIStyle(ms_Style.entryOdd);
                ms_Style.entryOdd.padding.left = 3;
                ms_Style.label = new GUIStyle(ms_Style.label);
                ms_Style.boldLabel = new GUIStyle(ms_Style.boldLabel);
                ms_Style.label.padding.left = 3;
                ms_Style.boldLabel.padding.left = 3;
                ms_Style.boldLabel.padding.top = 0;
                ms_Style.boldLabel.padding.bottom = 0;
                this.DoLocalSelectionChange();
            }
            EditorGUIUtility.SetIconSize(ms_IconSize);
            if ((Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.Escape))
            {
                this.CancelShowCustomDiff();
                Event.current.Use();
            }
            SplitterGUILayout.BeginHorizontalSplit(this.m_HorSplit, new GUILayoutOption[0]);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true) };
            Rect theRect = GUILayoutUtility.GetRect((float) 0f, (float) 0f, options);
            this.m_FileViewWin.DoGUI(this, theRect, hasFocus);
            GUILayout.EndVertical();
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            this.WebLikeHistory(hasFocus);
            GUILayout.EndVertical();
            SplitterGUILayout.EndHorizontalSplit();
            if (Event.current.type == EventType.Repaint)
            {
                Handles.color = Color.black;
                Handles.DrawLine(new Vector3((float) (this.m_HorSplit.realSizes[0] - 1), theRect.y, 0f), new Vector3((float) (this.m_HorSplit.realSizes[0] - 1), theRect.yMax, 0f));
                Handles.DrawLine(new Vector3(0f, theRect.yMax, 0f), new Vector3((float) Screen.width, theRect.yMax, 0f));
            }
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUI.enabled = (this.m_FileViewWin.SelType == ASHistoryFileView.SelectionType.DeletedItems) && enabled;
            if (GUILayout.Button(EditorGUIUtility.TextContent("Recover"), ms_Style.button, new GUILayoutOption[0]))
            {
                this.m_FileViewWin.DoRecover();
            }
            GUILayout.FlexibleSpace();
            if (this.m_InRevisionSelectMode)
            {
                GUI.enabled = enabled;
                GUILayout.Label(EditorGUIUtility.TextContent("Select revision to compare to"), ms_Style.boldLabel, new GUILayoutOption[0]);
            }
            GUILayout.Space(10f);
            GUI.enabled = this.IsComparableAssetSelected() && enabled;
            if (GUILayout.Button(EditorGUIUtility.TextContent("Compare to Local Version"), ms_Style.button, new GUILayoutOption[0]))
            {
                this.DoShowDiff(false, this.ChangeLogSelectionRev, -1);
                GUIUtility.ExitGUI();
            }
            GUI.enabled = ((this.ChangeLogSelectionRev > 0) && (this.m_ChangeLogSelectionGUID != string.Empty)) && enabled;
            if (GUILayout.Button(EditorGUIUtility.TextContent("Download Selected File"), ms_Style.button, new GUILayoutOption[0]))
            {
                this.DownloadFile();
            }
            GUILayout.Space(10f);
            GUI.enabled = (this.ChangeLogSelectionRev > 0) && enabled;
            if (GUILayout.Button((this.ChangeLogSelectionRev <= 0) ? "Revert Entire Project" : ("Revert Entire Project to " + this.ChangeLogSelectionRev), ms_Style.button, new GUILayoutOption[0]))
            {
                this.DoRevertProject();
            }
            GUI.enabled = enabled;
            GUILayout.EndHorizontal();
            GUILayout.Space(10f);
            if (!this.m_SplittersOk && (Event.current.type == EventType.Repaint))
            {
                this.m_SplittersOk = true;
                HandleUtility.Repaint();
            }
            EditorGUIUtility.SetIconSize(Vector2.zero);
            return true;
        }

        public void DoLocalSelectionChange()
        {
            if (this.m_NextSelectionMine)
            {
                this.m_NextSelectionMine = false;
            }
            else
            {
                Object[] filtered = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
                string[] guids = new string[0];
                switch (this.m_FileViewWin.SelType)
                {
                    case ASHistoryFileView.SelectionType.All:
                        if (Selection.objects.Length != 0)
                        {
                            Selection.objects = new Object[0];
                            this.m_NextSelectionMine = true;
                        }
                        this.m_SelectedPath = string.Empty;
                        this.m_SelectedGUID = string.Empty;
                        this.ClearLV();
                        break;

                    case ASHistoryFileView.SelectionType.Items:
                        if (filtered.Length >= 1)
                        {
                            this.m_SelectedPath = AssetDatabase.GetAssetPath(filtered[0]);
                            this.m_SelectedGUID = AssetDatabase.AssetPathToGUID(this.m_SelectedPath);
                            guids = this.m_FileViewWin.GetImplicitProjectViewSelection();
                            break;
                        }
                        this.m_SelectedPath = string.Empty;
                        this.m_SelectedGUID = string.Empty;
                        this.ClearLV();
                        return;

                    case ASHistoryFileView.SelectionType.DeletedItemsRoot:
                        if (Selection.objects.Length != 0)
                        {
                            Selection.objects = new Object[0];
                            this.m_NextSelectionMine = true;
                        }
                        guids = this.m_FileViewWin.GetAllDeletedItemGUIDs();
                        if (guids.Length != 0)
                        {
                            break;
                        }
                        this.ClearLV();
                        return;

                    case ASHistoryFileView.SelectionType.DeletedItems:
                        if (Selection.objects.Length != 0)
                        {
                            Selection.objects = new Object[0];
                            this.m_NextSelectionMine = true;
                        }
                        guids = this.m_FileViewWin.GetSelectedDeletedItemGUIDs();
                        break;
                }
                this.m_Changesets = AssetServer.GetHistorySelected(guids);
                if (this.m_Changesets != null)
                {
                    this.FilterItems(true);
                }
                else
                {
                    this.ClearLV();
                }
                if (((guids != null) && (this.m_GUIItems != null)) && (guids.Length == 1))
                {
                    this.MarkBoldItemsByGUID(this.m_SelectedGUID);
                }
                this.m_ParentWindow.Repaint();
            }
        }

        private void DoRevertProject()
        {
            if (this.ChangeLogSelectionRev > 0)
            {
                ASEditorBackend.ASWin.RevertProject(this.ChangeLogSelectionRev, this.m_Changesets);
            }
        }

        private void DoScroll()
        {
            float num3;
            float num4;
            int num = 0;
            int index = 0;
            while (index < this.m_ChangesetSelectionIndex)
            {
                if (this.m_GUIItems[index].inFilter)
                {
                    num += this.m_GUIItems[index].height;
                }
                index++;
            }
            if (this.m_ChangeLogSelectionGUID != string.Empty)
            {
                num3 = (num + ((2 + this.m_AssetSelectionIndex) * this.m_RowHeight)) + 5;
                num4 = (num3 - this.m_ScrollViewHeight) + this.m_RowHeight;
            }
            else
            {
                num3 = num;
                num4 = ((num3 - this.m_ScrollViewHeight) + this.m_GUIItems[index].height) - 10f;
            }
            this.m_ScrollPos.y = Mathf.Clamp(this.m_ScrollPos.y, num4, num3);
        }

        private void DoShowCustomDiff(bool binary)
        {
            this.ShowAssetsHistory();
            this.m_InRevisionSelectMode = true;
            this.m_BinaryDiff = binary;
            this.m_Rev1ForCustomDiff = this.ChangeLogSelectionRev;
        }

        private void DoShowDiff(bool binary, int ver1, int ver2)
        {
            List<string> list = new List<string>();
            List<CompareInfo> list2 = new List<CompareInfo>();
            if ((ver2 == -1) && (AssetDatabase.GUIDToAssetPath(this.m_ChangeLogSelectionGUID) == string.Empty))
            {
                Debug.Log("Cannot compare asset " + this.m_ChangeLogSelectionAssetName + " to local version because it does not exists.");
            }
            else
            {
                list.Add(this.m_ChangeLogSelectionGUID);
                list2.Add(new CompareInfo(ver1, ver2, !binary ? 0 : 1, !binary ? 1 : 0));
                Debug.Log("Comparing asset " + this.m_ChangeLogSelectionAssetName + " revisions " + ver1.ToString() + " and " + ((ver2 != -1) ? ver2.ToString() : "Local"));
                AssetServer.CompareFiles(list.ToArray(), list2.ToArray());
            }
        }

        private void DownloadFile()
        {
            if ((this.ChangeLogSelectionRev >= 0) && (this.m_ChangeLogSelectionGUID != string.Empty))
            {
                string[] textArray1 = new string[] { "Are you sure you want to download '", this.m_ChangeLogSelectionAssetName, "' from revision ", this.ChangeLogSelectionRev.ToString(), " and lose all changes?" };
                if (EditorUtility.DisplayDialog("Download file", string.Concat(textArray1), "Download", "Cancel"))
                {
                    AssetServer.DoRevertOnNextTick(this.ChangeLogSelectionRev, this.m_ChangeLogSelectionGUID);
                }
            }
        }

        private void DrawBadge(Rect offset, ChangeFlags flags, GUIStyle style, GUIContent content, float textColWidth)
        {
            if (Event.current.type == EventType.Repaint)
            {
                GUIContent badgeNew = null;
                if (this.HasFlag(flags, ChangeFlags.Undeleted) || this.HasFlag(flags, ChangeFlags.Created))
                {
                    badgeNew = ASMainWindow.constants.badgeNew;
                }
                else if (this.HasFlag(flags, ChangeFlags.Deleted))
                {
                    badgeNew = ASMainWindow.constants.badgeDelete;
                }
                else if (this.HasFlag(flags, ChangeFlags.Renamed) || this.HasFlag(flags, ChangeFlags.Moved))
                {
                    badgeNew = ASMainWindow.constants.badgeMove;
                }
                if (badgeNew != null)
                {
                    float num2;
                    if (style.CalcSize(content).x > (textColWidth - badgeNew.image.width))
                    {
                        num2 = (offset.xMax - badgeNew.image.width) - 5f;
                    }
                    else
                    {
                        num2 = textColWidth - badgeNew.image.width;
                    }
                    Rect position = new Rect(num2, (offset.y + (offset.height / 2f)) - (badgeNew.image.height / 2), (float) badgeNew.image.width, (float) badgeNew.image.height);
                    EditorGUIUtility.SetIconSize(Vector2.zero);
                    GUIStyle.none.Draw(position, badgeNew, false, false, false, false);
                    EditorGUIUtility.SetIconSize(ms_IconSize);
                }
            }
        }

        private void DrawParentView(Rect r, ref GUIHistoryListItem item, int changesetIndex, bool hasFocus)
        {
            ParentViewState assets = item.assets;
            GUIContent content = new GUIContent();
            Texture2D textured = EditorGUIUtility.FindTexture(EditorResourcesUtility.folderIconName);
            Event current = Event.current;
            hasFocus &= this.m_HistoryControlID == GUIUtility.keyboardControl;
            r.height = this.m_RowHeight;
            r.y += 3f;
            int index = -1;
            int num2 = (item.collapsedItemCount == 0) ? item.totalLineCount : 4;
            num2 += item.startShowingFrom;
            for (int i = 0; i < assets.folders.Length; i++)
            {
                GUIStyle label;
                ParentViewFolder folder = assets.folders[i];
                content.text = folder.name;
                content.image = textured;
                index++;
                if (index == num2)
                {
                    break;
                }
                if (index >= item.startShowingFrom)
                {
                    label = ms_Style.label;
                    if ((current.type == EventType.MouseDown) && r.Contains(current.mousePosition))
                    {
                        if (((this.ChangeLogSelectionRev == this.m_Changesets[changesetIndex].changeset) && (this.m_ChangeLogSelectionGUID == folder.guid)) && EditorGUI.actionKey)
                        {
                            this.ClearItemSelection();
                        }
                        else
                        {
                            this.ChangeLogSelectionRev = this.m_Changesets[changesetIndex].changeset;
                            this.m_ChangeLogSelectionGUID = folder.guid;
                            this.m_ChangeLogSelectionAssetName = folder.name;
                            this.m_FolderSelected = true;
                            this.m_AssetSelectionIndex = index;
                        }
                        this.m_ChangesetSelectionIndex = changesetIndex;
                        GUIUtility.keyboardControl = this.m_HistoryControlID;
                        ((ASMainWindow) this.m_ParentWindow).m_SearchToShow = ASMainWindow.ShowSearchField.HistoryList;
                        if (current.clickCount == 2)
                        {
                            this.ShowAssetsHistory();
                            GUIUtility.ExitGUI();
                        }
                        else if (current.button == 1)
                        {
                            GUIUtility.hotControl = 0;
                            r = new Rect(current.mousePosition.x, current.mousePosition.y, 1f, 1f);
                            EditorUtility.DisplayCustomMenu(r, this.m_DropDownMenuItems, -1, new EditorUtility.SelectMenuItemFunction(this.ContextMenuClick), null);
                        }
                        this.DoScroll();
                        current.Use();
                    }
                    bool on = (this.ChangeLogSelectionRev == this.m_Changesets[changesetIndex].changeset) && (this.m_ChangeLogSelectionGUID == folder.guid);
                    if (item.boldAssets[index] && !on)
                    {
                        GUI.Label(r, string.Empty, ms_Style.ping);
                    }
                    if (Event.current.type == EventType.Repaint)
                    {
                        label.Draw(r, content, false, false, on, hasFocus);
                        this.DrawBadge(r, folder.changeFlags, label, content, GUIClip.visibleRect.width - 150f);
                    }
                    r.y += this.m_RowHeight;
                }
                RectOffset padding = ms_Style.label.padding;
                padding.left += 0x10;
                RectOffset offset2 = ms_Style.boldLabel.padding;
                offset2.left += 0x10;
                try
                {
                    for (int j = 0; j < folder.files.Length; j++)
                    {
                        index++;
                        if (index == num2)
                        {
                            break;
                        }
                        if (index >= item.startShowingFrom)
                        {
                            label = ms_Style.label;
                            if ((current.type == EventType.MouseDown) && r.Contains(current.mousePosition))
                            {
                                if (((this.ChangeLogSelectionRev == this.m_Changesets[changesetIndex].changeset) && (this.m_ChangeLogSelectionGUID == folder.files[j].guid)) && EditorGUI.actionKey)
                                {
                                    this.ClearItemSelection();
                                }
                                else
                                {
                                    this.ChangeLogSelectionRev = this.m_Changesets[changesetIndex].changeset;
                                    this.m_ChangeLogSelectionGUID = folder.files[j].guid;
                                    this.m_ChangeLogSelectionAssetName = folder.files[j].name;
                                    this.m_FolderSelected = false;
                                    this.m_AssetSelectionIndex = index;
                                }
                                this.m_ChangesetSelectionIndex = changesetIndex;
                                GUIUtility.keyboardControl = this.m_HistoryControlID;
                                ((ASMainWindow) this.m_ParentWindow).m_SearchToShow = ASMainWindow.ShowSearchField.HistoryList;
                                if (current.clickCount == 2)
                                {
                                    if (this.IsComparableAssetSelected() && (this.m_SelectedGUID == this.m_ChangeLogSelectionGUID))
                                    {
                                        this.DoShowDiff(false, this.ChangeLogSelectionRev, -1);
                                    }
                                    else
                                    {
                                        this.ShowAssetsHistory();
                                        GUIUtility.ExitGUI();
                                    }
                                }
                                else if (current.button == 1)
                                {
                                    GUIUtility.hotControl = 0;
                                    r = new Rect(current.mousePosition.x, current.mousePosition.y, 1f, 1f);
                                    EditorUtility.DisplayCustomMenu(r, this.m_DropDownMenuItems, -1, new EditorUtility.SelectMenuItemFunction(this.ContextMenuClick), null);
                                }
                                this.DoScroll();
                                current.Use();
                            }
                            content.text = folder.files[j].name;
                            content.image = InternalEditorUtility.GetIconForFile(folder.files[j].name);
                            bool flag2 = (this.ChangeLogSelectionRev == this.m_Changesets[changesetIndex].changeset) && (this.m_ChangeLogSelectionGUID == folder.files[j].guid);
                            if (item.boldAssets[index] && !flag2)
                            {
                                GUI.Label(r, string.Empty, ms_Style.ping);
                            }
                            if (Event.current.type == EventType.Repaint)
                            {
                                label.Draw(r, content, false, false, flag2, hasFocus);
                                this.DrawBadge(r, folder.files[j].changeFlags, label, content, GUIClip.visibleRect.width - 150f);
                            }
                            r.y += this.m_RowHeight;
                        }
                    }
                    if (index == num2)
                    {
                        break;
                    }
                }
                finally
                {
                    RectOffset offset3 = ms_Style.label.padding;
                    offset3.left -= 0x10;
                    RectOffset offset4 = ms_Style.boldLabel.padding;
                    offset4.left -= 0x10;
                }
            }
            if (((index == num2) || (num2 >= item.totalLineCount)) && (item.collapsedItemCount != 0))
            {
                r.x += 19f;
                if (GUI.Button(r, item.collapsedItemCount.ToString() + " more...", ms_Style.foldout))
                {
                    GUIUtility.keyboardControl = this.m_HistoryControlID;
                    this.UncollapseListItem(ref item);
                }
            }
        }

        public void FilterItems(bool recreateGUIItems)
        {
            this.m_TotalHeight = 0;
            if ((this.m_Changesets == null) || (this.m_Changesets.Length == 0))
            {
                this.m_GUIItems = null;
            }
            else
            {
                if (recreateGUIItems)
                {
                    this.m_GUIItems = new GUIHistoryListItem[this.m_Changesets.Length];
                }
                string filterText = ((ASMainWindow) this.m_ParentWindow).m_SearchField.FilterText;
                bool flag = filterText.Trim() == string.Empty;
                for (int i = 0; i < this.m_Changesets.Length; i++)
                {
                    if (recreateGUIItems)
                    {
                        this.m_GUIItems[i] = new GUIHistoryListItem();
                        this.m_GUIItems[i].colAuthor = new GUIContent(this.m_Changesets[i].owner);
                        this.m_GUIItems[i].colRevision = new GUIContent(this.m_Changesets[i].changeset.ToString());
                        this.m_GUIItems[i].colDate = new GUIContent(this.m_Changesets[i].date);
                        this.m_GUIItems[i].colDescription = new GUIContent(this.m_Changesets[i].message);
                        this.m_GUIItems[i].assets = new ParentViewState();
                        this.m_GUIItems[i].assets.AddAssetItems(this.m_Changesets[i]);
                        this.m_GUIItems[i].totalLineCount = this.m_GUIItems[i].assets.GetLineCount();
                        this.m_GUIItems[i].height = ((this.m_RowHeight * (1 + this.m_GUIItems[i].totalLineCount)) + 20) + ((int) ms_Style.descriptionLabel.CalcHeight(this.m_GUIItems[i].colDescription, float.MaxValue));
                    }
                    this.m_GUIItems[i].boldAssets = new bool[this.m_GUIItems[i].assets.GetLineCount()];
                    int num2 = !flag ? this.CheckParentViewInFilterAndMarkBoldItems(this.m_GUIItems[i], filterText) : this.MarkBoldItemsBySelection(this.m_GUIItems[i]);
                    this.m_GUIItems[i].inFilter = (((flag || (num2 != -1)) || ((this.m_GUIItems[i].colDescription.text.IndexOf(filterText, StringComparison.InvariantCultureIgnoreCase) >= 0) || (this.m_GUIItems[i].colRevision.text.IndexOf(filterText, StringComparison.InvariantCultureIgnoreCase) >= 0))) || (this.m_GUIItems[i].colAuthor.text.IndexOf(filterText, StringComparison.InvariantCultureIgnoreCase) >= 0)) || (this.m_GUIItems[i].colDate.text.IndexOf(filterText, StringComparison.InvariantCultureIgnoreCase) >= 0);
                    if (recreateGUIItems && (this.m_GUIItems[i].totalLineCount > 5))
                    {
                        this.m_GUIItems[i].collapsedItemCount = (this.m_GUIItems[i].totalLineCount - 5) + 1;
                        this.m_GUIItems[i].height = ((this.m_RowHeight * 6) + 20) + ((int) ms_Style.descriptionLabel.CalcHeight(this.m_GUIItems[i].colDescription, float.MaxValue));
                    }
                    this.m_GUIItems[i].startShowingFrom = 0;
                    if (((this.m_GUIItems[i].collapsedItemCount != 0) && (this.m_GUIItems[i].totalLineCount > 5)) && (num2 >= 4))
                    {
                        if (((num2 + 5) - 1) > this.m_GUIItems[i].totalLineCount)
                        {
                            this.m_GUIItems[i].startShowingFrom = (this.m_GUIItems[i].totalLineCount - 5) + 1;
                        }
                        else
                        {
                            this.m_GUIItems[i].startShowingFrom = num2;
                        }
                    }
                    if (this.m_GUIItems[i].inFilter)
                    {
                        this.m_TotalHeight += this.m_GUIItems[i].height;
                    }
                }
            }
        }

        private int FindFirstUnfilteredItem(int fromIndex, int direction)
        {
            for (int i = fromIndex; (i >= 0) && (i < this.m_GUIItems.Length); i += direction)
            {
                if (this.m_GUIItems[i].inFilter)
                {
                    return i;
                }
            }
            return -1;
        }

        private void FinishShowCustomDiff()
        {
            if (this.m_Rev1ForCustomDiff != this.ChangeLogSelectionRev)
            {
                this.DoShowDiff(this.m_BinaryDiff, this.m_Rev1ForCustomDiff, this.ChangeLogSelectionRev);
            }
            else
            {
                Debug.Log("You chose to compare to the same revision.");
            }
            this.m_InRevisionSelectMode = false;
        }

        private void HandleWebLikeKeyboard()
        {
            Event current = Event.current;
            if ((current.GetTypeForControl(this.m_HistoryControlID) == EventType.KeyDown) && (this.m_GUIItems.Length != 0))
            {
                switch (current.keyCode)
                {
                    case KeyCode.KeypadEnter:
                    case KeyCode.Return:
                        if (this.IsComparableAssetSelected())
                        {
                            this.DoShowDiff(false, this.ChangeLogSelectionRev, -1);
                        }
                        goto Label_02A3;

                    case KeyCode.UpArrow:
                        this.MoveSelection(-1);
                        goto Label_02A3;

                    case KeyCode.DownArrow:
                        this.MoveSelection(1);
                        goto Label_02A3;

                    case KeyCode.RightArrow:
                        if ((this.m_ChangeLogSelectionGUID == string.Empty) && (this.m_GUIItems.Length > 0))
                        {
                            this.m_ChangeLogSelectionGUID = this.m_GUIItems[this.m_ChangesetSelectionIndex].assets.folders[0].guid;
                            this.m_ChangeLogSelectionAssetName = this.m_GUIItems[this.m_ChangesetSelectionIndex].assets.folders[0].name;
                            this.m_FolderSelected = true;
                            this.m_AssetSelectionIndex = 0;
                        }
                        goto Label_02A3;

                    case KeyCode.LeftArrow:
                        this.m_ChangeLogSelectionGUID = string.Empty;
                        goto Label_02A3;

                    case KeyCode.Home:
                        if (!(this.m_ChangeLogSelectionGUID == string.Empty))
                        {
                            this.MoveSelection(-999999);
                        }
                        else
                        {
                            int num = this.FindFirstUnfilteredItem(0, 1);
                            if (num != -1)
                            {
                                this.m_ChangesetSelectionIndex = num;
                            }
                            this.ChangeLogSelectionRev = this.m_Changesets[this.m_ChangesetSelectionIndex].changeset;
                        }
                        goto Label_02A3;

                    case KeyCode.End:
                        if (!(this.m_ChangeLogSelectionGUID == string.Empty))
                        {
                            this.MoveSelection(0xf423f);
                        }
                        else
                        {
                            int num2 = this.FindFirstUnfilteredItem(this.m_GUIItems.Length - 1, -1);
                            if (num2 != -1)
                            {
                                this.m_ChangesetSelectionIndex = num2;
                            }
                            this.ChangeLogSelectionRev = this.m_Changesets[this.m_ChangesetSelectionIndex].changeset;
                        }
                        goto Label_02A3;

                    case KeyCode.PageUp:
                        if (Application.platform != RuntimePlatform.OSXEditor)
                        {
                            this.MoveSelection(-Mathf.RoundToInt((float) (this.m_ScrollViewHeight / this.m_RowHeight)));
                        }
                        else
                        {
                            this.m_ScrollPos.y -= this.m_ScrollViewHeight;
                            if (this.m_ScrollPos.y < 0f)
                            {
                                this.m_ScrollPos.y = 0f;
                            }
                        }
                        goto Label_02A3;

                    case KeyCode.PageDown:
                        if (Application.platform != RuntimePlatform.OSXEditor)
                        {
                            this.MoveSelection(Mathf.RoundToInt((float) (this.m_ScrollViewHeight / this.m_RowHeight)));
                        }
                        else
                        {
                            this.m_ScrollPos.y += this.m_ScrollViewHeight;
                        }
                        goto Label_02A3;
                }
            }
            return;
        Label_02A3:
            this.DoScroll();
            current.Use();
        }

        private bool HasFlag(ChangeFlags flags, ChangeFlags flagToCheck)
        {
            return ((flagToCheck & flags) != ChangeFlags.None);
        }

        private bool IsComparableAssetSelected()
        {
            return (!this.m_FolderSelected && (this.m_ChangeLogSelectionGUID != string.Empty));
        }

        private void MarkBoldItemsByGUID(string guid)
        {
            for (int i = 0; i < this.m_GUIItems.Length; i++)
            {
                GUIHistoryListItem item = this.m_GUIItems[i];
                ParentViewState assets = item.assets;
                int index = 0;
                item.boldAssets = new bool[assets.GetLineCount()];
                for (int j = 0; j < assets.folders.Length; j++)
                {
                    ParentViewFolder folder = assets.folders[j];
                    if (folder.guid == guid)
                    {
                        item.boldAssets[index] = true;
                    }
                    index++;
                    for (int k = 0; k < folder.files.Length; k++)
                    {
                        if (folder.files[k].guid == guid)
                        {
                            item.boldAssets[index] = true;
                        }
                        index++;
                    }
                }
            }
        }

        private int MarkBoldItemsBySelection(GUIHistoryListItem item)
        {
            List<string> list = new List<string>();
            ParentViewState assets = item.assets;
            int num = -1;
            int index = 0;
            if (Selection.instanceIDs.Length == 0)
            {
                return 0;
            }
            foreach (int num3 in Selection.instanceIDs)
            {
                list.Add(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(num3)));
            }
            for (int i = 0; i < assets.folders.Length; i++)
            {
                ParentViewFolder folder = assets.folders[i];
                if (list.Contains(folder.guid))
                {
                    item.boldAssets[index] = true;
                    if (num == -1)
                    {
                        num = index;
                    }
                }
                index++;
                for (int j = 0; j < folder.files.Length; j++)
                {
                    if (list.Contains(folder.files[j].guid))
                    {
                        item.boldAssets[index] = true;
                        if (num == -1)
                        {
                            num = index;
                        }
                    }
                    index++;
                }
            }
            return num;
        }

        private void MoveSelection(int steps)
        {
            if (!(this.m_ChangeLogSelectionGUID == string.Empty))
            {
                this.m_AssetSelectionIndex += steps;
                if (this.m_AssetSelectionIndex < this.m_GUIItems[this.m_ChangesetSelectionIndex].startShowingFrom)
                {
                    this.m_AssetSelectionIndex = this.m_GUIItems[this.m_ChangesetSelectionIndex].startShowingFrom;
                }
                else
                {
                    int lineCount = this.m_GUIItems[this.m_ChangesetSelectionIndex].assets.GetLineCount();
                    if ((this.m_AssetSelectionIndex >= (4 + this.m_GUIItems[this.m_ChangesetSelectionIndex].startShowingFrom)) && (this.m_GUIItems[this.m_ChangesetSelectionIndex].collapsedItemCount != 0))
                    {
                        this.UncollapseListItem(ref this.m_GUIItems[this.m_ChangesetSelectionIndex]);
                    }
                    if (this.m_AssetSelectionIndex >= lineCount)
                    {
                        this.m_AssetSelectionIndex = lineCount - 1;
                    }
                }
                int folder = 0;
                int file = 0;
                if (this.m_GUIItems[this.m_ChangesetSelectionIndex].assets.IndexToFolderAndFile(this.m_AssetSelectionIndex, ref folder, ref file))
                {
                    if (file == -1)
                    {
                        this.m_ChangeLogSelectionGUID = this.m_GUIItems[this.m_ChangesetSelectionIndex].assets.folders[folder].guid;
                    }
                    else
                    {
                        this.m_ChangeLogSelectionGUID = this.m_GUIItems[this.m_ChangesetSelectionIndex].assets.folders[folder].files[file].guid;
                    }
                }
            }
            else
            {
                int direction = (int) Mathf.Sign((float) steps);
                steps = Mathf.Abs(steps);
                int num2 = 0;
                for (int i = 0; i < steps; i++)
                {
                    num2 = this.FindFirstUnfilteredItem(this.m_ChangesetSelectionIndex + direction, direction);
                    if (num2 == -1)
                    {
                        break;
                    }
                    this.m_ChangesetSelectionIndex = num2;
                }
                this.ChangeLogSelectionRev = this.m_Changesets[this.m_ChangesetSelectionIndex].changeset;
            }
        }

        public void OnSelectionChange()
        {
            if (Selection.objects.Length != 0)
            {
                this.m_FileViewWin.SelType = ASHistoryFileView.SelectionType.Items;
            }
            this.DoLocalSelectionChange();
        }

        private void ShowAssetsHistory()
        {
            if (AssetServer.IsAssetAvailable(this.m_ChangeLogSelectionGUID) != 0)
            {
                string[] guids = new string[] { this.m_ChangeLogSelectionGUID };
                this.m_FileViewWin.SelType = ASHistoryFileView.SelectionType.Items;
                AssetServer.SetSelectionFromGUIDs(guids);
            }
            else
            {
                this.m_FileViewWin.SelectDeletedItem(this.m_ChangeLogSelectionGUID);
                this.DoLocalSelectionChange();
            }
        }

        private void UncollapseListItem(ref GUIHistoryListItem item)
        {
            int num = (item.collapsedItemCount - 1) * this.m_RowHeight;
            item.collapsedItemCount = 0;
            item.startShowingFrom = 0;
            item.height += num;
            this.m_TotalHeight += num;
        }

        private void WebLikeHistory(bool hasFocus)
        {
            if (this.m_Changesets == null)
            {
                this.m_Changesets = new Changeset[0];
            }
            if (this.m_GUIItems != null)
            {
                this.m_HistoryControlID = GUIUtility.GetControlID(ms_HistoryControlHash, FocusType.Native);
                this.HandleWebLikeKeyboard();
                Event current = Event.current;
                if (current.GetTypeForControl(this.m_HistoryControlID) == EventType.ValidateCommand)
                {
                    current.Use();
                }
                else
                {
                    GUILayout.Space(1f);
                    this.m_ScrollPos = GUILayout.BeginScrollView(this.m_ScrollPos, new GUILayoutOption[0]);
                    int num = 0;
                    GUILayoutUtility.GetRect(1f, (float) (this.m_TotalHeight - 1));
                    if ((((current.type == EventType.Repaint) || (current.type == EventType.MouseDown)) || (current.type == EventType.MouseUp)) && (this.m_GUIItems != null))
                    {
                        for (int i = 0; i < this.m_Changesets.Length; i++)
                        {
                            if (this.m_GUIItems[i].inFilter)
                            {
                                if (((num + this.m_GUIItems[i].height) > GUIClip.visibleRect.y) && (num < GUIClip.visibleRect.yMax))
                                {
                                    Rect rect;
                                    float num3 = ms_Style.descriptionLabel.CalcHeight(this.m_GUIItems[i].colDescription, float.MaxValue);
                                    if (current.type == EventType.Repaint)
                                    {
                                        if ((this.ChangeLogSelectionRev == this.m_Changesets[i].changeset) && (Event.current.type == EventType.Repaint))
                                        {
                                            rect = new Rect(0f, (float) num, GUIClip.visibleRect.width, (float) (this.m_GUIItems[i].height - 10));
                                            ms_Style.selected.Draw(rect, false, false, false, false);
                                        }
                                        rect = new Rect(0f, (float) (num + 3), GUIClip.visibleRect.width, (float) this.m_GUIItems[i].height);
                                        GUI.Label(rect, this.m_GUIItems[i].colAuthor, ms_Style.boldLabel);
                                        rect = new Rect(GUIClip.visibleRect.width - 160f, (float) (num + 3), 60f, (float) this.m_GUIItems[i].height);
                                        GUI.Label(rect, this.m_GUIItems[i].colRevision, ms_Style.boldLabel);
                                        rect.x += 60f;
                                        rect.width = 100f;
                                        GUI.Label(rect, this.m_GUIItems[i].colDate, ms_Style.boldLabel);
                                        rect.x = ms_Style.boldLabel.margin.left;
                                        rect.y += this.m_RowHeight;
                                        rect.width = GUIClip.visibleRect.width;
                                        rect.height = num3;
                                        GUI.Label(rect, this.m_GUIItems[i].colDescription, ms_Style.descriptionLabel);
                                        rect.y += num3;
                                    }
                                    rect = new Rect(0f, (num + num3) + this.m_RowHeight, GUIClip.visibleRect.width, (this.m_GUIItems[i].height - num3) - this.m_RowHeight);
                                    this.DrawParentView(rect, ref this.m_GUIItems[i], i, hasFocus);
                                    if (current.type == EventType.MouseDown)
                                    {
                                        rect = new Rect(0f, (float) num, GUIClip.visibleRect.width, (float) (this.m_GUIItems[i].height - 10));
                                        if (rect.Contains(current.mousePosition))
                                        {
                                            this.ChangeLogSelectionRev = this.m_Changesets[i].changeset;
                                            this.m_ChangesetSelectionIndex = i;
                                            GUIUtility.keyboardControl = this.m_HistoryControlID;
                                            ((ASMainWindow) this.m_ParentWindow).m_SearchToShow = ASMainWindow.ShowSearchField.HistoryList;
                                            if (current.button == 1)
                                            {
                                                GUIUtility.hotControl = 0;
                                                rect = new Rect(current.mousePosition.x, current.mousePosition.y, 1f, 1f);
                                                EditorUtility.DisplayCustomMenu(rect, this.m_DropDownChangesetMenuItems, -1, new EditorUtility.SelectMenuItemFunction(this.ChangesetContextMenuClick), null);
                                                Event.current.Use();
                                            }
                                            this.DoScroll();
                                            current.Use();
                                        }
                                    }
                                }
                                num += this.m_GUIItems[i].height;
                            }
                        }
                    }
                    else if (this.m_GUIItems == null)
                    {
                        GUILayout.Label(EditorGUIUtility.TextContent("This item is not yet committed to the Asset Server"), new GUILayoutOption[0]);
                    }
                    if (Event.current.type == EventType.Repaint)
                    {
                        this.m_ScrollViewHeight = (int) GUIClip.visibleRect.height;
                    }
                    GUILayout.EndScrollView();
                }
            }
        }

        private int ChangeLogSelectionRev
        {
            get
            {
                return this.m_ChangeLogSelectionRev;
            }
            set
            {
                this.m_ChangeLogSelectionRev = value;
                if (this.m_InRevisionSelectMode)
                {
                    this.FinishShowCustomDiff();
                }
            }
        }

        internal class Constants
        {
            public GUIStyle boldLabel = "BoldLabel";
            public GUIStyle button = "Button";
            public GUIStyle descriptionLabel = "Label";
            public GUIStyle entryEven = "CN EntryBackEven";
            public GUIStyle entryOdd = "CN EntryBackOdd";
            public GUIStyle foldout = "IN Foldout";
            public GUIStyle label = "PR Label";
            public GUIStyle lvHeader = "OL title";
            public GUIStyle ping = new GUIStyle("PR Ping");
            public GUIStyle selected = "ServerUpdateChangesetOn";

            public Constants()
            {
                this.ping.overflow.left = -2;
                this.ping.overflow.right = -21;
                this.ping.padding.left = 0x30;
                this.ping.padding.right = 0;
            }
        }

        [Serializable]
        private class GUIHistoryListItem
        {
            public ParentViewState assets;
            public bool[] boldAssets;
            public GUIContent colAuthor;
            public GUIContent colDate;
            public GUIContent colDescription;
            public int collapsedItemCount;
            public GUIContent colRevision;
            public int height;
            public bool inFilter;
            public int startShowingFrom;
            public int totalLineCount;
        }
    }
}

