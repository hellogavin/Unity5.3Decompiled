namespace UnityEditor.TreeViewTests
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal class TreeViewTest
    {
        private BackendData m_BackendData;
        private TreeViewColumnHeader m_ColumnHeader;
        private EditorWindow m_EditorWindow;
        private GUIStyle m_HeaderStyle;
        private GUIStyle m_HeaderStyleRightAligned;
        private bool m_Lazy;
        private TreeView m_TreeView;

        public TreeViewTest(EditorWindow editorWindow, bool lazy)
        {
            this.m_EditorWindow = editorWindow;
            this.m_Lazy = lazy;
        }

        private string GetHeader()
        {
            object[] objArray1 = new object[] { !this.m_Lazy ? "FULL: " : "LAZY: ", "GUI items: ", this.GetNumItemsInTree(), "  (data items: ", this.GetNumItemsInData(), ")" };
            return string.Concat(objArray1);
        }

        public int GetNumItemsInData()
        {
            return this.m_BackendData.IDCounter;
        }

        public int GetNumItemsInTree()
        {
            LazyTestDataSource data = this.m_TreeView.data as LazyTestDataSource;
            if (data != null)
            {
                return data.itemCounter;
            }
            TestDataSource source2 = this.m_TreeView.data as TestDataSource;
            if (source2 != null)
            {
                return source2.itemCounter;
            }
            return -1;
        }

        public void Init(Rect rect, BackendData backendData)
        {
            if (this.m_TreeView == null)
            {
                ITreeViewDataSource source;
                this.m_BackendData = backendData;
                TreeViewState treeViewState = new TreeViewState {
                    columnWidths = new float[] { 250f, 90f, 93f, 98f, 74f, 78f }
                };
                this.m_TreeView = new TreeView(this.m_EditorWindow, treeViewState);
                ITreeViewGUI gui = new TestGUI(this.m_TreeView);
                ITreeViewDragging dragging = new TestDragging(this.m_TreeView, this.m_BackendData);
                if (this.m_Lazy)
                {
                    source = new LazyTestDataSource(this.m_TreeView, this.m_BackendData);
                }
                else
                {
                    source = new TestDataSource(this.m_TreeView, this.m_BackendData);
                }
                this.m_TreeView.Init(rect, source, gui, dragging);
                this.m_ColumnHeader = new TreeViewColumnHeader();
                this.m_ColumnHeader.columnWidths = treeViewState.columnWidths;
                this.m_ColumnHeader.minColumnWidth = 30f;
                this.m_ColumnHeader.columnRenderer = (Action<int, Rect>) Delegate.Combine(this.m_ColumnHeader.columnRenderer, new Action<int, Rect>(this.OnColumnRenderer));
            }
        }

        private void OnColumnRenderer(int column, Rect rect)
        {
            if (this.m_HeaderStyle == null)
            {
                this.m_HeaderStyle = new GUIStyle(EditorStyles.toolbarButton);
                this.m_HeaderStyle.padding.left = 4;
                this.m_HeaderStyle.alignment = TextAnchor.MiddleLeft;
                this.m_HeaderStyleRightAligned = new GUIStyle(EditorStyles.toolbarButton);
                this.m_HeaderStyleRightAligned.padding.right = 4;
                this.m_HeaderStyleRightAligned.alignment = TextAnchor.MiddleRight;
            }
            string[] strArray = new string[] { "Name", "Date Modified", "Size", "Kind", "Author", "Platform", "Faster", "Slower" };
            GUI.Label(rect, strArray[column], ((column % 2) != 0) ? this.m_HeaderStyleRightAligned : this.m_HeaderStyle);
        }

        public void OnGUI(Rect rect)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Keyboard, rect);
            Rect position = new Rect(rect.x, rect.y, rect.width, 17f);
            Rect screenRect = new Rect(rect.x, rect.yMax - 20f, rect.width, 20f);
            GUI.Label(position, string.Empty, EditorStyles.toolbar);
            this.m_ColumnHeader.OnGUI(position);
            Profiler.BeginSample("TREEVIEW");
            rect.y += position.height;
            rect.height -= position.height + screenRect.height;
            this.m_TreeView.OnEvent();
            this.m_TreeView.OnGUI(rect, controlID);
            Profiler.EndSample();
            GUILayout.BeginArea(screenRect, this.GetHeader(), EditorStyles.helpBox);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            this.m_BackendData.m_RecursiveFindParentsBelow = GUILayout.Toggle(this.m_BackendData.m_RecursiveFindParentsBelow, GUIContent.Temp("Recursive"), new GUILayoutOption[0]);
            if (GUILayout.Button("Ping", EditorStyles.miniButton, new GUILayoutOption[0]))
            {
                int id = this.GetNumItemsInData() / 2;
                this.m_TreeView.Frame(id, true, true);
                int[] selectedIDs = new int[] { id };
                this.m_TreeView.SetSelection(selectedIDs, false);
            }
            if (GUILayout.Button("Frame", EditorStyles.miniButton, new GUILayoutOption[0]))
            {
                int num5 = this.GetNumItemsInData() / 10;
                this.m_TreeView.Frame(num5, true, false);
                int[] numArray2 = new int[] { num5 };
                this.m_TreeView.SetSelection(numArray2, false);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }
}

