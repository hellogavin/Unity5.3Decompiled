namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    internal class PackageExportTreeView
    {
        private PackageExport m_PackageExport;
        private List<PackageExportTreeViewItem> m_Selection = new List<PackageExportTreeViewItem>();
        private TreeView m_TreeView;
        private static readonly bool s_UseFoldouts = true;

        public PackageExportTreeView(PackageExport packageExport, TreeViewState treeViewState, Rect startRect)
        {
            this.m_PackageExport = packageExport;
            this.m_TreeView = new TreeView(this.m_PackageExport, treeViewState);
            PackageExportTreeViewDataSource data = new PackageExportTreeViewDataSource(this.m_TreeView, this);
            PackageExportTreeViewGUI gui = new PackageExportTreeViewGUI(this.m_TreeView, this);
            this.m_TreeView.Init(startRect, data, gui, null);
            this.m_TreeView.ReloadData();
            this.m_TreeView.selectionChangedCallback = (Action<int[]>) Delegate.Combine(this.m_TreeView.selectionChangedCallback, new Action<int[]>(this.SelectionChanged));
            gui.itemWasToggled = (Action<PackageExportTreeViewItem>) Delegate.Combine(gui.itemWasToggled, new Action<PackageExportTreeViewItem>(this.ItemWasToggled));
            this.ComputeEnabledStateForFolders();
        }

        private void ComputeEnabledStateForFolders()
        {
            PackageExportTreeViewItem root = this.m_TreeView.data.root as PackageExportTreeViewItem;
            HashSet<PackageExportTreeViewItem> done = new HashSet<PackageExportTreeViewItem> {
                root
            };
            this.RecursiveComputeEnabledStateForFolders(root, done);
        }

        private void EnableChildrenRecursive(TreeViewItem parentItem, int enabled)
        {
            if (parentItem.hasChildren)
            {
                foreach (TreeViewItem item in parentItem.children)
                {
                    PackageExportTreeViewItem item2 = item as PackageExportTreeViewItem;
                    ExportPackageItem item3 = item2.item;
                    if (item3 != null)
                    {
                        item3.enabledStatus = enabled;
                    }
                    this.EnableChildrenRecursive(item2, enabled);
                }
            }
        }

        private EnabledState GetFolderChildrenEnabledState(PackageExportTreeViewItem folder)
        {
            ExportPackageItem item = folder.item;
            if ((item != null) && !item.isFolder)
            {
                Debug.LogError("Should be a folder item!");
            }
            if (!folder.hasChildren)
            {
                return EnabledState.None;
            }
            EnabledState notSet = EnabledState.NotSet;
            PackageExportTreeViewItem item2 = folder.children[0] as PackageExportTreeViewItem;
            ExportPackageItem item3 = item2.item;
            int num = (item3 != null) ? item3.enabledStatus : 1;
            for (int i = 1; i < folder.children.Count; i++)
            {
                item3 = (folder.children[i] as PackageExportTreeViewItem).item;
                if (num != item3.enabledStatus)
                {
                    notSet = EnabledState.Mixed;
                    break;
                }
            }
            if (notSet == EnabledState.NotSet)
            {
                notSet = (num != 1) ? EnabledState.None : EnabledState.All;
            }
            return notSet;
        }

        private void ItemWasToggled(PackageExportTreeViewItem pitem)
        {
            ExportPackageItem item = pitem.item;
            if (item != null)
            {
                if (this.m_Selection.Count <= 1)
                {
                    this.EnableChildrenRecursive(pitem, item.enabledStatus);
                }
                else
                {
                    foreach (PackageExportTreeViewItem item2 in this.m_Selection)
                    {
                        item2.item.enabledStatus = item.enabledStatus;
                    }
                }
                this.ComputeEnabledStateForFolders();
            }
        }

        public void OnGUI(Rect rect)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Keyboard);
            this.m_TreeView.OnGUI(rect, controlID);
            if ((((Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.Space)) && ((this.m_Selection != null) && (this.m_Selection.Count > 0))) && (GUIUtility.keyboardControl == controlID))
            {
                ExportPackageItem item = this.m_Selection[0].item;
                if (item != null)
                {
                    int num2 = (item.enabledStatus != 0) ? 0 : 1;
                    item.enabledStatus = num2;
                    this.ItemWasToggled(this.m_Selection[0]);
                }
                Event.current.Use();
            }
        }

        private void RecursiveComputeEnabledStateForFolders(PackageExportTreeViewItem pitem, HashSet<PackageExportTreeViewItem> done)
        {
            ExportPackageItem item = pitem.item;
            if ((item == null) || item.isFolder)
            {
                if (pitem.hasChildren)
                {
                    foreach (TreeViewItem item2 in pitem.children)
                    {
                        this.RecursiveComputeEnabledStateForFolders(item2 as PackageExportTreeViewItem, done);
                    }
                }
                if ((item != null) && !done.Contains(pitem))
                {
                    EnabledState folderChildrenEnabledState = this.GetFolderChildrenEnabledState(pitem);
                    item.enabledStatus = (int) folderChildrenEnabledState;
                    if (folderChildrenEnabledState == EnabledState.Mixed)
                    {
                        done.Add(pitem);
                        for (PackageExportTreeViewItem item3 = pitem.parent as PackageExportTreeViewItem; item3 != null; item3 = item3.parent as PackageExportTreeViewItem)
                        {
                            ExportPackageItem item4 = item3.item;
                            if ((item4 != null) && !done.Contains(item3))
                            {
                                item4.enabledStatus = 2;
                                done.Add(item3);
                            }
                        }
                    }
                }
            }
        }

        private void SelectionChanged(int[] selectedIDs)
        {
            this.m_Selection = new List<PackageExportTreeViewItem>();
            foreach (TreeViewItem item in this.m_TreeView.data.GetRows())
            {
                if (selectedIDs.Contains<int>(item.id))
                {
                    PackageExportTreeViewItem item2 = item as PackageExportTreeViewItem;
                    if (item2 != null)
                    {
                        this.m_Selection.Add(item2);
                    }
                }
            }
        }

        public void SetAllEnabled(int enabled)
        {
            this.EnableChildrenRecursive(this.m_TreeView.data.root, enabled);
            this.ComputeEnabledStateForFolders();
        }

        public ExportPackageItem[] items
        {
            get
            {
                return this.m_PackageExport.items;
            }
        }

        public enum EnabledState
        {
            All = 1,
            Mixed = 2,
            None = 0,
            NotSet = -1
        }

        private class PackageExportTreeViewDataSource : TreeViewDataSource
        {
            private PackageExportTreeView m_PackageExportView;

            public PackageExportTreeViewDataSource(TreeView treeView, PackageExportTreeView view) : base(treeView)
            {
                this.m_PackageExportView = view;
                base.rootIsCollapsable = false;
                base.showRootNode = false;
            }

            private TreeViewItem EnsureFolderPath(string folderPath, Dictionary<string, PackageExportTreeView.PackageExportTreeViewItem> treeViewFolders, bool initExpandedState)
            {
                if (folderPath == string.Empty)
                {
                    return base.m_RootItem;
                }
                TreeViewItem item = TreeViewUtility.FindItem(folderPath.GetHashCode(), base.m_RootItem);
                if (item != null)
                {
                    return item;
                }
                char[] separator = new char[] { '/' };
                string[] strArray = folderPath.Split(separator);
                string key = string.Empty;
                TreeViewItem rootItem = base.m_RootItem;
                int depth = -1;
                for (int i = 0; i < strArray.Length; i++)
                {
                    string displayName = strArray[i];
                    if (key != string.Empty)
                    {
                        key = key + '/';
                    }
                    key = key + displayName;
                    if ((i != 0) || (key != "Assets"))
                    {
                        PackageExportTreeView.PackageExportTreeViewItem item3;
                        depth++;
                        int hashCode = key.GetHashCode();
                        if (treeViewFolders.TryGetValue(key, out item3))
                        {
                            rootItem = item3;
                        }
                        else
                        {
                            PackageExportTreeView.PackageExportTreeViewItem child = new PackageExportTreeView.PackageExportTreeViewItem(null, hashCode, depth, rootItem, displayName);
                            rootItem.AddChild(child);
                            rootItem = child;
                            if (initExpandedState)
                            {
                                base.m_TreeView.state.expandedIDs.Add(hashCode);
                            }
                            treeViewFolders[key] = child;
                        }
                    }
                }
                return rootItem;
            }

            public override void FetchData()
            {
                int depth = -1;
                base.m_RootItem = new PackageExportTreeView.PackageExportTreeViewItem(null, "Assets".GetHashCode(), depth, null, "InvisibleAssetsFolder");
                bool initExpandedState = true;
                if (initExpandedState)
                {
                    base.m_TreeView.state.expandedIDs.Add(base.m_RootItem.id);
                }
                ExportPackageItem[] items = this.m_PackageExportView.items;
                Dictionary<string, PackageExportTreeView.PackageExportTreeViewItem> treeViewFolders = new Dictionary<string, PackageExportTreeView.PackageExportTreeViewItem>();
                for (int i = 0; i < items.Length; i++)
                {
                    ExportPackageItem itemIn = items[i];
                    if (!PackageImport.HasInvalidCharInFilePath(itemIn.assetPath))
                    {
                        string fileName = Path.GetFileName(itemIn.assetPath);
                        string directoryName = Path.GetDirectoryName(itemIn.assetPath);
                        TreeViewItem parent = this.EnsureFolderPath(directoryName, treeViewFolders, initExpandedState);
                        if (parent != null)
                        {
                            int hashCode = itemIn.assetPath.GetHashCode();
                            PackageExportTreeView.PackageExportTreeViewItem child = new PackageExportTreeView.PackageExportTreeViewItem(itemIn, hashCode, parent.depth + 1, parent, fileName);
                            parent.AddChild(child);
                            if (initExpandedState)
                            {
                                base.m_TreeView.state.expandedIDs.Add(hashCode);
                            }
                            if (itemIn.isFolder)
                            {
                                treeViewFolders[itemIn.assetPath] = child;
                            }
                        }
                    }
                }
                if (initExpandedState)
                {
                    base.m_TreeView.state.expandedIDs.Sort();
                }
            }

            public override bool IsExpandable(TreeViewItem item)
            {
                if (!PackageExportTreeView.s_UseFoldouts)
                {
                    return false;
                }
                return base.IsExpandable(item);
            }

            public override bool IsRenamingItemAllowed(TreeViewItem item)
            {
                return false;
            }
        }

        private class PackageExportTreeViewGUI : TreeViewGUI
        {
            public Action<PackageExportTreeView.PackageExportTreeViewItem> itemWasToggled;
            private PackageExportTreeView m_PackageExportView;

            public PackageExportTreeViewGUI(TreeView treeView, PackageExportTreeView view) : base(treeView)
            {
                this.m_PackageExportView = view;
                base.k_BaseIndent = 4f;
                if (!PackageExportTreeView.s_UseFoldouts)
                {
                    base.k_FoldoutWidth = 0f;
                }
            }

            private void DoIconAndText(TreeViewItem item, Rect contentRect, bool selected, bool focused)
            {
                EditorGUIUtility.SetIconSize(new Vector2(base.k_IconWidth, base.k_IconWidth));
                GUIStyle lineStyle = TreeViewGUI.s_Styles.lineStyle;
                lineStyle.padding.left = 0;
                if (Event.current.type == EventType.Repaint)
                {
                    lineStyle.Draw(contentRect, GUIContent.Temp(item.displayName, this.GetIconForNode(item)), false, false, selected, focused);
                }
                EditorGUIUtility.SetIconSize(Vector2.zero);
            }

            private void DoToggle(PackageExportTreeView.PackageExportTreeViewItem pitem, Rect toggleRect)
            {
                EditorGUI.BeginChangeCheck();
                Toggle(this.m_PackageExportView.items, pitem, toggleRect);
                if (EditorGUI.EndChangeCheck())
                {
                    if ((base.m_TreeView.GetSelection().Length <= 1) || !base.m_TreeView.GetSelection().Contains<int>(pitem.id))
                    {
                        int[] selectedIDs = new int[] { pitem.id };
                        base.m_TreeView.SetSelection(selectedIDs, false);
                        base.m_TreeView.NotifyListenersThatSelectionChanged();
                    }
                    if (this.itemWasToggled != null)
                    {
                        this.itemWasToggled(pitem);
                    }
                    Event.current.Use();
                }
            }

            protected override Texture GetIconForNode(TreeViewItem tItem)
            {
                PackageExportTreeView.PackageExportTreeViewItem item = tItem as PackageExportTreeView.PackageExportTreeViewItem;
                ExportPackageItem item2 = item.item;
                if ((item2 == null) || item2.isFolder)
                {
                    return Constants.folderIcon;
                }
                Texture cachedIcon = AssetDatabase.GetCachedIcon(item2.assetPath);
                if (cachedIcon != null)
                {
                    return cachedIcon;
                }
                return InternalEditorUtility.GetIconForFile(item2.assetPath);
            }

            public override void OnRowGUI(Rect rowRect, TreeViewItem tvItem, int row, bool selected, bool focused)
            {
                base.k_IndentWidth = 18f;
                base.k_FoldoutWidth = 18f;
                PackageExportTreeView.PackageExportTreeViewItem pitem = tvItem as PackageExportTreeView.PackageExportTreeViewItem;
                bool flag = Event.current.type == EventType.Repaint;
                if (selected && flag)
                {
                    TreeViewGUI.s_Styles.selectionStyle.Draw(rowRect, false, false, true, focused);
                }
                if (base.m_TreeView.data.IsExpandable(tvItem))
                {
                    this.DoFoldout(rowRect, tvItem, row);
                }
                Rect toggleRect = new Rect((base.k_BaseIndent + (tvItem.depth * base.indentWidth)) + base.k_FoldoutWidth, rowRect.y, 18f, rowRect.height);
                this.DoToggle(pitem, toggleRect);
                Rect contentRect = new Rect(toggleRect.xMax, rowRect.y, rowRect.width, rowRect.height);
                this.DoIconAndText(tvItem, contentRect, selected, focused);
            }

            protected override void RenameEnded()
            {
            }

            private static void Toggle(ExportPackageItem[] items, PackageExportTreeView.PackageExportTreeViewItem pitem, Rect toggleRect)
            {
                ExportPackageItem item = pitem.item;
                if (item != null)
                {
                    bool flag = item.enabledStatus > 0;
                    GUIStyle toggle = EditorStyles.toggle;
                    if (item.isFolder && (item.enabledStatus == 2))
                    {
                        toggle = EditorStyles.toggleMixed;
                    }
                    bool flag3 = GUI.Toggle(toggleRect, flag, GUIContent.none, toggle);
                    if (flag3 != flag)
                    {
                        item.enabledStatus = !flag3 ? 0 : 1;
                    }
                }
            }

            public int showPreviewForID { get; set; }

            internal static class Constants
            {
                public static Texture2D folderIcon = EditorGUIUtility.FindTexture(EditorResourcesUtility.folderIconName);
            }
        }

        private class PackageExportTreeViewItem : TreeViewItem
        {
            public PackageExportTreeViewItem(ExportPackageItem itemIn, int id, int depth, TreeViewItem parent, string displayName) : base(id, depth, parent, displayName)
            {
                this.item = itemIn;
            }

            public ExportPackageItem item { get; set; }
        }
    }
}

