namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class TreeViewAnimationInput
    {
        public Action<TreeViewAnimationInput> animationEnded;

        public void FireAnimationEndedEvent()
        {
            if (this.animationEnded != null)
            {
                this.animationEnded(this);
            }
        }

        public override string ToString()
        {
            object[] objArray1 = new object[] { "Input: startRow ", this.startRow, " endRow ", this.endRow, " rowsRect ", this.rowsRect, " startTime ", this.startTime, " anitmationDuration", this.animationDuration, " ", this.expanding, " ", this.item.displayName };
            return string.Concat(objArray1);
        }

        public double animationDuration { get; set; }

        public double elapsedTime
        {
            get
            {
                return (EditorApplication.timeSinceStartup - this.startTime);
            }
            set
            {
                this.startTime = EditorApplication.timeSinceStartup - value;
            }
        }

        public float elapsedTimeNormalized
        {
            get
            {
                return Mathf.Clamp01(((float) this.elapsedTime) / ((float) this.animationDuration));
            }
        }

        public int endRow { get; set; }

        public bool expanding { get; set; }

        public TreeViewItem item { get; set; }

        public Rect rowsRect { get; set; }

        public int startRow { get; set; }

        public Rect startRowRect { get; set; }

        public double startTime { get; set; }

        public TreeView treeView { get; set; }
    }
}

