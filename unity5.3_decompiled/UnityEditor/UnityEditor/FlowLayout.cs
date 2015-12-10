namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal class FlowLayout : GUILayoutGroup
    {
        private LineInfo[] m_LineInfo;
        private int m_Lines;

        public override void CalcHeight()
        {
            if (base.entries.Count == 0)
            {
                base.maxHeight = base.minHeight = 0f;
            }
            else
            {
                base.m_ChildMinHeight = base.m_ChildMaxHeight = 0f;
                int topBorder = 0;
                int bottomBorder = 0;
                base.m_StretchableCountY = 0;
                if (!base.isVertical)
                {
                    this.m_LineInfo = new LineInfo[this.m_Lines];
                    for (int i = 0; i < this.m_Lines; i++)
                    {
                        this.m_LineInfo[i].topBorder = 0x2710;
                        this.m_LineInfo[i].bottomBorder = 0x2710;
                    }
                    foreach (GUILayoutEntry entry in base.entries)
                    {
                        entry.CalcHeight();
                        int y = (int) entry.rect.y;
                        this.m_LineInfo[y].minSize = Mathf.Max(entry.minHeight, this.m_LineInfo[y].minSize);
                        this.m_LineInfo[y].maxSize = Mathf.Max(entry.maxHeight, this.m_LineInfo[y].maxSize);
                        this.m_LineInfo[y].topBorder = Mathf.Min(entry.margin.top, this.m_LineInfo[y].topBorder);
                        this.m_LineInfo[y].bottomBorder = Mathf.Min(entry.margin.bottom, this.m_LineInfo[y].bottomBorder);
                    }
                    for (int j = 0; j < this.m_Lines; j++)
                    {
                        base.m_ChildMinHeight += this.m_LineInfo[j].minSize;
                        base.m_ChildMaxHeight += this.m_LineInfo[j].maxSize;
                    }
                    for (int k = 1; k < this.m_Lines; k++)
                    {
                        float num7 = Mathf.Max(this.m_LineInfo[k - 1].bottomBorder, this.m_LineInfo[k].topBorder);
                        base.m_ChildMinHeight += num7;
                        base.m_ChildMaxHeight += num7;
                    }
                    topBorder = this.m_LineInfo[0].topBorder;
                    bottomBorder = this.m_LineInfo[this.m_LineInfo.Length - 1].bottomBorder;
                }
                float num8 = 0f;
                float num9 = 0f;
                this.margin.top = topBorder;
                this.margin.bottom = bottomBorder;
                num8 = num9 = 0f;
                base.minHeight = Mathf.Max(base.minHeight, (base.m_ChildMinHeight + num8) + num9);
                if (base.maxHeight == 0f)
                {
                    base.stretchHeight += base.m_StretchableCountY + (!base.style.stretchHeight ? 0 : 1);
                    base.maxHeight = (base.m_ChildMaxHeight + num8) + num9;
                }
                else
                {
                    base.stretchHeight = 0;
                }
                base.maxHeight = Mathf.Max(base.maxHeight, base.minHeight);
            }
        }

        public override void CalcWidth()
        {
            bool flag = base.minWidth != 0f;
            base.CalcWidth();
            if (!base.isVertical && !flag)
            {
                base.minWidth = 0f;
                foreach (GUILayoutEntry entry in base.entries)
                {
                    base.minWidth = Mathf.Max(base.m_ChildMinWidth, entry.minWidth);
                }
            }
        }

        public override void SetHorizontal(float x, float width)
        {
            base.SetHorizontal(x, width);
            if (base.resetCoords)
            {
                x = 0f;
            }
            if (base.isVertical)
            {
                Debug.LogError("Wordwrapped vertical group. Don't. Just Don't");
            }
            else
            {
                this.m_Lines = 0;
                float num = 0f;
                foreach (GUILayoutEntry entry in base.entries)
                {
                    if ((entry.rect.xMax - num) > (x + width))
                    {
                        num = entry.rect.x - entry.margin.left;
                        this.m_Lines++;
                    }
                    entry.SetHorizontal(entry.rect.x - num, entry.rect.width);
                    entry.rect.y = this.m_Lines;
                }
                this.m_Lines++;
            }
        }

        public override void SetVertical(float y, float height)
        {
            if (base.entries.Count == 0)
            {
                base.SetVertical(y, height);
            }
            else if (base.isVertical)
            {
                base.SetVertical(y, height);
            }
            else
            {
                if (base.resetCoords)
                {
                    y = 0f;
                }
                float num = y - this.margin.top;
                float num2 = y + this.margin.vertical;
                float num3 = num2 - (base.spacing * (this.m_Lines - 1));
                float t = 0f;
                if (base.m_ChildMinHeight != base.m_ChildMaxHeight)
                {
                    t = Mathf.Clamp((float) ((num3 - base.m_ChildMinHeight) / (base.m_ChildMaxHeight - base.m_ChildMinHeight)), (float) 0f, (float) 1f);
                }
                float num5 = num;
                for (int i = 0; i < this.m_Lines; i++)
                {
                    if (i > 0)
                    {
                        num5 += Mathf.Max(this.m_LineInfo[i].topBorder, this.m_LineInfo[i - 1].bottomBorder);
                    }
                    this.m_LineInfo[i].start = num5;
                    this.m_LineInfo[i].size = Mathf.Lerp(this.m_LineInfo[i].minSize, this.m_LineInfo[i].maxSize, t);
                    num5 += this.m_LineInfo[i].size + base.spacing;
                }
                foreach (GUILayoutEntry entry in base.entries)
                {
                    LineInfo info = this.m_LineInfo[(int) entry.rect.y];
                    if (entry.stretchHeight != 0)
                    {
                        entry.SetVertical(info.start + entry.margin.top, info.size - entry.margin.vertical);
                    }
                    else
                    {
                        entry.SetVertical(info.start + entry.margin.top, Mathf.Clamp(info.size - entry.margin.vertical, entry.minHeight, entry.maxHeight));
                    }
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct LineInfo
        {
            public float minSize;
            public float maxSize;
            public float start;
            public float size;
            public int topBorder;
            public int bottomBorder;
            public int expandSize;
        }
    }
}

