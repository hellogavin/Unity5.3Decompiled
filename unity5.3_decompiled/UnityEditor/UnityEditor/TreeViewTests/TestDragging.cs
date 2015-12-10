namespace UnityEditor.TreeViewTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;

    internal class TestDragging : TreeViewDragging
    {
        [CompilerGenerated]
        private static Func<TreeViewItem, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<TreeViewItem, BackendData.Foo> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<TreeViewItem, bool> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<TreeViewItem, int> <>f__am$cache4;
        private const string k_GenericDragID = "FooDragging";
        private BackendData m_BackendData;

        public TestDragging(TreeView treeView, BackendData data) : base(treeView)
        {
            this.m_BackendData = data;
        }

        public override DragAndDropVisualMode DoDrag(TreeViewItem parentItem, TreeViewItem targetItem, bool perform, TreeViewDragging.DropPosition dropPos)
        {
            FooDragData genericData = DragAndDrop.GetGenericData("FooDragging") as FooDragData;
            FooTreeViewItem item = targetItem as FooTreeViewItem;
            FooTreeViewItem item2 = parentItem as FooTreeViewItem;
            if ((item2 == null) || (genericData == null))
            {
                return DragAndDropVisualMode.None;
            }
            bool flag = this.ValidDrag(parentItem, genericData.m_DraggedItems);
            if (perform && flag)
            {
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = x => x is FooTreeViewItem;
                }
                if (<>f__am$cache2 == null)
                {
                    <>f__am$cache2 = x => ((FooTreeViewItem) x).foo;
                }
                List<BackendData.Foo> draggedItems = genericData.m_DraggedItems.Where<TreeViewItem>(<>f__am$cache1).Select<TreeViewItem, BackendData.Foo>(<>f__am$cache2).ToList<BackendData.Foo>();
                if (<>f__am$cache3 == null)
                {
                    <>f__am$cache3 = x => x is FooTreeViewItem;
                }
                if (<>f__am$cache4 == null)
                {
                    <>f__am$cache4 = x => ((FooTreeViewItem) x).id;
                }
                int[] selectedIDs = genericData.m_DraggedItems.Where<TreeViewItem>(<>f__am$cache3).Select<TreeViewItem, int>(<>f__am$cache4).ToArray<int>();
                this.m_BackendData.ReparentSelection(item2.foo, item.foo, draggedItems);
                base.m_TreeView.ReloadData();
                base.m_TreeView.SetSelection(selectedIDs, true);
            }
            return (!flag ? DragAndDropVisualMode.None : DragAndDropVisualMode.Move);
        }

        private List<TreeViewItem> GetItemsFromIDs(IEnumerable<int> draggedItemIDs)
        {
            return TreeViewUtility.FindItemsInList(draggedItemIDs, base.m_TreeView.data.GetRows());
        }

        public override void StartDrag(TreeViewItem draggedNode, List<int> draggedItemIDs)
        {
            DragAndDrop.PrepareStartDrag();
            DragAndDrop.SetGenericData("FooDragging", new FooDragData(this.GetItemsFromIDs(draggedItemIDs)));
            DragAndDrop.objectReferences = new Object[0];
            DragAndDrop.StartDrag(draggedItemIDs.Count + " Foo" + ((draggedItemIDs.Count <= 1) ? string.Empty : "s"));
        }

        private bool ValidDrag(TreeViewItem parent, List<TreeViewItem> draggedItems)
        {
            for (TreeViewItem item = parent; item != null; item = item.parent)
            {
                if (draggedItems.Contains(item))
                {
                    return false;
                }
            }
            return true;
        }

        private class FooDragData
        {
            public List<TreeViewItem> m_DraggedItems;

            public FooDragData(List<TreeViewItem> draggedItems)
            {
                this.m_DraggedItems = draggedItems;
            }
        }
    }
}

