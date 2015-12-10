namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal class VerticalGrid
    {
        private int m_Columns = 1;
        private float m_Height;
        private float m_HorizontalSpacing;
        private int m_Rows;

        public int CalcColumns()
        {
            float num = !this.useFixedHorizontalSpacing ? this.minHorizontalSpacing : this.fixedHorizontalSpacing;
            int a = (int) Mathf.Floor(((this.fixedWidth - this.leftMargin) - this.rightMargin) / (this.itemSize.x + num));
            return Mathf.Max(a, 1);
        }

        public Rect CalcRect(int itemIdx, float yOffset)
        {
            float num = Mathf.Floor((float) (itemIdx / this.columns));
            float num2 = itemIdx - (num * this.columns);
            if (this.useFixedHorizontalSpacing)
            {
                return new Rect(this.leftMargin + (num2 * (this.itemSize.x + this.fixedHorizontalSpacing)), ((num * (this.itemSize.y + this.verticalSpacing)) + this.topMargin) + yOffset, this.itemSize.x, this.itemSize.y);
            }
            return new Rect((this.leftMargin + (this.horizontalSpacing * 0.5f)) + (num2 * (this.itemSize.x + this.horizontalSpacing)), ((num * (this.itemSize.y + this.verticalSpacing)) + this.topMargin) + yOffset, this.itemSize.x, this.itemSize.y);
        }

        public int CalcRows(int itemCount)
        {
            int num = (int) Mathf.Ceil(((float) itemCount) / ((float) this.CalcColumns()));
            if (num < 0)
            {
                return 0x7fffffff;
            }
            return num;
        }

        public int GetMaxVisibleItems(float height)
        {
            int num = (int) Mathf.Ceil(((height - this.topMargin) - this.bottomMargin) / (this.itemSize.y + this.verticalSpacing));
            return (num * this.columns);
        }

        public void InitNumRowsAndColumns(int itemCount, int maxNumRows)
        {
            if (this.useFixedHorizontalSpacing)
            {
                this.m_Columns = this.CalcColumns();
                this.m_HorizontalSpacing = this.fixedHorizontalSpacing;
                this.m_Rows = Mathf.Min(maxNumRows, this.CalcRows(itemCount));
                this.m_Height = (((this.m_Rows * (this.itemSize.y + this.verticalSpacing)) - this.verticalSpacing) + this.topMargin) + this.bottomMargin;
            }
            else
            {
                this.m_Columns = this.CalcColumns();
                this.m_HorizontalSpacing = Mathf.Max((float) 0f, (float) ((this.fixedWidth - (((this.m_Columns * this.itemSize.x) + this.leftMargin) + this.rightMargin)) / ((float) this.m_Columns)));
                this.m_Rows = Mathf.Min(maxNumRows, this.CalcRows(itemCount));
                if (this.m_Rows == 1)
                {
                    this.m_HorizontalSpacing = this.minHorizontalSpacing;
                }
                this.m_Height = (((this.m_Rows * (this.itemSize.y + this.verticalSpacing)) - this.verticalSpacing) + this.topMargin) + this.bottomMargin;
            }
        }

        public bool IsVisibleInScrollView(float scrollViewHeight, float scrollPos, float gridStartY, int maxIndex, out int startIndex, out int endIndex)
        {
            startIndex = endIndex = 0;
            float num = scrollPos;
            float num2 = scrollPos + scrollViewHeight;
            float num3 = gridStartY + this.topMargin;
            if (num3 > num2)
            {
                return false;
            }
            if ((num3 + this.height) < num)
            {
                return false;
            }
            float num4 = this.itemSize.y + this.verticalSpacing;
            int num5 = Mathf.FloorToInt((num - num3) / num4);
            startIndex = num5 * this.columns;
            startIndex = Mathf.Clamp(startIndex, 0, maxIndex);
            int num6 = Mathf.FloorToInt((num2 - num3) / num4);
            endIndex = ((num6 + 1) * this.columns) - 1;
            endIndex = Mathf.Clamp(endIndex, 0, maxIndex);
            return true;
        }

        public override string ToString()
        {
            object[] args = new object[] { this.rows, this.columns, this.fixedWidth, this.itemSize };
            return string.Format("VerticalGrid: rows {0}, columns {1}, fixedWidth {2}, itemSize {3}", args);
        }

        public float bottomMargin { get; set; }

        public int columns
        {
            get
            {
                return this.m_Columns;
            }
        }

        public float fixedHorizontalSpacing { get; set; }

        public float fixedWidth { get; set; }

        public float height
        {
            get
            {
                return this.m_Height;
            }
        }

        public float horizontalSpacing
        {
            get
            {
                return this.m_HorizontalSpacing;
            }
        }

        public Vector2 itemSize { get; set; }

        public float leftMargin { get; set; }

        public float minHorizontalSpacing { get; set; }

        public float rightMargin { get; set; }

        public int rows
        {
            get
            {
                return this.m_Rows;
            }
        }

        public float topMargin { get; set; }

        public bool useFixedHorizontalSpacing { get; set; }

        public float verticalSpacing { get; set; }
    }
}

