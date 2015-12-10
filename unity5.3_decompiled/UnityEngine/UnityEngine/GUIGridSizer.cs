namespace UnityEngine
{
    using System;

    internal sealed class GUIGridSizer : GUILayoutEntry
    {
        private readonly int m_Count;
        private readonly float m_MaxButtonHeight;
        private readonly float m_MaxButtonWidth;
        private readonly float m_MinButtonHeight;
        private readonly float m_MinButtonWidth;
        private readonly int m_XCount;

        private GUIGridSizer(GUIContent[] contents, int xCount, GUIStyle buttonStyle, GUILayoutOption[] options) : base(0f, 0f, 0f, 0f, GUIStyle.none)
        {
            this.m_MinButtonWidth = -1f;
            this.m_MaxButtonWidth = -1f;
            this.m_MinButtonHeight = -1f;
            this.m_MaxButtonHeight = -1f;
            this.m_Count = contents.Length;
            this.m_XCount = xCount;
            this.ApplyStyleSettings(buttonStyle);
            this.ApplyOptions(options);
            if ((xCount != 0) && (contents.Length != 0))
            {
                float num = Mathf.Max(buttonStyle.margin.left, buttonStyle.margin.right) * (this.m_XCount - 1);
                float num2 = Mathf.Max(buttonStyle.margin.top, buttonStyle.margin.bottom) * (this.rows - 1);
                if (buttonStyle.fixedWidth != 0f)
                {
                    this.m_MinButtonWidth = this.m_MaxButtonWidth = buttonStyle.fixedWidth;
                }
                if (buttonStyle.fixedHeight != 0f)
                {
                    this.m_MinButtonHeight = this.m_MaxButtonHeight = buttonStyle.fixedHeight;
                }
                if (this.m_MinButtonWidth == -1f)
                {
                    if (base.minWidth != 0f)
                    {
                        this.m_MinButtonWidth = (base.minWidth - num) / ((float) this.m_XCount);
                    }
                    if (base.maxWidth != 0f)
                    {
                        this.m_MaxButtonWidth = (base.maxWidth - num) / ((float) this.m_XCount);
                    }
                }
                if (this.m_MinButtonHeight == -1f)
                {
                    if (base.minHeight != 0f)
                    {
                        this.m_MinButtonHeight = (base.minHeight - num2) / ((float) this.rows);
                    }
                    if (base.maxHeight != 0f)
                    {
                        this.m_MaxButtonHeight = (base.maxHeight - num2) / ((float) this.rows);
                    }
                }
                if (((this.m_MinButtonHeight == -1f) || (this.m_MaxButtonHeight == -1f)) || ((this.m_MinButtonWidth == -1f) || (this.m_MaxButtonWidth == -1f)))
                {
                    float a = 0f;
                    float num4 = 0f;
                    foreach (GUIContent content in contents)
                    {
                        Vector2 vector = buttonStyle.CalcSize(content);
                        num4 = Mathf.Max(num4, vector.x);
                        a = Mathf.Max(a, vector.y);
                    }
                    if (this.m_MinButtonWidth == -1f)
                    {
                        if (this.m_MaxButtonWidth != -1f)
                        {
                            this.m_MinButtonWidth = Mathf.Min(num4, this.m_MaxButtonWidth);
                        }
                        else
                        {
                            this.m_MinButtonWidth = num4;
                        }
                    }
                    if (this.m_MaxButtonWidth == -1f)
                    {
                        if (this.m_MinButtonWidth != -1f)
                        {
                            this.m_MaxButtonWidth = Mathf.Max(num4, this.m_MinButtonWidth);
                        }
                        else
                        {
                            this.m_MaxButtonWidth = num4;
                        }
                    }
                    if (this.m_MinButtonHeight == -1f)
                    {
                        if (this.m_MaxButtonHeight != -1f)
                        {
                            this.m_MinButtonHeight = Mathf.Min(a, this.m_MaxButtonHeight);
                        }
                        else
                        {
                            this.m_MinButtonHeight = a;
                        }
                    }
                    if (this.m_MaxButtonHeight == -1f)
                    {
                        if (this.m_MinButtonHeight != -1f)
                        {
                            base.maxHeight = Mathf.Max(base.maxHeight, this.m_MinButtonHeight);
                        }
                        this.m_MaxButtonHeight = base.maxHeight;
                    }
                }
                base.minWidth = (this.m_MinButtonWidth * this.m_XCount) + num;
                base.maxWidth = (this.m_MaxButtonWidth * this.m_XCount) + num;
                base.minHeight = (this.m_MinButtonHeight * this.rows) + num2;
                base.maxHeight = (this.m_MaxButtonHeight * this.rows) + num2;
            }
        }

        public static Rect GetRect(GUIContent[] contents, int xCount, GUIStyle style, GUILayoutOption[] options)
        {
            Rect rect = new Rect(0f, 0f, 0f, 0f);
            switch (Event.current.type)
            {
                case EventType.Layout:
                    GUILayoutUtility.current.topLevel.Add(new GUIGridSizer(contents, xCount, style, options));
                    return rect;

                case EventType.Used:
                    return GUILayoutEntry.kDummyRect;
            }
            return GUILayoutUtility.current.topLevel.GetNext().rect;
        }

        private int rows
        {
            get
            {
                int num = this.m_Count / this.m_XCount;
                if ((this.m_Count % this.m_XCount) != 0)
                {
                    num++;
                }
                return num;
            }
        }
    }
}

