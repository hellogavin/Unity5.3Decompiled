namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    internal static class TreeViewUtility
    {
        public static void DebugPrintToEditorLogRecursive(TreeViewItem item)
        {
            if (item != null)
            {
                Console.WriteLine(new string(' ', item.depth * 3) + item.displayName);
                if (item.hasChildren)
                {
                    foreach (TreeViewItem item2 in item.children)
                    {
                        DebugPrintToEditorLogRecursive(item2);
                    }
                }
            }
        }

        public static TreeViewItem FindItem(int id, TreeViewItem searchFromThisItem)
        {
            return FindItemRecursive(id, searchFromThisItem);
        }

        public static TreeViewItem FindItemInList<T>(int id, List<T> treeViewItems) where T: TreeViewItem
        {
            <FindItemInList>c__AnonStorey77<T> storey = new <FindItemInList>c__AnonStorey77<T> {
                id = id
            };
            return treeViewItems.FirstOrDefault<T>(new Func<T, bool>(storey.<>m__112));
        }

        private static TreeViewItem FindItemRecursive(int id, TreeViewItem item)
        {
            if (item != null)
            {
                if (item.id == id)
                {
                    return item;
                }
                if (!item.hasChildren)
                {
                    return null;
                }
                foreach (TreeViewItem item2 in item.children)
                {
                    TreeViewItem item3 = FindItemRecursive(id, item2);
                    if (item3 != null)
                    {
                        return item3;
                    }
                }
            }
            return null;
        }

        public static List<TreeViewItem> FindItemsInList(IEnumerable<int> itemIDs, List<TreeViewItem> treeViewItems)
        {
            <FindItemsInList>c__AnonStorey76 storey = new <FindItemsInList>c__AnonStorey76 {
                itemIDs = itemIDs
            };
            return treeViewItems.Where<TreeViewItem>(new Func<TreeViewItem, bool>(storey.<>m__111)).ToList<TreeViewItem>();
        }

        public static void SetChildParentReferences(List<TreeViewItem> visibleItems, TreeViewItem root)
        {
            for (int i = 0; i < visibleItems.Count; i++)
            {
                visibleItems[i].parent = null;
            }
            int capacity = 0;
            for (int j = 0; j < visibleItems.Count; j++)
            {
                SetChildParentReferences(j, visibleItems);
                if (visibleItems[j].parent == null)
                {
                    capacity++;
                }
            }
            if (capacity > 0)
            {
                List<TreeViewItem> list = new List<TreeViewItem>(capacity);
                for (int k = 0; k < visibleItems.Count; k++)
                {
                    if (visibleItems[k].parent == null)
                    {
                        list.Add(visibleItems[k]);
                        visibleItems[k].parent = root;
                    }
                }
                root.children = list;
            }
        }

        private static void SetChildParentReferences(int parentIndex, List<TreeViewItem> visibleItems)
        {
            TreeViewItem item = visibleItems[parentIndex];
            if (((item.children == null) || (item.children.Count <= 0)) || (item.children[0] == null))
            {
                int depth = item.depth;
                int capacity = 0;
                for (int i = parentIndex + 1; i < visibleItems.Count; i++)
                {
                    if (visibleItems[i].depth == (depth + 1))
                    {
                        capacity++;
                    }
                    if (visibleItems[i].depth <= depth)
                    {
                        break;
                    }
                }
                List<TreeViewItem> newChildList = null;
                if (capacity != 0)
                {
                    newChildList = new List<TreeViewItem>(capacity);
                    capacity = 0;
                    for (int j = parentIndex + 1; j < visibleItems.Count; j++)
                    {
                        if (visibleItems[j].depth == (depth + 1))
                        {
                            visibleItems[j].parent = item;
                            newChildList.Add(visibleItems[j]);
                            capacity++;
                        }
                        if (visibleItems[j].depth <= depth)
                        {
                            break;
                        }
                    }
                }
                SetChildren(item, newChildList);
            }
        }

        private static void SetChildren(TreeViewItem item, List<TreeViewItem> newChildList)
        {
            if (!LazyTreeViewDataSource.IsChildListForACollapsedParent(item.children) || (newChildList != null))
            {
                item.children = newChildList;
            }
        }

        [CompilerGenerated]
        private sealed class <FindItemInList>c__AnonStorey77<T> where T: TreeViewItem
        {
            internal int id;

            internal bool <>m__112(T t)
            {
                return (t.id == this.id);
            }
        }

        [CompilerGenerated]
        private sealed class <FindItemsInList>c__AnonStorey76
        {
            internal IEnumerable<int> itemIDs;

            internal bool <>m__111(TreeViewItem x)
            {
                return this.itemIDs.Contains<int>(x.id);
            }
        }
    }
}

