namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.Audio;
    using UnityEditorInternal;
    using UnityEngine;

    internal class AudioMixerGroupViewList
    {
        [CompilerGenerated]
        private static Func<AudioMixerGroupController, GUID> <>f__am$cache5;
        private AudioMixerController m_Controller;
        private ReorderableListWithRenameAndScrollView m_ReorderableListWithRenameAndScrollView;
        private readonly ReorderableListWithRenameAndScrollView.State m_State;
        private List<MixerGroupView> m_Views;
        private static Styles s_Styles;

        public AudioMixerGroupViewList(ReorderableListWithRenameAndScrollView.State state)
        {
            this.m_State = state;
        }

        private void Add()
        {
            this.m_Controller.CloneViewFromCurrent();
            this.LoadFromBackend();
            int index = this.m_Views.Count - 1;
            this.m_Controller.currentViewIndex = index;
            this.m_ReorderableListWithRenameAndScrollView.BeginRename(index, 0f);
        }

        public void CustomDrawElement(Rect r, int index, bool isActive, bool isFocused)
        {
            Event current = Event.current;
            if (((current.type == EventType.MouseUp) && (current.button == 1)) && r.Contains(current.mousePosition))
            {
                ViewsContexttMenu.Show(r, index, this);
                current.Use();
            }
            bool isSelected = (index == this.m_ReorderableListWithRenameAndScrollView.list.index) && !this.m_ReorderableListWithRenameAndScrollView.IsRenamingIndex(index);
            this.m_ReorderableListWithRenameAndScrollView.DrawElementText(r, index, isActive, isSelected, isFocused);
        }

        private void Delete(int index)
        {
            if (this.m_Views.Count <= 1)
            {
                Debug.Log("Deleting all views is not allowed");
            }
            else
            {
                this.m_Controller.DeleteView(index);
                this.LoadFromBackend();
            }
        }

        private void DuplicateCurrentView()
        {
            this.m_Controller.CloneViewFromCurrent();
            this.LoadFromBackend();
        }

        public void EndDragChild(ReorderableList list)
        {
            this.m_Views = this.m_ReorderableListWithRenameAndScrollView.list.list as List<MixerGroupView>;
            this.SaveToBackend();
        }

        private string GetNameOfElement(int index)
        {
            MixerGroupView view = this.m_Views[index];
            return view.name;
        }

        public float GetTotalHeight()
        {
            if (this.m_Controller == null)
            {
                return 0f;
            }
            return (this.m_ReorderableListWithRenameAndScrollView.list.GetHeight() + 22f);
        }

        private void LoadFromBackend()
        {
            this.m_Views.Clear();
            this.m_Views.AddRange(this.m_Controller.views);
        }

        public void NameChanged(int index, string newName)
        {
            this.LoadFromBackend();
            MixerGroupView view = this.m_Views[index];
            view.name = newName;
            this.m_Views[index] = view;
            this.SaveToBackend();
        }

        public void OnEvent()
        {
            if (this.m_Controller != null)
            {
                this.m_ReorderableListWithRenameAndScrollView.OnEvent();
            }
        }

        public void OnGUI(Rect rect)
        {
            Rect rect2;
            Rect rect3;
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            EditorGUI.BeginDisabledGroup(this.m_Controller == null);
            AudioMixerDrawUtils.DrawRegionBg(rect, out rect2, out rect3);
            AudioMixerDrawUtils.HeaderLabel(rect2, s_Styles.header, s_Styles.viewsIcon);
            EditorGUI.EndDisabledGroup();
            if (this.m_Controller != null)
            {
                if (this.m_ReorderableListWithRenameAndScrollView.list.index != this.m_Controller.currentViewIndex)
                {
                    this.m_ReorderableListWithRenameAndScrollView.list.index = this.m_Controller.currentViewIndex;
                    this.m_ReorderableListWithRenameAndScrollView.FrameItem(this.m_Controller.currentViewIndex);
                }
                this.m_ReorderableListWithRenameAndScrollView.OnGUI(rect3);
                if (GUI.Button(new Rect(rect2.xMax - 15f, rect2.y + 3f, 15f, 15f), s_Styles.addButton, EditorStyles.label))
                {
                    this.Add();
                }
            }
        }

        public void OnMixerControllerChanged(AudioMixerController controller)
        {
            this.m_Controller = controller;
            this.RecreateListControl();
        }

        public void OnUndoRedoPerformed()
        {
            this.RecreateListControl();
        }

        private void RecreateListControl()
        {
            if (this.m_Controller != null)
            {
                ReorderableList list;
                this.m_Views = new List<MixerGroupView>(this.m_Controller.views);
                if (this.m_Views.Count == 0)
                {
                    MixerGroupView item = new MixerGroupView();
                    if (<>f__am$cache5 == null)
                    {
                        <>f__am$cache5 = gr => gr.groupID;
                    }
                    item.guids = this.m_Controller.GetAllAudioGroupsSlow().Select<AudioMixerGroupController, GUID>(<>f__am$cache5).ToArray<GUID>();
                    item.name = "View";
                    this.m_Views.Add(item);
                    this.SaveToBackend();
                }
                list = new ReorderableList(this.m_Views, typeof(MixerGroupView), true, false, false, false) {
                    onReorderCallback = (ReorderableList.ReorderCallbackDelegate) Delegate.Combine(list.onReorderCallback, new ReorderableList.ReorderCallbackDelegate(this.EndDragChild)),
                    elementHeight = 16f,
                    headerHeight = 0f,
                    footerHeight = 0f,
                    showDefaultBackground = false,
                    index = this.m_Controller.currentViewIndex
                };
                if (this.m_Controller.currentViewIndex >= list.count)
                {
                    Debug.LogError(string.Concat(new object[] { "State mismatch, currentViewIndex: ", this.m_Controller.currentViewIndex, ", num items: ", list.count }));
                }
                this.m_ReorderableListWithRenameAndScrollView = new ReorderableListWithRenameAndScrollView(list, this.m_State);
                this.m_ReorderableListWithRenameAndScrollView.onSelectionChanged = (Action<int>) Delegate.Combine(this.m_ReorderableListWithRenameAndScrollView.onSelectionChanged, new Action<int>(this.SelectionChanged));
                this.m_ReorderableListWithRenameAndScrollView.onNameChangedAtIndex = (Action<int, string>) Delegate.Combine(this.m_ReorderableListWithRenameAndScrollView.onNameChangedAtIndex, new Action<int, string>(this.NameChanged));
                this.m_ReorderableListWithRenameAndScrollView.onDeleteItemAtIndex = (Action<int>) Delegate.Combine(this.m_ReorderableListWithRenameAndScrollView.onDeleteItemAtIndex, new Action<int>(this.Delete));
                this.m_ReorderableListWithRenameAndScrollView.onGetNameAtIndex = (Func<int, string>) Delegate.Combine(this.m_ReorderableListWithRenameAndScrollView.onGetNameAtIndex, new Func<int, string>(this.GetNameOfElement));
                this.m_ReorderableListWithRenameAndScrollView.onCustomDrawElement = (ReorderableList.ElementCallbackDelegate) Delegate.Combine(this.m_ReorderableListWithRenameAndScrollView.onCustomDrawElement, new ReorderableList.ElementCallbackDelegate(this.CustomDrawElement));
            }
        }

        private void Rename(int index)
        {
            this.m_ReorderableListWithRenameAndScrollView.BeginRename(index, 0f);
        }

        private void SaveToBackend()
        {
            this.m_Controller.views = this.m_Views.ToArray();
        }

        public void SelectionChanged(int selectedIndex)
        {
            this.LoadFromBackend();
            this.m_Controller.SetView(selectedIndex);
        }

        private class Styles
        {
            public GUIContent addButton = new GUIContent("+");
            public GUIContent header = new GUIContent("Views", "A view is the saved visiblity state of the current Mixer Groups. Use views to setup often used combinations of Mixer Groups.");
            public Texture2D viewsIcon = EditorGUIUtility.FindTexture("AudioMixerView Icon");
        }

        internal class ViewsContexttMenu
        {
            private static void Delete(object userData)
            {
                AudioMixerGroupViewList.ViewsContexttMenu.data data = userData as AudioMixerGroupViewList.ViewsContexttMenu.data;
                data.list.Delete(data.viewIndex);
            }

            private static void Duplicate(object userData)
            {
                AudioMixerGroupViewList.ViewsContexttMenu.data data = userData as AudioMixerGroupViewList.ViewsContexttMenu.data;
                data.list.m_Controller.currentViewIndex = data.viewIndex;
                data.list.DuplicateCurrentView();
            }

            private static void Rename(object userData)
            {
                AudioMixerGroupViewList.ViewsContexttMenu.data data = userData as AudioMixerGroupViewList.ViewsContexttMenu.data;
                data.list.Rename(data.viewIndex);
            }

            public static void Show(Rect buttonRect, int viewIndex, AudioMixerGroupViewList list)
            {
                GenericMenu menu = new GenericMenu();
                data userData = new data {
                    viewIndex = viewIndex,
                    list = list
                };
                menu.AddItem(new GUIContent("Rename"), false, new GenericMenu.MenuFunction2(AudioMixerGroupViewList.ViewsContexttMenu.Rename), userData);
                menu.AddItem(new GUIContent("Duplicate"), false, new GenericMenu.MenuFunction2(AudioMixerGroupViewList.ViewsContexttMenu.Duplicate), userData);
                menu.AddItem(new GUIContent("Delete"), false, new GenericMenu.MenuFunction2(AudioMixerGroupViewList.ViewsContexttMenu.Delete), userData);
                menu.DropDown(buttonRect);
            }

            private class data
            {
                public AudioMixerGroupViewList list;
                public int viewIndex;
            }
        }
    }
}

