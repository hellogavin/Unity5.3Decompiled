namespace UnityEditor
{
    using System;
    using System.Collections.Generic;

    internal class SketchUpNode : TreeViewItem
    {
        public SketchUpNodeInfo Info;

        public SketchUpNode(int id, int depth, TreeViewItem parent, string displayName, SketchUpNodeInfo info) : base(id, depth, parent, displayName)
        {
            this.Info = info;
            this.children = new List<TreeViewItem>();
        }

        private void ToggleChildren(bool toggle)
        {
            foreach (TreeViewItem item in this.children)
            {
                SketchUpNode node = item as SketchUpNode;
                node.Info.enabled = toggle;
                node.ToggleChildren(toggle);
            }
        }

        private void ToggleParent(bool toggle)
        {
            SketchUpNode parent = this.parent as SketchUpNode;
            if (parent != null)
            {
                parent.ToggleParent(toggle);
                parent.Info.enabled = toggle;
            }
        }

        public bool Enabled
        {
            get
            {
                return this.Info.enabled;
            }
            set
            {
                if (this.Info.enabled != value)
                {
                    if (value)
                    {
                        this.ToggleParent(value);
                    }
                    this.ToggleChildren(value);
                    this.Info.enabled = value;
                }
            }
        }
    }
}

