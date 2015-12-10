namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEditor.Audio;
    using UnityEditorInternal;
    using UnityEngine;

    internal class AudioMixerSnapshotListView
    {
        private AudioMixerController m_Controller;
        private ReorderableListWithRenameAndScrollView m_ReorderableListWithRenameAndScrollView;
        private List<AudioMixerSnapshotController> m_Snapshots;
        private ReorderableListWithRenameAndScrollView.State m_State;
        private static Styles s_Styles;

        public AudioMixerSnapshotListView(ReorderableListWithRenameAndScrollView.State state)
        {
            this.m_State = state;
        }

        private void Add()
        {
            Undo.RecordObject(this.m_Controller, "Add new snapshot");
            this.m_Controller.CloneNewSnapshotFromTarget(true);
            this.LoadFromBackend();
            this.Rename(this.m_Controller.TargetSnapshot);
            this.UpdateViews();
        }

        public void CustomDrawElement(Rect r, int index, bool isActive, bool isFocused)
        {
            Event current = Event.current;
            if (((current.type == EventType.MouseUp) && (current.button == 1)) && r.Contains(current.mousePosition))
            {
                SnapshotMenu.Show(r, this.m_Snapshots[index], this);
                current.Use();
            }
            bool isSelected = (index == this.m_ReorderableListWithRenameAndScrollView.list.index) && !this.m_ReorderableListWithRenameAndScrollView.IsRenamingIndex(index);
            r.width -= 19f;
            this.m_ReorderableListWithRenameAndScrollView.DrawElementText(r, index, isActive, isSelected, isFocused);
            if (this.m_Controller.startSnapshot == this.m_Snapshots[index])
            {
                r.x = (r.xMax + 5f) + 5f;
                r.y += (r.height - 14f) / 2f;
                float num3 = 14f;
                r.height = num3;
                r.width = num3;
                GUI.Label(r, s_Styles.starIcon, GUIStyle.none);
            }
        }

        private void Delete(int index)
        {
            this.DeleteSnapshot(this.m_Snapshots[index]);
        }

        private void DeleteSnapshot(AudioMixerSnapshotController snapshot)
        {
            if (this.m_Controller.snapshots.Length <= 1)
            {
                Debug.Log("You must have at least 1 snapshot in an AudioMixer.");
            }
            else
            {
                this.m_Controller.RemoveSnapshot(snapshot);
                this.LoadFromBackend();
                this.m_ReorderableListWithRenameAndScrollView.list.index = this.GetSnapshotIndex(this.m_Controller.TargetSnapshot);
                this.UpdateViews();
            }
        }

        private void DuplicateCurrentSnapshot()
        {
            Undo.RecordObject(this.m_Controller, "Duplicate current snapshot");
            this.m_Controller.CloneNewSnapshotFromTarget(true);
            this.LoadFromBackend();
            this.UpdateViews();
        }

        public void EndDragChild(ReorderableList list)
        {
            this.m_Snapshots = this.m_ReorderableListWithRenameAndScrollView.list.list as List<AudioMixerSnapshotController>;
            this.SaveToBackend();
        }

        private string GetNameOfElement(int index)
        {
            return this.m_Snapshots[index].name;
        }

        private int GetSnapshotIndex(AudioMixerSnapshotController snapshot)
        {
            for (int i = 0; i < this.m_Snapshots.Count; i++)
            {
                if (this.m_Snapshots[i] == snapshot)
                {
                    return i;
                }
            }
            return 0;
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
            if (this.m_Controller != null)
            {
                this.m_Snapshots.Clear();
                this.m_Snapshots.AddRange(this.m_Controller.snapshots);
            }
        }

        public void NameChanged(int index, string newName)
        {
            this.m_Snapshots[index].name = newName;
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
            AudioMixerDrawUtils.HeaderLabel(rect2, s_Styles.header, s_Styles.snapshotsIcon);
            EditorGUI.EndDisabledGroup();
            if (this.m_Controller != null)
            {
                int snapshotIndex = this.GetSnapshotIndex(this.m_Controller.TargetSnapshot);
                if (snapshotIndex != this.m_ReorderableListWithRenameAndScrollView.list.index)
                {
                    this.m_ReorderableListWithRenameAndScrollView.list.index = snapshotIndex;
                    this.m_ReorderableListWithRenameAndScrollView.FrameItem(snapshotIndex);
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
            this.LoadFromBackend();
        }

        private void RecreateListControl()
        {
            if (this.m_Controller != null)
            {
                this.m_Snapshots = new List<AudioMixerSnapshotController>(this.m_Controller.snapshots);
                ReorderableList list = new ReorderableList(this.m_Snapshots, typeof(AudioMixerSnapshotController), true, false, false, false) {
                    onReorderCallback = new ReorderableList.ReorderCallbackDelegate(this.EndDragChild),
                    elementHeight = 16f,
                    headerHeight = 0f,
                    footerHeight = 0f,
                    showDefaultBackground = false,
                    index = this.GetSnapshotIndex(this.m_Controller.TargetSnapshot)
                };
                this.m_ReorderableListWithRenameAndScrollView = new ReorderableListWithRenameAndScrollView(list, this.m_State);
                this.m_ReorderableListWithRenameAndScrollView.onSelectionChanged = (Action<int>) Delegate.Combine(this.m_ReorderableListWithRenameAndScrollView.onSelectionChanged, new Action<int>(this.SelectionChanged));
                this.m_ReorderableListWithRenameAndScrollView.onNameChangedAtIndex = (Action<int, string>) Delegate.Combine(this.m_ReorderableListWithRenameAndScrollView.onNameChangedAtIndex, new Action<int, string>(this.NameChanged));
                this.m_ReorderableListWithRenameAndScrollView.onDeleteItemAtIndex = (Action<int>) Delegate.Combine(this.m_ReorderableListWithRenameAndScrollView.onDeleteItemAtIndex, new Action<int>(this.Delete));
                this.m_ReorderableListWithRenameAndScrollView.onGetNameAtIndex = (Func<int, string>) Delegate.Combine(this.m_ReorderableListWithRenameAndScrollView.onGetNameAtIndex, new Func<int, string>(this.GetNameOfElement));
                this.m_ReorderableListWithRenameAndScrollView.onCustomDrawElement = (ReorderableList.ElementCallbackDelegate) Delegate.Combine(this.m_ReorderableListWithRenameAndScrollView.onCustomDrawElement, new ReorderableList.ElementCallbackDelegate(this.CustomDrawElement));
            }
        }

        private void Rename(AudioMixerSnapshotController snapshot)
        {
            this.m_ReorderableListWithRenameAndScrollView.BeginRename(this.GetSnapshotIndex(snapshot), 0f);
        }

        private void SaveToBackend()
        {
            this.m_Controller.snapshots = this.m_Snapshots.ToArray();
            this.m_Controller.OnSubAssetChanged();
        }

        public void SelectionChanged(int index)
        {
            if (index >= this.m_Snapshots.Count)
            {
                index = this.m_Snapshots.Count - 1;
            }
            this.m_Controller.TargetSnapshot = this.m_Snapshots[index];
            this.UpdateViews();
        }

        private void SetAsStartupSnapshot(AudioMixerSnapshotController snapshot)
        {
            this.m_Controller.startSnapshot = snapshot;
        }

        private void UpdateViews()
        {
            AudioMixerWindow window = (AudioMixerWindow) WindowLayout.FindEditorWindowOfType(typeof(AudioMixerWindow));
            if (window != null)
            {
                window.Repaint();
            }
            InspectorWindow.RepaintAllInspectors();
        }

        internal class SnapshotMenu
        {
            private static void Delete(object userData)
            {
                AudioMixerSnapshotListView.SnapshotMenu.data data = userData as AudioMixerSnapshotListView.SnapshotMenu.data;
                data.list.DeleteSnapshot(data.snapshot);
            }

            private static void Duplicate(object userData)
            {
                AudioMixerSnapshotListView.SnapshotMenu.data data = userData as AudioMixerSnapshotListView.SnapshotMenu.data;
                data.list.DuplicateCurrentSnapshot();
            }

            private static void Rename(object userData)
            {
                AudioMixerSnapshotListView.SnapshotMenu.data data = userData as AudioMixerSnapshotListView.SnapshotMenu.data;
                data.list.Rename(data.snapshot);
            }

            private static void SetAsStartupSnapshot(object userData)
            {
                AudioMixerSnapshotListView.SnapshotMenu.data data = userData as AudioMixerSnapshotListView.SnapshotMenu.data;
                data.list.SetAsStartupSnapshot(data.snapshot);
            }

            public static void Show(Rect buttonRect, AudioMixerSnapshotController snapshot, AudioMixerSnapshotListView list)
            {
                GenericMenu menu = new GenericMenu();
                data userData = new data {
                    snapshot = snapshot,
                    list = list
                };
                menu.AddItem(new GUIContent("Set as start Snapshot"), false, new GenericMenu.MenuFunction2(AudioMixerSnapshotListView.SnapshotMenu.SetAsStartupSnapshot), userData);
                menu.AddSeparator(string.Empty);
                menu.AddItem(new GUIContent("Rename"), false, new GenericMenu.MenuFunction2(AudioMixerSnapshotListView.SnapshotMenu.Rename), userData);
                menu.AddItem(new GUIContent("Duplicate"), false, new GenericMenu.MenuFunction2(AudioMixerSnapshotListView.SnapshotMenu.Duplicate), userData);
                menu.AddItem(new GUIContent("Delete"), false, new GenericMenu.MenuFunction2(AudioMixerSnapshotListView.SnapshotMenu.Delete), userData);
                menu.DropDown(buttonRect);
            }

            private class data
            {
                public AudioMixerSnapshotListView list;
                public AudioMixerSnapshotController snapshot;
            }
        }

        private class Styles
        {
            public GUIContent addButton = new GUIContent("+");
            public GUIContent header = new GUIContent("Snapshots", "A snapshot is a set of values for all parameters in the mixer. When using the mixer you modify parameters in the selected snapshot. Blend between multiple snapshots at runtime.");
            public Texture2D snapshotsIcon = EditorGUIUtility.FindTexture("AudioMixerSnapshot Icon");
            public GUIContent starIcon = new GUIContent(EditorGUIUtility.FindTexture("Favorite"), "Start snapshot");
        }
    }
}

