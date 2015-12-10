namespace UnityEditorInternal
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    internal class AddCurvesPopupHierarchyGUI : TreeViewGUI
    {
        public EditorWindow owner;
        private GUIStyle plusButtonBackgroundStyle;
        private GUIStyle plusButtonStyle;
        private const float plusButtonWidth = 17f;

        public AddCurvesPopupHierarchyGUI(TreeView treeView, AnimationWindowState state, EditorWindow owner) : base(treeView, true)
        {
            this.plusButtonStyle = new GUIStyle("OL Plus");
            this.plusButtonBackgroundStyle = new GUIStyle("Tag MenuItem");
            this.owner = owner;
            this.state = state;
        }

        public override bool BeginRename(TreeViewItem item, float delay)
        {
            return false;
        }

        protected override Texture GetIconForNode(TreeViewItem item)
        {
            if (item != null)
            {
                return item.icon;
            }
            return null;
        }

        protected override bool IsRenaming(int id)
        {
            return false;
        }

        public override void OnRowGUI(Rect rowRect, TreeViewItem node, int row, bool selected, bool focused)
        {
            base.OnRowGUI(rowRect, node, row, selected, focused);
            AddCurvesPopupPropertyNode node2 = node as AddCurvesPopupPropertyNode;
            if (((node2 != null) && (node2.curveBindings != null)) && (node2.curveBindings.Length != 0))
            {
                Rect position = new Rect(rowRect.width - 17f, rowRect.yMin, 17f, this.plusButtonStyle.fixedHeight);
                GUI.Box(position, GUIContent.none, this.plusButtonBackgroundStyle);
                if (GUI.Button(position, GUIContent.none, this.plusButtonStyle))
                {
                    AddCurvesPopup.AddNewCurve(node2);
                    this.owner.Close();
                }
            }
        }

        protected override void RenameEnded()
        {
        }

        protected override void SyncFakeItem()
        {
        }

        public bool showPlusButton { get; set; }

        public AnimationWindowState state { get; set; }
    }
}

