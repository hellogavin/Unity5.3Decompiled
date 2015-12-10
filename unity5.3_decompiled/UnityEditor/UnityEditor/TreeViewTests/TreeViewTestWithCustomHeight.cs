namespace UnityEditor.TreeViewTests
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal class TreeViewTestWithCustomHeight
    {
        private BackendData m_BackendData;
        private TreeView m_TreeView;

        public TreeViewTestWithCustomHeight(EditorWindow editorWindow, BackendData backendData, Rect rect)
        {
            TestDataSource source;
            this.m_BackendData = backendData;
            TreeViewState treeViewState = new TreeViewState();
            this.m_TreeView = new TreeView(editorWindow, treeViewState);
            TestGUICustomItemHeights gui = new TestGUICustomItemHeights(this.m_TreeView);
            TestDragging dragging = new TestDragging(this.m_TreeView, this.m_BackendData);
            source = new TestDataSource(this.m_TreeView, this.m_BackendData) {
                onVisibleRowsChanged = (Action) Delegate.Combine(source.onVisibleRowsChanged, new Action(gui.CalculateRowRects))
            };
            this.m_TreeView.Init(rect, source, gui, dragging);
            source.SetExpanded(source.root, true);
        }

        public void OnGUI(Rect rect)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Keyboard, rect);
            this.m_TreeView.OnGUI(rect, controlID);
        }
    }
}

