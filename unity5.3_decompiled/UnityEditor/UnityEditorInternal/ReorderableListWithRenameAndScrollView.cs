namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    internal class ReorderableListWithRenameAndScrollView
    {
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map15;
        public GUIStyle listElementStyle;
        private int m_FrameIndex = -1;
        private bool m_HadKeyFocusAtMouseDown;
        private int m_LastSelectedIndex = -1;
        private ReorderableList m_ReorderableList;
        private State m_State;
        public ReorderableList.ElementCallbackDelegate onCustomDrawElement;
        public Action<int> onDeleteItemAtIndex;
        public Func<int, string> onGetNameAtIndex;
        public Action<int, string> onNameChangedAtIndex;
        public Action<int> onSelectionChanged;
        public GUIStyle renameOverlayStyle;
        private static Styles s_Styles;

        public ReorderableListWithRenameAndScrollView(ReorderableList list, State state)
        {
            this.m_State = state;
            this.m_ReorderableList = list;
            this.m_ReorderableList.drawElementCallback = (ReorderableList.ElementCallbackDelegate) Delegate.Combine(this.m_ReorderableList.drawElementCallback, new ReorderableList.ElementCallbackDelegate(this.DrawElement));
            this.m_ReorderableList.onSelectCallback = (ReorderableList.SelectCallbackDelegate) Delegate.Combine(this.m_ReorderableList.onSelectCallback, new ReorderableList.SelectCallbackDelegate(this.SelectCallback));
            this.m_ReorderableList.onMouseUpCallback = (ReorderableList.SelectCallbackDelegate) Delegate.Combine(this.m_ReorderableList.onMouseUpCallback, new ReorderableList.SelectCallbackDelegate(this.MouseUpCallback));
            this.m_ReorderableList.onReorderCallback = (ReorderableList.ReorderCallbackDelegate) Delegate.Combine(this.m_ReorderableList.onReorderCallback, new ReorderableList.ReorderCallbackDelegate(this.ReorderCallback));
        }

        public void BeginRename(int index, float delay)
        {
            this.GetRenameOverlay().BeginRename(this.onGetNameAtIndex(index), index, delay);
            this.m_ReorderableList.index = index;
            this.m_LastSelectedIndex = index;
            this.FrameItem(index);
        }

        private bool CanBeginRename()
        {
            return (!this.GetRenameOverlay().IsRenaming() && (this.m_ReorderableList.index >= 0));
        }

        private void CommandHandling()
        {
            Event current = Event.current;
            if (Event.current.type == EventType.ExecuteCommand)
            {
                string commandName = current.commandName;
                if (commandName != null)
                {
                    int num;
                    if (<>f__switch$map15 == null)
                    {
                        Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
                        dictionary.Add("OnLostFocus", 0);
                        <>f__switch$map15 = dictionary;
                    }
                    if (<>f__switch$map15.TryGetValue(commandName, out num) && (num == 0))
                    {
                        this.EndRename(true);
                        current.Use();
                    }
                }
            }
        }

        public virtual void DoRenameOverlay()
        {
            if (this.GetRenameOverlay().IsRenaming() && !this.GetRenameOverlay().OnGUI())
            {
                this.RenameEnded();
            }
        }

        public void DrawElement(Rect r, int index, bool isActive, bool isFocused)
        {
            if (this.IsRenamingIndex(index))
            {
                if ((r.width >= 0f) && (r.height >= 0f))
                {
                    r.x -= 2f;
                    this.GetRenameOverlay().editFieldRect = r;
                }
                this.DoRenameOverlay();
            }
            else if (this.onCustomDrawElement != null)
            {
                this.onCustomDrawElement(r, index, isActive, isFocused);
            }
            else
            {
                this.DrawElementText(r, index, isActive, index == this.m_ReorderableList.index, isFocused);
            }
        }

        public void DrawElementText(Rect r, int index, bool isActive, bool isSelected, bool isFocused)
        {
            if ((Event.current.type == EventType.Repaint) && (this.onGetNameAtIndex != null))
            {
                this.elementStyle.Draw(r, this.onGetNameAtIndex(index), false, false, isSelected, true);
            }
        }

        public void EndRename(bool acceptChanges)
        {
            if (this.GetRenameOverlay().IsRenaming())
            {
                this.GetRenameOverlay().EndRename(acceptChanges);
                this.RenameEnded();
            }
        }

        private void EnsureRowIsVisible(int index, float scrollGUIHeight)
        {
            if (index >= 0)
            {
                float max = (this.m_ReorderableList.elementHeight * index) + 2f;
                float min = ((max - scrollGUIHeight) + this.m_ReorderableList.elementHeight) + 3f;
                this.m_State.m_ScrollPos.y = Mathf.Clamp(this.m_State.m_ScrollPos.y, min, max);
            }
        }

        public void FrameItem(int index)
        {
            this.m_FrameIndex = index;
        }

        private RenameOverlay GetRenameOverlay()
        {
            return this.m_State.m_RenameOverlay;
        }

        public bool IsRenamingIndex(int index)
        {
            return ((this.GetRenameOverlay().IsRenaming() && (this.GetRenameOverlay().userData == index)) && !this.GetRenameOverlay().isWaitingForDelay);
        }

        private void KeyboardHandling()
        {
            Event current = Event.current;
            if ((current.type == EventType.KeyDown) && this.m_ReorderableList.HasKeyboardControl())
            {
                KeyCode keyCode = Event.current.keyCode;
                switch (keyCode)
                {
                    case KeyCode.Home:
                        current.Use();
                        this.m_ReorderableList.index = 0;
                        this.FrameItem(this.m_ReorderableList.index);
                        return;

                    case KeyCode.End:
                        current.Use();
                        this.m_ReorderableList.index = this.m_ReorderableList.count - 1;
                        this.FrameItem(this.m_ReorderableList.index);
                        return;

                    case KeyCode.F2:
                        if (this.CanBeginRename() && (Application.platform == RuntimePlatform.WindowsEditor))
                        {
                            this.BeginRename(this.m_ReorderableList.index, 0f);
                            current.Use();
                        }
                        return;
                }
                if (keyCode != KeyCode.Return)
                {
                    if (keyCode == KeyCode.Delete)
                    {
                        this.RemoveSelected();
                        current.Use();
                        return;
                    }
                    if (keyCode != KeyCode.KeypadEnter)
                    {
                        return;
                    }
                }
                if (this.CanBeginRename() && (Application.platform == RuntimePlatform.OSXEditor))
                {
                    this.BeginRename(this.m_ReorderableList.index, 0f);
                    current.Use();
                }
            }
        }

        public void MouseUpCallback(ReorderableList list)
        {
            if (this.m_HadKeyFocusAtMouseDown && (list.index == this.m_LastSelectedIndex))
            {
                this.BeginRename(list.index, 0.5f);
            }
            this.m_LastSelectedIndex = list.index;
        }

        public void OnEvent()
        {
            this.GetRenameOverlay().OnEvent();
        }

        public void OnGUI(Rect rect)
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            if (this.onGetNameAtIndex == null)
            {
                Debug.LogError("Ensure to set: 'onGetNameAtIndex'");
            }
            Event current = Event.current;
            if ((current.type == EventType.MouseDown) && rect.Contains(current.mousePosition))
            {
                this.m_HadKeyFocusAtMouseDown = this.m_ReorderableList.HasKeyboardControl();
            }
            if (this.m_FrameIndex != -1)
            {
                this.EnsureRowIsVisible(this.m_FrameIndex, rect.height);
                this.m_FrameIndex = -1;
            }
            GUILayout.BeginArea(rect);
            this.m_State.m_ScrollPos = GUILayout.BeginScrollView(this.m_State.m_ScrollPos, new GUILayoutOption[0]);
            this.m_ReorderableList.DoLayoutList();
            GUILayout.EndScrollView();
            GUILayout.EndArea();
            AudioMixerDrawUtils.DrawScrollDropShadow(rect, this.m_State.m_ScrollPos.y, this.m_ReorderableList.GetHeight());
            this.KeyboardHandling();
            this.CommandHandling();
        }

        private void RemoveSelected()
        {
            if ((this.m_ReorderableList.index < 0) || (this.m_ReorderableList.index >= this.m_ReorderableList.count))
            {
                Debug.Log("Invalid index to remove " + this.m_ReorderableList.index);
            }
            else if (this.onDeleteItemAtIndex != null)
            {
                this.onDeleteItemAtIndex(this.m_ReorderableList.index);
            }
        }

        private void RenameEnded()
        {
            if (this.GetRenameOverlay().userAcceptedRename && (this.onNameChangedAtIndex != null))
            {
                string str = !string.IsNullOrEmpty(this.GetRenameOverlay().name) ? this.GetRenameOverlay().name : this.GetRenameOverlay().originalName;
                int userData = this.GetRenameOverlay().userData;
                this.onNameChangedAtIndex(userData, str);
            }
            if (this.GetRenameOverlay().HasKeyboardFocus())
            {
                this.m_ReorderableList.GrabKeyboardFocus();
            }
            this.GetRenameOverlay().Clear();
        }

        public void ReorderCallback(ReorderableList list)
        {
            this.m_LastSelectedIndex = list.index;
        }

        public void SelectCallback(ReorderableList list)
        {
            this.FrameItem(list.index);
            if (this.onSelectionChanged != null)
            {
                this.onSelectionChanged(list.index);
            }
        }

        public GUIStyle elementStyle
        {
            get
            {
                if (this.listElementStyle == null)
                {
                }
                return s_Styles.reorderableListLabel;
            }
        }

        public GUIStyle elementStyleRightAligned
        {
            get
            {
                return s_Styles.reorderableListLabelRightAligned;
            }
        }

        public ReorderableList list
        {
            get
            {
                return this.m_ReorderableList;
            }
        }

        [Serializable]
        public class State
        {
            public RenameOverlay m_RenameOverlay = new RenameOverlay();
            public Vector2 m_ScrollPos = new Vector2(0f, 0f);
        }

        public class Styles
        {
            public GUIStyle reorderableListLabel = new GUIStyle("PR Label");
            public GUIStyle reorderableListLabelRightAligned;

            public Styles()
            {
                Texture2D background = this.reorderableListLabel.hover.background;
                this.reorderableListLabel.normal.background = background;
                this.reorderableListLabel.active.background = background;
                this.reorderableListLabel.focused.background = background;
                this.reorderableListLabel.onNormal.background = background;
                this.reorderableListLabel.onHover.background = background;
                this.reorderableListLabel.onActive.background = background;
                this.reorderableListLabel.onFocused.background = background;
                int num = 0;
                this.reorderableListLabel.padding.right = num;
                this.reorderableListLabel.padding.left = num;
                this.reorderableListLabel.alignment = TextAnchor.MiddleLeft;
                this.reorderableListLabelRightAligned = new GUIStyle(this.reorderableListLabel);
                this.reorderableListLabelRightAligned.alignment = TextAnchor.MiddleRight;
                this.reorderableListLabelRightAligned.clipping = TextClipping.Overflow;
            }
        }
    }
}

