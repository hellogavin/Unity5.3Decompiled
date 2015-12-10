namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class SketchUpTreeViewGUI : TreeViewGUI
    {
        private readonly Texture2D k_Icon;
        private readonly Texture2D k_Root;

        public SketchUpTreeViewGUI(TreeView treeView) : base(treeView)
        {
            this.k_Root = EditorGUIUtility.FindTexture("DefaultAsset Icon");
            this.k_Icon = EditorGUIUtility.FindTexture("Mesh Icon");
            base.k_BaseIndent = 20f;
        }

        protected override Texture GetIconForNode(TreeViewItem item)
        {
            return (((item.children == null) || (item.children.Count <= 0)) ? this.k_Icon : this.k_Root);
        }

        public override void OnRowGUI(Rect rowRect, TreeViewItem node, int row, bool selected, bool focused)
        {
            this.DoNodeGUI(rowRect, row, node, selected, focused, false);
            SketchUpNode node2 = node as SketchUpNode;
            Rect position = new Rect(2f, rowRect.y, rowRect.height, rowRect.height);
            node2.Enabled = GUI.Toggle(position, node2.Enabled, GUIContent.none, SketchUpImportDlg.Styles.styles.toggleStyle);
        }

        protected override void RenameEnded()
        {
        }

        protected override void SyncFakeItem()
        {
        }
    }
}

