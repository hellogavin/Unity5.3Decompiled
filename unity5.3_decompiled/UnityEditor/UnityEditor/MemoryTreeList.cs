namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class MemoryTreeList
    {
        private const float kBaseIndent = 4f;
        protected const float kColumnSize = 70f;
        protected const float kFoldoutSize = 14f;
        private const float kIndentPx = 16f;
        protected const float kNameColumnSize = 300f;
        protected const float kRowHeight = 16f;
        protected const float kSmallMargin = 4f;
        protected int m_ControlID;
        protected MemoryTreeList m_DetailView;
        protected EditorWindow m_EditorWindow;
        public MemoryElementSelection m_MemorySelection = new MemoryElementSelection();
        protected MemoryElement m_Root;
        protected Vector2 m_ScrollPosition;
        protected float m_SelectionOffset;
        protected SplitterState m_Splitter;
        private static Styles m_Styles;
        protected float m_VisibleHeight;

        public MemoryTreeList(EditorWindow editorWindow, MemoryTreeList detailview)
        {
            this.m_EditorWindow = editorWindow;
            this.m_DetailView = detailview;
            this.m_ControlID = GUIUtility.GetPermanentControlID();
            this.SetupSplitter();
        }

        private static float Clamp(float value, float min, float max)
        {
            return ((value >= min) ? ((value <= max) ? value : max) : min);
        }

        protected static void DrawBackground(int row, bool selected)
        {
            Rect position = GenerateRect(row);
            GUIStyle style = ((row % 2) != 0) ? styles.entryOdd : styles.entryEven;
            if (Event.current.type == EventType.Repaint)
            {
                style.Draw(position, GUIContent.none, false, false, selected, false);
            }
        }

        protected virtual void DrawData(Rect rect, MemoryElement memoryElement, int indent, int row, bool selected)
        {
            if (Event.current.type == EventType.Repaint)
            {
                string text = memoryElement.name + "(" + memoryElement.memoryInfo.className + ")";
                styles.numberLabel.Draw(rect, text, false, false, false, selected);
            }
        }

        protected virtual void DrawHeader()
        {
            GUILayout.Label("Referenced By:", styles.header, new GUILayoutOption[0]);
        }

        protected virtual void DrawItem(MemoryElement memoryElement, ref int row, int indent)
        {
            bool selected = this.m_MemorySelection.isSelected(memoryElement);
            DrawBackground(row, selected);
            Rect rect = GenerateRect(row);
            rect.x = (4f + (indent * 16f)) - 14f;
            Rect position = rect;
            position.width = 14f;
            if (memoryElement.ChildCount() > 0)
            {
                memoryElement.expanded = GUI.Toggle(position, memoryElement.expanded, GUIContent.none, styles.foldout);
            }
            rect.x += 14f;
            if (selected)
            {
                this.m_SelectionOffset = ((float) row) * 16f;
            }
            if ((Event.current.type == EventType.MouseDown) && rect.Contains(Event.current.mousePosition))
            {
                this.RowClicked(Event.current, memoryElement);
            }
            this.DrawData(rect, memoryElement, indent, row, selected);
            if (memoryElement.expanded)
            {
                this.DrawRecursiveData(memoryElement, ref row, indent + 1);
            }
        }

        protected void DrawRecursiveData(MemoryElement element, ref int row, int indent)
        {
            if (element.ChildCount() != 0)
            {
                element.ExpandChildren();
                foreach (MemoryElement element2 in element.children)
                {
                    row++;
                    this.DrawItem(element2, ref row, indent);
                }
            }
        }

        protected void EnsureVisible()
        {
            int row = 0;
            this.RecursiveFindSelected(this.m_Root, ref row);
            this.m_ScrollPosition.y = Clamp(this.m_ScrollPosition.y, this.m_SelectionOffset - this.m_VisibleHeight, this.m_SelectionOffset - 16f);
        }

        protected static Rect GenerateRect(int row)
        {
            return new Rect(1f, 16f * row, GUIClip.visibleRect.width, 16f);
        }

        public MemoryElement GetRoot()
        {
            return this.m_Root;
        }

        protected void HandleKeyboard()
        {
            Event current = Event.current;
            if (((current.GetTypeForControl(this.m_ControlID) == EventType.KeyDown) && (this.m_ControlID == GUIUtility.keyboardControl)) && (this.m_MemorySelection.Selected != null))
            {
                int num;
                KeyCode keyCode = current.keyCode;
                switch (keyCode)
                {
                    case KeyCode.UpArrow:
                        this.m_MemorySelection.MoveUp();
                        break;

                    case KeyCode.DownArrow:
                        this.m_MemorySelection.MoveDown();
                        break;

                    case KeyCode.RightArrow:
                        if (this.m_MemorySelection.Selected.ChildCount() > 0)
                        {
                            this.m_MemorySelection.Selected.expanded = true;
                        }
                        break;

                    case KeyCode.LeftArrow:
                        if (!this.m_MemorySelection.Selected.expanded)
                        {
                            this.m_MemorySelection.MoveParent();
                        }
                        else
                        {
                            this.m_MemorySelection.Selected.expanded = false;
                        }
                        break;

                    case KeyCode.Home:
                        this.m_MemorySelection.MoveFirst();
                        break;

                    case KeyCode.End:
                        this.m_MemorySelection.MoveLast();
                        break;

                    case KeyCode.PageUp:
                        num = Mathf.RoundToInt(this.m_VisibleHeight / 16f);
                        for (int i = 0; i < num; i++)
                        {
                            this.m_MemorySelection.MoveUp();
                        }
                        break;

                    case KeyCode.PageDown:
                        num = Mathf.RoundToInt(this.m_VisibleHeight / 16f);
                        for (int j = 0; j < num; j++)
                        {
                            this.m_MemorySelection.MoveDown();
                        }
                        break;

                    default:
                        if (keyCode == KeyCode.Return)
                        {
                            if (this.m_MemorySelection.Selected.memoryInfo != null)
                            {
                                Selection.instanceIDs = new int[0];
                                Selection.activeInstanceID = this.m_MemorySelection.Selected.memoryInfo.instanceId;
                            }
                        }
                        else
                        {
                            return;
                        }
                        break;
                }
                this.RowClicked(current, this.m_MemorySelection.Selected);
                this.EnsureVisible();
                this.m_EditorWindow.Repaint();
            }
        }

        public void OnGUI()
        {
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            SplitterGUILayout.BeginHorizontalSplit(this.m_Splitter, EditorStyles.toolbar, new GUILayoutOption[0]);
            this.DrawHeader();
            SplitterGUILayout.EndHorizontalSplit();
            if (this.m_Root == null)
            {
                GUILayout.EndVertical();
            }
            else
            {
                this.HandleKeyboard();
                this.m_ScrollPosition = GUILayout.BeginScrollView(this.m_ScrollPosition, styles.background);
                int row = 0;
                foreach (MemoryElement element in this.m_Root.children)
                {
                    this.DrawItem(element, ref row, 1);
                    row++;
                }
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
                GUILayoutUtility.GetRect((float) 0f, (float) (row * 16f), options);
                if (Event.current.type == EventType.Repaint)
                {
                    this.m_VisibleHeight = GUIClip.visibleRect.height;
                }
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
            }
        }

        private void RecursiveFindSelected(MemoryElement element, ref int row)
        {
            if (this.m_MemorySelection.isSelected(element))
            {
                this.m_SelectionOffset = ((float) row) * 16f;
            }
            row++;
            if (element.expanded && (element.ChildCount() != 0))
            {
                element.ExpandChildren();
                foreach (MemoryElement element2 in element.children)
                {
                    this.RecursiveFindSelected(element2, ref row);
                }
            }
        }

        protected void RowClicked(Event evt, MemoryElement memoryElement)
        {
            this.m_MemorySelection.SetSelection(memoryElement);
            GUIUtility.keyboardControl = this.m_ControlID;
            if (((evt.clickCount == 2) && (memoryElement.memoryInfo != null)) && (memoryElement.memoryInfo.instanceId != 0))
            {
                Selection.instanceIDs = new int[0];
                Selection.activeInstanceID = memoryElement.memoryInfo.instanceId;
            }
            evt.Use();
            if (memoryElement.memoryInfo != null)
            {
                EditorGUIUtility.PingObject(memoryElement.memoryInfo.instanceId);
            }
            if (this.m_DetailView != null)
            {
                this.m_DetailView.SetRoot((memoryElement.memoryInfo != null) ? new MemoryElement(memoryElement.memoryInfo, false) : null);
            }
            this.m_EditorWindow.Repaint();
        }

        public void SetRoot(MemoryElement root)
        {
            this.m_Root = root;
            if (this.m_Root != null)
            {
                this.m_Root.ExpandChildren();
            }
            if (this.m_DetailView != null)
            {
                this.m_DetailView.SetRoot(null);
            }
        }

        protected virtual void SetupSplitter()
        {
            float[] relativeSizes = new float[1];
            int[] minSizes = new int[1];
            relativeSizes[0] = 300f;
            minSizes[0] = 100;
            this.m_Splitter = new SplitterState(relativeSizes, minSizes, null);
        }

        protected static Styles styles
        {
            get
            {
                if (m_Styles == null)
                {
                }
                return (m_Styles = new Styles());
            }
        }

        internal class Styles
        {
            public GUIStyle background = "OL Box";
            public GUIStyle entryEven = "OL EntryBackEven";
            public GUIStyle entryOdd = "OL EntryBackOdd";
            public GUIStyle foldout = "IN foldout";
            public GUIStyle header = "OL title";
            public GUIStyle numberLabel = "OL Label";
        }
    }
}

