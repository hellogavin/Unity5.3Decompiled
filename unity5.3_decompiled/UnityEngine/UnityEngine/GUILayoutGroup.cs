namespace UnityEngine
{
    using System;
    using System.Collections.Generic;

    internal class GUILayoutGroup : GUILayoutEntry
    {
        public List<GUILayoutEntry> entries;
        public bool isVertical;
        public bool isWindow;
        protected float m_ChildMaxHeight;
        protected float m_ChildMaxWidth;
        protected float m_ChildMinHeight;
        protected float m_ChildMinWidth;
        private int m_Cursor;
        private readonly RectOffset m_Margin;
        protected int m_StretchableCountX;
        protected int m_StretchableCountY;
        protected bool m_UserSpecifiedHeight;
        protected bool m_UserSpecifiedWidth;
        public bool resetCoords;
        public bool sameSize;
        public float spacing;
        public int windowID;

        public GUILayoutGroup() : base(0f, 0f, 0f, 0f, GUIStyle.none)
        {
            this.entries = new List<GUILayoutEntry>();
            this.isVertical = true;
            this.sameSize = true;
            this.windowID = -1;
            this.m_StretchableCountX = 100;
            this.m_StretchableCountY = 100;
            this.m_ChildMinWidth = 100f;
            this.m_ChildMaxWidth = 100f;
            this.m_ChildMinHeight = 100f;
            this.m_ChildMaxHeight = 100f;
            this.m_Margin = new RectOffset();
        }

        public GUILayoutGroup(GUIStyle _style, GUILayoutOption[] options) : base(0f, 0f, 0f, 0f, _style)
        {
            this.entries = new List<GUILayoutEntry>();
            this.isVertical = true;
            this.sameSize = true;
            this.windowID = -1;
            this.m_StretchableCountX = 100;
            this.m_StretchableCountY = 100;
            this.m_ChildMinWidth = 100f;
            this.m_ChildMaxWidth = 100f;
            this.m_ChildMinHeight = 100f;
            this.m_ChildMaxHeight = 100f;
            this.m_Margin = new RectOffset();
            if (options != null)
            {
                this.ApplyOptions(options);
            }
            this.m_Margin.left = _style.margin.left;
            this.m_Margin.right = _style.margin.right;
            this.m_Margin.top = _style.margin.top;
            this.m_Margin.bottom = _style.margin.bottom;
        }

        public void Add(GUILayoutEntry e)
        {
            this.entries.Add(e);
        }

        public override void ApplyOptions(GUILayoutOption[] options)
        {
            if (options != null)
            {
                base.ApplyOptions(options);
                foreach (GUILayoutOption option in options)
                {
                    switch (option.type)
                    {
                        case GUILayoutOption.Type.fixedWidth:
                        case GUILayoutOption.Type.minWidth:
                        case GUILayoutOption.Type.maxWidth:
                            this.m_UserSpecifiedHeight = true;
                            break;

                        case GUILayoutOption.Type.fixedHeight:
                        case GUILayoutOption.Type.minHeight:
                        case GUILayoutOption.Type.maxHeight:
                            this.m_UserSpecifiedWidth = true;
                            break;

                        case GUILayoutOption.Type.spacing:
                            this.spacing = (int) option.value;
                            break;
                    }
                }
            }
        }

        protected override void ApplyStyleSettings(GUIStyle style)
        {
            base.ApplyStyleSettings(style);
            RectOffset margin = style.margin;
            this.m_Margin.left = margin.left;
            this.m_Margin.right = margin.right;
            this.m_Margin.top = margin.top;
            this.m_Margin.bottom = margin.bottom;
        }

        public override void CalcHeight()
        {
            if (this.entries.Count == 0)
            {
                base.maxHeight = base.minHeight = base.style.padding.vertical;
            }
            else
            {
                int b = 0;
                int bottom = 0;
                this.m_ChildMinHeight = 0f;
                this.m_ChildMaxHeight = 0f;
                this.m_StretchableCountY = 0;
                if (this.isVertical)
                {
                    int a = 0;
                    bool flag = true;
                    foreach (GUILayoutEntry entry in this.entries)
                    {
                        entry.CalcHeight();
                        RectOffset margin = entry.margin;
                        if (entry.style != GUILayoutUtility.spaceStyle)
                        {
                            int num4;
                            if (!flag)
                            {
                                num4 = Mathf.Max(a, margin.top);
                            }
                            else
                            {
                                num4 = 0;
                                flag = false;
                            }
                            this.m_ChildMinHeight += (entry.minHeight + this.spacing) + num4;
                            this.m_ChildMaxHeight += (entry.maxHeight + this.spacing) + num4;
                            a = margin.bottom;
                            this.m_StretchableCountY += entry.stretchHeight;
                        }
                        else
                        {
                            this.m_ChildMinHeight += entry.minHeight;
                            this.m_ChildMaxHeight += entry.maxHeight;
                            this.m_StretchableCountY += entry.stretchHeight;
                        }
                    }
                    this.m_ChildMinHeight -= this.spacing;
                    this.m_ChildMaxHeight -= this.spacing;
                    if (this.entries.Count != 0)
                    {
                        b = this.entries[0].margin.top;
                        bottom = a;
                    }
                    else
                    {
                        bottom = b = 0;
                    }
                }
                else
                {
                    bool flag2 = true;
                    foreach (GUILayoutEntry entry2 in this.entries)
                    {
                        entry2.CalcHeight();
                        RectOffset offset2 = entry2.margin;
                        if (entry2.style != GUILayoutUtility.spaceStyle)
                        {
                            if (!flag2)
                            {
                                b = Mathf.Min(offset2.top, b);
                                bottom = Mathf.Min(offset2.bottom, bottom);
                            }
                            else
                            {
                                b = offset2.top;
                                bottom = offset2.bottom;
                                flag2 = false;
                            }
                            this.m_ChildMinHeight = Mathf.Max(entry2.minHeight, this.m_ChildMinHeight);
                            this.m_ChildMaxHeight = Mathf.Max(entry2.maxHeight, this.m_ChildMaxHeight);
                        }
                        this.m_StretchableCountY += entry2.stretchHeight;
                    }
                }
                float num5 = 0f;
                float num6 = 0f;
                if ((base.style != GUIStyle.none) || this.m_UserSpecifiedHeight)
                {
                    num5 = Mathf.Max(base.style.padding.top, b);
                    num6 = Mathf.Max(base.style.padding.bottom, bottom);
                }
                else
                {
                    this.m_Margin.top = b;
                    this.m_Margin.bottom = bottom;
                    num5 = num6 = 0f;
                }
                base.minHeight = Mathf.Max(base.minHeight, (this.m_ChildMinHeight + num5) + num6);
                if (base.maxHeight == 0f)
                {
                    base.stretchHeight += this.m_StretchableCountY + (!base.style.stretchHeight ? 0 : 1);
                    base.maxHeight = (this.m_ChildMaxHeight + num5) + num6;
                }
                else
                {
                    base.stretchHeight = 0;
                }
                base.maxHeight = Mathf.Max(base.maxHeight, base.minHeight);
                if (base.style.fixedHeight != 0f)
                {
                    base.maxHeight = base.minHeight = base.style.fixedHeight;
                    base.stretchHeight = 0;
                }
            }
        }

        public override void CalcWidth()
        {
            if (this.entries.Count == 0)
            {
                base.maxWidth = base.minWidth = base.style.padding.horizontal;
            }
            else
            {
                int b = 0;
                int right = 0;
                this.m_ChildMinWidth = 0f;
                this.m_ChildMaxWidth = 0f;
                this.m_StretchableCountX = 0;
                bool flag = true;
                if (this.isVertical)
                {
                    foreach (GUILayoutEntry entry in this.entries)
                    {
                        entry.CalcWidth();
                        RectOffset margin = entry.margin;
                        if (entry.style != GUILayoutUtility.spaceStyle)
                        {
                            if (!flag)
                            {
                                b = Mathf.Min(margin.left, b);
                                right = Mathf.Min(margin.right, right);
                            }
                            else
                            {
                                b = margin.left;
                                right = margin.right;
                                flag = false;
                            }
                            this.m_ChildMinWidth = Mathf.Max(entry.minWidth + margin.horizontal, this.m_ChildMinWidth);
                            this.m_ChildMaxWidth = Mathf.Max(entry.maxWidth + margin.horizontal, this.m_ChildMaxWidth);
                        }
                        this.m_StretchableCountX += entry.stretchWidth;
                    }
                    this.m_ChildMinWidth -= b + right;
                    this.m_ChildMaxWidth -= b + right;
                }
                else
                {
                    int num3 = 0;
                    foreach (GUILayoutEntry entry2 in this.entries)
                    {
                        entry2.CalcWidth();
                        RectOffset offset2 = entry2.margin;
                        if (entry2.style != GUILayoutUtility.spaceStyle)
                        {
                            int num4;
                            if (!flag)
                            {
                                num4 = (num3 <= offset2.left) ? offset2.left : num3;
                            }
                            else
                            {
                                num4 = 0;
                                flag = false;
                            }
                            this.m_ChildMinWidth += (entry2.minWidth + this.spacing) + num4;
                            this.m_ChildMaxWidth += (entry2.maxWidth + this.spacing) + num4;
                            num3 = offset2.right;
                            this.m_StretchableCountX += entry2.stretchWidth;
                        }
                        else
                        {
                            this.m_ChildMinWidth += entry2.minWidth;
                            this.m_ChildMaxWidth += entry2.maxWidth;
                            this.m_StretchableCountX += entry2.stretchWidth;
                        }
                    }
                    this.m_ChildMinWidth -= this.spacing;
                    this.m_ChildMaxWidth -= this.spacing;
                    if (this.entries.Count != 0)
                    {
                        b = this.entries[0].margin.left;
                        right = num3;
                    }
                    else
                    {
                        b = right = 0;
                    }
                }
                float num5 = 0f;
                float num6 = 0f;
                if ((base.style != GUIStyle.none) || this.m_UserSpecifiedWidth)
                {
                    num5 = Mathf.Max(base.style.padding.left, b);
                    num6 = Mathf.Max(base.style.padding.right, right);
                }
                else
                {
                    this.m_Margin.left = b;
                    this.m_Margin.right = right;
                    num5 = num6 = 0f;
                }
                base.minWidth = Mathf.Max(base.minWidth, (this.m_ChildMinWidth + num5) + num6);
                if (base.maxWidth == 0f)
                {
                    base.stretchWidth += this.m_StretchableCountX + (!base.style.stretchWidth ? 0 : 1);
                    base.maxWidth = (this.m_ChildMaxWidth + num5) + num6;
                }
                else
                {
                    base.stretchWidth = 0;
                }
                base.maxWidth = Mathf.Max(base.maxWidth, base.minWidth);
                if (base.style.fixedWidth != 0f)
                {
                    base.maxWidth = base.minWidth = base.style.fixedWidth;
                    base.stretchWidth = 0;
                }
            }
        }

        public Rect GetLast()
        {
            if (this.m_Cursor == 0)
            {
                Debug.LogError("You cannot call GetLast immediately after beginning a group.");
                return GUILayoutEntry.kDummyRect;
            }
            if (this.m_Cursor <= this.entries.Count)
            {
                GUILayoutEntry entry = this.entries[this.m_Cursor - 1];
                return entry.rect;
            }
            Debug.LogError(string.Concat(new object[] { "Getting control ", this.m_Cursor, "'s position in a group with only ", this.entries.Count, " controls when doing ", Event.current.type }));
            return GUILayoutEntry.kDummyRect;
        }

        public GUILayoutEntry GetNext()
        {
            if (this.m_Cursor < this.entries.Count)
            {
                GUILayoutEntry entry = this.entries[this.m_Cursor];
                this.m_Cursor++;
                return entry;
            }
            object[] objArray1 = new object[] { "Getting control ", this.m_Cursor, "'s position in a group with only ", this.entries.Count, " controls when doing ", Event.current.rawType, "\nAborting" };
            throw new ArgumentException(string.Concat(objArray1));
        }

        public Rect PeekNext()
        {
            if (this.m_Cursor < this.entries.Count)
            {
                GUILayoutEntry entry = this.entries[this.m_Cursor];
                return entry.rect;
            }
            object[] objArray1 = new object[] { "Getting control ", this.m_Cursor, "'s position in a group with only ", this.entries.Count, " controls when doing ", Event.current.rawType, "\nAborting" };
            throw new ArgumentException(string.Concat(objArray1));
        }

        public void ResetCursor()
        {
            this.m_Cursor = 0;
        }

        public override void SetHorizontal(float x, float width)
        {
            base.SetHorizontal(x, width);
            if (this.resetCoords)
            {
                x = 0f;
            }
            RectOffset padding = base.style.padding;
            if (this.isVertical)
            {
                if (base.style != GUIStyle.none)
                {
                    foreach (GUILayoutEntry entry in this.entries)
                    {
                        float num = Mathf.Max(entry.margin.left, padding.left);
                        float num2 = x + num;
                        float num3 = (width - Mathf.Max(entry.margin.right, padding.right)) - num;
                        if (entry.stretchWidth != 0)
                        {
                            entry.SetHorizontal(num2, num3);
                        }
                        else
                        {
                            entry.SetHorizontal(num2, Mathf.Clamp(num3, entry.minWidth, entry.maxWidth));
                        }
                    }
                }
                else
                {
                    float num4 = x - this.margin.left;
                    float num5 = width + this.margin.horizontal;
                    foreach (GUILayoutEntry entry2 in this.entries)
                    {
                        if (entry2.stretchWidth != 0)
                        {
                            entry2.SetHorizontal(num4 + entry2.margin.left, num5 - entry2.margin.horizontal);
                        }
                        else
                        {
                            entry2.SetHorizontal(num4 + entry2.margin.left, Mathf.Clamp(num5 - entry2.margin.horizontal, entry2.minWidth, entry2.maxWidth));
                        }
                    }
                }
            }
            else
            {
                if (base.style != GUIStyle.none)
                {
                    float left = padding.left;
                    float right = padding.right;
                    if (this.entries.Count != 0)
                    {
                        left = Mathf.Max(left, (float) this.entries[0].margin.left);
                        right = Mathf.Max(right, (float) this.entries[this.entries.Count - 1].margin.right);
                    }
                    x += left;
                    width -= right + left;
                }
                float num8 = width - (this.spacing * (this.entries.Count - 1));
                float t = 0f;
                if (this.m_ChildMinWidth != this.m_ChildMaxWidth)
                {
                    t = Mathf.Clamp((float) ((num8 - this.m_ChildMinWidth) / (this.m_ChildMaxWidth - this.m_ChildMinWidth)), (float) 0f, (float) 1f);
                }
                float num10 = 0f;
                if ((num8 > this.m_ChildMaxWidth) && (this.m_StretchableCountX > 0))
                {
                    num10 = (num8 - this.m_ChildMaxWidth) / ((float) this.m_StretchableCountX);
                }
                int num11 = 0;
                bool flag = true;
                foreach (GUILayoutEntry entry3 in this.entries)
                {
                    float f = Mathf.Lerp(entry3.minWidth, entry3.maxWidth, t) + (num10 * entry3.stretchWidth);
                    if (entry3.style != GUILayoutUtility.spaceStyle)
                    {
                        int num13 = entry3.margin.left;
                        if (flag)
                        {
                            num13 = 0;
                            flag = false;
                        }
                        int num14 = (num11 <= num13) ? num13 : num11;
                        x += num14;
                        num11 = entry3.margin.right;
                    }
                    entry3.SetHorizontal(Mathf.Round(x), Mathf.Round(f));
                    x += f + this.spacing;
                }
            }
        }

        public override void SetVertical(float y, float height)
        {
            base.SetVertical(y, height);
            if (this.entries.Count != 0)
            {
                RectOffset padding = base.style.padding;
                if (this.resetCoords)
                {
                    y = 0f;
                }
                if (this.isVertical)
                {
                    if (base.style != GUIStyle.none)
                    {
                        float top = padding.top;
                        float bottom = padding.bottom;
                        if (this.entries.Count != 0)
                        {
                            top = Mathf.Max(top, (float) this.entries[0].margin.top);
                            bottom = Mathf.Max(bottom, (float) this.entries[this.entries.Count - 1].margin.bottom);
                        }
                        y += top;
                        height -= bottom + top;
                    }
                    float num3 = height - (this.spacing * (this.entries.Count - 1));
                    float t = 0f;
                    if (this.m_ChildMinHeight != this.m_ChildMaxHeight)
                    {
                        t = Mathf.Clamp((float) ((num3 - this.m_ChildMinHeight) / (this.m_ChildMaxHeight - this.m_ChildMinHeight)), (float) 0f, (float) 1f);
                    }
                    float num5 = 0f;
                    if ((num3 > this.m_ChildMaxHeight) && (this.m_StretchableCountY > 0))
                    {
                        num5 = (num3 - this.m_ChildMaxHeight) / ((float) this.m_StretchableCountY);
                    }
                    int num6 = 0;
                    bool flag = true;
                    foreach (GUILayoutEntry entry in this.entries)
                    {
                        float f = Mathf.Lerp(entry.minHeight, entry.maxHeight, t) + (num5 * entry.stretchHeight);
                        if (entry.style != GUILayoutUtility.spaceStyle)
                        {
                            int num8 = entry.margin.top;
                            if (flag)
                            {
                                num8 = 0;
                                flag = false;
                            }
                            int num9 = (num6 <= num8) ? num8 : num6;
                            y += num9;
                            num6 = entry.margin.bottom;
                        }
                        entry.SetVertical(Mathf.Round(y), Mathf.Round(f));
                        y += f + this.spacing;
                    }
                }
                else if (base.style != GUIStyle.none)
                {
                    foreach (GUILayoutEntry entry2 in this.entries)
                    {
                        float num10 = Mathf.Max(entry2.margin.top, padding.top);
                        float num11 = y + num10;
                        float num12 = (height - Mathf.Max(entry2.margin.bottom, padding.bottom)) - num10;
                        if (entry2.stretchHeight != 0)
                        {
                            entry2.SetVertical(num11, num12);
                        }
                        else
                        {
                            entry2.SetVertical(num11, Mathf.Clamp(num12, entry2.minHeight, entry2.maxHeight));
                        }
                    }
                }
                else
                {
                    float num13 = y - this.margin.top;
                    float num14 = height + this.margin.vertical;
                    foreach (GUILayoutEntry entry3 in this.entries)
                    {
                        if (entry3.stretchHeight != 0)
                        {
                            entry3.SetVertical(num13 + entry3.margin.top, num14 - entry3.margin.vertical);
                        }
                        else
                        {
                            entry3.SetVertical(num13 + entry3.margin.top, Mathf.Clamp(num14 - entry3.margin.vertical, entry3.minHeight, entry3.maxHeight));
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            string str = string.Empty;
            string str2 = string.Empty;
            for (int i = 0; i < GUILayoutEntry.indent; i++)
            {
                str2 = str2 + " ";
            }
            string str3 = str;
            object[] objArray1 = new object[] { str3, base.ToString(), " Margins: ", this.m_ChildMinHeight, " {\n" };
            str = string.Concat(objArray1);
            GUILayoutEntry.indent += 4;
            foreach (GUILayoutEntry entry in this.entries)
            {
                str = str + entry.ToString() + "\n";
            }
            str = str + str2 + "}";
            GUILayoutEntry.indent -= 4;
            return str;
        }

        public override RectOffset margin
        {
            get
            {
                return this.m_Margin;
            }
        }
    }
}

