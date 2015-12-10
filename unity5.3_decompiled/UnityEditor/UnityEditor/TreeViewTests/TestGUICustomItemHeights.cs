namespace UnityEditor.TreeViewTests
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal class TestGUICustomItemHeights : TreeViewGUIWithCustomItemsHeights
    {
        private float m_Column1Width;
        protected Rect m_DraggingInsertionMarkerRect;

        public TestGUICustomItemHeights(TreeView treeView) : base(treeView)
        {
            this.m_Column1Width = 300f;
        }

        public override void BeginRowGUI()
        {
            this.m_DraggingInsertionMarkerRect.x = -1f;
        }

        public override void EndRowGUI()
        {
            base.EndRowGUI();
            if ((this.m_DraggingInsertionMarkerRect.x >= 0f) && (Event.current.type == EventType.Repaint))
            {
                Rect draggingInsertionMarkerRect = this.m_DraggingInsertionMarkerRect;
                draggingInsertionMarkerRect.height = 2f;
                draggingInsertionMarkerRect.y -= draggingInsertionMarkerRect.height / 2f;
                if (!base.m_TreeView.dragging.drawRowMarkerAbove)
                {
                    draggingInsertionMarkerRect.y += this.m_DraggingInsertionMarkerRect.height;
                }
                EditorGUI.DrawRect(draggingInsertionMarkerRect, Color.white);
            }
        }

        protected override Vector2 GetSizeOfRow(TreeViewItem item)
        {
            return new Vector2(base.m_TreeView.GetTotalRect().width, !item.hasChildren ? 36f : 20f);
        }

        public override void OnRowGUI(Rect rowRect, TreeViewItem item, int row, bool selected, bool focused)
        {
            rowRect.height--;
            Rect rect = rowRect;
            Rect rect2 = rowRect;
            rect.width = this.m_Column1Width;
            rect.xMin += this.GetFoldoutIndent(item);
            rect2.xMin += this.m_Column1Width + 1f;
            float foldoutIndent = this.GetFoldoutIndent(item);
            Rect position = rowRect;
            int itemControlID = TreeView.GetItemControlID(item);
            bool flag = false;
            if (base.m_TreeView.dragging != null)
            {
                flag = (base.m_TreeView.dragging.GetDropTargetControlID() == itemControlID) && base.m_TreeView.data.CanBeParent(item);
            }
            bool flag2 = base.m_TreeView.data.IsExpandable(item);
            Color color = new Color(0f, 0.22f, 0.44f);
            Color color2 = new Color(0.1f, 0.1f, 0.1f);
            EditorGUI.DrawRect(rect, !selected ? color2 : color);
            EditorGUI.DrawRect(rect2, !selected ? color2 : color);
            if (flag)
            {
                EditorGUI.DrawRect(new Rect(rowRect.x, rowRect.y, 3f, rowRect.height), Color.yellow);
            }
            if (Event.current.type == EventType.Repaint)
            {
                Rect rect4 = rect;
                rect4.xMin += base.m_FoldoutWidth;
                GUI.Label(rect4, item.displayName, EditorStyles.largeLabel);
                if (rowRect.height > 20f)
                {
                    rect4.y += 16f;
                    GUI.Label(rect4, "Ut tincidunt tortor. Donec nonummy, enim in lacinia pulvinar", EditorStyles.miniLabel);
                }
                if ((base.m_TreeView.dragging != null) && (base.m_TreeView.dragging.GetRowMarkerControlID() == itemControlID))
                {
                    this.m_DraggingInsertionMarkerRect = new Rect(rowRect.x + foldoutIndent, rowRect.y, rowRect.width - foldoutIndent, rowRect.height);
                }
            }
            if (flag2)
            {
                position.x = foldoutIndent;
                position.width = base.m_FoldoutWidth;
                EditorGUI.BeginChangeCheck();
                bool expand = GUI.Toggle(position, base.m_TreeView.data.IsExpanded(item), GUIContent.none, Styles.foldout);
                if (EditorGUI.EndChangeCheck())
                {
                    base.m_TreeView.UserInputChangedExpandedState(item, row, expand);
                }
            }
        }

        internal class Styles
        {
            public static GUIStyle foldout = "IN Foldout";
        }
    }
}

