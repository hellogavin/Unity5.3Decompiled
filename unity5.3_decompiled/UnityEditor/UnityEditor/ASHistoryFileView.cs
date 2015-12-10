namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEditorInternal;
    using UnityEngine;

    [Serializable]
    internal class ASHistoryFileView
    {
        private GUIContent[] dropDownMenuItems = new GUIContent[] { new GUIContent("Recover") };
        private float m_BaseIndent = 16f;
        private DeletedAsset[] m_DeletedItems;
        private bool m_DeletedItemsInitialized;
        private bool m_DeletedItemsToggle;
        private ParentViewState m_DelPVstate = new ParentViewState();
        private int[] m_ExpandedArray = new int[0];
        private int m_FileViewControlID = -1;
        private float m_FoldoutSize = 14f;
        private float m_Indent = 16f;
        private static float m_RowHeight = 16f;
        private Rect m_ScreenRect;
        public Vector2 m_ScrollPosition;
        private SelectionType m_SelType;
        private float m_SpaceAtTheTop = (m_RowHeight + 6f);
        private static int ms_FileViewHash = "FileView".GetHashCode();
        private static Styles ms_Styles = null;
        private static bool OSX = (Application.platform == RuntimePlatform.OSXEditor);

        public ASHistoryFileView()
        {
            this.m_DelPVstate.lv = new ListViewState(0);
            this.m_DelPVstate.lv.totalRows = 0;
        }

        private void AllProjectKeyboard()
        {
            if ((Event.current.keyCode == KeyCode.DownArrow) && (this.GetFirst() != null))
            {
                Selection.activeObject = this.GetFirst().pptrValue;
                this.FrameObject(Selection.activeObject);
                this.SelType = SelectionType.Items;
                Event.current.Use();
            }
        }

        private void AssetViewKeyboard()
        {
            switch (Event.current.keyCode)
            {
                case KeyCode.KeypadEnter:
                case KeyCode.Return:
                    if (Application.platform == RuntimePlatform.WindowsEditor)
                    {
                        this.OpenAssetSelection();
                        GUIUtility.ExitGUI();
                    }
                    break;

                case KeyCode.UpArrow:
                {
                    Event.current.Use();
                    HierarchyProperty firstSelected = this.GetFirstSelected();
                    if (firstSelected != null)
                    {
                        if (firstSelected.instanceID != this.GetFirst().instanceID)
                        {
                            if (firstSelected.Previous(this.m_ExpandedArray))
                            {
                                Object pptrValue = firstSelected.pptrValue;
                                this.SelectionClick(firstSelected);
                                this.FrameObject(pptrValue);
                            }
                        }
                        else
                        {
                            this.SelType = SelectionType.All;
                            Selection.objects = new Object[0];
                            this.ScrollTo(0f);
                        }
                    }
                    break;
                }
                case KeyCode.DownArrow:
                {
                    Event.current.Use();
                    HierarchyProperty lastSelected = this.GetLastSelected();
                    if ((Application.platform != RuntimePlatform.OSXEditor) || !Event.current.command)
                    {
                        if (lastSelected != null)
                        {
                            if (lastSelected.instanceID == this.GetLast().instanceID)
                            {
                                this.SelType = SelectionType.DeletedItemsRoot;
                                Selection.objects = new Object[0];
                                this.ScrollToDeletedItem(-1);
                            }
                            else if (lastSelected.Next(this.m_ExpandedArray))
                            {
                                Object target = lastSelected.pptrValue;
                                this.SelectionClick(lastSelected);
                                this.FrameObject(target);
                            }
                        }
                    }
                    else
                    {
                        this.OpenAssetSelection();
                        GUIUtility.ExitGUI();
                    }
                    break;
                }
                case KeyCode.RightArrow:
                {
                    HierarchyProperty activeSelected = this.GetActiveSelected();
                    if (activeSelected != null)
                    {
                        this.SetExpanded(activeSelected.instanceID, true);
                    }
                    break;
                }
                case KeyCode.LeftArrow:
                {
                    HierarchyProperty property = this.GetActiveSelected();
                    if (property != null)
                    {
                        this.SetExpanded(property.instanceID, false);
                    }
                    break;
                }
                case KeyCode.Home:
                    if (this.GetFirst() != null)
                    {
                        Selection.activeObject = this.GetFirst().pptrValue;
                        this.FrameObject(Selection.activeObject);
                    }
                    break;

                case KeyCode.End:
                    if (this.GetLast() != null)
                    {
                        Selection.activeObject = this.GetLast().pptrValue;
                        this.FrameObject(Selection.activeObject);
                    }
                    break;

                case KeyCode.PageUp:
                {
                    Event.current.Use();
                    if (OSX)
                    {
                        this.m_ScrollPosition.y -= this.m_ScreenRect.height;
                        if (this.m_ScrollPosition.y < 0f)
                        {
                            this.m_ScrollPosition.y = 0f;
                        }
                        break;
                    }
                    HierarchyProperty first = this.GetFirstSelected();
                    if (first == null)
                    {
                        if (this.GetFirst() != null)
                        {
                            Selection.activeObject = this.GetFirst().pptrValue;
                            this.FrameObject(Selection.activeObject);
                        }
                        break;
                    }
                    for (int i = 0; i < (this.m_ScreenRect.height / m_RowHeight); i++)
                    {
                        if (!first.Previous(this.m_ExpandedArray))
                        {
                            first = this.GetFirst();
                            break;
                        }
                    }
                    Object obj4 = first.pptrValue;
                    this.SelectionClick(first);
                    this.FrameObject(obj4);
                    break;
                }
                case KeyCode.PageDown:
                    Event.current.Use();
                    if (!OSX)
                    {
                        HierarchyProperty last = this.GetLastSelected();
                        if (last == null)
                        {
                            if (this.GetLast() != null)
                            {
                                Selection.activeObject = this.GetLast().pptrValue;
                                this.FrameObject(Selection.activeObject);
                            }
                            break;
                        }
                        for (int j = 0; j < (this.m_ScreenRect.height / m_RowHeight); j++)
                        {
                            if (!last.Next(this.m_ExpandedArray))
                            {
                                last = this.GetLast();
                                break;
                            }
                        }
                        Object obj5 = last.pptrValue;
                        this.SelectionClick(last);
                        this.FrameObject(obj5);
                        break;
                    }
                    this.m_ScrollPosition.y += this.m_ScreenRect.height;
                    break;

                default:
                    return;
            }
            Event.current.Use();
        }

        private void ContextMenuClick(object userData, string[] options, int selected)
        {
            if ((selected >= 0) && (selected == 0))
            {
                this.DoRecover();
            }
        }

        private int ControlIDForProperty(HierarchyProperty property)
        {
            if (property != null)
            {
                return (property.row + 0x989680);
            }
            return -1;
        }

        private void DeletedItemsKeyboard(ASHistoryWindow parentWin)
        {
            int row = this.m_DelPVstate.lv.row;
            int currSelected = row;
            if (this.DeletedItemsToggle)
            {
                switch (Event.current.keyCode)
                {
                    case KeyCode.UpArrow:
                        if (currSelected <= 0)
                        {
                            this.SelType = SelectionType.DeletedItemsRoot;
                            this.ScrollToDeletedItem(-1);
                            parentWin.DoLocalSelectionChange();
                        }
                        else
                        {
                            currSelected--;
                        }
                        goto Label_01AE;

                    case KeyCode.DownArrow:
                        if (currSelected < (this.m_DelPVstate.lv.totalRows - 1))
                        {
                            currSelected++;
                        }
                        goto Label_01AE;

                    case KeyCode.Home:
                        currSelected = 0;
                        goto Label_01AE;

                    case KeyCode.End:
                        currSelected = this.m_DelPVstate.lv.totalRows - 1;
                        goto Label_01AE;

                    case KeyCode.PageUp:
                        if (!OSX)
                        {
                            currSelected -= (int) (this.m_ScreenRect.height / m_RowHeight);
                            if (currSelected < 0)
                            {
                                currSelected = 0;
                            }
                        }
                        else
                        {
                            this.m_ScrollPosition.y -= this.m_ScreenRect.height;
                            if (this.m_ScrollPosition.y < 0f)
                            {
                                this.m_ScrollPosition.y = 0f;
                            }
                        }
                        goto Label_01AE;

                    case KeyCode.PageDown:
                        if (!OSX)
                        {
                            currSelected += (int) (this.m_ScreenRect.height / m_RowHeight);
                            if (currSelected > (this.m_DelPVstate.lv.totalRows - 1))
                            {
                                currSelected = this.m_DelPVstate.lv.totalRows - 1;
                            }
                        }
                        else
                        {
                            this.m_ScrollPosition.y += this.m_ScreenRect.height;
                        }
                        goto Label_01AE;
                }
            }
            return;
        Label_01AE:
            Event.current.Use();
            if (currSelected != row)
            {
                this.m_DelPVstate.lv.row = currSelected;
                ListViewShared.MultiSelection(null, row, currSelected, ref this.m_DelPVstate.initialSelectedItem, ref this.m_DelPVstate.selectedItems);
                this.ScrollToDeletedItem(currSelected);
                parentWin.DoLocalSelectionChange();
            }
        }

        private void DeletedItemsRootKeyboard(ASHistoryWindow parentWin)
        {
            switch (Event.current.keyCode)
            {
                case KeyCode.UpArrow:
                    this.SelType = SelectionType.Items;
                    if (this.GetLast() != null)
                    {
                        Selection.activeObject = this.GetLast().pptrValue;
                        this.FrameObject(Selection.activeObject);
                    }
                    break;

                case KeyCode.DownArrow:
                    if ((this.m_DelPVstate.selectedItems.Length > 0) && this.DeletedItemsToggle)
                    {
                        this.SelType = SelectionType.DeletedItems;
                        this.m_DelPVstate.selectedItems[0] = true;
                        this.m_DelPVstate.lv.row = 0;
                        this.ScrollToDeletedItem(0);
                    }
                    break;

                case KeyCode.RightArrow:
                    this.DeletedItemsToggle = true;
                    break;

                case KeyCode.LeftArrow:
                    this.DeletedItemsToggle = false;
                    break;

                default:
                    return;
            }
            if (this.SelType != SelectionType.Items)
            {
                parentWin.DoLocalSelectionChange();
            }
            Event.current.Use();
        }

        public void DoDeletedItemsGUI(ASHistoryWindow parentWin, Rect theRect, GUIStyle s, float offset, float endOffset, bool focused)
        {
            Event current = Event.current;
            Texture2D textured = EditorGUIUtility.FindTexture(EditorResourcesUtility.folderIconName);
            offset += 3f;
            Rect position = new Rect(this.m_Indent, offset, theRect.width - this.m_Indent, m_RowHeight);
            if ((current.type == EventType.MouseDown) && position.Contains(current.mousePosition))
            {
                GUIUtility.keyboardControl = this.m_FileViewControlID;
                this.SelType = SelectionType.DeletedItemsRoot;
                this.ScrollToDeletedItem(-1);
                parentWin.DoLocalSelectionChange();
            }
            position.width -= position.x;
            position.x = 0f;
            GUIContent content = new GUIContent("Deleted Assets") {
                image = textured
            };
            int baseIndent = (int) this.m_BaseIndent;
            s.padding.left = baseIndent;
            if (current.type == EventType.Repaint)
            {
                s.Draw(position, content, false, false, this.SelType == SelectionType.DeletedItemsRoot, focused);
            }
            Rect rect2 = new Rect(this.m_BaseIndent - this.m_FoldoutSize, offset, this.m_FoldoutSize, m_RowHeight);
            if (!this.m_DeletedItemsInitialized || (this.m_DelPVstate.lv.totalRows != 0))
            {
                this.DeletedItemsToggle = GUI.Toggle(rect2, this.DeletedItemsToggle, GUIContent.none, ms_Styles.foldout);
            }
            offset += m_RowHeight;
            if (this.DeletedItemsToggle)
            {
                int row = this.m_DelPVstate.lv.row;
                int index = 0;
                int num4 = -1;
                int file = -1;
                int num6 = 0;
                while ((offset <= endOffset) && (num6 < this.m_DelPVstate.lv.totalRows))
                {
                    if ((offset + m_RowHeight) >= 0f)
                    {
                        if (num4 == -1)
                        {
                            this.m_DelPVstate.IndexToFolderAndFile(num6, ref num4, ref file);
                        }
                        position = new Rect(0f, offset, (float) Screen.width, m_RowHeight);
                        ParentViewFolder folder = this.m_DelPVstate.folders[num4];
                        if ((current.type == EventType.MouseDown) && position.Contains(current.mousePosition))
                        {
                            if (!(((current.button == 1) && (this.SelType == SelectionType.DeletedItems)) && this.m_DelPVstate.selectedItems[index]))
                            {
                                GUIUtility.keyboardControl = this.m_FileViewControlID;
                                this.SelType = SelectionType.DeletedItems;
                                this.m_DelPVstate.lv.row = index;
                                ListViewShared.MultiSelection(null, row, this.m_DelPVstate.lv.row, ref this.m_DelPVstate.initialSelectedItem, ref this.m_DelPVstate.selectedItems);
                                this.ScrollToDeletedItem(index);
                                parentWin.DoLocalSelectionChange();
                            }
                            if ((current.button == 1) && (this.SelType == SelectionType.DeletedItems))
                            {
                                GUIUtility.hotControl = 0;
                                Rect rect3 = new Rect(current.mousePosition.x, current.mousePosition.y, 1f, 1f);
                                EditorUtility.DisplayCustomMenu(rect3, this.dropDownMenuItems, -1, new EditorUtility.SelectMenuItemFunction(this.ContextMenuClick), null);
                            }
                            Event.current.Use();
                        }
                        if (file != -1)
                        {
                            content.text = folder.files[file].name;
                            content.image = InternalEditorUtility.GetIconForFile(folder.files[file].name);
                            baseIndent = (int) (this.m_BaseIndent + (this.m_Indent * 2f));
                        }
                        else
                        {
                            content.text = folder.name;
                            content.image = textured;
                            baseIndent = (int) (this.m_BaseIndent + this.m_Indent);
                        }
                        s.padding.left = baseIndent;
                        if (Event.current.type == EventType.Repaint)
                        {
                            s.Draw(position, content, false, false, this.m_DelPVstate.selectedItems[index], focused);
                        }
                        this.m_DelPVstate.NextFileFolder(ref num4, ref file);
                        index++;
                    }
                    num6++;
                    offset += m_RowHeight;
                }
            }
        }

        private void DoFramingMindSelectionType()
        {
            switch (this.m_SelType)
            {
                case SelectionType.All:
                    this.ScrollTo(0f);
                    break;

                case SelectionType.Items:
                    this.FrameObject(Selection.activeObject);
                    break;

                case SelectionType.DeletedItemsRoot:
                    this.ScrollToDeletedItem(-1);
                    break;

                case SelectionType.DeletedItems:
                    this.ScrollToDeletedItem(this.m_DelPVstate.lv.row);
                    break;
            }
        }

        public void DoGUI(ASHistoryWindow parentWin, Rect theRect, bool focused)
        {
            if (ms_Styles == null)
            {
                ms_Styles = new Styles();
            }
            this.m_ScreenRect = theRect;
            Hashtable hashtable = new Hashtable();
            foreach (Object obj2 in Selection.objects)
            {
                hashtable.Add(obj2.GetInstanceID(), null);
            }
            this.m_FileViewControlID = GUIUtility.GetControlID(ms_FileViewHash, FocusType.Native);
            this.KeyboardGUI(parentWin);
            focused &= GUIUtility.keyboardControl == this.m_FileViewControlID;
            HierarchyProperty property = new HierarchyProperty(HierarchyType.Assets);
            int num2 = property.CountRemaining(this.m_ExpandedArray);
            int num3 = !this.DeletedItemsToggle ? 0 : this.m_DelPVstate.lv.totalRows;
            Rect viewRect = new Rect(0f, 0f, 1f, (((num2 + 2) + num3) * m_RowHeight) + 16f);
            this.m_ScrollPosition = GUI.BeginScrollView(this.m_ScreenRect, this.m_ScrollPosition, viewRect);
            theRect.width = (viewRect.height <= this.m_ScreenRect.height) ? theRect.width : (theRect.width - 18f);
            int count = Mathf.RoundToInt((this.m_ScrollPosition.y - 6f) - m_RowHeight) / Mathf.RoundToInt(m_RowHeight);
            if (count > num2)
            {
                count = num2;
            }
            else if (count < 0)
            {
                count = 0;
                this.m_ScrollPosition.y = 0f;
            }
            float y = 0f;
            GUIContent content = new GUIContent();
            Event current = Event.current;
            GUIStyle s = new GUIStyle(ms_Styles.label);
            Texture2D textured = EditorGUIUtility.FindTexture(EditorResourcesUtility.folderIconName);
            y = (count * m_RowHeight) + 3f;
            float endOffset = this.m_ScreenRect.height + this.m_ScrollPosition.y;
            Rect position = new Rect(0f, y, theRect.width, m_RowHeight);
            if ((current.type == EventType.MouseDown) && position.Contains(current.mousePosition))
            {
                this.SelType = SelectionType.All;
                GUIUtility.keyboardControl = this.m_FileViewControlID;
                this.ScrollTo(0f);
                parentWin.DoLocalSelectionChange();
                current.Use();
            }
            content = new GUIContent("Entire Project") {
                image = textured
            };
            int baseIndent = (int) this.m_BaseIndent;
            s.padding.left = 3;
            if (Event.current.type == EventType.Repaint)
            {
                s.Draw(position, content, false, false, this.SelType == SelectionType.All, focused);
            }
            y += m_RowHeight + 3f;
            property.Reset();
            property.Skip(count, this.m_ExpandedArray);
            while (property.Next(this.m_ExpandedArray) && (y <= endOffset))
            {
                int instanceID = property.instanceID;
                position = new Rect(0f, y, theRect.width, m_RowHeight);
                if (Event.current.type == EventType.Repaint)
                {
                    content.text = property.name;
                    content.image = property.icon;
                    baseIndent = (int) (this.m_BaseIndent + (this.m_Indent * property.depth));
                    s.padding.left = baseIndent;
                    bool on = hashtable.Contains(instanceID);
                    s.Draw(position, content, false, false, on, focused);
                }
                if (property.hasChildren)
                {
                    bool flag2 = property.IsExpanded(this.m_ExpandedArray);
                    GUI.changed = false;
                    Rect rect3 = new Rect((this.m_BaseIndent + (this.m_Indent * property.depth)) - this.m_FoldoutSize, y, this.m_FoldoutSize, m_RowHeight);
                    flag2 = GUI.Toggle(rect3, flag2, GUIContent.none, ms_Styles.foldout);
                    if (GUI.changed)
                    {
                        if (Event.current.alt)
                        {
                            this.SetExpandedRecurse(instanceID, flag2);
                        }
                        else
                        {
                            this.SetExpanded(instanceID, flag2);
                        }
                    }
                }
                if (((current.type == EventType.MouseDown) && (Event.current.button == 0)) && position.Contains(Event.current.mousePosition))
                {
                    GUIUtility.keyboardControl = this.m_FileViewControlID;
                    if (Event.current.clickCount == 2)
                    {
                        AssetDatabase.OpenAsset(instanceID);
                        GUIUtility.ExitGUI();
                    }
                    else if (position.Contains(current.mousePosition))
                    {
                        this.SelectionClick(property);
                    }
                    current.Use();
                }
                y += m_RowHeight;
            }
            y += 3f;
            this.DoDeletedItemsGUI(parentWin, theRect, s, y, endOffset, focused);
            GUI.EndScrollView();
            switch (current.type)
            {
                case EventType.MouseDown:
                    if ((current.button == 0) && this.m_ScreenRect.Contains(current.mousePosition))
                    {
                        GUIUtility.hotControl = this.m_FileViewControlID;
                        current.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == this.m_FileViewControlID)
                    {
                        if (this.m_ScreenRect.Contains(current.mousePosition))
                        {
                            Selection.activeObject = null;
                        }
                        GUIUtility.hotControl = 0;
                        current.Use();
                    }
                    break;
            }
            this.HandleFraming();
        }

        public void DoRecover()
        {
            string[] selectedDeletedItemGUIDs = this.GetSelectedDeletedItemGUIDs();
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            int num = 0;
            for (int i = 0; i < selectedDeletedItemGUIDs.Length; i++)
            {
                for (int j = 0; j < this.m_DeletedItems.Length; j++)
                {
                    if (this.m_DeletedItems[j].guid == selectedDeletedItemGUIDs[i])
                    {
                        dictionary[this.m_DeletedItems[j].guid] = j;
                        break;
                    }
                }
            }
            DeletedAsset[] assets = new DeletedAsset[dictionary.Count];
            while (dictionary.Count != 0)
            {
                DeletedAsset asset = null;
                foreach (KeyValuePair<string, int> pair in dictionary)
                {
                    asset = this.m_DeletedItems[pair.Value];
                    if (!dictionary.ContainsKey(asset.parent))
                    {
                        assets[num++] = asset;
                        break;
                    }
                }
                dictionary.Remove(asset.guid);
            }
            AssetServer.SetAfterActionFinishedCallback("ASEditorBackend", "CBReinitASMainWindow");
            AssetServer.DoRecoverOnNextTick(assets);
        }

        public void FilterItems(string filterText)
        {
        }

        private bool FrameObject(Object target)
        {
            if (target != null)
            {
                HierarchyProperty property = new HierarchyProperty(HierarchyType.Assets);
                if (property.Find(target.GetInstanceID(), null))
                {
                    while (property.Parent())
                    {
                        this.SetExpanded(property.instanceID, true);
                    }
                }
                property.Reset();
                if (property.Find(target.GetInstanceID(), this.m_ExpandedArray))
                {
                    this.ScrollTo((m_RowHeight * property.row) + this.m_SpaceAtTheTop);
                    return true;
                }
            }
            return false;
        }

        private HierarchyProperty GetActiveSelected()
        {
            return this.GetFirstSelected();
        }

        public string[] GetAllDeletedItemGUIDs()
        {
            if (!this.m_DeletedItemsInitialized)
            {
                this.SetupDeletedItems();
            }
            string[] strArray = new string[this.m_DeletedItems.Length];
            for (int i = 0; i < strArray.Length; i++)
            {
                strArray[i] = this.m_DeletedItems[i].guid;
            }
            return strArray;
        }

        private HierarchyProperty GetFirst()
        {
            HierarchyProperty property = new HierarchyProperty(HierarchyType.Assets);
            if (property.Next(this.m_ExpandedArray))
            {
                return property;
            }
            return null;
        }

        private HierarchyProperty GetFirstSelected()
        {
            int row = 0x3b9aca00;
            HierarchyProperty property = null;
            foreach (Object obj2 in Selection.objects)
            {
                HierarchyProperty property2 = new HierarchyProperty(HierarchyType.Assets);
                if (property2.Find(obj2.GetInstanceID(), this.m_ExpandedArray) && (property2.row < row))
                {
                    row = property2.row;
                    property = property2;
                }
            }
            return property;
        }

        public string[] GetImplicitProjectViewSelection()
        {
            bool flag2;
            HierarchyProperty property = new HierarchyProperty(HierarchyType.Assets);
            bool retHasSelectionInside = false;
            if (!property.Next(null))
            {
                return new string[0];
            }
            Hashtable selection = new Hashtable();
            foreach (Object obj2 in Selection.objects)
            {
                selection.Add(obj2.GetInstanceID(), null);
            }
            List<int> list = this.GetOneFolderImplicitSelection(property, selection, false, ref retHasSelectionInside, out flag2);
            string[] strArray = new string[list.Count];
            for (int i = 0; i < strArray.Length; i++)
            {
                strArray[i] = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(list[i]));
            }
            return strArray;
        }

        private HierarchyProperty GetLast()
        {
            HierarchyProperty property = new HierarchyProperty(HierarchyType.Assets);
            int count = property.CountRemaining(this.m_ExpandedArray);
            property.Reset();
            if (property.Skip(count, this.m_ExpandedArray))
            {
                return property;
            }
            return null;
        }

        private HierarchyProperty GetLastSelected()
        {
            int row = -1;
            HierarchyProperty property = null;
            foreach (Object obj2 in Selection.objects)
            {
                HierarchyProperty property2 = new HierarchyProperty(HierarchyType.Assets);
                if (property2.Find(obj2.GetInstanceID(), this.m_ExpandedArray) && (property2.row > row))
                {
                    row = property2.row;
                    property = property2;
                }
            }
            return property;
        }

        private List<int> GetOneFolderImplicitSelection(HierarchyProperty property, Hashtable selection, bool rootSelected, ref bool retHasSelectionInside, out bool eof)
        {
            int depth = property.depth;
            bool flag = false;
            bool flag2 = false;
            eof = false;
            List<int> list = new List<int>();
            List<int> list2 = new List<int>();
            List<int> collection = new List<int>();
            do
            {
                if (property.depth > depth)
                {
                    collection.AddRange(this.GetOneFolderImplicitSelection(property, selection, rootSelected || flag2, ref flag, out eof));
                }
                if ((depth != property.depth) || eof)
                {
                    break;
                }
                if (rootSelected && !flag)
                {
                    list.Add(property.instanceID);
                }
                if (selection.Contains(property.instanceID))
                {
                    list2.Add(property.instanceID);
                    flag = true;
                    flag2 = true;
                }
                else
                {
                    flag2 = false;
                }
                eof = !property.Next(null);
            }
            while (!eof);
            retHasSelectionInside |= flag;
            List<int> list4 = (rootSelected && !flag) ? list : list2;
            list4.AddRange(collection);
            return list4;
        }

        public string[] GetSelectedDeletedItemGUIDs()
        {
            List<string> list = new List<string>();
            int index = 0;
            for (int i = 0; i < this.m_DelPVstate.folders.Length; i++)
            {
                ParentViewFolder folder = this.m_DelPVstate.folders[i];
                if (this.m_DelPVstate.selectedItems[index])
                {
                    list.Add(folder.guid);
                }
                for (int j = 0; j < folder.files.Length; j++)
                {
                    index++;
                    if (this.m_DelPVstate.selectedItems[index])
                    {
                        list.Add(folder.files[j].guid);
                    }
                }
                index++;
            }
            return list.ToArray();
        }

        private void HandleFraming()
        {
            if (((Event.current.type == EventType.ExecuteCommand) || (Event.current.type == EventType.ValidateCommand)) && (Event.current.commandName == "FrameSelected"))
            {
                if (Event.current.type == EventType.ExecuteCommand)
                {
                    this.DoFramingMindSelectionType();
                }
                HandleUtility.Repaint();
                Event.current.Use();
            }
        }

        private void KeyboardGUI(ASHistoryWindow parentWin)
        {
            if ((Event.current.GetTypeForControl(this.m_FileViewControlID) == EventType.KeyDown) && (this.m_FileViewControlID == GUIUtility.keyboardControl))
            {
                switch (this.SelType)
                {
                    case SelectionType.All:
                        this.AllProjectKeyboard();
                        break;

                    case SelectionType.Items:
                        this.AssetViewKeyboard();
                        break;

                    case SelectionType.DeletedItemsRoot:
                        this.DeletedItemsRootKeyboard(parentWin);
                        break;

                    case SelectionType.DeletedItems:
                        this.DeletedItemsKeyboard(parentWin);
                        break;
                }
            }
        }

        private void OpenAssetSelection()
        {
            foreach (int num in Selection.instanceIDs)
            {
                if (AssetDatabase.Contains(num))
                {
                    AssetDatabase.OpenAsset(num);
                }
            }
        }

        private void ScrollTo(float scrollTop)
        {
            float min = (scrollTop - this.m_ScreenRect.height) + m_RowHeight;
            this.m_ScrollPosition.y = Mathf.Clamp(this.m_ScrollPosition.y, min, scrollTop);
        }

        private void ScrollToDeletedItem(int index)
        {
            HierarchyProperty property = new HierarchyProperty(HierarchyType.Assets);
            float scrollTop = (this.m_SpaceAtTheTop + (property.CountRemaining(this.m_ExpandedArray) * m_RowHeight)) + 6f;
            if (index == -1)
            {
                this.ScrollTo(scrollTop);
            }
            else
            {
                this.ScrollTo(scrollTop + ((index + 1) * m_RowHeight));
            }
        }

        public void SelectDeletedItem(string guid)
        {
            this.SelType = SelectionType.DeletedItems;
            this.DeletedItemsToggle = true;
            int index = 0;
            for (int i = 0; i < this.m_DelPVstate.folders.Length; i++)
            {
                ParentViewFolder folder = this.m_DelPVstate.folders[i];
                this.m_DelPVstate.selectedItems[index] = false;
                if (folder.guid == guid)
                {
                    this.m_DelPVstate.selectedItems[index] = true;
                    this.ScrollToDeletedItem(index);
                    return;
                }
                for (int j = 0; j < folder.files.Length; j++)
                {
                    index++;
                    this.m_DelPVstate.selectedItems[index] = false;
                    if (folder.files[j].guid == guid)
                    {
                        this.m_DelPVstate.selectedItems[index] = true;
                        this.ScrollToDeletedItem(index);
                        return;
                    }
                }
                index++;
            }
        }

        private void SelectionClick(HierarchyProperty property)
        {
            if (EditorGUI.actionKey)
            {
                ArrayList list = new ArrayList();
                list.AddRange(Selection.objects);
                if (list.Contains(property.pptrValue))
                {
                    list.Remove(property.pptrValue);
                }
                else
                {
                    list.Add(property.pptrValue);
                }
                Selection.objects = list.ToArray(typeof(Object)) as Object[];
            }
            else if (!Event.current.shift)
            {
                Selection.activeObject = property.pptrValue;
            }
            else
            {
                HierarchyProperty property4;
                HierarchyProperty property5;
                HierarchyProperty firstSelected = this.GetFirstSelected();
                HierarchyProperty lastSelected = this.GetLastSelected();
                if (!firstSelected.isValid)
                {
                    Selection.activeObject = property.pptrValue;
                    return;
                }
                if (property.row > lastSelected.row)
                {
                    property4 = firstSelected;
                    property5 = property;
                }
                else
                {
                    property4 = property;
                    property5 = lastSelected;
                }
                ArrayList list2 = new ArrayList();
                list2.Add(property4.pptrValue);
                while (property4.Next(this.m_ExpandedArray))
                {
                    list2.Add(property4.pptrValue);
                    if (property4.instanceID == property5.instanceID)
                    {
                        break;
                    }
                }
                Selection.objects = list2.ToArray(typeof(Object)) as Object[];
            }
            this.SelType = SelectionType.Items;
            this.FrameObject(Selection.activeObject);
        }

        private void SetExpanded(int instanceID, bool expand)
        {
            Hashtable hashtable = new Hashtable();
            for (int i = 0; i < this.m_ExpandedArray.Length; i++)
            {
                hashtable.Add(this.m_ExpandedArray[i], null);
            }
            if (expand != hashtable.Contains(instanceID))
            {
                if (expand)
                {
                    hashtable.Add(instanceID, null);
                }
                else
                {
                    hashtable.Remove(instanceID);
                }
                this.m_ExpandedArray = new int[hashtable.Count];
                int index = 0;
                IEnumerator enumerator = hashtable.Keys.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        this.m_ExpandedArray[index] = (int) enumerator.Current;
                        index++;
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
            }
            InternalEditorUtility.expandedProjectWindowItems = this.m_ExpandedArray;
        }

        private void SetExpandedRecurse(int instanceID, bool expand)
        {
            HierarchyProperty property = new HierarchyProperty(HierarchyType.Assets);
            if (property.Find(instanceID, this.m_ExpandedArray))
            {
                this.SetExpanded(instanceID, expand);
                int depth = property.depth;
                while (property.Next(null) && (property.depth > depth))
                {
                    this.SetExpanded(property.instanceID, expand);
                }
            }
        }

        private void SetupDeletedItems()
        {
            this.m_DeletedItems = AssetServer.GetServerDeletedItems();
            this.m_DelPVstate.Clear();
            this.m_DelPVstate.lv = new ListViewState(0);
            this.m_DelPVstate.AddAssetItems(this.m_DeletedItems);
            this.m_DelPVstate.AddAssetItems(AssetServer.GetLocalDeletedItems());
            this.m_DelPVstate.SetLineCount();
            this.m_DelPVstate.selectedItems = new bool[this.m_DelPVstate.lv.totalRows];
            this.m_DeletedItemsInitialized = true;
        }

        private bool DeletedItemsToggle
        {
            get
            {
                return this.m_DeletedItemsToggle;
            }
            set
            {
                this.m_DeletedItemsToggle = value;
                if (this.m_DeletedItemsToggle && !this.m_DeletedItemsInitialized)
                {
                    this.SetupDeletedItems();
                }
            }
        }

        public SelectionType SelType
        {
            get
            {
                return this.m_SelType;
            }
            set
            {
                if ((this.m_SelType == SelectionType.DeletedItems) && (value != SelectionType.DeletedItems))
                {
                    for (int i = 0; i < this.m_DelPVstate.selectedItems.Length; i++)
                    {
                        this.m_DelPVstate.selectedItems[i] = false;
                    }
                }
                this.m_SelType = value;
            }
        }

        public enum SelectionType
        {
            None,
            All,
            Items,
            DeletedItemsRoot,
            DeletedItems
        }

        private class Styles
        {
            public GUIStyle foldout = "IN Foldout";
            public GUIStyle insertion = "PR Insertion";
            public GUIStyle label = "PR Label";
            public GUIStyle ping = new GUIStyle("PR Ping");
            public GUIStyle toolbarButton = "ToolbarButton";

            public Styles()
            {
                this.ping.overflow.left = -2;
                this.ping.overflow.right = -21;
                this.ping.padding.left = 0x30;
                this.ping.padding.right = 0;
            }
        }
    }
}

