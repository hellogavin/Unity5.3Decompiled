namespace UnityEditorInternal
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal class AudioProfilerTreeViewState : TreeViewState
    {
        [SerializeField]
        public int prevSelectedColumn = 5;
        [SerializeField]
        public int selectedColumn = 3;
        [SerializeField]
        public bool sortByDescendingOrder = true;

        public void SetSelectedColumn(int index)
        {
            if (index != this.selectedColumn)
            {
                this.prevSelectedColumn = this.selectedColumn;
            }
            else
            {
                this.sortByDescendingOrder = !this.sortByDescendingOrder;
            }
            this.selectedColumn = index;
        }
    }
}

