namespace UnityEditor
{
    using System;
    using UnityEngine;

    [Serializable]
    internal class ListViewState
    {
        private const int c_rowHeight = 0x10;
        public int column;
        public int customDraggedFromID;
        public int draggedFrom;
        public int draggedTo;
        public bool drawDropHere;
        public Rect dropHereRect;
        public string[] fileNames;
        public int ID;
        internal ListViewShared.InternalLayoutedListViewState ilvState;
        public int row;
        public int rowHeight;
        public Vector2 scrollPos;
        public bool selectionChanged;
        public int totalRows;

        public ListViewState()
        {
            this.dropHereRect = new Rect(0f, 0f, 0f, 0f);
            this.ilvState = new ListViewShared.InternalLayoutedListViewState();
            this.Init(0, 0x10);
        }

        public ListViewState(int totalRows)
        {
            this.dropHereRect = new Rect(0f, 0f, 0f, 0f);
            this.ilvState = new ListViewShared.InternalLayoutedListViewState();
            this.Init(totalRows, 0x10);
        }

        public ListViewState(int totalRows, int rowHeight)
        {
            this.dropHereRect = new Rect(0f, 0f, 0f, 0f);
            this.ilvState = new ListViewShared.InternalLayoutedListViewState();
            this.Init(totalRows, rowHeight);
        }

        private void Init(int totalRows, int rowHeight)
        {
            this.row = -1;
            this.column = 0;
            this.scrollPos = Vector2.zero;
            this.totalRows = totalRows;
            this.rowHeight = rowHeight;
            this.selectionChanged = false;
        }
    }
}

