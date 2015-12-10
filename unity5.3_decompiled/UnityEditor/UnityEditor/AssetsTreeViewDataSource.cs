namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using UnityEditor.ProjectWindowCallback;
    using UnityEditorInternal;
    using UnityEngine;

    internal class AssetsTreeViewDataSource : LazyTreeViewDataSource
    {
        private const HierarchyType k_HierarchyType = HierarchyType.Assets;
        private readonly int m_RootInstanceID;

        public AssetsTreeViewDataSource(TreeView treeView, int rootInstanceID, bool showRootNode, bool rootNodeIsCollapsable) : base(treeView)
        {
            this.m_RootInstanceID = rootInstanceID;
            this.showRootNode = showRootNode;
            base.rootIsCollapsable = rootNodeIsCollapsable;
        }

        private static string CreateDisplayName(int instanceID)
        {
            return Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(instanceID));
        }

        public override void FetchData()
        {
            int depth = 0;
            base.m_RootItem = new TreeViewItem(this.m_RootInstanceID, depth, null, CreateDisplayName(this.m_RootInstanceID));
            if (!base.showRootNode)
            {
                this.SetExpanded(base.m_RootItem, true);
            }
            IHierarchyProperty property = new HierarchyProperty(HierarchyType.Assets);
            property.Reset();
            if (!property.Find(this.m_RootInstanceID, null))
            {
                Debug.LogError("Root Asset with id " + this.m_RootInstanceID + " not found!!");
            }
            int minDepth = property.depth + (!base.showRootNode ? 1 : 0);
            int[] expanded = base.expandedIDs.ToArray();
            Texture2D textured = EditorGUIUtility.FindTexture(EditorResourcesUtility.emptyFolderIconName);
            base.m_VisibleRows = new List<TreeViewItem>();
            while (property.NextWithDepthCheck(expanded, minDepth))
            {
                if (!this.foldersOnly || property.isFolder)
                {
                    TreeViewItem item;
                    depth = property.depth - minDepth;
                    if (property.isFolder)
                    {
                        item = new FolderTreeItem(property.instanceID, depth, null, property.name);
                    }
                    else
                    {
                        item = new NonFolderTreeItem(property.instanceID, depth, null, property.name);
                    }
                    if (property.isFolder && !property.hasChildren)
                    {
                        item.icon = textured;
                    }
                    else
                    {
                        item.icon = property.icon;
                    }
                    if (property.hasChildren)
                    {
                        item.AddChild(null);
                    }
                    base.m_VisibleRows.Add(item);
                }
            }
            TreeViewUtility.SetChildParentReferences(base.m_VisibleRows, base.m_RootItem);
            if (this.foldersFirst)
            {
                FoldersFirstRecursive(base.m_RootItem);
                base.m_VisibleRows.Clear();
                base.GetVisibleItemsRecursive(base.m_RootItem, base.m_VisibleRows);
            }
            base.m_NeedRefreshVisibleFolders = false;
            bool revealSelectionAndFrameLastSelected = false;
            base.m_TreeView.SetSelection(Selection.instanceIDs, revealSelectionAndFrameLastSelected);
        }

        private static void FoldersFirstRecursive(TreeViewItem item)
        {
            if (item.hasChildren)
            {
                TreeViewItem[] sourceArray = item.children.ToArray();
                for (int i = 0; i < item.children.Count; i++)
                {
                    if (sourceArray[i] == null)
                    {
                        continue;
                    }
                    if (sourceArray[i] is NonFolderTreeItem)
                    {
                        for (int j = i + 1; j < sourceArray.Length; j++)
                        {
                            if (sourceArray[j] is FolderTreeItem)
                            {
                                TreeViewItem item2 = sourceArray[j];
                                int length = j - i;
                                Array.Copy(sourceArray, i, sourceArray, i + 1, length);
                                sourceArray[i] = item2;
                                break;
                            }
                        }
                    }
                    FoldersFirstRecursive(sourceArray[i]);
                }
                item.children = new List<TreeViewItem>(sourceArray);
            }
        }

        protected CreateAssetUtility GetCreateAssetUtility()
        {
            return base.m_TreeView.state.createAssetUtility;
        }

        public int GetInsertAfterItemIDForNewItem(string newName, TreeViewItem parentItem, bool isCreatingNewFolder, bool foldersFirst)
        {
            if (!parentItem.hasChildren)
            {
                return parentItem.id;
            }
            int id = parentItem.id;
            for (int i = 0; i < parentItem.children.Count; i++)
            {
                int instanceID = parentItem.children[i].id;
                bool flag = parentItem.children[i] is FolderTreeItem;
                if ((foldersFirst && flag) && !isCreatingNewFolder)
                {
                    id = instanceID;
                }
                else
                {
                    if ((foldersFirst && !flag) && isCreatingNewFolder)
                    {
                        return id;
                    }
                    if (EditorUtility.NaturalCompare(Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(instanceID)), newName) > 0)
                    {
                        return id;
                    }
                    id = instanceID;
                }
            }
            return id;
        }

        protected override HashSet<int> GetParentsAbove(int id)
        {
            return new HashSet<int>(ProjectWindowUtil.GetAncestors(id));
        }

        protected override HashSet<int> GetParentsBelow(int id)
        {
            HashSet<int> set = new HashSet<int>();
            IHierarchyProperty property = new HierarchyProperty(HierarchyType.Assets);
            if (property.Find(id, null))
            {
                set.Add(id);
                int depth = property.depth;
                while (property.Next(null) && (property.depth > depth))
                {
                    if (property.hasChildren)
                    {
                        set.Add(property.instanceID);
                    }
                }
            }
            return set;
        }

        public override void InsertFakeItem(int id, int parentID, string name, Texture2D icon)
        {
            bool isCreatingNewFolder = this.GetCreateAssetUtility().endAction is DoCreateFolder;
            TreeViewItem item = this.FindItem(id);
            if (item != null)
            {
                Debug.LogError(string.Concat(new object[] { "Cannot insert fake Item because id is not unique ", id, " Item already there: ", item.displayName }));
            }
            else if (this.FindItem(parentID) == null)
            {
                Debug.LogError("No parent Item found");
            }
            else
            {
                TreeViewItem rootItem;
                this.SetExpanded(parentID, true);
                List<TreeViewItem> rows = this.GetRows();
                int indexOfID = TreeView.GetIndexOfID(rows, parentID);
                if (indexOfID >= 0)
                {
                    rootItem = rows[indexOfID];
                }
                else
                {
                    rootItem = base.m_RootItem;
                }
                int depth = rootItem.depth + ((rootItem != base.m_RootItem) ? 1 : 0);
                base.m_FakeItem = new TreeViewItem(id, depth, rootItem, name);
                base.m_FakeItem.icon = icon;
                int num3 = this.GetInsertAfterItemIDForNewItem(name, rootItem, isCreatingNewFolder, this.foldersFirst);
                int index = TreeView.GetIndexOfID(rows, num3);
                if (index < 0)
                {
                    if (rows.Count > 0)
                    {
                        rows.Insert(0, base.m_FakeItem);
                    }
                    else
                    {
                        rows.Add(base.m_FakeItem);
                    }
                }
                else
                {
                    while (++index < rows.Count)
                    {
                        if (rows[index].depth <= depth)
                        {
                            break;
                        }
                    }
                    if (index < rows.Count)
                    {
                        rows.Insert(index, base.m_FakeItem);
                    }
                    else
                    {
                        rows.Add(base.m_FakeItem);
                    }
                }
                base.m_NeedRefreshVisibleFolders = false;
                base.m_TreeView.Frame(base.m_FakeItem.id, true, false);
                base.m_TreeView.Repaint();
            }
        }

        public override bool IsRenamingItemAllowed(TreeViewItem item)
        {
            if (AssetDatabase.IsSubAsset(item.id))
            {
                return false;
            }
            return (item.parent != null);
        }

        public override void OnExpandedStateChanged()
        {
            InternalEditorUtility.expandedProjectWindowItems = base.expandedIDs.ToArray();
            base.OnExpandedStateChanged();
        }

        public bool foldersFirst { get; set; }

        public bool foldersOnly { get; set; }

        private class FolderTreeItem : TreeViewItem
        {
            public FolderTreeItem(int id, int depth, TreeViewItem parent, string displayName) : base(id, depth, parent, displayName)
            {
            }
        }

        private class NonFolderTreeItem : TreeViewItem
        {
            public NonFolderTreeItem(int id, int depth, TreeViewItem parent, string displayName) : base(id, depth, parent, displayName)
            {
            }
        }

        internal class SemiNumericDisplayNameListComparer : IComparer<TreeViewItem>
        {
            public int Compare(TreeViewItem x, TreeViewItem y)
            {
                if (x == y)
                {
                    return 0;
                }
                if (x == null)
                {
                    return -1;
                }
                if (y == null)
                {
                    return 1;
                }
                return EditorUtility.NaturalCompare(x.displayName, y.displayName);
            }
        }
    }
}

