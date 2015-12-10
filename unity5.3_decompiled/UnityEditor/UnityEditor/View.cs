namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    internal class View : ScriptableObject
    {
        [SerializeField]
        private MonoReloadableIntPtr m_ViewPtr;
        [SerializeField]
        private View[] m_Children = new View[0];
        [NonSerialized]
        private View m_Parent;
        [NonSerialized]
        private ContainerWindow m_Window;
        [SerializeField]
        private Rect m_Position = new Rect(0f, 0f, 100f, 100f);
        [SerializeField]
        internal Vector2 m_MinSize;
        [SerializeField]
        internal Vector2 m_MaxSize;
        public View()
        {
            base.hideFlags = HideFlags.DontSave;
        }

        internal virtual void Reflow()
        {
            foreach (View view in this.children)
            {
                view.Reflow();
            }
        }

        internal string DebugHierarchy(int level)
        {
            string str = string.Empty;
            string str2 = string.Empty;
            for (int i = 0; i < level; i++)
            {
                str = str + "  ";
            }
            object[] objArray1 = new object[] { str2, str, this.ToString(), " p:", this.position };
            str2 = string.Concat(objArray1);
            if (this.children.Length > 0)
            {
                str2 = str2 + " {\n";
                foreach (View view in this.children)
                {
                    str2 = str2 + view.DebugHierarchy(level + 2);
                }
                return (str2 + str + " }\n");
            }
            return (str2 + "\n");
        }

        internal virtual void Initialize(ContainerWindow win)
        {
            this.SetWindow(win);
            foreach (View view in this.m_Children)
            {
                view.m_Parent = this;
                view.Initialize(win);
            }
        }

        public Vector2 minSize
        {
            get
            {
                return this.m_MinSize;
            }
        }
        public Vector2 maxSize
        {
            get
            {
                return this.m_MaxSize;
            }
        }
        internal void SetMinMaxSizes(Vector2 min, Vector2 max)
        {
            if ((this.minSize != min) || (this.maxSize != max))
            {
                this.m_MinSize = min;
                this.m_MaxSize = max;
                if (this.m_Parent != null)
                {
                    this.m_Parent.ChildrenMinMaxChanged();
                }
                if ((this.window != null) && (this.window.mainView == this))
                {
                    this.window.SetMinMaxSizes(min, max);
                }
            }
        }

        protected virtual void ChildrenMinMaxChanged()
        {
        }

        public View[] allChildren
        {
            get
            {
                ArrayList list = new ArrayList();
                foreach (View view in this.m_Children)
                {
                    list.AddRange(view.allChildren);
                }
                list.Add(this);
                return (View[]) list.ToArray(typeof(View));
            }
        }
        public Rect position
        {
            get
            {
                return this.m_Position;
            }
            set
            {
                this.SetPosition(value);
            }
        }
        protected virtual void SetPosition(Rect newPos)
        {
            this.m_Position = newPos;
        }

        internal void SetPositionOnly(Rect newPos)
        {
            this.m_Position = newPos;
        }

        public Rect windowPosition
        {
            get
            {
                if (this.m_Parent == null)
                {
                    return this.position;
                }
                Rect windowPosition = this.parent.windowPosition;
                return new Rect(windowPosition.x + this.position.x, windowPosition.y + this.position.y, this.position.width, this.position.height);
            }
        }
        public Rect screenPosition
        {
            get
            {
                Rect windowPosition = this.windowPosition;
                if (this.window != null)
                {
                    Vector2 vector = this.window.WindowToScreenPoint(Vector2.zero);
                    windowPosition.x += vector.x;
                    windowPosition.y += vector.y;
                }
                return windowPosition;
            }
        }
        public ContainerWindow window
        {
            get
            {
                return this.m_Window;
            }
        }
        public View parent
        {
            get
            {
                return this.m_Parent;
            }
        }
        public View[] children
        {
            get
            {
                return this.m_Children;
            }
        }
        public int IndexOfChild(View child)
        {
            int num = 0;
            foreach (View view in this.m_Children)
            {
                if (view == child)
                {
                    return num;
                }
                num++;
            }
            return -1;
        }

        public void OnDestroy()
        {
            foreach (View view in this.m_Children)
            {
                Object.DestroyImmediate(view, true);
            }
        }

        public void AddChild(View child)
        {
            this.AddChild(child, this.m_Children.Length);
        }

        public virtual void AddChild(View child, int idx)
        {
            Array.Resize<View>(ref this.m_Children, this.m_Children.Length + 1);
            if (idx != (this.m_Children.Length - 1))
            {
                Array.Copy(this.m_Children, idx, this.m_Children, idx + 1, (this.m_Children.Length - idx) - 1);
            }
            this.m_Children[idx] = child;
            if (child.m_Parent != null)
            {
                child.m_Parent.RemoveChild(child);
            }
            child.m_Parent = this;
            child.SetWindowRecurse(this.window);
            this.ChildrenMinMaxChanged();
        }

        public virtual void RemoveChild(View child)
        {
            int index = Array.IndexOf<View>(this.m_Children, child);
            if (index == -1)
            {
                Debug.LogError("Unable to remove child - it's not IN the view");
            }
            else
            {
                this.RemoveChild(index);
            }
        }

        public virtual void RemoveChild(int idx)
        {
            View view = this.m_Children[idx];
            view.m_Parent = null;
            view.SetWindowRecurse(null);
            Array.Copy(this.m_Children, idx + 1, this.m_Children, idx, (this.m_Children.Length - idx) - 1);
            Array.Resize<View>(ref this.m_Children, this.m_Children.Length - 1);
            this.ChildrenMinMaxChanged();
        }

        protected virtual void SetWindow(ContainerWindow win)
        {
            this.m_Window = win;
        }

        internal void SetWindowRecurse(ContainerWindow win)
        {
            this.SetWindow(win);
            foreach (View view in this.m_Children)
            {
                view.SetWindowRecurse(win);
            }
        }

        protected virtual bool OnFocus()
        {
            return true;
        }
    }
}

