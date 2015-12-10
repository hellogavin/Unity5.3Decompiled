namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEditorInternal;

    internal class AssetsTreeViewDragging : TreeViewDragging
    {
        public AssetsTreeViewDragging(TreeView treeView) : base(treeView)
        {
        }

        public override DragAndDropVisualMode DoDrag(TreeViewItem parentItem, TreeViewItem targetItem, bool perform, TreeViewDragging.DropPosition dropPos)
        {
            HierarchyProperty property = new HierarchyProperty(HierarchyType.Assets);
            if ((parentItem == null) || !property.Find(parentItem.id, null))
            {
                property = null;
            }
            return InternalEditorUtility.ProjectWindowDrag(property, perform);
        }

        public override void StartDrag(TreeViewItem draggedItem, List<int> draggedItemIDs)
        {
            DragAndDrop.PrepareStartDrag();
            DragAndDrop.objectReferences = ProjectWindowUtil.GetDragAndDropObjects(draggedItem.id, draggedItemIDs);
            DragAndDrop.paths = ProjectWindowUtil.GetDragAndDropPaths(draggedItem.id, draggedItemIDs);
            if (DragAndDrop.objectReferences.Length > 1)
            {
                DragAndDrop.StartDrag("<Multiple>");
            }
            else
            {
                DragAndDrop.StartDrag(ObjectNames.GetDragAndDropTitle(InternalEditorUtility.GetObjectFromInstanceID(draggedItem.id)));
            }
        }
    }
}

