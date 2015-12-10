namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.Audio;
    using UnityEngine;

    internal class AudioMixerGroupTreeView
    {
        [CompilerGenerated]
        private static Func<AudioMixerGroupController, int> <>f__am$cache8;
        [CompilerGenerated]
        private static Func<AudioMixerGroupController, int> <>f__am$cache9;
        private TreeView m_AudioGroupTree;
        private AudioGroupDataSource m_AudioGroupTreeDataSource;
        private TreeViewState m_AudioGroupTreeState;
        private AudioMixerController m_Controller;
        private AudioMixerGroupController m_ScrollToItem;
        private AudioGroupTreeViewGUI m_TreeViewGUI;
        private int m_TreeViewKeyboardControlID;
        private static Styles s_Styles;

        public AudioMixerGroupTreeView(AudioMixerWindow mixerWindow, TreeViewState treeState)
        {
            this.m_AudioGroupTreeState = treeState;
            this.m_AudioGroupTree = new TreeView(mixerWindow, this.m_AudioGroupTreeState);
            this.m_AudioGroupTree.deselectOnUnhandledMouseDown = false;
            this.m_AudioGroupTree.selectionChangedCallback = (Action<int[]>) Delegate.Combine(this.m_AudioGroupTree.selectionChangedCallback, new Action<int[]>(this.OnTreeSelectionChanged));
            this.m_AudioGroupTree.contextClickItemCallback = (Action<int>) Delegate.Combine(this.m_AudioGroupTree.contextClickItemCallback, new Action<int>(this.OnTreeViewContextClick));
            this.m_AudioGroupTree.expandedStateChanged = (Action) Delegate.Combine(this.m_AudioGroupTree.expandedStateChanged, new Action(this.SaveExpandedState));
            this.m_TreeViewGUI = new AudioGroupTreeViewGUI(this.m_AudioGroupTree);
            this.m_TreeViewGUI.NodeWasToggled = (Action<AudioMixerTreeViewNode, bool>) Delegate.Combine(this.m_TreeViewGUI.NodeWasToggled, new Action<AudioMixerTreeViewNode, bool>(this.OnNodeToggled));
            this.m_AudioGroupTreeDataSource = new AudioGroupDataSource(this.m_AudioGroupTree, this.m_Controller);
            this.m_AudioGroupTree.Init(mixerWindow.position, this.m_AudioGroupTreeDataSource, this.m_TreeViewGUI, new AudioGroupTreeViewDragging(this.m_AudioGroupTree, this));
            this.m_AudioGroupTree.ReloadData();
        }

        public void AddAudioMixerGroup(AudioMixerGroupController parent)
        {
            if ((parent != null) && (this.m_Controller != null))
            {
                Object[] objectsToUndo = new Object[] { this.m_Controller, parent };
                Undo.RecordObjects(objectsToUndo, "Add Child Group");
                AudioMixerGroupController child = this.m_Controller.CreateNewGroup("New Group", true);
                this.m_Controller.AddChildToParent(child, parent);
                this.m_Controller.AddGroupToCurrentView(child);
                AudioMixerGroupController[] controllerArray1 = new AudioMixerGroupController[] { child };
                Selection.objects = controllerArray1;
                this.m_Controller.OnUnitySelectionChanged();
                int[] selectedIDs = new int[] { child.GetInstanceID() };
                this.m_AudioGroupTree.SetSelection(selectedIDs, true);
                this.ReloadTree();
                this.m_AudioGroupTree.BeginNameEditing(0f);
            }
        }

        public void AddChildGroupPopupCallback(object obj)
        {
            AudioMixerGroupPopupContext context = (AudioMixerGroupPopupContext) obj;
            if ((context.groups != null) && (context.groups.Length > 0))
            {
                this.AddAudioMixerGroup(context.groups[0]);
            }
        }

        public void AddSiblingGroupPopupCallback(object obj)
        {
            AudioMixerGroupPopupContext context = (AudioMixerGroupPopupContext) obj;
            if ((context.groups != null) && (context.groups.Length > 0))
            {
                AudioMixerTreeViewNode node = this.m_AudioGroupTree.FindNode(context.groups[0].GetInstanceID()) as AudioMixerTreeViewNode;
                if (node != null)
                {
                    AudioMixerTreeViewNode parent = node.parent as AudioMixerTreeViewNode;
                    this.AddAudioMixerGroup(parent.group);
                }
            }
        }

        public void DeleteGroups(List<AudioMixerGroupController> groups, bool recordUndo)
        {
            foreach (AudioMixerGroupController controller in groups)
            {
                if (controller.HasDependentMixers())
                {
                    if (EditorUtility.DisplayDialog("Referenced Group", "Deleted group is referenced by another AudioMixer, are you sure?", "Delete", "Cancel"))
                    {
                        break;
                    }
                    return;
                }
            }
            if (recordUndo)
            {
                Undo.RegisterCompleteObjectUndo(this.m_Controller, "Delete Group" + PluralIfNeeded(groups.Count));
            }
            this.m_Controller.DeleteGroups(groups.ToArray());
            this.ReloadTree();
        }

        private void DeleteGroupsPopupCallback(object obj)
        {
            ((AudioMixerGroupTreeView) obj).DeleteGroups(this.GetGroupSelectionWithoutMasterGroup(), true);
        }

        private void DuplicateGroupPopupCallback(object obj)
        {
            ((AudioMixerGroupTreeView) obj).DuplicateGroups(this.GetGroupSelectionWithoutMasterGroup(), true);
        }

        public void DuplicateGroups(List<AudioMixerGroupController> groups, bool recordUndo)
        {
            if (recordUndo)
            {
                Undo.RecordObject(this.m_Controller, "Duplicate group" + PluralIfNeeded(groups.Count));
            }
            List<AudioMixerGroupController> source = this.m_Controller.DuplicateGroups(groups.ToArray());
            if (source.Count > 0)
            {
                this.ReloadTree();
                if (<>f__am$cache8 == null)
                {
                    <>f__am$cache8 = audioMixerGroup => audioMixerGroup.GetInstanceID();
                }
                int[] selectedIDs = source.Select<AudioMixerGroupController, int>(<>f__am$cache8).ToArray<int>();
                this.m_AudioGroupTree.SetSelection(selectedIDs, false);
                this.m_AudioGroupTree.Frame(selectedIDs[selectedIDs.Length - 1], true, false);
            }
        }

        public void EndRenaming()
        {
            this.m_AudioGroupTree.EndNameEditing(true);
        }

        private List<AudioMixerGroupController> GetAudioMixerGroupsFromNodeIDs(int[] instanceIDs)
        {
            List<AudioMixerGroupController> list = new List<AudioMixerGroupController>();
            foreach (int num in instanceIDs)
            {
                TreeViewItem item = this.m_AudioGroupTree.FindNode(num);
                if (item != null)
                {
                    AudioMixerTreeViewNode node = item as AudioMixerTreeViewNode;
                    if (node != null)
                    {
                        list.Add(node.group);
                    }
                }
            }
            return list;
        }

        private List<AudioMixerGroupController> GetGroupSelectionWithoutMasterGroup()
        {
            List<AudioMixerGroupController> audioMixerGroupsFromNodeIDs = this.GetAudioMixerGroupsFromNodeIDs(this.m_AudioGroupTree.GetSelection());
            audioMixerGroupsFromNodeIDs.Remove(this.m_Controller.masterGroup);
            return audioMixerGroupsFromNodeIDs;
        }

        public float GetTotalHeight()
        {
            if (this.m_Controller == null)
            {
                return 0f;
            }
            return (this.m_AudioGroupTree.gui.GetTotalSize().y + 22f);
        }

        private static string GetUniqueAudioMixerName(AudioMixerController controller)
        {
            return ("AudioMixer_" + controller.GetInstanceID());
        }

        private void HandleCommandEvents(int treeViewKeyboardControlID)
        {
            if (GUIUtility.keyboardControl == treeViewKeyboardControlID)
            {
                EventType type = Event.current.type;
                switch (type)
                {
                    case EventType.ExecuteCommand:
                    case EventType.ValidateCommand:
                    {
                        bool flag = type == EventType.ExecuteCommand;
                        if ((Event.current.commandName == "Delete") || (Event.current.commandName == "SoftDelete"))
                        {
                            Event.current.Use();
                            if (flag)
                            {
                                this.DeleteGroups(this.GetGroupSelectionWithoutMasterGroup(), true);
                                GUIUtility.ExitGUI();
                            }
                        }
                        else if (Event.current.commandName == "Duplicate")
                        {
                            Event.current.Use();
                            if (flag)
                            {
                                this.DuplicateGroups(this.GetGroupSelectionWithoutMasterGroup(), true);
                            }
                        }
                        break;
                    }
                }
            }
        }

        private void HandleKeyboardEvents(int treeViewKeyboardControlID)
        {
            if (GUIUtility.keyboardControl == treeViewKeyboardControlID)
            {
                Event current = Event.current;
                if ((current.keyCode == KeyCode.Space) && (current.type == EventType.KeyDown))
                {
                    int[] selection = this.m_AudioGroupTree.GetSelection();
                    if (selection.Length > 0)
                    {
                        AudioMixerTreeViewNode node = this.m_AudioGroupTree.FindNode(selection[0]) as AudioMixerTreeViewNode;
                        bool flag = this.m_Controller.CurrentViewContainsGroup(node.group.groupID);
                        this.OnNodeToggled(node, !flag);
                        current.Use();
                    }
                }
            }
        }

        public void InitSelection(bool revealSelectionAndFrameLastSelected)
        {
            if (this.m_Controller != null)
            {
                List<AudioMixerGroupController> cachedSelection = this.m_Controller.CachedSelection;
                if (<>f__am$cache9 == null)
                {
                    <>f__am$cache9 = x => x.GetInstanceID();
                }
                this.m_AudioGroupTree.SetSelection(cachedSelection.Select<AudioMixerGroupController, int>(<>f__am$cache9).ToArray<int>(), revealSelectionAndFrameLastSelected);
            }
        }

        private void LoadExpandedState()
        {
            int[] intArray = SessionState.GetIntArray(GetUniqueAudioMixerName(this.m_Controller), null);
            if (intArray != null)
            {
                this.m_AudioGroupTreeState.expandedIDs = new List<int>(intArray);
            }
            else
            {
                this.m_AudioGroupTree.state.expandedIDs = new List<int>();
                this.m_AudioGroupTree.data.SetExpandedWithChildren(this.m_AudioGroupTree.data.root, true);
            }
        }

        public void OnGUI(Rect rect)
        {
            Rect rect2;
            Rect rect3;
            int controlID = GUIUtility.GetControlID(FocusType.Keyboard);
            this.m_ScrollToItem = null;
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            this.m_AudioGroupTree.OnEvent();
            EditorGUI.BeginDisabledGroup(this.m_Controller == null);
            AudioMixerDrawUtils.DrawRegionBg(rect, out rect2, out rect3);
            AudioMixerDrawUtils.HeaderLabel(rect2, s_Styles.header, s_Styles.audioMixerGroupIcon);
            EditorGUI.EndDisabledGroup();
            if (this.m_Controller != null)
            {
                AudioMixerGroupController parent = (this.m_Controller.CachedSelection.Count != 1) ? this.m_Controller.masterGroup : this.m_Controller.CachedSelection[0];
                EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
                if (GUI.Button(new Rect(rect2.xMax - 15f, rect2.y + 3f, 15f, 15f), s_Styles.addText, EditorStyles.label))
                {
                    this.AddAudioMixerGroup(parent);
                }
                EditorGUI.EndDisabledGroup();
                this.m_AudioGroupTree.OnGUI(rect3, controlID);
                AudioMixerDrawUtils.DrawScrollDropShadow(rect3, this.m_AudioGroupTree.state.scrollPos.y, this.m_AudioGroupTree.gui.GetTotalSize().y);
                this.HandleKeyboardEvents(controlID);
                this.HandleCommandEvents(controlID);
            }
        }

        public void OnMixerControllerChanged(AudioMixerController controller)
        {
            if (this.m_Controller != controller)
            {
                this.m_TreeViewGUI.m_Controller = controller;
                this.m_Controller = controller;
                this.m_AudioGroupTreeDataSource.m_Controller = controller;
                if (controller != null)
                {
                    this.ReloadTree();
                    this.InitSelection(false);
                    this.LoadExpandedState();
                    this.m_AudioGroupTree.data.SetExpandedWithChildren(this.m_AudioGroupTree.data.root, true);
                }
            }
        }

        private void OnNodeToggled(AudioMixerTreeViewNode node, bool nodeWasEnabled)
        {
            List<AudioMixerGroupController> audioMixerGroupsFromNodeIDs = this.GetAudioMixerGroupsFromNodeIDs(this.m_AudioGroupTree.GetSelection());
            if (!audioMixerGroupsFromNodeIDs.Contains(node.group))
            {
                audioMixerGroupsFromNodeIDs = new List<AudioMixerGroupController> {
                    node.group
                };
            }
            List<GUID> list2 = new List<GUID>();
            foreach (AudioMixerGroupController controller in this.m_Controller.GetAllAudioGroupsSlow())
            {
                bool flag = this.m_Controller.CurrentViewContainsGroup(controller.groupID);
                bool flag2 = audioMixerGroupsFromNodeIDs.Contains(controller);
                bool flag3 = flag && !flag2;
                if (!flag && flag2)
                {
                    flag3 = nodeWasEnabled;
                }
                if (flag3)
                {
                    list2.Add(controller.groupID);
                }
            }
            this.m_Controller.SetCurrentViewVisibility(list2.ToArray());
        }

        public void OnTreeSelectionChanged(int[] selection)
        {
            List<AudioMixerGroupController> audioMixerGroupsFromNodeIDs = this.GetAudioMixerGroupsFromNodeIDs(selection);
            Selection.objects = audioMixerGroupsFromNodeIDs.ToArray();
            this.m_Controller.OnUnitySelectionChanged();
            if (audioMixerGroupsFromNodeIDs.Count == 1)
            {
                this.m_ScrollToItem = audioMixerGroupsFromNodeIDs[0];
            }
            InspectorWindow.RepaintAllInspectors();
        }

        public void OnTreeViewContextClick(int index)
        {
            TreeViewItem userData = this.m_AudioGroupTree.FindNode(index);
            if (userData != null)
            {
                AudioMixerTreeViewNode node = userData as AudioMixerTreeViewNode;
                if ((node != null) && (node.group != null))
                {
                    GenericMenu menu = new GenericMenu();
                    if (!EditorApplication.isPlaying)
                    {
                        menu.AddItem(new GUIContent("Add child group"), false, new GenericMenu.MenuFunction2(this.AddChildGroupPopupCallback), new AudioMixerGroupPopupContext(this.m_Controller, node.group));
                        if (node.group != this.m_Controller.masterGroup)
                        {
                            menu.AddItem(new GUIContent("Add sibling group"), false, new GenericMenu.MenuFunction2(this.AddSiblingGroupPopupCallback), new AudioMixerGroupPopupContext(this.m_Controller, node.group));
                            menu.AddSeparator(string.Empty);
                            menu.AddItem(new GUIContent("Rename"), false, new GenericMenu.MenuFunction2(this.RenameGroupCallback), userData);
                            AudioMixerGroupController[] controllerArray = this.GetGroupSelectionWithoutMasterGroup().ToArray();
                            menu.AddItem(new GUIContent((controllerArray.Length <= 1) ? "Duplicate group (and children)" : "Duplicate groups (and children)"), false, new GenericMenu.MenuFunction2(this.DuplicateGroupPopupCallback), this);
                            menu.AddItem(new GUIContent((controllerArray.Length <= 1) ? "Remove group (and children)" : "Remove groups (and children)"), false, new GenericMenu.MenuFunction2(this.DeleteGroupsPopupCallback), this);
                        }
                    }
                    else
                    {
                        menu.AddDisabledItem(new GUIContent("Modifying group topology in play mode is not allowed"));
                    }
                    menu.ShowAsContext();
                }
            }
        }

        public void OnUndoRedoPerformed()
        {
            this.ReloadTree();
        }

        private static string PluralIfNeeded(int count)
        {
            return ((count <= 1) ? string.Empty : "s");
        }

        public void ReloadTree()
        {
            this.m_AudioGroupTree.ReloadData();
            if (this.m_Controller != null)
            {
                this.m_Controller.SanitizeGroupViews();
                this.m_Controller.OnSubAssetChanged();
            }
        }

        private void RenameGroupCallback(object obj)
        {
            TreeViewItem item = (TreeViewItem) obj;
            int[] selectedIDs = new int[] { item.id };
            this.m_AudioGroupTree.SetSelection(selectedIDs, false);
            this.m_AudioGroupTree.BeginNameEditing(0f);
        }

        private void SaveExpandedState()
        {
            SessionState.SetIntArray(GetUniqueAudioMixerName(this.m_Controller), this.m_AudioGroupTreeState.expandedIDs.ToArray());
        }

        public void UseScrollView(bool useScrollView)
        {
            this.m_AudioGroupTree.SetUseScrollView(useScrollView);
        }

        public AudioMixerController Controller
        {
            get
            {
                return this.m_Controller;
            }
        }

        public AudioMixerGroupController ScrollToItem
        {
            get
            {
                return this.m_ScrollToItem;
            }
        }

        private class Styles
        {
            public GUIContent addText = new GUIContent("+", "Add child group");
            public Texture2D audioMixerGroupIcon = EditorGUIUtility.FindTexture("AudioMixerGroup Icon");
            public GUIContent header = new GUIContent("Groups", "An Audio Mixer Group is used by e.g Audio Sources to modify the audio output before it reaches the Audio Listener. An Audio Mixer Group will route its output to another Audio Mixer Group if it is made a child of that group. The Master Group will route its output to the Audio Listener if it doesn't route its output into another Mixer.");
            public GUIStyle optionsButton = "PaneOptions";
        }
    }
}

