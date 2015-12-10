namespace UnityEngine
{
    using System;

    internal sealed class GUIScrollGroup : GUILayoutGroup
    {
        public bool allowHorizontalScroll = true;
        public bool allowVerticalScroll = true;
        public float calcMaxHeight;
        public float calcMaxWidth;
        public float calcMinHeight;
        public float calcMinWidth;
        public float clientHeight;
        public float clientWidth;
        public GUIStyle horizontalScrollbar;
        public bool needsHorizontalScrollbar;
        public bool needsVerticalScrollbar;
        public GUIStyle verticalScrollbar;

        public override void CalcHeight()
        {
            float minHeight = base.minHeight;
            float maxHeight = base.maxHeight;
            if (this.allowVerticalScroll)
            {
                base.minHeight = 0f;
                base.maxHeight = 0f;
            }
            base.CalcHeight();
            this.calcMinHeight = base.minHeight;
            this.calcMaxHeight = base.maxHeight;
            if (this.needsHorizontalScrollbar)
            {
                float num3 = this.horizontalScrollbar.fixedHeight + this.horizontalScrollbar.margin.top;
                base.minHeight += num3;
                base.maxHeight += num3;
            }
            if (this.allowVerticalScroll)
            {
                if (base.minHeight > 32f)
                {
                    base.minHeight = 32f;
                }
                if (minHeight != 0f)
                {
                    base.minHeight = minHeight;
                }
                if (maxHeight != 0f)
                {
                    base.maxHeight = maxHeight;
                    base.stretchHeight = 0;
                }
            }
        }

        public override void CalcWidth()
        {
            float minWidth = base.minWidth;
            float maxWidth = base.maxWidth;
            if (this.allowHorizontalScroll)
            {
                base.minWidth = 0f;
                base.maxWidth = 0f;
            }
            base.CalcWidth();
            this.calcMinWidth = base.minWidth;
            this.calcMaxWidth = base.maxWidth;
            if (this.allowHorizontalScroll)
            {
                if (base.minWidth > 32f)
                {
                    base.minWidth = 32f;
                }
                if (minWidth != 0f)
                {
                    base.minWidth = minWidth;
                }
                if (maxWidth != 0f)
                {
                    base.maxWidth = maxWidth;
                    base.stretchWidth = 0;
                }
            }
        }

        public override void SetHorizontal(float x, float width)
        {
            float num = !this.needsVerticalScrollbar ? width : ((width - this.verticalScrollbar.fixedWidth) - this.verticalScrollbar.margin.left);
            if (this.allowHorizontalScroll && (num < this.calcMinWidth))
            {
                this.needsHorizontalScrollbar = true;
                base.minWidth = this.calcMinWidth;
                base.maxWidth = this.calcMaxWidth;
                base.SetHorizontal(x, this.calcMinWidth);
                this.rect.width = width;
                this.clientWidth = this.calcMinWidth;
            }
            else
            {
                this.needsHorizontalScrollbar = false;
                if (this.allowHorizontalScroll)
                {
                    base.minWidth = this.calcMinWidth;
                    base.maxWidth = this.calcMaxWidth;
                }
                base.SetHorizontal(x, num);
                this.rect.width = width;
                this.clientWidth = num;
            }
        }

        public override void SetVertical(float y, float height)
        {
            float num = height;
            if (this.needsHorizontalScrollbar)
            {
                num -= this.horizontalScrollbar.fixedHeight + this.horizontalScrollbar.margin.top;
            }
            if (this.allowVerticalScroll && (num < this.calcMinHeight))
            {
                if (!this.needsHorizontalScrollbar && !this.needsVerticalScrollbar)
                {
                    this.clientWidth = (this.rect.width - this.verticalScrollbar.fixedWidth) - this.verticalScrollbar.margin.left;
                    if (this.clientWidth < this.calcMinWidth)
                    {
                        this.clientWidth = this.calcMinWidth;
                    }
                    float width = this.rect.width;
                    this.SetHorizontal(this.rect.x, this.clientWidth);
                    this.CalcHeight();
                    this.rect.width = width;
                }
                float minHeight = base.minHeight;
                float maxHeight = base.maxHeight;
                base.minHeight = this.calcMinHeight;
                base.maxHeight = this.calcMaxHeight;
                base.SetVertical(y, this.calcMinHeight);
                base.minHeight = minHeight;
                base.maxHeight = maxHeight;
                this.rect.height = height;
                this.clientHeight = this.calcMinHeight;
            }
            else
            {
                if (this.allowVerticalScroll)
                {
                    base.minHeight = this.calcMinHeight;
                    base.maxHeight = this.calcMaxHeight;
                }
                base.SetVertical(y, num);
                this.rect.height = height;
                this.clientHeight = num;
            }
        }
    }
}

