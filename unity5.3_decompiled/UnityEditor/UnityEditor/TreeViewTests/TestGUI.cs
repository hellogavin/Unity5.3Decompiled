namespace UnityEditor.TreeViewTests
{
    using System;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    internal class TestGUI : TreeViewGUI
    {
        private Texture2D m_FolderIcon;
        private Texture2D m_Icon;
        private GUIStyle m_LabelStyle;
        private GUIStyle m_LabelStyleRightAlign;

        public TestGUI(TreeView treeView) : base(treeView)
        {
            this.m_FolderIcon = EditorGUIUtility.FindTexture(EditorResourcesUtility.folderIconName);
            this.m_Icon = EditorGUIUtility.FindTexture("boo Script Icon");
        }

        protected override void DrawIconAndLabel(Rect rect, TreeViewItem item, string label, bool selected, bool focused, bool useBoldFont, bool isPinging)
        {
            if (this.m_LabelStyle == null)
            {
                this.m_LabelStyle = new GUIStyle(TreeViewGUI.s_Styles.lineStyle);
                int num2 = 6;
                this.m_LabelStyle.padding.right = num2;
                this.m_LabelStyle.padding.left = num2;
                this.m_LabelStyleRightAlign = new GUIStyle(TreeViewGUI.s_Styles.lineStyle);
                num2 = 6;
                this.m_LabelStyleRightAlign.padding.left = num2;
                this.m_LabelStyleRightAlign.padding.right = num2;
                this.m_LabelStyleRightAlign.alignment = TextAnchor.MiddleRight;
            }
            if ((isPinging || (this.columnWidths == null)) || (this.columnWidths.Length == 0))
            {
                base.DrawIconAndLabel(rect, item, label, selected, focused, useBoldFont, isPinging);
            }
            else
            {
                Rect rect2 = rect;
                for (int i = 0; i < this.columnWidths.Length; i++)
                {
                    rect2.width = this.columnWidths[i];
                    if (i == 0)
                    {
                        base.DrawIconAndLabel(rect2, item, label, selected, focused, useBoldFont, isPinging);
                    }
                    else
                    {
                        GUI.Label(rect2, "Zksdf SDFS DFASDF ", ((i % 2) != 0) ? this.m_LabelStyleRightAlign : this.m_LabelStyle);
                    }
                    rect2.x += rect2.width;
                }
            }
        }

        protected override Texture GetIconForNode(TreeViewItem item)
        {
            return (!item.hasChildren ? this.m_Icon : this.m_FolderIcon);
        }

        protected override void RenameEnded()
        {
        }

        protected override void SyncFakeItem()
        {
        }

        private float[] columnWidths
        {
            get
            {
                return base.m_TreeView.state.columnWidths;
            }
        }
    }
}

