namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class ColumnView
    {
        public float columnWidth = 150f;
        private readonly List<int> m_CachedSelectedIndices = new List<int>();
        private int m_ColumnToFocusKeyboard = -1;
        private readonly List<ListViewState> m_ListViewStates = new List<ListViewState>();
        private Vector2 m_ScrollPosition;
        private string m_SearchText = string.Empty;
        public int minimumNumberOfColumns = 1;
        private static Styles s_Styles;

        private void DoDoubleClick(ListViewElement element, ColumnViewElement columnViewElement, ObjectColumnFunction selectedSearchItemFunction, ObjectColumnFunction selectedRegularItemFunction)
        {
            if (((Event.current.type == EventType.MouseDown) && element.position.Contains(Event.current.mousePosition)) && ((Event.current.button == 0) && (Event.current.clickCount == 2)))
            {
                if (this.isSearching)
                {
                    this.DoSearchItemSelectedEvent(selectedSearchItemFunction, columnViewElement.value);
                }
                else
                {
                    DoItemSelectedEvent(selectedRegularItemFunction, columnViewElement.value);
                }
            }
        }

        private static void DoDragAndDrop(ListViewState listView, ListViewElement element, List<ColumnViewElement> columnViewElements, ObjectColumnGetDataFunction getDataForDraggingFunction)
        {
            if (((GUIUtility.hotControl == listView.ID) && (Event.current.type == EventType.MouseDown)) && (element.position.Contains(Event.current.mousePosition) && (Event.current.button == 0)))
            {
                DragAndDropDelay stateObject = (DragAndDropDelay) GUIUtility.GetStateObject(typeof(DragAndDropDelay), listView.ID);
                stateObject.mouseDownPosition = Event.current.mousePosition;
            }
            if (((GUIUtility.hotControl == listView.ID) && (Event.current.type == EventType.MouseDrag)) && GUIClip.visibleRect.Contains(Event.current.mousePosition))
            {
                DragAndDropDelay delay2 = (DragAndDropDelay) GUIUtility.GetStateObject(typeof(DragAndDropDelay), listView.ID);
                if (delay2.CanStartDrag())
                {
                    object data = (getDataForDraggingFunction != null) ? getDataForDraggingFunction(columnViewElements[listView.row].value) : null;
                    if (data != null)
                    {
                        DragAndDrop.PrepareStartDrag();
                        DragAndDrop.objectReferences = new Object[0];
                        DragAndDrop.paths = null;
                        DragAndDrop.SetGenericData("CustomDragData", data);
                        DragAndDrop.StartDrag(columnViewElements[listView.row].name);
                        Event.current.Use();
                    }
                }
            }
        }

        private void DoDummyColumn()
        {
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(this.columnWidth + 1f) };
            GUILayout.Box(GUIContent.none, s_Styles.background, options);
        }

        private static void DoItemSelectedEvent(ObjectColumnFunction selectedRegularItemFunction, object value)
        {
            if (selectedRegularItemFunction != null)
            {
                selectedRegularItemFunction(value);
            }
            Event.current.Use();
        }

        private int DoListColumn(ListViewState listView, List<ColumnViewElement> columnViewElements, int columnIndex, int selectedIndex, ObjectColumnFunction selectedSearchItemFunction, ObjectColumnFunction selectedRegularItemFunction, ObjectColumnGetDataFunction getDataForDraggingFunction)
        {
            if (((Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.Return)) && (listView.row > -1))
            {
                if (this.isSearching && (selectedSearchItemFunction != null))
                {
                    this.DoSearchItemSelectedEvent(selectedSearchItemFunction, columnViewElements[selectedIndex].value);
                }
                if ((!this.isSearching && (GUIUtility.keyboardControl == listView.ID)) && (selectedRegularItemFunction != null))
                {
                    DoItemSelectedEvent(selectedRegularItemFunction, columnViewElements[selectedIndex].value);
                }
            }
            if (((GUIUtility.keyboardControl == listView.ID) && (Event.current.type == EventType.KeyDown)) && !this.isSearching)
            {
                switch (Event.current.keyCode)
                {
                    case KeyCode.RightArrow:
                        this.m_ColumnToFocusKeyboard = columnIndex + 1;
                        Event.current.Use();
                        break;

                    case KeyCode.LeftArrow:
                        this.m_ColumnToFocusKeyboard = columnIndex - 1;
                        Event.current.Use();
                        break;
                }
            }
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(this.columnWidth) };
            IEnumerator enumerator = ListViewGUILayout.ListView(listView, s_Styles.background, options).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    ListViewElement current = (ListViewElement) enumerator.Current;
                    ColumnViewElement columnViewElement = columnViewElements[current.row];
                    if ((current.row == listView.row) && (Event.current.type == EventType.Repaint))
                    {
                        Rect position = current.position;
                        position.x++;
                        position.y++;
                        s_Styles.selected.Draw(position, false, true, true, GUIUtility.keyboardControl == listView.ID);
                    }
                    GUILayout.Label(columnViewElement.name, new GUILayoutOption[0]);
                    if (columnViewElement.value is List<ColumnViewElement>)
                    {
                        Rect rect2 = current.position;
                        rect2.x = (rect2.xMax - s_Styles.categoryArrowIcon.width) - 5f;
                        rect2.y += 2f;
                        GUI.Label(rect2, s_Styles.categoryArrowIcon);
                    }
                    this.DoDoubleClick(current, columnViewElement, selectedSearchItemFunction, selectedRegularItemFunction);
                    DoDragAndDrop(listView, current, columnViewElements, getDataForDraggingFunction);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            if (Event.current.type == EventType.Layout)
            {
                selectedIndex = listView.row;
            }
            return selectedIndex;
        }

        private static void DoPreviewColumn(object selectedObject, ObjectColumnFunction previewColumnFunction)
        {
            GUILayout.BeginVertical(s_Styles.background, new GUILayoutOption[0]);
            if (previewColumnFunction != null)
            {
                previewColumnFunction(selectedObject);
            }
            GUILayout.EndVertical();
        }

        private void DoSearchItemSelectedEvent(ObjectColumnFunction selectedSearchItemFunction, object value)
        {
            this.m_SearchText = string.Empty;
            DoItemSelectedEvent(selectedSearchItemFunction, value);
        }

        private static void InitStyles()
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
        }

        public void OnGUI(List<ColumnViewElement> elements, ObjectColumnFunction previewColumnFunction)
        {
            this.OnGUI(elements, previewColumnFunction, null, null, null);
        }

        public void OnGUI(List<ColumnViewElement> elements, ObjectColumnFunction previewColumnFunction, ObjectColumnFunction selectedSearchItemFunction, ObjectColumnFunction selectedRegularItemFunction, ObjectColumnGetDataFunction getDataForDraggingFunction)
        {
            object obj2;
            InitStyles();
            this.m_ScrollPosition = GUILayout.BeginScrollView(this.m_ScrollPosition, new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            List<ColumnViewElement> columnViewElements = elements;
            int columnIndex = 0;
            do
            {
                if (this.m_ListViewStates.Count == columnIndex)
                {
                    this.m_ListViewStates.Add(new ListViewState());
                }
                if (this.m_CachedSelectedIndices.Count == columnIndex)
                {
                    this.m_CachedSelectedIndices.Add(-1);
                }
                ListViewState listView = this.m_ListViewStates[columnIndex];
                listView.totalRows = columnViewElements.Count;
                if (columnIndex == 0)
                {
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MaxWidth(this.columnWidth) };
                    GUILayout.BeginVertical(options);
                }
                int selectedIndex = this.m_CachedSelectedIndices[columnIndex];
                selectedIndex = this.DoListColumn(listView, columnViewElements, columnIndex, selectedIndex, (columnIndex != 0) ? null : selectedSearchItemFunction, selectedRegularItemFunction, getDataForDraggingFunction);
                if ((Event.current.type == EventType.Layout) && (this.m_ColumnToFocusKeyboard == columnIndex))
                {
                    this.m_ColumnToFocusKeyboard = -1;
                    GUIUtility.keyboardControl = listView.ID;
                    if ((listView.row == -1) && (columnViewElements.Count != 0))
                    {
                        selectedIndex = listView.row = 0;
                    }
                }
                if (columnIndex == 0)
                {
                    if (this.isSearching)
                    {
                        KeyCode keyCode = StealImportantListviewKeys();
                        if (keyCode != KeyCode.None)
                        {
                            ListViewShared.SendKey(this.m_ListViewStates[0], keyCode);
                        }
                    }
                    this.m_SearchText = EditorGUILayout.ToolbarSearchField(this.m_SearchText, new GUILayoutOption[0]);
                    GUILayout.EndVertical();
                }
                if (selectedIndex >= columnViewElements.Count)
                {
                    selectedIndex = -1;
                }
                if (((Event.current.type == EventType.Layout) && (this.m_CachedSelectedIndices[columnIndex] != selectedIndex)) && (this.m_ListViewStates.Count > (columnIndex + 1)))
                {
                    int index = columnIndex + 1;
                    int count = this.m_ListViewStates.Count - (columnIndex + 1);
                    this.m_ListViewStates.RemoveRange(index, count);
                    this.m_CachedSelectedIndices.RemoveRange(index, count);
                }
                this.m_CachedSelectedIndices[columnIndex] = selectedIndex;
                obj2 = (selectedIndex <= -1) ? null : columnViewElements[selectedIndex].value;
                columnViewElements = obj2 as List<ColumnViewElement>;
                columnIndex++;
            }
            while (columnViewElements != null);
            while (columnIndex < this.minimumNumberOfColumns)
            {
                this.DoDummyColumn();
                columnIndex++;
            }
            DoPreviewColumn(obj2, previewColumnFunction);
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
        }

        public void SetKeyboardFocusColumn(int column)
        {
            this.m_ColumnToFocusKeyboard = column;
        }

        public void SetSelected(int column, int selectionIndex)
        {
            if (this.m_ListViewStates.Count == column)
            {
                this.m_ListViewStates.Add(new ListViewState());
            }
            if (this.m_CachedSelectedIndices.Count == column)
            {
                this.m_CachedSelectedIndices.Add(-1);
            }
            this.m_CachedSelectedIndices[column] = selectionIndex;
            this.m_ListViewStates[column].row = selectionIndex;
        }

        private static KeyCode StealImportantListviewKeys()
        {
            if (Event.current.type == EventType.KeyDown)
            {
                KeyCode keyCode = Event.current.keyCode;
                switch (keyCode)
                {
                    case KeyCode.UpArrow:
                    case KeyCode.DownArrow:
                    case KeyCode.PageUp:
                    case KeyCode.PageDown:
                        Event.current.Use();
                        return keyCode;
                }
            }
            return KeyCode.None;
        }

        public bool isSearching
        {
            get
            {
                return (this.searchText != string.Empty);
            }
        }

        public string searchText
        {
            get
            {
                return this.m_SearchText;
            }
        }

        public delegate void ObjectColumnFunction(object value);

        public delegate object ObjectColumnGetDataFunction(object value);

        public class Styles
        {
            public GUIStyle background = "OL Box";
            public Texture2D categoryArrowIcon = EditorStyles.foldout.normal.background;
            public GUIStyle selected = "PR Label";
        }
    }
}

