namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    internal class AudioProfilerView
    {
        private int delayedPingObject;
        private AudioProfilerBackend m_Backend;
        private AudioProfilerViewColumnHeader m_ColumnHeader;
        private EditorWindow m_EditorWindow;
        private GUIStyle m_HeaderStyle;
        private TreeView m_TreeView;
        private AudioProfilerTreeViewState m_TreeViewState;

        public AudioProfilerView(EditorWindow editorWindow, AudioProfilerTreeViewState state)
        {
            this.m_EditorWindow = editorWindow;
            this.m_TreeViewState = state;
        }

        public int GetNumItemsInData()
        {
            return this.m_Backend.items.Count;
        }

        public void Init(Rect rect, AudioProfilerBackend backend)
        {
            this.m_HeaderStyle = "PR Label";
            if (this.m_TreeView == null)
            {
                this.m_Backend = backend;
                if (this.m_TreeViewState.columnWidths == null)
                {
                    int num = AudioProfilerInfoHelper.GetLastColumnIndex() + 1;
                    this.m_TreeViewState.columnWidths = new float[num];
                    for (int i = 0; i < num; i++)
                    {
                        this.m_TreeViewState.columnWidths[i] = (i < 0x12) ? ((float) 0x37) : ((float) 80);
                    }
                    this.m_TreeViewState.columnWidths[0] = 200f;
                    this.m_TreeViewState.columnWidths[1] = 200f;
                    this.m_TreeViewState.columnWidths[2] = 80f;
                    this.m_TreeViewState.columnWidths[3] = 80f;
                }
                this.m_TreeView = new TreeView(this.m_EditorWindow, this.m_TreeViewState);
                ITreeViewGUI gui = new AudioProfilerViewGUI(this.m_TreeView);
                ITreeViewDataSource data = new AudioProfilerDataSource(this.m_TreeView, this.m_Backend);
                this.m_TreeView.Init(rect, data, gui, null);
                this.m_ColumnHeader = new AudioProfilerViewColumnHeader(this.m_TreeViewState, this.m_Backend);
                this.m_ColumnHeader.columnWidths = this.m_TreeViewState.columnWidths;
                this.m_ColumnHeader.minColumnWidth = 30f;
                this.m_TreeView.selectionChangedCallback = (Action<int[]>) Delegate.Combine(this.m_TreeView.selectionChangedCallback, new Action<int[]>(this.OnTreeSelectionChanged));
            }
        }

        public void OnGUI(Rect rect, bool allowSorting)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Keyboard, rect);
            Rect position = new Rect(rect.x, rect.y, rect.width, 17f);
            Rect rect3 = new Rect(rect.x, rect.yMax - 20f, rect.width, 20f);
            GUI.Label(position, string.Empty, this.m_HeaderStyle);
            this.m_ColumnHeader.OnGUI(position, allowSorting);
            rect.y += position.height;
            rect.height -= position.height + rect3.height;
            this.m_TreeView.OnEvent();
            this.m_TreeView.OnGUI(rect, controlID);
        }

        public void OnTreeSelectionChanged(int[] selection)
        {
            if (selection.Length == 1)
            {
                AudioProfilerTreeViewItem item2 = this.m_TreeView.FindNode(selection[0]) as AudioProfilerTreeViewItem;
                if (item2 != null)
                {
                    EditorGUIUtility.PingObject(item2.info.info.assetInstanceId);
                    this.delayedPingObject = item2.info.info.objectInstanceId;
                    EditorApplication.CallDelayed(new EditorApplication.CallbackFunction(this.PingObjectDelayed), 1f);
                }
            }
        }

        private void PingObjectDelayed()
        {
            EditorGUIUtility.PingObject(this.delayedPingObject);
        }

        internal class AudioProfilerDataSource : TreeViewDataSource
        {
            private AudioProfilerBackend m_Backend;

            public AudioProfilerDataSource(TreeView treeView, AudioProfilerBackend backend) : base(treeView)
            {
                this.m_Backend = backend;
                this.m_Backend.OnUpdate = new AudioProfilerBackend.DataUpdateDelegate(this.FetchData);
                base.showRootNode = false;
                base.rootIsCollapsable = false;
                this.FetchData();
            }

            public override bool CanBeParent(TreeViewItem item)
            {
                return item.hasChildren;
            }

            public override void FetchData()
            {
                AudioProfilerView.AudioProfilerTreeViewItem parentNode = new AudioProfilerView.AudioProfilerTreeViewItem(1, 0, null, "ROOT", new AudioProfilerInfoWrapper(new AudioProfilerInfo(), "ROOT", "ROOT", false));
                this.FillTreeItems(parentNode, 1, 0, this.m_Backend.items);
                base.m_RootItem = parentNode;
                this.SetExpandedWithChildren(base.m_RootItem, true);
                base.m_NeedRefreshVisibleFolders = true;
            }

            private void FillTreeItems(AudioProfilerView.AudioProfilerTreeViewItem parentNode, int depth, int parentId, List<AudioProfilerInfoWrapper> items)
            {
                int capacity = 0;
                foreach (AudioProfilerInfoWrapper wrapper in items)
                {
                    if (parentId == (!wrapper.addToRoot ? wrapper.info.parentId : 0))
                    {
                        capacity++;
                    }
                }
                if (capacity > 0)
                {
                    parentNode.children = new List<TreeViewItem>(capacity);
                    foreach (AudioProfilerInfoWrapper wrapper2 in items)
                    {
                        if (parentId == (!wrapper2.addToRoot ? wrapper2.info.parentId : 0))
                        {
                            AudioProfilerView.AudioProfilerTreeViewItem item = new AudioProfilerView.AudioProfilerTreeViewItem(wrapper2.info.uniqueId, !wrapper2.addToRoot ? depth : 1, parentNode, wrapper2.objectName, wrapper2);
                            parentNode.children.Add(item);
                            this.FillTreeItems(item, depth + 1, wrapper2.info.uniqueId, items);
                        }
                    }
                }
            }

            public override bool IsRenamingItemAllowed(TreeViewItem item)
            {
                return false;
            }
        }

        internal class AudioProfilerTreeViewItem : TreeViewItem
        {
            public AudioProfilerTreeViewItem(int id, int depth, TreeViewItem parent, string displayName, AudioProfilerInfoWrapper info) : base(id, depth, parent, displayName)
            {
                this.info = info;
            }

            public AudioProfilerInfoWrapper info { get; set; }
        }

        internal class AudioProfilerViewColumnHeader
        {
            private AudioProfilerBackend m_Backend;
            private GUIStyle m_HeaderStyle;
            private AudioProfilerTreeViewState m_TreeViewState;

            public AudioProfilerViewColumnHeader(AudioProfilerTreeViewState state, AudioProfilerBackend backend)
            {
                this.m_TreeViewState = state;
                this.m_Backend = backend;
                this.minColumnWidth = 10f;
                this.dragWidth = 6f;
            }

            public void OnGUI(Rect rect, bool allowSorting)
            {
                float x = rect.x;
                int lastColumnIndex = AudioProfilerInfoHelper.GetLastColumnIndex();
                for (int i = 0; i <= lastColumnIndex; i++)
                {
                    Rect position = new Rect(x, rect.y, this.columnWidths[i], rect.height);
                    x += this.columnWidths[i];
                    Rect rect3 = new Rect(x - (this.dragWidth / 2f), rect.y, 3f, rect.height);
                    float num5 = EditorGUI.MouseDeltaReader(rect3, true).x;
                    if (num5 != 0f)
                    {
                        this.columnWidths[i] += num5;
                        this.columnWidths[i] = Mathf.Max(this.columnWidths[i], this.minColumnWidth);
                    }
                    if (this.m_HeaderStyle == null)
                    {
                        this.m_HeaderStyle = new GUIStyle("PR Label");
                        this.m_HeaderStyle.padding.left = 4;
                    }
                    this.m_HeaderStyle.alignment = (i != 0) ? TextAnchor.MiddleRight : TextAnchor.MiddleLeft;
                    string[] strArray = new string[] { 
                        "Object", "Asset", "Volume", "Audibility", "Plays", "3D", "Paused", "Muted", "Virtual", "OneShot", "Looped", "Distance", "MinDist", "MaxDist", "Time", "Duration", 
                        "Frequency", "Stream", "Compressed", "NonBlocking", "User", "Memory", "MemoryPoint"
                     };
                    string text = strArray[i];
                    if (allowSorting && (i == this.m_TreeViewState.selectedColumn))
                    {
                        text = text + (!this.m_TreeViewState.sortByDescendingOrder ? " ▲" : " ▼");
                    }
                    GUI.Label(position, text, this.m_HeaderStyle);
                    if ((allowSorting && (Event.current.type == EventType.MouseDown)) && position.Contains(Event.current.mousePosition))
                    {
                        this.m_TreeViewState.SetSelectedColumn(i);
                        this.m_Backend.UpdateSorting();
                    }
                    if (Event.current.type == EventType.Repaint)
                    {
                        EditorGUIUtility.AddCursorRect(rect3, MouseCursor.SplitResizeLeftRight);
                    }
                }
            }

            public float[] columnWidths { get; set; }

            public float dragWidth { get; set; }

            public float minColumnWidth { get; set; }
        }

        internal class AudioProfilerViewGUI : TreeViewGUI
        {
            public AudioProfilerViewGUI(TreeView treeView) : base(treeView)
            {
                base.k_IconWidth = 0f;
            }

            protected override void DrawIconAndLabel(Rect rect, TreeViewItem item, string label, bool selected, bool focused, bool useBoldFont, bool isPinging)
            {
                GUIStyle style = !useBoldFont ? TreeViewGUI.s_Styles.lineStyle : TreeViewGUI.s_Styles.lineBoldStyle;
                style.alignment = TextAnchor.MiddleLeft;
                style.padding.left = 0;
                base.DrawIconAndLabel(new Rect(rect.x, rect.y, this.columnWidths[0], rect.height), item, label, selected, focused, useBoldFont, isPinging);
                style.alignment = TextAnchor.MiddleRight;
                rect.x += this.columnWidths[0];
                AudioProfilerView.AudioProfilerTreeViewItem item2 = item as AudioProfilerView.AudioProfilerTreeViewItem;
                for (int i = 1; i < this.columnWidths.Length; i++)
                {
                    rect.width = this.columnWidths[i] - 3f;
                    style.Draw(rect, AudioProfilerInfoHelper.GetColumnString(item2.info, (AudioProfilerInfoHelper.ColumnIndices) i), false, false, selected, focused);
                    rect.x += this.columnWidths[i];
                }
                style.alignment = TextAnchor.MiddleLeft;
            }

            protected override Texture GetIconForNode(TreeViewItem item)
            {
                return null;
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
}

