namespace UnityEditorInternal.VersionControl
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.VersionControl;
    using UnityEditorInternal;
    using UnityEngine;

    [Serializable]
    public class ListControl
    {
        private ActionDelegate actionDelegate;
        private ListItem active;
        private Texture2D blueTex;
        private const string c_changeKeyPrefix = "_chkeyprfx_";
        internal const string c_emptyChangeListMessage = "Empty change list";
        private const float c_lineHeight = 16f;
        private const string c_metaSuffix = ".meta";
        private const float c_scrollWidth = 14f;
        private GUIContent calcSizeTmpContent = new GUIContent();
        private Texture2D defaultIcon;
        private bool dragAcceptOnly;
        private SelectDirection dragAdjust = SelectDirection.Current;
        private int dragCount;
        private DragDelegate dragDelegate;
        private ListItem dragTarget;
        private ExpandDelegate expandDelegate;
        private Texture2D greyTex;
        [SerializeField]
        private ListState m_listState;
        private string menuDefault;
        private string menuFolder;
        private Dictionary<string, ListItem> pathSearch = new Dictionary<string, ListItem>();
        private bool readOnly;
        private ListItem root = new ListItem();
        private static int s_uniqueIDCount = 1;
        private static Dictionary<int, ListControl> s_uniqueIDList = new Dictionary<int, ListControl>();
        private bool scrollVisible;
        private Dictionary<string, ListItem> selectList = new Dictionary<string, ListItem>();
        private ListItem singleSelect;
        [NonSerialized]
        private int uniqueID = s_uniqueIDCount++;
        private List<ListItem> visibleList = new List<ListItem>();
        private Texture2D yellowTex;

        public ListControl()
        {
            s_uniqueIDList.Add(this.uniqueID, this);
            this.active = this.root;
            this.Clear();
        }

        public ListItem Add(ListItem parent, string name, Asset asset)
        {
            ListItem item = (parent == null) ? this.root : parent;
            ListItem listItem = new ListItem {
                Name = name,
                Asset = asset
            };
            item.Add(listItem);
            ListItem twinAsset = this.GetTwinAsset(listItem);
            if (((twinAsset != null) && (listItem.Asset != null)) && (twinAsset.Asset.state == (listItem.Asset.state & ~Asset.States.MetaFile)))
            {
                listItem.Hidden = true;
            }
            if ((listItem.Asset != null) && (listItem.Asset.path.Length > 0))
            {
                this.pathSearch[listItem.Asset.path.ToLower()] = listItem;
            }
            return listItem;
        }

        public ListItem Add(ListItem parent, string name, ChangeSet change)
        {
            ListItem item = (parent == null) ? this.root : parent;
            ListItem listItem = new ListItem {
                Name = name
            };
            if (change == null)
            {
            }
            listItem.Change = new ChangeSet(name);
            item.Add(listItem);
            this.pathSearch["_chkeyprfx_" + change.id.ToString()] = listItem;
            return listItem;
        }

        private void CallExpandedEvent(ListItem item, bool remove)
        {
            if (item.Change != null)
            {
                if (item.Expanded)
                {
                    if (this.expandDelegate != null)
                    {
                        this.expandDelegate(item.Change, item);
                    }
                    this.listState.Expanded.Add(item.Change.id);
                }
                else if (remove)
                {
                    this.listState.Expanded.Remove(item.Change.id);
                }
            }
            for (ListItem item2 = item.FirstChild; item2 != null; item2 = item2.Next)
            {
                this.CallExpandedEvent(item2, remove);
            }
        }

        public void Clear()
        {
            this.root.Clear();
            this.pathSearch.Clear();
            this.root.Name = "ROOT";
            this.root.Expanded = true;
        }

        private void CreateResources()
        {
            if (this.blueTex == null)
            {
                this.blueTex = new Texture2D(1, 1);
                this.blueTex.SetPixel(0, 0, new Color(0.23f, 0.35f, 0.55f));
                this.blueTex.hideFlags = HideFlags.HideAndDontSave;
                this.blueTex.name = "BlueTex";
                this.blueTex.Apply();
            }
            if (this.greyTex == null)
            {
                this.greyTex = new Texture2D(1, 1);
                this.greyTex.SetPixel(0, 0, new Color(0.3f, 0.3f, 0.3f));
                this.greyTex.hideFlags = HideFlags.HideAndDontSave;
                this.greyTex.name = "GrayTex";
                this.greyTex.Apply();
            }
            if (this.yellowTex == null)
            {
                this.yellowTex = new Texture2D(1, 1);
                this.yellowTex.SetPixel(0, 0, new Color(0.5f, 0.5f, 0.2f));
                this.yellowTex.name = "YellowTex";
                this.yellowTex.hideFlags = HideFlags.HideAndDontSave;
                this.yellowTex.Apply();
            }
            if (this.defaultIcon == null)
            {
                this.defaultIcon = EditorGUIUtility.LoadIcon("vcs_document");
                this.defaultIcon.hideFlags = HideFlags.HideAndDontSave;
            }
        }

        private string DisplayName(ListItem item)
        {
            string name = item.Name;
            string str2 = string.Empty;
            while (str2 == string.Empty)
            {
                int index = name.IndexOf('\n');
                if (index < 0)
                {
                    break;
                }
                str2 = name.Substring(0, index).Trim();
                name = name.Substring(index + 1);
            }
            if (str2 != string.Empty)
            {
                name = str2;
            }
            name = name.Trim();
            if ((name == string.Empty) && (item.Change != null))
            {
                name = item.Change.id.ToString() + " " + item.Change.description;
            }
            return name;
        }

        private void DrawItem(ListItem item, Rect area, float x, float y, bool focus, bool selected)
        {
            bool flag = item == this.dragTarget;
            bool flag2 = selected;
            if (selected)
            {
                Texture2D image = !focus ? this.greyTex : this.blueTex;
                GUI.DrawTexture(new Rect(area.x, y, area.width, 16f), image, ScaleMode.StretchToFill, false);
            }
            else if (!flag)
            {
                if (((this.dragTarget != null) && (item == this.dragTarget.Parent)) && (this.dragAdjust != SelectDirection.Current))
                {
                    GUI.DrawTexture(new Rect(area.x, y, area.width, 16f), this.yellowTex, ScaleMode.StretchToFill, false);
                    flag2 = true;
                }
            }
            else
            {
                SelectDirection dragAdjust = this.dragAdjust;
                if (dragAdjust == SelectDirection.Up)
                {
                    if (item.PrevOpenVisible != item.Parent)
                    {
                        GUI.DrawTexture(new Rect(x, y - 1f, area.width, 2f), this.yellowTex, ScaleMode.StretchToFill, false);
                    }
                }
                else if (dragAdjust == SelectDirection.Down)
                {
                    GUI.DrawTexture(new Rect(x, (y + 16f) - 1f, area.width, 2f), this.yellowTex, ScaleMode.StretchToFill, false);
                }
                else if (item.CanAccept)
                {
                    GUI.DrawTexture(new Rect(area.x, y, area.width, 16f), this.yellowTex, ScaleMode.StretchToFill, false);
                    flag2 = true;
                }
            }
            if (item.HasActions)
            {
                for (int i = 0; i < item.Actions.Length; i++)
                {
                    this.calcSizeTmpContent.text = item.Actions[i];
                    Vector2 vector = GUI.skin.button.CalcSize(this.calcSizeTmpContent);
                    if (GUI.Button(new Rect(x, y, vector.x, 16f), item.Actions[i]))
                    {
                        this.actionDelegate(item, i);
                    }
                    x += vector.x + 4f;
                }
            }
            if (item.CanExpand)
            {
                EditorGUI.Foldout(new Rect(x, y, 16f, 16f), item.Expanded, GUIContent.none);
            }
            Texture icon = item.Icon;
            Color color = GUI.color;
            Color contentColor = GUI.contentColor;
            if (item.Dummy)
            {
                GUI.color = new Color(0.65f, 0.65f, 0.65f);
            }
            if (!item.Dummy)
            {
                if (icon == null)
                {
                    icon = InternalEditorUtility.GetIconForFile(item.Name);
                }
                Rect position = new Rect(x + 14f, y, 16f, 16f);
                if (icon != null)
                {
                    GUI.DrawTexture(position, icon);
                }
                if (item.Asset != null)
                {
                    Rect itemRect = position;
                    itemRect.width += 12f;
                    itemRect.x -= 6f;
                    Overlay.DrawOverlay(item.Asset, itemRect);
                }
            }
            string t = this.DisplayName(item);
            Vector2 vector2 = EditorStyles.label.CalcSize(EditorGUIUtility.TempContent(t));
            float num2 = x + 32f;
            if (flag2)
            {
                GUI.contentColor = new Color(3f, 3f, 3f);
                GUI.Label(new Rect(num2, y, area.width - num2, 18f), t);
            }
            else
            {
                GUI.Label(new Rect(num2, y, area.width - num2, 18f), t);
            }
            if (this.HasHiddenMetaFile(item))
            {
                GUI.color = new Color(0.55f, 0.55f, 0.55f);
                float num3 = (num2 + vector2.x) + 2f;
                GUI.Label(new Rect(num3, y, area.width - num3, 18f), "+meta");
            }
            GUI.contentColor = contentColor;
            GUI.color = color;
        }

        private void DrawItems(Rect area, bool focus)
        {
            float y = area.y;
            foreach (ListItem item in this.visibleList)
            {
                float x = area.x + ((item.Indent - 1) * 0x12);
                bool selected = !this.readOnly ? this.IsSelected(item) : false;
                if (((item.Parent != null) && (item.Parent.Parent != null)) && (item.Parent.Parent.Parent == null))
                {
                    x -= 16f;
                }
                this.DrawItem(item, area, x, y, focus, selected);
                y += 16f;
            }
        }

        internal void ExpandLastItem()
        {
            if (this.root.LastChild != null)
            {
                this.root.LastChild.Expanded = true;
                this.CallExpandedEvent(this.root.LastChild, true);
            }
        }

        ~ListControl()
        {
            s_uniqueIDList.Remove(this.uniqueID);
        }

        public ListItem FindItemWithIdentifier(int identifier)
        {
            return this.root.FindWithIdentifierRecurse(identifier);
        }

        public static ListControl FromID(int id)
        {
            try
            {
                return s_uniqueIDList[id];
            }
            catch
            {
                return null;
            }
        }

        internal ListItem GetChangeSetItem(ChangeSet change)
        {
            if (change != null)
            {
                for (ListItem item = this.root.FirstChild; item != null; item = item.Next)
                {
                    ChangeSet set = item.Item as ChangeSet;
                    if ((set != null) && (set.id == change.id))
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        private ListItem GetItemAt(Rect area, Vector2 pos)
        {
            int num = (int) ((pos.y - area.y) / 16f);
            if ((num >= 0) && (num < this.visibleList.Count))
            {
                return this.visibleList[num];
            }
            return null;
        }

        private ListItem GetTwin(ListItem item)
        {
            ListItem twinAsset = this.GetTwinAsset(item);
            if (twinAsset != null)
            {
                return twinAsset;
            }
            return this.GetTwinMeta(item);
        }

        private ListItem GetTwinAsset(ListItem item)
        {
            ListItem prev = item.Prev;
            if ((item.Name.EndsWith(".meta") && (prev != null)) && ((prev.Asset != null) && (AssetDatabase.GetTextMetaFilePathFromAssetPath(prev.Asset.path).ToLower() == item.Asset.path.ToLower())))
            {
                return prev;
            }
            return null;
        }

        private ListItem GetTwinMeta(ListItem item)
        {
            ListItem next = item.Next;
            if ((!item.Name.EndsWith(".meta") && (next != null)) && ((next.Asset != null) && (next.Asset.path.ToLower() == AssetDatabase.GetTextMetaFilePathFromAssetPath(item.Asset.path).ToLower())))
            {
                return next;
            }
            return null;
        }

        private void HandleKeyInput(Event e)
        {
            if ((e.type == EventType.KeyDown) && (this.selectList.Count != 0))
            {
                if ((e.keyCode == KeyCode.UpArrow) || (e.keyCode == KeyCode.DownArrow))
                {
                    ListItem prevOpenSkip = null;
                    if (e.keyCode == KeyCode.UpArrow)
                    {
                        prevOpenSkip = this.SelectedFirstIn(this.active);
                        if (prevOpenSkip != null)
                        {
                            prevOpenSkip = prevOpenSkip.PrevOpenSkip;
                        }
                    }
                    else
                    {
                        prevOpenSkip = this.SelectedLastIn(this.active);
                        if (prevOpenSkip != null)
                        {
                            prevOpenSkip = prevOpenSkip.NextOpenSkip;
                        }
                    }
                    if (prevOpenSkip != null)
                    {
                        if (!this.ScrollUpTo(prevOpenSkip))
                        {
                            this.ScrollDownTo(prevOpenSkip);
                        }
                        if (e.shift)
                        {
                            this.SelectionFlow(prevOpenSkip);
                        }
                        else
                        {
                            this.SelectedSet(prevOpenSkip);
                        }
                    }
                }
                if ((e.keyCode == KeyCode.LeftArrow) || (e.keyCode == KeyCode.RightArrow))
                {
                    ListItem item = this.SelectedCurrentIn(this.active);
                    item.Expanded = e.keyCode == KeyCode.RightArrow;
                    this.CallExpandedEvent(item, true);
                }
                if ((e.keyCode == KeyCode.Return) && (GUIUtility.keyboardControl == 0))
                {
                    this.SelectedCurrentIn(this.active).Asset.Edit();
                }
            }
        }

        private bool HandleMouse(Rect area)
        {
            Event current = Event.current;
            bool flag = false;
            bool flag2 = area.Contains(current.mousePosition);
            if ((current.type == EventType.MouseDown) && flag2)
            {
                flag = true;
                this.dragCount = 0;
                GUIUtility.keyboardControl = 0;
                this.singleSelect = this.GetItemAt(area, current.mousePosition);
                if ((this.singleSelect != null) && !this.singleSelect.Dummy)
                {
                    if (((current.button == 0) && (current.clickCount > 1)) && (this.singleSelect.Asset != null))
                    {
                        this.singleSelect.Asset.Edit();
                    }
                    if (current.button < 2)
                    {
                        float num = area.x + ((this.singleSelect.Indent - 1) * 0x12);
                        if (((current.mousePosition.x >= num) && (current.mousePosition.x < (num + 16f))) && this.singleSelect.CanExpand)
                        {
                            this.singleSelect.Expanded = !this.singleSelect.Expanded;
                            this.CallExpandedEvent(this.singleSelect, true);
                            this.singleSelect = null;
                        }
                        else if (current.control || current.command)
                        {
                            if (current.button == 1)
                            {
                                this.SelectedAdd(this.singleSelect);
                            }
                            else
                            {
                                this.SelectedToggle(this.singleSelect);
                            }
                            this.singleSelect = null;
                        }
                        else if (current.shift)
                        {
                            this.SelectionFlow(this.singleSelect);
                            this.singleSelect = null;
                        }
                        else if (!this.IsSelected(this.singleSelect))
                        {
                            this.SelectedSet(this.singleSelect);
                            this.singleSelect = null;
                        }
                    }
                }
                else if (current.button == 0)
                {
                    this.SelectedClear();
                    this.singleSelect = null;
                }
            }
            else if (((current.type == EventType.MouseUp) || (current.type == EventType.ContextClick)) && flag2)
            {
                GUIUtility.keyboardControl = 0;
                this.singleSelect = this.GetItemAt(area, current.mousePosition);
                this.dragCount = 0;
                flag = true;
                if ((this.singleSelect != null) && !this.singleSelect.Dummy)
                {
                    if (current.type == EventType.ContextClick)
                    {
                        this.singleSelect = null;
                        if (!this.IsSelectedAsset() && !string.IsNullOrEmpty(this.menuFolder))
                        {
                            s_uniqueIDList[this.uniqueID] = this;
                            EditorUtility.DisplayPopupMenu(new Rect(current.mousePosition.x, current.mousePosition.y, 0f, 0f), this.menuFolder, new MenuCommand(null, this.uniqueID));
                        }
                        else if (!string.IsNullOrEmpty(this.menuDefault))
                        {
                            s_uniqueIDList[this.uniqueID] = this;
                            EditorUtility.DisplayPopupMenu(new Rect(current.mousePosition.x, current.mousePosition.y, 0f, 0f), this.menuDefault, new MenuCommand(null, this.uniqueID));
                        }
                    }
                    else if ((((current.type != EventType.ContextClick) && (current.button == 0)) && (!current.control && !current.command)) && (!current.shift && this.IsSelected(this.singleSelect)))
                    {
                        this.SelectedSet(this.singleSelect);
                        this.singleSelect = null;
                    }
                }
            }
            if ((current.type == EventType.MouseDrag) && flag2)
            {
                this.dragCount++;
                if ((this.dragCount > 2) && (Selection.objects.Length > 0))
                {
                    DragAndDrop.PrepareStartDrag();
                    if (this.singleSelect != null)
                    {
                        DragAndDrop.objectReferences = new Object[] { this.singleSelect.Asset.Load() };
                    }
                    else
                    {
                        DragAndDrop.objectReferences = Selection.objects;
                    }
                    DragAndDrop.StartDrag("Move");
                }
            }
            if (current.type == EventType.DragUpdated)
            {
                flag = true;
                DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                this.dragTarget = this.GetItemAt(area, current.mousePosition);
                if (this.dragTarget != null)
                {
                    if (this.IsSelected(this.dragTarget))
                    {
                        this.dragTarget = null;
                    }
                    else if (this.dragAcceptOnly)
                    {
                        if (!this.dragTarget.CanAccept)
                        {
                            this.dragTarget = null;
                        }
                    }
                    else
                    {
                        bool flag3 = !this.dragTarget.CanAccept || (this.dragTarget.PrevOpenVisible != this.dragTarget.Parent);
                        bool flag4 = !this.dragTarget.CanAccept || (this.dragTarget.NextOpenVisible != this.dragTarget.FirstChild);
                        float num2 = !this.dragTarget.CanAccept ? 8f : 2f;
                        int num3 = (int) ((current.mousePosition.y - area.y) / 16f);
                        float num4 = area.y + (num3 * 16f);
                        this.dragAdjust = SelectDirection.Current;
                        if (flag3 && (current.mousePosition.y <= (num4 + num2)))
                        {
                            this.dragAdjust = SelectDirection.Up;
                        }
                        else if (flag4 && (current.mousePosition.y >= ((num4 + 16f) - num2)))
                        {
                            this.dragAdjust = SelectDirection.Down;
                        }
                    }
                }
            }
            if ((current.type == EventType.DragPerform) && (this.dragTarget != null))
            {
                ListItem item = (this.dragAdjust != SelectDirection.Current) ? this.dragTarget.Parent : this.dragTarget;
                if (((this.dragDelegate != null) && (item != null)) && item.CanAccept)
                {
                    this.dragDelegate(item.Change);
                }
                this.dragTarget = null;
            }
            if (current.type == EventType.DragExited)
            {
                this.dragTarget = null;
            }
            return flag;
        }

        private void HandleSelectAll()
        {
            if ((Event.current.type == EventType.ValidateCommand) && (Event.current.commandName == "SelectAll"))
            {
                Event.current.Use();
            }
            else if ((Event.current.type == EventType.ExecuteCommand) && (Event.current.commandName == "SelectAll"))
            {
                this.SelectedAll();
                Event.current.Use();
            }
        }

        private bool HasHiddenMetaFile(ListItem item)
        {
            ListItem twinMeta = this.GetTwinMeta(item);
            return ((twinMeta != null) && twinMeta.Hidden);
        }

        private bool IsSelected(ListItem item)
        {
            if (item.Asset != null)
            {
                return this.selectList.ContainsKey(item.Asset.path.ToLower());
            }
            return ((item.Change != null) && this.selectList.ContainsKey("_chkeyprfx_" + item.Change.id.ToString()));
        }

        private bool IsSelectedAsset()
        {
            foreach (KeyValuePair<string, ListItem> pair in this.selectList)
            {
                if ((pair.Value != null) && (pair.Value.Asset != null))
                {
                    return true;
                }
            }
            return false;
        }

        private void LoadExpanded(ListItem item)
        {
            if (item.Change != null)
            {
                item.Expanded = this.listState.Expanded.Contains(item.Change.id);
            }
            for (ListItem item2 = item.FirstChild; item2 != null; item2 = item2.Next)
            {
                this.LoadExpanded(item2);
            }
        }

        public bool OnGUI(Rect area, bool focus)
        {
            bool flag = false;
            this.CreateResources();
            Event current = Event.current;
            int openCount = this.active.OpenCount;
            int num2 = (int) (area.height / 16f);
            if (current.type == EventType.ScrollWheel)
            {
                flag = true;
                ListState listState = this.listState;
                listState.Scroll += current.delta.y;
                this.listState.Scroll = Mathf.Clamp(this.listState.Scroll, 0f, (float) (openCount - num2));
            }
            if (openCount > num2)
            {
                Rect position = new Rect((area.x + area.width) - 14f, area.y, 14f, area.height);
                area.width -= 14f;
                float scroll = this.listState.Scroll;
                this.listState.Scroll = GUI.VerticalScrollbar(position, this.listState.Scroll, (float) num2, 0f, (float) openCount);
                this.listState.Scroll = Mathf.Clamp(this.listState.Scroll, 0f, (float) (openCount - num2));
                if (scroll != this.listState.Scroll)
                {
                    flag = true;
                }
                if (!this.scrollVisible)
                {
                    this.scrollVisible = true;
                }
            }
            else if (this.scrollVisible)
            {
                this.scrollVisible = false;
            }
            this.UpdateVisibleList(area, this.listState.Scroll);
            if (focus && !this.readOnly)
            {
                if (current.isKey)
                {
                    flag = true;
                    this.HandleKeyInput(current);
                }
                this.HandleSelectAll();
                flag = this.HandleMouse(area) || flag;
                if ((current.type == EventType.DragUpdated) && area.Contains(current.mousePosition))
                {
                    if (current.mousePosition.y < (area.y + 16f))
                    {
                        this.listState.Scroll = Mathf.Clamp(this.listState.Scroll - 1f, 0f, (float) (openCount - num2));
                    }
                    else if (current.mousePosition.y > ((area.y + area.height) - 16f))
                    {
                        this.listState.Scroll = Mathf.Clamp(this.listState.Scroll + 1f, 0f, (float) (openCount - num2));
                    }
                }
            }
            this.DrawItems(area, focus);
            return flag;
        }

        private ListItem PathSearchFind(string path)
        {
            try
            {
                return this.pathSearch[path.ToLower()];
            }
            catch
            {
                return null;
            }
        }

        private void PathSearchUpdate(ListItem item)
        {
            if ((item.Asset != null) && (item.Asset.path.Length > 0))
            {
                this.pathSearch.Add(item.Asset.path.ToLower(), item);
            }
            else if (item.Change != null)
            {
                this.pathSearch.Add("_chkeyprfx_" + item.Change.id.ToString(), item);
                return;
            }
            for (ListItem item2 = item.FirstChild; item2 != null; item2 = item2.Next)
            {
                this.PathSearchUpdate(item2);
            }
        }

        public void Refresh()
        {
            this.Refresh(true);
        }

        public void Refresh(bool updateExpanded)
        {
            if (updateExpanded)
            {
                this.LoadExpanded(this.root);
                this.root.Name = "ROOT";
                this.root.Expanded = true;
                this.listState.Expanded.Clear();
                this.CallExpandedEvent(this.root, false);
            }
            this.SelectedRefresh();
        }

        private bool ScrollDownTo(ListItem item)
        {
            int scroll = (int) this.listState.Scroll;
            for (ListItem item2 = (this.visibleList.Count <= 0) ? null : this.visibleList[this.visibleList.Count - 1]; (item2 != null) && (scroll >= 0); item2 = item2.NextOpenVisible)
            {
                if (item2 == item)
                {
                    this.listState.Scroll = scroll;
                    return true;
                }
                scroll++;
            }
            return false;
        }

        private bool ScrollUpTo(ListItem item)
        {
            int scroll = (int) this.listState.Scroll;
            for (ListItem item2 = (this.visibleList.Count <= 0) ? null : this.visibleList[0]; (item2 != null) && (scroll >= 0); item2 = item2.PrevOpenVisible)
            {
                if (item2 == item)
                {
                    this.listState.Scroll = scroll;
                    return true;
                }
                scroll--;
            }
            return false;
        }

        public void SelectedAdd(ListItem item)
        {
            if (!item.Dummy)
            {
                ListItem item2 = this.SelectedCurrentIn(this.active);
                if (item.Exclusive || ((item2 != null) && item2.Exclusive))
                {
                    this.SelectedSet(item);
                }
                else
                {
                    string str = item.Asset.path.ToLower();
                    int count = this.selectList.Count;
                    this.selectList[str] = item;
                    ListItem twin = this.GetTwin(item);
                    if (twin != null)
                    {
                        this.selectList[twin.Asset.path.ToLower()] = twin;
                    }
                    if (count != this.selectList.Count)
                    {
                        int[] instanceIDs = Selection.instanceIDs;
                        int index = 0;
                        if (instanceIDs != null)
                        {
                            index = instanceIDs.Length;
                        }
                        str = !str.EndsWith(".meta") ? str : str.Substring(0, str.Length - 5);
                        char[] trimChars = new char[] { '/' };
                        int mainAssetInstanceID = AssetDatabase.GetMainAssetInstanceID(str.TrimEnd(trimChars));
                        if (mainAssetInstanceID != 0)
                        {
                            int[] destinationArray = new int[index + 1];
                            destinationArray[index] = mainAssetInstanceID;
                            Array.Copy(instanceIDs, destinationArray, index);
                            Selection.instanceIDs = destinationArray;
                        }
                    }
                }
            }
        }

        public void SelectedAll()
        {
            this.SelectedClear();
            this.SelectedAllHelper(this.Root);
        }

        private void SelectedAllHelper(ListItem _root)
        {
            for (ListItem item = _root.FirstChild; item != null; item = item.Next)
            {
                if (item.HasChildren)
                {
                    this.SelectedAllHelper(item);
                }
                if (item.Asset != null)
                {
                    this.SelectedAdd(item);
                }
            }
        }

        private void SelectedClear()
        {
            this.selectList.Clear();
            Selection.activeObject = null;
            Selection.instanceIDs = new int[0];
        }

        private ListItem SelectedCurrentIn(ListItem root)
        {
            foreach (KeyValuePair<string, ListItem> pair in this.selectList)
            {
                if (pair.Value.IsChildOf(root))
                {
                    return pair.Value;
                }
            }
            return null;
        }

        private ListItem SelectedFirstIn(ListItem root)
        {
            ListItem item = this.SelectedCurrentIn(root);
            for (ListItem item2 = item; item2 != null; item2 = item2.PrevOpenVisible)
            {
                if (this.IsSelected(item2))
                {
                    item = item2;
                }
            }
            return item;
        }

        private ListItem SelectedLastIn(ListItem root)
        {
            ListItem item = this.SelectedCurrentIn(root);
            for (ListItem item2 = item; item2 != null; item2 = item2.NextOpenVisible)
            {
                if (this.IsSelected(item2))
                {
                    item = item2;
                }
            }
            return item;
        }

        private void SelectedRefresh()
        {
            Dictionary<string, ListItem> dictionary = new Dictionary<string, ListItem>();
            foreach (KeyValuePair<string, ListItem> pair in this.selectList)
            {
                dictionary[pair.Key] = this.PathSearchFind(pair.Key);
            }
            this.selectList = dictionary;
        }

        private void SelectedRemove(ListItem item)
        {
            string key = item.Asset.path.ToLower();
            this.selectList.Remove(key);
            this.selectList.Remove(!key.EndsWith(".meta") ? (key + ".meta") : key.Substring(0, key.Length - 5));
            key = !key.EndsWith(".meta") ? key : key.Substring(0, key.Length - 5);
            char[] trimChars = new char[] { '/' };
            int mainAssetInstanceID = AssetDatabase.GetMainAssetInstanceID(key.TrimEnd(trimChars));
            int[] instanceIDs = Selection.instanceIDs;
            if ((mainAssetInstanceID != 0) && (instanceIDs.Length > 0))
            {
                int index = Array.IndexOf<int>(instanceIDs, mainAssetInstanceID);
                if (index >= 0)
                {
                    int[] destinationArray = new int[instanceIDs.Length - 1];
                    Array.Copy(instanceIDs, destinationArray, index);
                    if (index < (instanceIDs.Length - 1))
                    {
                        Array.Copy(instanceIDs, index + 1, destinationArray, index, (instanceIDs.Length - index) - 1);
                    }
                    Selection.instanceIDs = destinationArray;
                }
            }
        }

        public void SelectedSet(ListItem item)
        {
            if (!item.Dummy)
            {
                this.SelectedClear();
                if (item.Asset != null)
                {
                    this.SelectedAdd(item);
                }
                else if (item.Change != null)
                {
                    this.selectList["_chkeyprfx_" + item.Change.id.ToString()] = item;
                }
            }
        }

        private void SelectedToggle(ListItem item)
        {
            if (this.IsSelected(item))
            {
                this.SelectedRemove(item);
            }
            else
            {
                this.SelectedAdd(item);
            }
        }

        private void SelectionFlow(ListItem item)
        {
            if (this.selectList.Count == 0)
            {
                this.SelectedSet(item);
            }
            else if (!this.SelectionFlowDown(item))
            {
                this.SelectionFlowUp(item);
            }
        }

        private bool SelectionFlowDown(ListItem item)
        {
            ListItem item2;
            ListItem item3 = item;
            for (item2 = item; item2 != null; item2 = item2.NextOpenVisible)
            {
                if (this.IsSelected(item2))
                {
                    item3 = item2;
                }
            }
            if (item == item3)
            {
                return false;
            }
            this.SelectedClear();
            this.SelectedAdd(item3);
            for (item2 = item; item2 != item3; item2 = item2.NextOpenVisible)
            {
                this.SelectedAdd(item2);
            }
            return true;
        }

        private bool SelectionFlowUp(ListItem item)
        {
            ListItem item2;
            ListItem item3 = item;
            for (item2 = item; item2 != null; item2 = item2.PrevOpenVisible)
            {
                if (this.IsSelected(item2))
                {
                    item3 = item2;
                }
            }
            if (item == item3)
            {
                return false;
            }
            this.SelectedClear();
            this.SelectedAdd(item3);
            for (item2 = item; item2 != item3; item2 = item2.PrevOpenVisible)
            {
                this.SelectedAdd(item2);
            }
            return true;
        }

        public void Sync()
        {
            this.SelectedClear();
            foreach (Object obj2 in Selection.objects)
            {
                if (AssetDatabase.IsMainAsset(obj2))
                {
                    string path = Application.dataPath.Substring(0, Application.dataPath.Length - 6) + AssetDatabase.GetAssetPath(obj2);
                    ListItem item = this.PathSearchFind(path);
                    if (item != null)
                    {
                        this.SelectedAdd(item);
                    }
                }
            }
        }

        private void UpdateVisibleList(Rect area, float scrollPos)
        {
            float y = area.y;
            float num2 = (area.y + area.height) - 16f;
            ListItem nextOpenVisible = this.active.NextOpenVisible;
            this.visibleList.Clear();
            for (float i = 0f; i < scrollPos; i++)
            {
                if (nextOpenVisible == null)
                {
                    return;
                }
                nextOpenVisible = nextOpenVisible.NextOpenVisible;
            }
            for (ListItem item2 = nextOpenVisible; (item2 != null) && (y < num2); item2 = item2.NextOpenVisible)
            {
                this.visibleList.Add(item2);
                y += 16f;
            }
        }

        public ActionDelegate ActionEvent
        {
            get
            {
                return this.actionDelegate;
            }
            set
            {
                this.actionDelegate = value;
            }
        }

        public bool DragAcceptOnly
        {
            get
            {
                return this.dragAcceptOnly;
            }
            set
            {
                this.dragAcceptOnly = value;
            }
        }

        public DragDelegate DragEvent
        {
            get
            {
                return this.dragDelegate;
            }
            set
            {
                this.dragDelegate = value;
            }
        }

        public ChangeSets EmptyChangeSets
        {
            get
            {
                ListItem firstChild = this.root.FirstChild;
                ChangeSets sets = new ChangeSets();
                while (firstChild != null)
                {
                    ChangeSet change = firstChild.Change;
                    if ((((change != null) && firstChild.HasChildren) && (firstChild.FirstChild.Item == null)) && (firstChild.FirstChild.Name == "Empty change list"))
                    {
                        sets.Add(change);
                    }
                    firstChild = firstChild.Next;
                }
                return sets;
            }
        }

        public ExpandDelegate ExpandEvent
        {
            get
            {
                return this.expandDelegate;
            }
            set
            {
                this.expandDelegate = value;
            }
        }

        public ListState listState
        {
            get
            {
                if (this.m_listState == null)
                {
                    this.m_listState = new ListState();
                }
                return this.m_listState;
            }
        }

        public string MenuDefault
        {
            get
            {
                return this.menuDefault;
            }
            set
            {
                this.menuDefault = value;
            }
        }

        public string MenuFolder
        {
            get
            {
                return this.menuFolder;
            }
            set
            {
                this.menuFolder = value;
            }
        }

        public bool ReadOnly
        {
            get
            {
                return this.readOnly;
            }
            set
            {
                this.readOnly = value;
            }
        }

        public ListItem Root
        {
            get
            {
                return this.root;
            }
        }

        public AssetList SelectedAssets
        {
            get
            {
                AssetList list = new AssetList();
                foreach (KeyValuePair<string, ListItem> pair in this.selectList)
                {
                    if (pair.Value.Item is Asset)
                    {
                        list.Add(pair.Value.Item as Asset);
                    }
                }
                return list;
            }
        }

        public ChangeSets SelectedChangeSets
        {
            get
            {
                ChangeSets sets = new ChangeSets();
                foreach (KeyValuePair<string, ListItem> pair in this.selectList)
                {
                    if ((pair.Value != null) && (pair.Value.Item is ChangeSet))
                    {
                        sets.Add(pair.Value.Item as ChangeSet);
                    }
                }
                return sets;
            }
        }

        public int Size
        {
            get
            {
                return this.visibleList.Count;
            }
        }

        public delegate void ActionDelegate(ListItem item, int actionIdx);

        public delegate void DragDelegate(ChangeSet target);

        public delegate void ExpandDelegate(ChangeSet expand, ListItem item);

        [Serializable]
        public class ListState
        {
            [SerializeField]
            public List<string> Expanded = new List<string>();
            [SerializeField]
            public float Scroll;
        }

        public enum SelectDirection
        {
            Up,
            Down,
            Current
        }
    }
}

