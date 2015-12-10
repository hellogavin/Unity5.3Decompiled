namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;

    internal class ProfilerHierarchyGUI
    {
        private static int hierarchyViewHash = "HierarchyView".GetHashCode();
        private const float kBaseIndent = 4f;
        private const int kFirst = -999999;
        private const float kFoldoutSize = 14f;
        private const float kIndent = 16f;
        private const float kInstrumentationButtonOffset = 5f;
        private const float kInstrumentationButtonWidth = 30f;
        private const int kLast = 0xf423f;
        private const float kRowHeight = 16f;
        private const float kScrollbarWidth = 16f;
        private const float kSmallMargin = 4f;
        private string[] m_ColumnNames;
        private string m_ColumnSettingsName;
        private ProfilerColumn[] m_ColumnsToShow;
        private bool m_DetailPane;
        private string m_DetailViewSelectedProperty = string.Empty;
        private int m_DoScroll;
        private bool m_ExpandAll;
        private SerializedStringTable m_ExpandedHash = new SerializedStringTable();
        private GUIContent[] m_HeaderContent;
        private int m_ScrollViewHeight;
        private GUIContent m_SearchHeader;
        private SearchResults m_SearchResults;
        private int m_SelectedIndex = -1;
        private bool m_SetKeyboardFocus;
        private ProfilerColumn m_SortType = ProfilerColumn.TotalTime;
        private SplitterState m_Splitter;
        private int[] m_SplitterMinWidths;
        private float[] m_SplitterRelativeSizes;
        private Vector2 m_TextScroll = Vector2.zero;
        private bool[] m_VisibleColumns;
        private IProfilerWindowController m_Window;
        protected static Styles ms_Styles;

        public ProfilerHierarchyGUI(IProfilerWindowController window, string columnSettingsName, ProfilerColumn[] columnsToShow, string[] columnNames, bool detailPane, ProfilerColumn sort)
        {
            this.m_Window = window;
            this.m_ColumnNames = columnNames;
            this.m_ColumnSettingsName = columnSettingsName;
            this.m_ColumnsToShow = columnsToShow;
            this.m_DetailPane = detailPane;
            this.m_SortType = sort;
            this.m_HeaderContent = new GUIContent[columnNames.Length];
            this.m_Splitter = null;
            for (int i = 0; i < this.m_HeaderContent.Length; i++)
            {
                this.m_HeaderContent[i] = !this.m_ColumnNames[i].StartsWith("|") ? new GUIContent(this.m_ColumnNames[i]) : EditorGUIUtility.IconContent("ProfilerColumn." + columnsToShow[i].ToString(), this.m_ColumnNames[i]);
            }
            if (columnsToShow.Length != columnNames.Length)
            {
                throw new ArgumentException("Number of columns to show does not match number of column names.");
            }
            this.m_SearchHeader = new GUIContent("Search");
            this.m_VisibleColumns = new bool[columnNames.Length];
            for (int j = 0; j < this.m_VisibleColumns.Length; j++)
            {
                this.m_VisibleColumns[j] = true;
            }
            this.m_SearchResults = new SearchResults();
            this.m_SearchResults.Init(100);
            this.m_Window.Repaint();
        }

        private bool AllowSearching()
        {
            if ((Profiler.enabled && (ProfilerDriver.profileEditor || EditorApplication.isPlaying)) && ProfilerDriver.deepProfiling)
            {
                return false;
            }
            return true;
        }

        private bool ColIsVisible(int index)
        {
            return (((index >= 0) && (index <= this.m_VisibleColumns.Length)) && this.m_VisibleColumns[index]);
        }

        private void ColumnContextClick(object userData, string[] options, int selected)
        {
            this.SetColumnVisible(selected, !this.ColIsVisible(selected));
        }

        private static string DetailViewSelectedPropertyPath(ProfilerProperty property)
        {
            if (((property != null) && (property.instanceIDs != null)) && ((property.instanceIDs.Length != 0) && (property.instanceIDs[0] != 0)))
            {
                return DetailViewSelectedPropertyPath(property, property.instanceIDs[0]);
            }
            return string.Empty;
        }

        private static string DetailViewSelectedPropertyPath(ProfilerProperty property, int instanceId)
        {
            return (property.propertyPath + "/" + instanceId);
        }

        public void DoGUI(ProfilerProperty property, string searchString, bool expandAll)
        {
            this.m_ExpandAll = expandAll;
            this.SetupSplitter();
            this.DoScrolling();
            int controlID = GUIUtility.GetControlID(hierarchyViewHash, FocusType.Keyboard);
            this.DrawColumnsHeader(searchString);
            this.m_TextScroll = EditorGUILayout.BeginScrollView(this.m_TextScroll, ms_Styles.background, new GUILayoutOption[0]);
            int rowCount = this.DrawProfilingData(property, searchString, controlID);
            property.Cleanup();
            this.UnselectIfClickedOnEmptyArea(rowCount);
            if (Event.current.type == EventType.Repaint)
            {
                this.m_ScrollViewHeight = (int) GUIClip.visibleRect.height;
            }
            GUILayout.EndScrollView();
            this.HandleKeyboard(controlID);
            if (this.m_SetKeyboardFocus && (Event.current.type == EventType.Repaint))
            {
                this.m_SetKeyboardFocus = false;
                GUIUtility.keyboardControl = controlID;
                this.m_Window.Repaint();
            }
        }

        private void DoScroll()
        {
            this.m_DoScroll = 2;
        }

        private void DoScrolling()
        {
            if (this.m_DoScroll > 0)
            {
                this.m_DoScroll--;
                if (this.m_DoScroll == 0)
                {
                    float max = 16f * this.selectedIndex;
                    float min = (max - this.m_ScrollViewHeight) + 16f;
                    this.m_TextScroll.y = Mathf.Clamp(this.m_TextScroll.y, min, max);
                }
                else
                {
                    this.m_Window.Repaint();
                }
            }
        }

        private void DoSearchingDisabledInfoGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            TextAnchor alignment = EditorStyles.label.alignment;
            EditorStyles.label.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(0f, 10f, GUIClip.visibleRect.width, 30f), styles.disabledSearchText, EditorStyles.label);
            EditorStyles.label.alignment = alignment;
            EditorGUI.EndDisabledGroup();
        }

        private void DrawColumnsHeader(string searchString)
        {
            bool flag = false;
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if ((Event.current.type == EventType.MouseDown) && (Event.current.button == 1))
            {
                flag = true;
                Event.current.type = EventType.Used;
            }
            SplitterGUILayout.BeginHorizontalSplit(this.m_Splitter, GUIStyle.none, new GUILayoutOption[0]);
            this.DrawTitle(!this.IsSearchActive() ? this.m_HeaderContent[0] : this.m_SearchHeader, 0);
            for (int i = 1; i < this.m_ColumnNames.Length; i++)
            {
                this.DrawTitle(this.m_HeaderContent[i], i);
            }
            SplitterGUILayout.EndHorizontalSplit();
            GUILayout.EndHorizontal();
            if (flag)
            {
                Event.current.type = EventType.MouseDown;
                this.HandleHeaderMouse(GUILayoutUtility.GetLastRect());
            }
            GUILayout.Space(1f);
        }

        private bool DrawProfileDataItem(ProfilerProperty property, int rowCount, bool selected, int id)
        {
            bool flag = false;
            Event current = Event.current;
            Rect rowRect = this.GetRowRect(rowCount);
            Rect position = rowRect;
            GUIStyle rowBackgroundStyle = this.GetRowBackgroundStyle(rowCount);
            if (current.type == EventType.Repaint)
            {
                rowBackgroundStyle.Draw(position, GUIContent.none, false, false, selected, false);
            }
            float x = (property.depth * 16f) + 4f;
            if (property.HasChildren)
            {
                flag = this.IsExpanded(property.propertyPath);
                GUI.changed = false;
                x -= 14f;
                Rect rect3 = new Rect(x, position.y, 14f, 16f);
                flag = GUI.Toggle(rect3, flag, GUIContent.none, styles.foldout);
                if (GUI.changed)
                {
                    this.SetExpanded(property, flag);
                }
                x += 16f;
            }
            string column = property.GetColumn(this.m_ColumnsToShow[0]);
            if (current.type == EventType.Repaint)
            {
                this.DrawTextColumn(ref position, column, 0, (this.m_ColumnsToShow[0] != ProfilerColumn.FunctionName) ? 4f : x, selected);
            }
            if (ProfilerInstrumentationPopup.InstrumentationEnabled && ProfilerInstrumentationPopup.FunctionHasInstrumentationPopup(column))
            {
                float num2 = ((position.x + x) + 5f) + styles.numberLabel.CalcSize(new GUIContent(column)).x;
                num2 = Mathf.Clamp(num2, 0f, (this.m_Splitter.realSizes[0] - 30f) + 2f);
                Rect rect4 = new Rect(num2, position.y, 30f, 16f);
                if (GUI.Button(rect4, styles.instrumentationIcon, styles.miniPullDown))
                {
                    ProfilerInstrumentationPopup.Show(rect4, column);
                }
            }
            if (current.type == EventType.Repaint)
            {
                styles.numberLabel.alignment = TextAnchor.MiddleRight;
                int index = 1;
                for (int i = 1; i < this.m_VisibleColumns.Length; i++)
                {
                    if (this.ColIsVisible(i))
                    {
                        position.x += this.m_Splitter.realSizes[index - 1];
                        position.width = this.m_Splitter.realSizes[index] - 4f;
                        index++;
                        styles.numberLabel.Draw(position, property.GetColumn(this.m_ColumnsToShow[i]), false, false, false, selected);
                    }
                }
                styles.numberLabel.alignment = TextAnchor.MiddleLeft;
            }
            if ((current.type == EventType.MouseDown) && rowRect.Contains(current.mousePosition))
            {
                GUIUtility.hotControl = 0;
                if (!EditorGUI.actionKey)
                {
                    if (this.m_DetailPane)
                    {
                        if ((current.clickCount == 1) && (property.instanceIDs.Length > 0))
                        {
                            string str2 = DetailViewSelectedPropertyPath(property);
                            if (this.m_DetailViewSelectedProperty != str2)
                            {
                                this.m_DetailViewSelectedProperty = str2;
                                Object gameObject = EditorUtility.InstanceIDToObject(property.instanceIDs[0]);
                                if (gameObject is Component)
                                {
                                    gameObject = ((Component) gameObject).gameObject;
                                }
                                if (gameObject != null)
                                {
                                    EditorGUIUtility.PingObject(gameObject.GetInstanceID());
                                }
                            }
                            else
                            {
                                this.m_DetailViewSelectedProperty = string.Empty;
                            }
                        }
                        else if (current.clickCount == 2)
                        {
                            SelectObjectsInHierarchyView(property);
                            this.m_DetailViewSelectedProperty = DetailViewSelectedPropertyPath(property);
                        }
                    }
                    else
                    {
                        this.RowMouseDown(property.propertyPath);
                    }
                    this.DoScroll();
                }
                else if (!this.m_DetailPane)
                {
                    this.m_Window.ClearSelectedPropertyPath();
                }
                else
                {
                    this.m_DetailViewSelectedProperty = string.Empty;
                }
                GUIUtility.keyboardControl = id;
                current.Use();
            }
            if (((selected && (GUIUtility.keyboardControl == id)) && (current.type == EventType.KeyDown)) && ((current.keyCode == KeyCode.Return) || (current.keyCode == KeyCode.KeypadEnter)))
            {
                SelectObjectsInHierarchyView(property);
            }
            return flag;
        }

        private int DrawProfilingData(ProfilerProperty property, string searchString, int id)
        {
            int num = 0;
            if (this.IsSearchActive())
            {
                num = this.DrawSearchResult(property, searchString, id);
            }
            else
            {
                num = this.DrawTreeView(property, id);
            }
            if (num == 0)
            {
                Rect rowRect = this.GetRowRect(0);
                rowRect.height = 1f;
                GUI.Label(rowRect, GUIContent.none, styles.entryEven);
            }
            return num;
        }

        private int DrawSearchResult(ProfilerProperty property, string searchString, int id)
        {
            if (!this.AllowSearching())
            {
                this.DoSearchingDisabledInfoGUI();
                return 0;
            }
            this.m_SearchResults.Filter(property, this.m_ColumnsToShow, searchString, this.m_Window.GetActiveVisibleFrameIndex(), this.sortType);
            this.m_SearchResults.Draw(this, id);
            return this.m_SearchResults.numRows;
        }

        protected void DrawTextColumn(ref Rect currentRect, string text, int index, float margin, bool selected)
        {
            if (index != 0)
            {
                currentRect.x += this.m_Splitter.realSizes[index - 1];
            }
            currentRect.x += margin;
            currentRect.width = this.m_Splitter.realSizes[index] - margin;
            styles.numberLabel.Draw(currentRect, text, false, false, false, selected);
            currentRect.x -= margin;
        }

        private void DrawTitle(GUIContent name, int index)
        {
            if (this.ColIsVisible(index))
            {
                ProfilerColumn column = this.m_ColumnsToShow[index];
                bool flag = this.sortType == column;
                if ((index != 0) ? GUILayout.Toggle(flag, name, styles.rightHeader, new GUILayoutOption[] { GUILayout.Width((float) this.m_SplitterMinWidths[index]) }) : GUILayout.Toggle(flag, name, styles.header, new GUILayoutOption[0]))
                {
                    this.sortType = column;
                }
            }
        }

        private int DrawTreeView(ProfilerProperty property, int id)
        {
            this.m_SelectedIndex = -1;
            bool enterChildren = true;
            int rowCount = 0;
            string selectedPropertyPath = ProfilerDriver.selectedPropertyPath;
            while (property.Next(enterChildren))
            {
                string propertyPath = property.propertyPath;
                bool selected = !this.m_DetailPane ? (propertyPath == selectedPropertyPath) : ((this.m_DetailViewSelectedProperty != string.Empty) && (this.m_DetailViewSelectedProperty == DetailViewSelectedPropertyPath(property)));
                if (selected)
                {
                    this.m_SelectedIndex = rowCount;
                }
                bool flag3 = Event.current.type != EventType.Layout;
                if (flag3 & ((this.m_ScrollViewHeight == 0) || (((rowCount * 16f) <= (this.m_ScrollViewHeight + this.m_TextScroll.y)) && (((rowCount + 1) * 16f) > this.m_TextScroll.y))))
                {
                    enterChildren = this.DrawProfileDataItem(property, rowCount, selected, id);
                }
                else
                {
                    enterChildren = property.HasChildren && this.IsExpanded(propertyPath);
                }
                rowCount++;
            }
            return rowCount;
        }

        public void FrameSelection()
        {
            if (!string.IsNullOrEmpty(ProfilerDriver.selectedPropertyPath))
            {
                this.m_Window.SetSearch(string.Empty);
                char[] separator = new char[] { '/' };
                string[] strArray = ProfilerDriver.selectedPropertyPath.Split(separator);
                string expandedName = strArray[0];
                for (int i = 1; i < strArray.Length; i++)
                {
                    this.SetExpanded(expandedName, true);
                    expandedName = expandedName + "/" + strArray[i];
                }
                this.DoScroll();
            }
        }

        public ProfilerProperty GetDetailedProperty(ProfilerProperty property)
        {
            bool enterChildren = true;
            string selectedPropertyPath = ProfilerDriver.selectedPropertyPath;
            while (property.Next(enterChildren))
            {
                string propertyPath = property.propertyPath;
                if (propertyPath == selectedPropertyPath)
                {
                    ProfilerProperty property2 = new ProfilerProperty();
                    property2.InitializeDetailProperty(property);
                    return property2;
                }
                if (property.HasChildren)
                {
                    enterChildren = this.IsExpanded(propertyPath);
                }
            }
            return null;
        }

        private GUIStyle GetRowBackgroundStyle(int rowIndex)
        {
            return (((rowIndex % 2) != 0) ? styles.entryOdd : styles.entryEven);
        }

        private Rect GetRowRect(int rowIndex)
        {
            return new Rect(1f, 16f * rowIndex, GUIClip.visibleRect.width, 16f);
        }

        private int[] GetVisibleDropDownIndexList()
        {
            List<int> list = new List<int>();
            for (int i = 0; i < this.m_ColumnNames.Length; i++)
            {
                if (this.m_VisibleColumns[i])
                {
                    list.Add(i);
                }
            }
            return list.ToArray();
        }

        private void HandleHeaderMouse(Rect columnHeaderRect)
        {
            Event current = Event.current;
            if (((current.type == EventType.MouseDown) && (current.button == 1)) && columnHeaderRect.Contains(current.mousePosition))
            {
                GUIUtility.hotControl = 0;
                Rect position = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 1f, 1f);
                EditorUtility.DisplayCustomMenu(position, this.m_ColumnNames, this.GetVisibleDropDownIndexList(), new EditorUtility.SelectMenuItemFunction(this.ColumnContextClick), null);
                current.Use();
            }
        }

        private void HandleKeyboard(int id)
        {
            int num;
            Event current = Event.current;
            if ((current.GetTypeForControl(id) == EventType.KeyDown) && (id == GUIUtility.keyboardControl))
            {
                bool flag = this.IsSearchActive();
                num = 0;
                switch (current.keyCode)
                {
                    case KeyCode.UpArrow:
                        num = -1;
                        goto Label_0161;

                    case KeyCode.DownArrow:
                        num = 1;
                        goto Label_0161;

                    case KeyCode.RightArrow:
                        if (!flag)
                        {
                            this.SetExpanded(ProfilerDriver.selectedPropertyPath, true);
                        }
                        goto Label_0161;

                    case KeyCode.LeftArrow:
                        if (!flag)
                        {
                            this.SetExpanded(ProfilerDriver.selectedPropertyPath, false);
                        }
                        goto Label_0161;

                    case KeyCode.Home:
                        num = -999999;
                        goto Label_0161;

                    case KeyCode.End:
                        num = 0xf423f;
                        goto Label_0161;

                    case KeyCode.PageUp:
                        if (Application.platform != RuntimePlatform.OSXEditor)
                        {
                            num = -Mathf.RoundToInt(((float) this.m_ScrollViewHeight) / 16f);
                            goto Label_0161;
                        }
                        this.m_TextScroll.y -= this.m_ScrollViewHeight;
                        if (this.m_TextScroll.y < 0f)
                        {
                            this.m_TextScroll.y = 0f;
                        }
                        current.Use();
                        return;

                    case KeyCode.PageDown:
                        if (Application.platform != RuntimePlatform.OSXEditor)
                        {
                            num = Mathf.RoundToInt(((float) this.m_ScrollViewHeight) / 16f);
                            goto Label_0161;
                        }
                        this.m_TextScroll.y += this.m_ScrollViewHeight;
                        current.Use();
                        return;
                }
            }
            return;
        Label_0161:
            if (num != 0)
            {
                this.MoveSelection(num);
            }
            this.DoScroll();
            current.Use();
        }

        private bool IsExpanded(string expanded)
        {
            return (this.m_ExpandAll || this.m_ExpandedHash.Contains(expanded));
        }

        private bool IsSearchActive()
        {
            return this.m_Window.IsSearching();
        }

        private void MoveSelection(int steps)
        {
            if (this.IsSearchActive())
            {
                this.m_SearchResults.MoveSelection(steps, this);
            }
            else
            {
                int num = this.m_SelectedIndex + steps;
                if (num < 0)
                {
                    num = 0;
                }
                ProfilerProperty property = this.m_Window.CreateProperty(this.m_DetailPane);
                if (this.m_DetailPane)
                {
                    ProfilerProperty detailedProperty = this.GetDetailedProperty(property);
                    property.Cleanup();
                    property = detailedProperty;
                }
                if (property != null)
                {
                    bool enterChildren = true;
                    int num2 = 0;
                    int instanceId = -1;
                    while (property.Next(enterChildren))
                    {
                        if ((this.m_DetailPane && (property.instanceIDs != null)) && ((property.instanceIDs.Length > 0) && (property.instanceIDs[0] != 0)))
                        {
                            instanceId = property.instanceIDs[0];
                        }
                        if (num2 == num)
                        {
                            break;
                        }
                        if (property.HasChildren)
                        {
                            enterChildren = !this.m_DetailPane && this.IsExpanded(property.propertyPath);
                        }
                        num2++;
                    }
                    if (this.m_DetailPane)
                    {
                        this.m_DetailViewSelectedProperty = DetailViewSelectedPropertyPath(property, instanceId);
                    }
                    else
                    {
                        this.m_Window.SetSelectedPropertyPath(property.propertyPath);
                    }
                    property.Cleanup();
                }
            }
        }

        private void RowMouseDown(string propertyPath)
        {
            if (propertyPath == ProfilerDriver.selectedPropertyPath)
            {
                this.m_Window.ClearSelectedPropertyPath();
            }
            else
            {
                this.m_Window.SetSelectedPropertyPath(propertyPath);
            }
        }

        private void SaveColumns()
        {
            string str = string.Empty;
            for (int i = 0; i < this.m_VisibleColumns.Length; i++)
            {
                str = str + (!this.ColIsVisible(i) ? '0' : '1');
            }
            EditorPrefs.SetString(this.m_ColumnSettingsName, str);
        }

        public void SelectFirstRow()
        {
            this.MoveSelection(-999999);
        }

        private static void SelectObjectsInHierarchyView(ProfilerProperty property)
        {
            int[] instanceIDs = property.instanceIDs;
            List<Object> list = new List<Object>();
            foreach (int num in instanceIDs)
            {
                Object item = EditorUtility.InstanceIDToObject(num);
                Component component = item as Component;
                if (component != null)
                {
                    list.Add(component.gameObject);
                }
                else if (item != null)
                {
                    list.Add(item);
                }
            }
            if (list.Count != 0)
            {
                Selection.objects = list.ToArray();
            }
        }

        private void SetColumnVisible(int index, bool enabled)
        {
            this.SetupSplitter();
            if ((index != 0) && (this.m_VisibleColumns[index] != enabled))
            {
                this.m_VisibleColumns[index] = enabled;
                int num = 0;
                for (int i = 0; i < index; i++)
                {
                    if (this.ColIsVisible(i))
                    {
                        num++;
                    }
                }
                if (enabled)
                {
                    ArrayUtility.Insert<float>(ref this.m_Splitter.relativeSizes, num, this.m_SplitterRelativeSizes[index]);
                    ArrayUtility.Insert<int>(ref this.m_Splitter.minSizes, num, this.m_SplitterMinWidths[index]);
                }
                else
                {
                    ArrayUtility.RemoveAt<float>(ref this.m_Splitter.relativeSizes, num);
                    ArrayUtility.RemoveAt<int>(ref this.m_Splitter.minSizes, num);
                }
                this.m_Splitter = new SplitterState(this.m_Splitter.relativeSizes, this.m_Splitter.minSizes, null);
                this.SaveColumns();
            }
        }

        private void SetExpanded(string expandedName, bool expanded)
        {
            if (expanded != this.IsExpanded(expandedName))
            {
                if (expanded)
                {
                    this.m_ExpandedHash.Set(expandedName);
                }
                else
                {
                    this.m_ExpandedHash.Remove(expandedName);
                }
            }
        }

        private void SetExpanded(ProfilerProperty property, bool expanded)
        {
            this.SetExpanded(property.propertyPath, expanded);
        }

        public void SetKeyboardFocus()
        {
            this.m_SetKeyboardFocus = true;
        }

        private void SetupSplitter()
        {
            if ((this.m_Splitter == null) || (this.m_SplitterMinWidths == null))
            {
                this.m_SplitterRelativeSizes = new float[this.m_ColumnNames.Length + 1];
                this.m_SplitterMinWidths = new int[this.m_ColumnNames.Length + 1];
                for (int i = 0; i < this.m_ColumnNames.Length; i++)
                {
                    this.m_SplitterMinWidths[i] = (int) styles.header.CalcSize(this.m_HeaderContent[i]).x;
                    this.m_SplitterRelativeSizes[i] = 70f;
                    if (this.m_HeaderContent[i].image != null)
                    {
                        this.m_SplitterRelativeSizes[i] = 1f;
                    }
                }
                this.m_SplitterMinWidths[this.m_ColumnNames.Length] = 0x10;
                this.m_SplitterRelativeSizes[this.m_ColumnNames.Length] = 0f;
                if (this.m_ColumnsToShow[0] == ProfilerColumn.FunctionName)
                {
                    this.m_SplitterRelativeSizes[0] = 400f;
                    this.m_SplitterMinWidths[0] = 100;
                }
                this.m_Splitter = new SplitterState(this.m_SplitterRelativeSizes, this.m_SplitterMinWidths, null);
                string str = EditorPrefs.GetString(this.m_ColumnSettingsName);
                for (int j = 0; j < this.m_ColumnNames.Length; j++)
                {
                    if ((j < str.Length) && (str[j] == '0'))
                    {
                        this.SetColumnVisible(j, false);
                    }
                }
            }
        }

        private void UnselectIfClickedOnEmptyArea(int rowCount)
        {
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinHeight(16f * rowCount) };
            Rect rect = GUILayoutUtility.GetRect(GUIClip.visibleRect.width, (float) (16f * rowCount), options);
            if (((Event.current.type == EventType.MouseDown) && (Event.current.mousePosition.y > rect.y)) && (Event.current.mousePosition.y < Screen.height))
            {
                if (!this.m_DetailPane)
                {
                    this.m_Window.ClearSelectedPropertyPath();
                }
                else
                {
                    this.m_DetailViewSelectedProperty = string.Empty;
                }
                Event.current.Use();
            }
        }

        public int selectedIndex
        {
            get
            {
                if (this.IsSearchActive())
                {
                    return this.m_SearchResults.selectedSearchIndex;
                }
                return this.m_SelectedIndex;
            }
            set
            {
                if (this.IsSearchActive())
                {
                    this.m_SearchResults.selectedSearchIndex = value;
                }
                else
                {
                    this.m_SelectedIndex = value;
                }
            }
        }

        public ProfilerColumn sortType
        {
            get
            {
                return this.m_SortType;
            }
            private set
            {
                this.m_SortType = value;
            }
        }

        protected static Styles styles
        {
            get
            {
                if (ms_Styles == null)
                {
                }
                return (ms_Styles = new Styles());
            }
        }

        internal class SearchResults
        {
            private ProfilerColumn[] m_ColumnsToShow;
            private bool m_FoundAllResults;
            private int m_LastFrameIndex;
            private string m_LastSearchString;
            private ProfilerColumn m_LastSortType;
            private int m_NumResultsUsed;
            private SearchResult[] m_SearchResults;
            private int m_SelectedSearchIndex;

            public void Draw(ProfilerHierarchyGUI gui, int controlID)
            {
                int num;
                int num2;
                this.HandleCommandEvents(gui);
                Event current = Event.current;
                string selectedPropertyPath = ProfilerDriver.selectedPropertyPath;
                GetFirstAndLastRowVisible(this.m_NumResultsUsed, 16f, gui.m_TextScroll.y, (float) gui.m_ScrollViewHeight, out num, out num2);
                for (int i = num; i <= num2; i++)
                {
                    bool on = selectedPropertyPath == this.m_SearchResults[i].propertyPath;
                    Rect rowRect = gui.GetRowRect(i);
                    GUIStyle rowBackgroundStyle = gui.GetRowBackgroundStyle(i);
                    if ((current.type == EventType.MouseDown) && rowRect.Contains(current.mousePosition))
                    {
                        this.m_SelectedSearchIndex = i;
                        gui.RowMouseDown(this.m_SearchResults[i].propertyPath);
                        GUIUtility.keyboardControl = controlID;
                        current.Use();
                    }
                    if (current.type == EventType.Repaint)
                    {
                        rowBackgroundStyle.Draw(rowRect, GUIContent.none, false, false, on, GUIUtility.keyboardControl == controlID);
                        if (rowRect.Contains(current.mousePosition))
                        {
                            string tooltip = this.m_SearchResults[i].propertyPath.Replace("/", "/\n");
                            if (this.m_SelectedSearchIndex >= 0)
                            {
                                tooltip = tooltip + "\n\n(Press 'F' to frame selection)";
                            }
                            GUI.Label(rowRect, GUIContent.Temp(string.Empty, tooltip));
                        }
                        gui.DrawTextColumn(ref rowRect, this.m_SearchResults[i].columnValues[0], 0, 4f, on);
                        ProfilerHierarchyGUI.styles.numberLabel.alignment = TextAnchor.MiddleRight;
                        int index = 1;
                        for (int j = 1; j < gui.m_VisibleColumns.Length; j++)
                        {
                            if (gui.ColIsVisible(j))
                            {
                                rowRect.x += gui.m_Splitter.realSizes[index - 1];
                                rowRect.width = gui.m_Splitter.realSizes[index] - 4f;
                                index++;
                                ProfilerHierarchyGUI.styles.numberLabel.Draw(rowRect, this.m_SearchResults[i].columnValues[j], false, false, false, on);
                            }
                        }
                        ProfilerHierarchyGUI.styles.numberLabel.alignment = TextAnchor.MiddleLeft;
                    }
                }
                if (!this.m_FoundAllResults && (current.type == EventType.Repaint))
                {
                    int numResultsUsed = this.m_NumResultsUsed;
                    Rect position = new Rect(1f, 16f * numResultsUsed, GUIClip.visibleRect.width, 16f);
                    GUIStyle style2 = ((numResultsUsed % 2) != 0) ? ProfilerHierarchyGUI.styles.entryOdd : ProfilerHierarchyGUI.styles.entryEven;
                    GUI.Label(position, GUIContent.Temp(string.Empty, ProfilerHierarchyGUI.styles.notShowingAllResults.tooltip), GUIStyle.none);
                    style2.Draw(position, GUIContent.none, false, false, false, false);
                    gui.DrawTextColumn(ref position, ProfilerHierarchyGUI.styles.notShowingAllResults.text, 0, 4f, false);
                }
            }

            public void Filter(ProfilerProperty property, ProfilerColumn[] columns, string searchString, int frameIndex, ProfilerColumn sortType)
            {
                if (((searchString != this.m_LastSearchString) || (frameIndex != this.m_LastFrameIndex)) || (sortType != this.m_LastSortType))
                {
                    this.m_LastSearchString = searchString;
                    this.m_LastFrameIndex = frameIndex;
                    this.m_LastSortType = sortType;
                    this.IterateProfilingData(property, columns, searchString);
                }
            }

            private static void GetFirstAndLastRowVisible(int numRows, float rowHeight, float scrollBarY, float scrollAreaHeight, out int firstRowVisible, out int lastRowVisible)
            {
                firstRowVisible = (int) Mathf.Floor(scrollBarY / rowHeight);
                lastRowVisible = firstRowVisible + ((int) Mathf.Ceil(scrollAreaHeight / rowHeight));
                firstRowVisible = Mathf.Max(firstRowVisible, 0);
                lastRowVisible = Mathf.Min(lastRowVisible, numRows - 1);
            }

            private void HandleCommandEvents(ProfilerHierarchyGUI gui)
            {
                Event current = Event.current;
                EventType type = current.type;
                switch (type)
                {
                    case EventType.ExecuteCommand:
                    case EventType.ValidateCommand:
                    {
                        bool flag = type == EventType.ExecuteCommand;
                        if (Event.current.commandName == "FrameSelected")
                        {
                            if (flag)
                            {
                                gui.FrameSelection();
                            }
                            current.Use();
                        }
                        break;
                    }
                }
            }

            public void Init(int maxNumberSearchResults)
            {
                this.m_SearchResults = new SearchResult[maxNumberSearchResults];
                this.m_NumResultsUsed = 0;
                this.m_LastSearchString = string.Empty;
                this.m_LastFrameIndex = -1;
                this.m_FoundAllResults = false;
                this.m_ColumnsToShow = null;
                this.m_SelectedSearchIndex = -1;
            }

            private void IterateProfilingData(ProfilerProperty property, ProfilerColumn[] columns, string searchString)
            {
                this.m_NumResultsUsed = 0;
                this.m_ColumnsToShow = columns;
                this.m_FoundAllResults = true;
                this.m_SelectedSearchIndex = -1;
                int index = 0;
                string selectedPropertyPath = ProfilerDriver.selectedPropertyPath;
                while (property.Next(true))
                {
                    if (index >= this.m_SearchResults.Length)
                    {
                        this.m_FoundAllResults = false;
                        break;
                    }
                    string propertyPath = property.propertyPath;
                    int startIndex = Mathf.Max(propertyPath.LastIndexOf('/'), 0);
                    if (propertyPath.IndexOf(searchString, startIndex, StringComparison.CurrentCultureIgnoreCase) > -1)
                    {
                        string[] strArray = new string[this.m_ColumnsToShow.Length];
                        for (int i = 0; i < this.m_ColumnsToShow.Length; i++)
                        {
                            strArray[i] = property.GetColumn(this.m_ColumnsToShow[i]);
                        }
                        this.m_SearchResults[index].propertyPath = propertyPath;
                        this.m_SearchResults[index].columnValues = strArray;
                        if (propertyPath == selectedPropertyPath)
                        {
                            this.m_SelectedSearchIndex = index;
                        }
                        index++;
                    }
                }
                this.m_NumResultsUsed = index;
            }

            public void MoveSelection(int steps, ProfilerHierarchyGUI gui)
            {
                int index = Mathf.Clamp(this.m_SelectedSearchIndex + steps, 0, this.m_NumResultsUsed - 1);
                if (index != this.m_SelectedSearchIndex)
                {
                    this.m_SelectedSearchIndex = index;
                    gui.m_Window.SetSelectedPropertyPath(this.m_SearchResults[index].propertyPath);
                }
            }

            public int numRows
            {
                get
                {
                    return (this.m_NumResultsUsed + (!this.m_FoundAllResults ? 1 : 0));
                }
            }

            public int selectedSearchIndex
            {
                get
                {
                    return this.m_SelectedSearchIndex;
                }
                set
                {
                    if (value < this.m_NumResultsUsed)
                    {
                        this.m_SelectedSearchIndex = value;
                    }
                    else
                    {
                        this.m_SelectedSearchIndex = -1;
                    }
                    if (this.m_SelectedSearchIndex >= 0)
                    {
                        string propertyPath = this.m_SearchResults[this.m_SelectedSearchIndex].propertyPath;
                        if (propertyPath != ProfilerDriver.selectedPropertyPath)
                        {
                            ProfilerDriver.selectedPropertyPath = propertyPath;
                        }
                    }
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            private struct SearchResult
            {
                public string propertyPath;
                public string[] columnValues;
            }
        }

        internal class Styles
        {
            public GUIStyle background = "OL Box";
            public GUIContent disabledSearchText = new GUIContent("Showing search results are disabled while recording with deep profiling.\nStop recording to view search results.");
            public GUIStyle entryEven = "OL EntryBackEven";
            public GUIStyle entryOdd = "OL EntryBackOdd";
            public GUIStyle foldout = "IN foldout";
            public GUIStyle header = "OL title";
            public GUIContent instrumentationIcon = EditorGUIUtility.IconContent("Profiler.Record", "Record|Record profiling information");
            public GUIStyle miniPullDown = "MiniPullDown";
            public GUIContent notShowingAllResults = new GUIContent("...", "Narrow your search. Not all search results can be shown.");
            public GUIStyle numberLabel = "OL Label";
            public GUIStyle rightHeader = "OL title TextRight";
        }
    }
}

