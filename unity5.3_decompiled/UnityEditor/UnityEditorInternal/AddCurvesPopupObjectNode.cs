namespace UnityEditorInternal
{
    using System;
    using UnityEditor;

    internal class AddCurvesPopupObjectNode : TreeViewItem
    {
        public AddCurvesPopupObjectNode(TreeViewItem parent, string path, string className) : base((path + className).GetHashCode(), parent.depth + 1, parent, className)
        {
        }
    }
}

