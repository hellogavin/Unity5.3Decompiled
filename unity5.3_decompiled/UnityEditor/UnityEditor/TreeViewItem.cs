namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class TreeViewItem : IComparable<TreeViewItem>
    {
        private List<TreeViewItem> m_Children;
        private int m_Depth;
        private string m_DisplayName;
        private Texture2D m_Icon;
        private int m_ID;
        private TreeViewItem m_Parent;
        private object m_UserData;

        public TreeViewItem(int id, int depth, TreeViewItem parent, string displayName)
        {
            this.m_Depth = depth;
            this.m_Parent = parent;
            this.m_ID = id;
            this.m_DisplayName = displayName;
        }

        public void AddChild(TreeViewItem child)
        {
            if (this.m_Children == null)
            {
                this.m_Children = new List<TreeViewItem>();
            }
            this.m_Children.Add(child);
            if (child != null)
            {
                child.parent = this;
            }
        }

        public virtual int CompareTo(TreeViewItem other)
        {
            return this.displayName.CompareTo(other.displayName);
        }

        public override string ToString()
        {
            object[] args = new object[] { this.displayName, this.id, !this.hasChildren ? 0 : this.children.Count, this.depth, (this.parent == null) ? -1 : this.parent.id };
            return string.Format("Item: '{0}' ({1}), has {2} children, depth {3}, parent id {4}", args);
        }

        public virtual List<TreeViewItem> children
        {
            get
            {
                return this.m_Children;
            }
            set
            {
                this.m_Children = value;
            }
        }

        public virtual int depth
        {
            get
            {
                return this.m_Depth;
            }
            set
            {
                this.m_Depth = value;
            }
        }

        public virtual string displayName
        {
            get
            {
                return this.m_DisplayName;
            }
            set
            {
                this.m_DisplayName = value;
            }
        }

        public virtual bool hasChildren
        {
            get
            {
                return ((this.m_Children != null) && (this.m_Children.Count > 0));
            }
        }

        public virtual Texture2D icon
        {
            get
            {
                return this.m_Icon;
            }
            set
            {
                this.m_Icon = value;
            }
        }

        public virtual int id
        {
            get
            {
                return this.m_ID;
            }
            set
            {
                this.m_ID = value;
            }
        }

        public virtual TreeViewItem parent
        {
            get
            {
                return this.m_Parent;
            }
            set
            {
                this.m_Parent = value;
            }
        }

        public virtual object userData
        {
            get
            {
                return this.m_UserData;
            }
            set
            {
                this.m_UserData = value;
            }
        }
    }
}

