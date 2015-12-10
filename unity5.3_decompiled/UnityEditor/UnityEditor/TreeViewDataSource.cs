namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal abstract class TreeViewDataSource : ITreeViewDataSource
    {
        protected TreeViewItem m_FakeItem;
        protected bool m_NeedRefreshVisibleFolders = true;
        protected TreeViewItem m_RootItem;
        protected readonly TreeView m_TreeView;
        protected List<TreeViewItem> m_VisibleRows;
        public Action onVisibleRowsChanged;

        public TreeViewDataSource(TreeView treeView)
        {
            this.m_TreeView = treeView;
            this.showRootNode = true;
            this.rootIsCollapsable = false;
        }

        public virtual bool CanBeMultiSelected(TreeViewItem item)
        {
            return true;
        }

        public virtual bool CanBeParent(TreeViewItem item)
        {
            return true;
        }

        protected virtual List<TreeViewItem> ExpandedRows(TreeViewItem root)
        {
            List<TreeViewItem> items = new List<TreeViewItem>();
            this.GetVisibleItemsRecursive(this.m_RootItem, items);
            return items;
        }

        public abstract void FetchData();
        public virtual TreeViewItem FindItem(int id)
        {
            return TreeViewUtility.FindItem(id, this.m_RootItem);
        }

        public virtual int[] GetExpandedIDs()
        {
            return this.expandedIDs.ToArray();
        }

        public virtual TreeViewItem GetItem(int row)
        {
            return this.GetRows()[row];
        }

        public virtual int GetRow(int id)
        {
            List<TreeViewItem> rows = this.GetRows();
            for (int i = 0; i < rows.Count; i++)
            {
                if (rows[i].id == id)
                {
                    return i;
                }
            }
            return -1;
        }

        public virtual List<TreeViewItem> GetRows()
        {
            this.InitIfNeeded();
            return this.m_VisibleRows;
        }

        protected void GetVisibleItemsRecursive(TreeViewItem item, List<TreeViewItem> items)
        {
            if ((item != this.m_RootItem) || this.showRootNode)
            {
                items.Add(item);
            }
            if (item.hasChildren && this.IsExpanded(item))
            {
                foreach (TreeViewItem item2 in item.children)
                {
                    this.GetVisibleItemsRecursive(item2, items);
                }
            }
        }

        public virtual bool HasFakeItem()
        {
            return (this.m_FakeItem != null);
        }

        public virtual void InitIfNeeded()
        {
            if ((this.m_VisibleRows == null) || this.m_NeedRefreshVisibleFolders)
            {
                if (this.m_RootItem != null)
                {
                    if (this.m_TreeView.isSearching)
                    {
                        this.m_VisibleRows = this.Search(this.m_RootItem, this.m_TreeView.searchString.ToLower());
                    }
                    else
                    {
                        this.m_VisibleRows = this.ExpandedRows(this.m_RootItem);
                    }
                }
                else
                {
                    Debug.LogError("TreeView root item is null. Ensure that your TreeViewDataSource sets up at least a root item.");
                    this.m_VisibleRows = new List<TreeViewItem>();
                }
                this.m_NeedRefreshVisibleFolders = false;
                if (this.onVisibleRowsChanged != null)
                {
                    this.onVisibleRowsChanged();
                }
                this.m_TreeView.Repaint();
            }
        }

        public virtual void InsertFakeItem(int id, int parentID, string name, Texture2D icon)
        {
            Debug.LogError("InsertFakeItem missing implementation");
        }

        public virtual bool IsExpandable(TreeViewItem item)
        {
            if (this.m_TreeView.isSearching)
            {
                return false;
            }
            return item.hasChildren;
        }

        public virtual bool IsExpanded(int id)
        {
            return (this.expandedIDs.BinarySearch(id) >= 0);
        }

        public virtual bool IsExpanded(TreeViewItem item)
        {
            return this.IsExpanded(item.id);
        }

        public virtual bool IsRenamingItemAllowed(TreeViewItem item)
        {
            return true;
        }

        public virtual bool IsRevealed(int id)
        {
            return (TreeView.GetIndexOfID(this.GetRows(), id) >= 0);
        }

        public virtual void OnExpandedStateChanged()
        {
            if (this.m_TreeView.expandedStateChanged != null)
            {
                this.m_TreeView.expandedStateChanged();
            }
        }

        public virtual void OnInitialize()
        {
        }

        public virtual void OnSearchChanged()
        {
            this.m_NeedRefreshVisibleFolders = true;
        }

        public void ReloadData()
        {
            this.m_FakeItem = null;
            this.FetchData();
        }

        public virtual void RemoveFakeItem()
        {
            if (this.HasFakeItem())
            {
                List<TreeViewItem> rows = this.GetRows();
                int indexOfID = TreeView.GetIndexOfID(rows, this.m_FakeItem.id);
                if (indexOfID != -1)
                {
                    rows.RemoveAt(indexOfID);
                }
                this.m_FakeItem = null;
            }
        }

        public virtual void RevealItem(int id)
        {
            if (!this.IsRevealed(id))
            {
                TreeViewItem item = this.FindItem(id);
                if (item != null)
                {
                    for (TreeViewItem item2 = item.parent; item2 != null; item2 = item2.parent)
                    {
                        this.SetExpanded(item2, true);
                    }
                }
            }
        }

        protected virtual List<TreeViewItem> Search(TreeViewItem root, string search)
        {
            List<TreeViewItem> searchResult = new List<TreeViewItem>();
            if (this.showRootNode)
            {
                this.SearchRecursive(root, search, searchResult);
                searchResult.Sort(new TreeViewItemAlphaNumericSort());
                return searchResult;
            }
            int num = !this.alwaysAddFirstItemToSearchResult ? 0 : 1;
            if (root.hasChildren)
            {
                for (int i = num; i < root.children.Count; i++)
                {
                    this.SearchRecursive(root.children[i], search, searchResult);
                }
                searchResult.Sort(new TreeViewItemAlphaNumericSort());
                if (this.alwaysAddFirstItemToSearchResult)
                {
                    searchResult.Insert(0, root.children[0]);
                }
            }
            return searchResult;
        }

        protected void SearchRecursive(TreeViewItem item, string search, List<TreeViewItem> searchResult)
        {
            if (item.displayName.ToLower().Contains(search))
            {
                searchResult.Add(item);
            }
            if (item.children != null)
            {
                foreach (TreeViewItem item2 in item.children)
                {
                    this.SearchRecursive(item2, search, searchResult);
                }
            }
        }

        public virtual bool SetExpanded(int id, bool expand)
        {
            bool flag = this.IsExpanded(id);
            if (expand == flag)
            {
                return false;
            }
            if (expand)
            {
                this.expandedIDs.Add(id);
                this.expandedIDs.Sort();
            }
            else
            {
                this.expandedIDs.Remove(id);
            }
            this.m_NeedRefreshVisibleFolders = true;
            this.OnExpandedStateChanged();
            return true;
        }

        public virtual void SetExpanded(TreeViewItem item, bool expand)
        {
            this.SetExpanded(item.id, expand);
        }

        public virtual void SetExpandedIDs(int[] ids)
        {
            this.expandedIDs = new List<int>(ids);
            this.expandedIDs.Sort();
            this.m_NeedRefreshVisibleFolders = true;
            this.OnExpandedStateChanged();
        }

        public virtual void SetExpandedWithChildren(TreeViewItem fromItem, bool expand)
        {
            Stack<TreeViewItem> stack = new Stack<TreeViewItem>();
            stack.Push(fromItem);
            HashSet<int> other = new HashSet<int>();
            while (stack.Count > 0)
            {
                TreeViewItem item = stack.Pop();
                if (item.hasChildren)
                {
                    other.Add(item.id);
                    foreach (TreeViewItem item2 in item.children)
                    {
                        stack.Push(item2);
                    }
                }
            }
            HashSet<int> source = new HashSet<int>(this.expandedIDs);
            if (expand)
            {
                source.UnionWith(other);
            }
            else
            {
                source.ExceptWith(other);
            }
            this.SetExpandedIDs(source.ToArray<int>());
        }

        public bool alwaysAddFirstItemToSearchResult { get; set; }

        protected List<int> expandedIDs
        {
            get
            {
                return this.m_TreeView.state.expandedIDs;
            }
            set
            {
                this.m_TreeView.state.expandedIDs = value;
            }
        }

        public TreeViewItem root
        {
            get
            {
                return this.m_RootItem;
            }
        }

        public bool rootIsCollapsable { get; set; }

        public virtual int rowCount
        {
            get
            {
                return this.GetRows().Count;
            }
        }

        public bool showRootNode { get; set; }
    }
}

