namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditorInternal;

    internal abstract class LazyTreeViewDataSource : TreeViewDataSource
    {
        public LazyTreeViewDataSource(TreeView treeView) : base(treeView)
        {
        }

        public static List<TreeViewItem> CreateChildListForCollapsedParent()
        {
            return new List<TreeViewItem> { null };
        }

        public override TreeViewItem FindItem(int itemID)
        {
            this.RevealItem(itemID);
            return base.FindItem(itemID);
        }

        protected abstract HashSet<int> GetParentsAbove(int id);
        protected abstract HashSet<int> GetParentsBelow(int id);
        public override List<TreeViewItem> GetRows()
        {
            this.InitIfNeeded();
            return base.m_VisibleRows;
        }

        public override void InitIfNeeded()
        {
            if ((base.m_VisibleRows == null) || base.m_NeedRefreshVisibleFolders)
            {
                this.FetchData();
                base.m_NeedRefreshVisibleFolders = false;
                if (base.onVisibleRowsChanged != null)
                {
                    base.onVisibleRowsChanged();
                }
                base.m_TreeView.Repaint();
            }
        }

        public static bool IsChildListForACollapsedParent(List<TreeViewItem> childList)
        {
            return (((childList != null) && (childList.Count == 1)) && (childList[0] == null));
        }

        public override void RevealItem(int itemID)
        {
            HashSet<int> source = new HashSet<int>(base.expandedIDs);
            int count = source.Count;
            HashSet<int> parentsAbove = this.GetParentsAbove(itemID);
            source.UnionWith(parentsAbove);
            if (count != source.Count)
            {
                this.SetExpandedIDs(source.ToArray<int>());
                if (base.m_NeedRefreshVisibleFolders)
                {
                    this.FetchData();
                }
            }
        }

        public override bool SetExpanded(int id, bool expand)
        {
            if (base.SetExpanded(id, expand))
            {
                InternalEditorUtility.expandedProjectWindowItems = base.expandedIDs.ToArray();
                return true;
            }
            return false;
        }

        public override void SetExpandedWithChildren(TreeViewItem item, bool expand)
        {
            HashSet<int> source = new HashSet<int>(base.expandedIDs);
            HashSet<int> parentsBelow = this.GetParentsBelow(item.id);
            if (expand)
            {
                source.UnionWith(parentsBelow);
            }
            else
            {
                source.ExceptWith(parentsBelow);
            }
            this.SetExpandedIDs(source.ToArray<int>());
        }
    }
}

