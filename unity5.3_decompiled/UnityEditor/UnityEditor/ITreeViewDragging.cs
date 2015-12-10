namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal interface ITreeViewDragging
    {
        bool CanStartDrag(TreeViewItem targetItem, List<int> draggedItemIDs, Vector2 mouseDownPosition);
        void DragCleanup(bool revertExpanded);
        bool DragElement(TreeViewItem targetItem, Rect targetItemRect, bool firstItem);
        int GetDropTargetControlID();
        int GetRowMarkerControlID();
        void OnInitialize();
        void StartDrag(TreeViewItem draggedItem, List<int> draggedItemIDs);

        bool drawRowMarkerAbove { get; set; }
    }
}

