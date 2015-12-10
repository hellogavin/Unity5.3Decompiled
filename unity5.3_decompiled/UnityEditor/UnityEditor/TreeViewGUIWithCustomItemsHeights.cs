namespace UnityEditor
{
    using mscorlib;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal abstract class TreeViewGUIWithCustomItemsHeights : ITreeViewGUI
    {
        protected float m_BaseIndent = 2f;
        protected float m_FoldoutWidth = 12f;
        protected float m_IndentWidth = 14f;
        private float m_MaxWidthOfRows;
        private List<Rect> m_RowRects = new List<Rect>();
        protected readonly TreeView m_TreeView;

        public TreeViewGUIWithCustomItemsHeights(TreeView treeView)
        {
            this.m_TreeView = treeView;
        }

        protected virtual float AddSpaceBefore(TreeViewItem item)
        {
            return 0f;
        }

        public virtual void BeginPingNode(TreeViewItem item, float topPixelOfRow, float availableWidth)
        {
            throw new NotImplementedException();
        }

        public virtual bool BeginRename(TreeViewItem item, float delay)
        {
            throw new NotImplementedException();
        }

        public virtual void BeginRowGUI()
        {
        }

        public void CalculateRowRects()
        {
            if (!this.m_TreeView.isSearching)
            {
                List<TreeViewItem> rows = this.m_TreeView.data.GetRows();
                this.m_RowRects = new List<Rect>(rows.Count);
                float y = 2f;
                this.m_MaxWidthOfRows = 1f;
                for (int i = 0; i < rows.Count; i++)
                {
                    TreeViewItem item = rows[i];
                    float num4 = this.AddSpaceBefore(item);
                    y += num4;
                    Vector2 sizeOfRow = this.GetSizeOfRow(item);
                    this.m_RowRects.Add(new Rect(0f, y, sizeOfRow.x, sizeOfRow.y));
                    y += sizeOfRow.y;
                    if (sizeOfRow.x > this.m_MaxWidthOfRows)
                    {
                        this.m_MaxWidthOfRows = sizeOfRow.x;
                    }
                }
            }
        }

        public virtual void EndPingNode()
        {
            throw new NotImplementedException();
        }

        public virtual void EndRename()
        {
            throw new NotImplementedException();
        }

        public virtual void EndRowGUI()
        {
        }

        public virtual float GetContentIndent(TreeViewItem item)
        {
            return (this.GetFoldoutIndent(item) + this.m_FoldoutWidth);
        }

        public void GetFirstAndLastRowVisible(out int firstRowVisible, out int lastRowVisible)
        {
            float y = this.m_TreeView.state.scrollPos.y;
            float height = this.m_TreeView.GetTotalRect().height;
            int rowCount = this.m_TreeView.data.rowCount;
            if (rowCount != this.m_RowRects.Count)
            {
                Debug.LogError("Mismatch in state: rows vs cached rects. Did you remember to hook up: dataSource.onVisibleRowsChanged += gui.CalculateRowRects ?");
                this.CalculateRowRects();
            }
            int num4 = -1;
            int num5 = -1;
            for (int i = 0; i < this.m_RowRects.Count; i++)
            {
                Rect rect2 = this.m_RowRects[i];
                if (rect2.y > y)
                {
                    Rect rect3 = this.m_RowRects[i];
                    if (rect3.y < (y + height))
                    {
                        goto Label_00E0;
                    }
                }
                Rect rect4 = this.m_RowRects[i];
                System.Boolean ReflectorVariable0 = true;
                goto Label_00E1;
            Label_00E0:
                ReflectorVariable0 = false;
            Label_00E1:
                if (ReflectorVariable0 ? ((rect4.yMax > y) && (this.m_RowRects[i].yMax < (y + height))) : true)
                {
                    if (num4 == -1)
                    {
                        num4 = i;
                    }
                    num5 = i;
                }
            }
            if ((num4 != -1) && (num5 != -1))
            {
                firstRowVisible = num4;
                lastRowVisible = num5;
            }
            else
            {
                firstRowVisible = 0;
                lastRowVisible = rowCount - 1;
            }
        }

        public virtual float GetFoldoutIndent(TreeViewItem item)
        {
            if (this.m_TreeView.isSearching)
            {
                return this.m_BaseIndent;
            }
            return (this.m_BaseIndent + (item.depth * this.indentWidth));
        }

        public int GetNumRowsOnPageUpDown(TreeViewItem fromItem, bool pageUp, float heightOfTreeView)
        {
            Debug.LogError("GetNumRowsOnPageUpDown: Not impemented");
            return (int) Mathf.Floor(heightOfTreeView / 30f);
        }

        public Rect GetRectForFraming(int row)
        {
            return this.GetRowRect(row, 1f);
        }

        public Rect GetRowRect(int row, float rowWidth)
        {
            if (this.m_RowRects.Count == 0)
            {
                Debug.LogError("Ensure precalc rects");
                return new Rect();
            }
            return this.m_RowRects[row];
        }

        protected virtual Vector2 GetSizeOfRow(TreeViewItem item)
        {
            return new Vector2(this.m_TreeView.GetTotalRect().width, 16f);
        }

        public Vector2 GetTotalSize()
        {
            if (this.m_RowRects.Count == 0)
            {
                return new Vector2(0f, 0f);
            }
            Rect rect = this.m_RowRects[this.m_RowRects.Count - 1];
            return new Vector2(this.m_MaxWidthOfRows, rect.yMax);
        }

        public virtual void OnInitialize()
        {
        }

        public abstract void OnRowGUI(Rect rowRect, TreeViewItem item, int row, bool selected, bool focused);

        public virtual float bottomRowMargin { get; private set; }

        public virtual float halfDropBetweenHeight
        {
            get
            {
                return 8f;
            }
        }

        protected float indentWidth
        {
            get
            {
                return this.m_IndentWidth;
            }
        }

        public virtual float topRowMargin { get; private set; }
    }
}

