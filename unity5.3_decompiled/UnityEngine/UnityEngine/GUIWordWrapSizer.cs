namespace UnityEngine
{
    using System;

    internal sealed class GUIWordWrapSizer : GUILayoutEntry
    {
        private readonly GUIContent m_Content;
        private readonly float m_ForcedMaxHeight;
        private readonly float m_ForcedMinHeight;

        public GUIWordWrapSizer(GUIStyle style, GUIContent content, GUILayoutOption[] options) : base(0f, 0f, 0f, 0f, style)
        {
            this.m_Content = new GUIContent(content);
            this.ApplyOptions(options);
            this.m_ForcedMinHeight = base.minHeight;
            this.m_ForcedMaxHeight = base.maxHeight;
        }

        public override void CalcHeight()
        {
            if ((this.m_ForcedMinHeight == 0f) || (this.m_ForcedMaxHeight == 0f))
            {
                float num = base.style.CalcHeight(this.m_Content, this.rect.width);
                if (this.m_ForcedMinHeight == 0f)
                {
                    base.minHeight = num;
                }
                else
                {
                    base.minHeight = this.m_ForcedMinHeight;
                }
                if (this.m_ForcedMaxHeight == 0f)
                {
                    base.maxHeight = num;
                }
                else
                {
                    base.maxHeight = this.m_ForcedMaxHeight;
                }
            }
        }

        public override void CalcWidth()
        {
            if ((base.minWidth == 0f) || (base.maxWidth == 0f))
            {
                float num;
                float num2;
                base.style.CalcMinMaxWidth(this.m_Content, out num, out num2);
                if (base.minWidth == 0f)
                {
                    base.minWidth = num;
                }
                if (base.maxWidth == 0f)
                {
                    base.maxWidth = num2;
                }
            }
        }
    }
}

