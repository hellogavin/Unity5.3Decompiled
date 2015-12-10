namespace UnityEngine
{
    using System;

    internal class GUILayoutEntry
    {
        protected static int indent = 0;
        internal static Rect kDummyRect = new Rect(0f, 0f, 1f, 1f);
        private GUIStyle m_Style;
        public float maxHeight;
        public float maxWidth;
        public float minHeight;
        public float minWidth;
        public Rect rect;
        public int stretchHeight;
        public int stretchWidth;

        public GUILayoutEntry(float _minWidth, float _maxWidth, float _minHeight, float _maxHeight, GUIStyle _style)
        {
            this.rect = new Rect(0f, 0f, 0f, 0f);
            this.m_Style = GUIStyle.none;
            this.minWidth = _minWidth;
            this.maxWidth = _maxWidth;
            this.minHeight = _minHeight;
            this.maxHeight = _maxHeight;
            if (_style == null)
            {
                _style = GUIStyle.none;
            }
            this.style = _style;
        }

        public GUILayoutEntry(float _minWidth, float _maxWidth, float _minHeight, float _maxHeight, GUIStyle _style, GUILayoutOption[] options)
        {
            this.rect = new Rect(0f, 0f, 0f, 0f);
            this.m_Style = GUIStyle.none;
            this.minWidth = _minWidth;
            this.maxWidth = _maxWidth;
            this.minHeight = _minHeight;
            this.maxHeight = _maxHeight;
            this.style = _style;
            this.ApplyOptions(options);
        }

        public virtual void ApplyOptions(GUILayoutOption[] options)
        {
            if (options != null)
            {
                foreach (GUILayoutOption option in options)
                {
                    switch (option.type)
                    {
                        case GUILayoutOption.Type.fixedWidth:
                            this.minWidth = this.maxWidth = (float) option.value;
                            this.stretchWidth = 0;
                            break;

                        case GUILayoutOption.Type.fixedHeight:
                            this.minHeight = this.maxHeight = (float) option.value;
                            this.stretchHeight = 0;
                            break;

                        case GUILayoutOption.Type.minWidth:
                            this.minWidth = (float) option.value;
                            if (this.maxWidth < this.minWidth)
                            {
                                this.maxWidth = this.minWidth;
                            }
                            break;

                        case GUILayoutOption.Type.maxWidth:
                            this.maxWidth = (float) option.value;
                            if (this.minWidth > this.maxWidth)
                            {
                                this.minWidth = this.maxWidth;
                            }
                            this.stretchWidth = 0;
                            break;

                        case GUILayoutOption.Type.minHeight:
                            this.minHeight = (float) option.value;
                            if (this.maxHeight < this.minHeight)
                            {
                                this.maxHeight = this.minHeight;
                            }
                            break;

                        case GUILayoutOption.Type.maxHeight:
                            this.maxHeight = (float) option.value;
                            if (this.minHeight > this.maxHeight)
                            {
                                this.minHeight = this.maxHeight;
                            }
                            this.stretchHeight = 0;
                            break;

                        case GUILayoutOption.Type.stretchWidth:
                            this.stretchWidth = (int) option.value;
                            break;

                        case GUILayoutOption.Type.stretchHeight:
                            this.stretchHeight = (int) option.value;
                            break;
                    }
                }
                if ((this.maxWidth != 0f) && (this.maxWidth < this.minWidth))
                {
                    this.maxWidth = this.minWidth;
                }
                if ((this.maxHeight != 0f) && (this.maxHeight < this.minHeight))
                {
                    this.maxHeight = this.minHeight;
                }
            }
        }

        protected virtual void ApplyStyleSettings(GUIStyle style)
        {
            this.stretchWidth = ((style.fixedWidth != 0f) || !style.stretchWidth) ? 0 : 1;
            this.stretchHeight = ((style.fixedHeight != 0f) || !style.stretchHeight) ? 0 : 1;
            this.m_Style = style;
        }

        public virtual void CalcHeight()
        {
        }

        public virtual void CalcWidth()
        {
        }

        public virtual void SetHorizontal(float x, float width)
        {
            this.rect.x = x;
            this.rect.width = width;
        }

        public virtual void SetVertical(float y, float height)
        {
            this.rect.y = y;
            this.rect.height = height;
        }

        public override string ToString()
        {
            string str = string.Empty;
            for (int i = 0; i < indent; i++)
            {
                str = str + " ";
            }
            object[] objArray1 = new object[12];
            objArray1[0] = str;
            object[] args = new object[] { (this.style == null) ? "NULL" : this.style.name, base.GetType(), this.rect.x, this.rect.xMax, this.rect.y, this.rect.yMax };
            objArray1[1] = UnityString.Format("{1}-{0} (x:{2}-{3}, y:{4}-{5})", args);
            objArray1[2] = "   -   W: ";
            objArray1[3] = this.minWidth;
            objArray1[4] = "-";
            objArray1[5] = this.maxWidth;
            objArray1[6] = (this.stretchWidth == 0) ? string.Empty : "+";
            objArray1[7] = ", H: ";
            objArray1[8] = this.minHeight;
            objArray1[9] = "-";
            objArray1[10] = this.maxHeight;
            objArray1[11] = (this.stretchHeight == 0) ? string.Empty : "+";
            return string.Concat(objArray1);
        }

        public virtual RectOffset margin
        {
            get
            {
                return this.style.margin;
            }
        }

        public GUIStyle style
        {
            get
            {
                return this.m_Style;
            }
            set
            {
                this.m_Style = value;
                this.ApplyStyleSettings(value);
            }
        }
    }
}

