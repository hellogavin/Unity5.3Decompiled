namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class SketchUpImportDlg : EditorWindow
    {
        private const float kBottomHeight = 30f;
        private const float kHeaderHeight = 25f;
        private SketchUpDataSource m_DataSource;
        private SketchUpTreeViewGUI m_ImportGUI;
        private WeakReference m_ModelEditor;
        private int[] m_Selection;
        private TreeView m_TreeView;
        private TreeViewState m_TreeViewState;
        private readonly Vector2 m_WindowMinSize = new Vector2(350f, 350f);

        private void HandleKeyboardEvents()
        {
            Event current = Event.current;
            if (((current.type == EventType.KeyDown) && (((current.keyCode == KeyCode.Space) || (current.keyCode == KeyCode.Return)) || (current.keyCode == KeyCode.KeypadEnter))) && ((this.m_Selection != null) && (this.m_Selection.Length > 0)))
            {
                SketchUpNode node = this.m_TreeView.FindNode(this.m_Selection[0]) as SketchUpNode;
                if ((node != null) && (node != this.m_DataSource.root))
                {
                    node.Enabled = !node.Enabled;
                    current.Use();
                    base.Repaint();
                }
            }
        }

        public void Init(SketchUpNodeInfo[] nodes, SketchUpImporterModelEditor suModelEditor)
        {
            base.titleContent = Styles.styles.windowTitle;
            base.minSize = this.m_WindowMinSize;
            base.position = new Rect(base.position.x, base.position.y, base.minSize.x, base.minSize.y);
            this.m_TreeViewState = new TreeViewState();
            this.m_TreeView = new TreeView(this, this.m_TreeViewState);
            this.m_ImportGUI = new SketchUpTreeViewGUI(this.m_TreeView);
            this.m_DataSource = new SketchUpDataSource(this.m_TreeView, nodes);
            this.m_TreeView.Init(base.position, this.m_DataSource, this.m_ImportGUI, null);
            this.m_TreeView.selectionChangedCallback = (Action<int[]>) Delegate.Combine(this.m_TreeView.selectionChangedCallback, new Action<int[]>(this.OnTreeSelectionChanged));
            this.m_ModelEditor = new WeakReference(suModelEditor);
            this.isModal = false;
        }

        internal static void Launch(SketchUpNodeInfo[] nodes, SketchUpImporterModelEditor suModelEditor)
        {
            SketchUpImportDlg windowDontShow = EditorWindow.GetWindowDontShow<SketchUpImportDlg>();
            windowDontShow.Init(nodes, suModelEditor);
            windowDontShow.ShowAuxWindow();
        }

        internal static int[] LaunchAsModal(SketchUpNodeInfo[] nodes)
        {
            SketchUpImportDlg windowDontShow = EditorWindow.GetWindowDontShow<SketchUpImportDlg>();
            windowDontShow.Init(nodes, null);
            windowDontShow.isModal = true;
            windowDontShow.ShowModal();
            return windowDontShow.m_DataSource.FetchEnableNodes();
        }

        private void OnGUI()
        {
            Rect position = new Rect(0f, 0f, base.position.width, base.position.height);
            int controlID = GUIUtility.GetControlID(FocusType.Keyboard, position);
            Rect rect2 = new Rect(0f, 0f, base.position.width, 25f);
            GUI.Label(rect2, string.Empty, Styles.styles.headerStyle);
            GUI.Label(new Rect(10f, 2f, base.position.width, 25f), Styles.styles.nodesLabel);
            Rect screenRect = new Rect(position.x, position.yMax - 30f, position.width, 30f);
            GUILayout.BeginArea(screenRect);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1f) };
            GUILayout.Box(string.Empty, Styles.styles.boxBackground, options);
            GUILayout.Space(2f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            bool flag = false;
            if (this.isModal)
            {
                if (GUILayout.Button(Styles.styles.okButton, new GUILayoutOption[0]))
                {
                    flag = true;
                }
            }
            else if (GUILayout.Button(Styles.styles.cancelButton, new GUILayoutOption[0]))
            {
                flag = true;
            }
            else if (GUILayout.Button(Styles.styles.okButton, new GUILayoutOption[0]))
            {
                flag = true;
                if (this.m_ModelEditor.IsAlive)
                {
                    (this.m_ModelEditor.Target as SketchUpImporterModelEditor).SetSelectedNodes(this.m_DataSource.FetchEnableNodes());
                }
            }
            GUILayout.Space(10f);
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
            position.y = 18f;
            position.height -= (rect2.height + screenRect.height) - 7f;
            this.m_TreeView.OnEvent();
            this.m_TreeView.OnGUI(position, controlID);
            this.HandleKeyboardEvents();
            if (flag)
            {
                base.Close();
            }
        }

        private void OnLostFocus()
        {
            if (!this.isModal)
            {
                base.Close();
            }
        }

        public void OnTreeSelectionChanged(int[] selection)
        {
            this.m_Selection = selection;
        }

        private bool isModal { get; set; }

        internal class Styles
        {
            public readonly GUIStyle boxBackground = "OL Box";
            public readonly float buttonWidth = 32f;
            public readonly GUIContent cancelButton = EditorGUIUtility.TextContent("Cancel");
            public readonly GUIStyle headerStyle = new GUIStyle(EditorStyles.toolbarButton);
            public readonly GUIContent nodesLabel = EditorGUIUtility.TextContent("Select the SketchUp nodes to import|Nodes in the file hierarchy");
            public readonly GUIContent okButton = EditorGUIUtility.TextContent("OK");
            private static SketchUpImportDlg.Styles s_Styles;
            public readonly GUIStyle toggleStyle;
            public readonly GUIContent windowTitle = EditorGUIUtility.TextContent("SketchUp Node Selection Dialog|SketchUp Node Selection Dialog");

            public Styles()
            {
                this.headerStyle.padding.left = 4;
                this.headerStyle.alignment = TextAnchor.MiddleLeft;
                this.toggleStyle = new GUIStyle(EditorStyles.toggle);
                this.toggleStyle.padding.left = 8;
                this.toggleStyle.alignment = TextAnchor.MiddleCenter;
            }

            public static SketchUpImportDlg.Styles styles
            {
                get
                {
                    if (s_Styles == null)
                    {
                    }
                    return (s_Styles = new SketchUpImportDlg.Styles());
                }
            }
        }
    }
}

