namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.Audio;
    using UnityEngine;
    using UnityEngine.Audio;

    internal class AudioMixersTreeView
    {
        [CompilerGenerated]
        private static Func<AudioMixerController, int> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<AudioMixerController, int> <>f__am$cache5;
        private const string kExpandedStateIdentifier = "AudioMixerWindowMixers";
        private const int kObjectSelectorID = 0x4bc;
        private List<AudioMixerController> m_DraggedMixers;
        private TreeView m_TreeView;
        private int m_TreeViewKeyboardControlID;
        private static Styles s_Styles;

        public AudioMixersTreeView(AudioMixerWindow mixerWindow, TreeViewState treeState, Func<List<AudioMixerController>> getAllControllersCallback)
        {
            this.m_TreeView = new TreeView(mixerWindow, treeState);
            this.m_TreeView.deselectOnUnhandledMouseDown = false;
            this.m_TreeView.selectionChangedCallback = (Action<int[]>) Delegate.Combine(this.m_TreeView.selectionChangedCallback, new Action<int[]>(this.OnTreeSelectionChanged));
            this.m_TreeView.contextClickItemCallback = (Action<int>) Delegate.Combine(this.m_TreeView.contextClickItemCallback, new Action<int>(this.OnTreeViewContextClick));
            AudioMixersTreeViewGUI gui = new AudioMixersTreeViewGUI(this.m_TreeView);
            AudioMixersDataSource data = new AudioMixersDataSource(this.m_TreeView, getAllControllersCallback);
            AudioMixerTreeViewDragging dragging = new AudioMixerTreeViewDragging(this.m_TreeView, new Action<List<AudioMixerController>, AudioMixerController>(this.OnMixersDroppedOnMixerCallback));
            this.m_TreeView.Init(mixerWindow.position, data, gui, dragging);
            this.m_TreeView.ReloadData();
        }

        public void DeleteAudioMixerCallback(object obj)
        {
            AudioMixerController controller = (AudioMixerController) obj;
            if (controller != null)
            {
                Selection.activeObject = controller;
                ProjectBrowser.DeleteSelectedAssets(true);
            }
        }

        public void EndRenaming()
        {
            this.m_TreeView.EndNameEditing(true);
        }

        public float GetTotalHeight()
        {
            return (22f + Mathf.Max(20f, this.m_TreeView.gui.GetTotalSize().y));
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
                                ProjectBrowser.DeleteSelectedAssets(true);
                            }
                        }
                        else if (Event.current.commandName == "Duplicate")
                        {
                            Event.current.Use();
                            if (flag)
                            {
                                ProjectWindowUtil.DuplicateSelectedAssets();
                            }
                        }
                        break;
                    }
                }
            }
        }

        private void HandleObjectSelectorResult()
        {
            Event current = Event.current;
            if (current.type == EventType.ExecuteCommand)
            {
                string commandName = current.commandName;
                if ((commandName == "ObjectSelectorUpdated") && (ObjectSelector.get.objectSelectorID == 0x4bc))
                {
                    if ((this.m_DraggedMixers == null) || (this.m_DraggedMixers.Count == 0))
                    {
                        Debug.LogError("Unexpected invalid mixer list used for dragging");
                    }
                    Object currentObject = ObjectSelector.GetCurrentObject();
                    AudioMixerGroup group = (currentObject == null) ? null : (currentObject as AudioMixerGroup);
                    Undo.RecordObjects(this.m_DraggedMixers.ToArray(), "Set output group for mixer" + ((this.m_DraggedMixers.Count <= 1) ? string.Empty : "s"));
                    foreach (AudioMixerController controller in this.m_DraggedMixers)
                    {
                        if (controller != null)
                        {
                            controller.outputAudioMixerGroup = group;
                        }
                        else
                        {
                            Debug.LogError("invalid mixer: is null");
                        }
                    }
                    GUI.changed = true;
                    current.Use();
                    this.ReloadTree();
                    if (<>f__am$cache5 == null)
                    {
                        <>f__am$cache5 = i => i.GetInstanceID();
                    }
                    int[] selectedIDs = this.m_DraggedMixers.Select<AudioMixerController, int>(<>f__am$cache5).ToArray<int>();
                    this.m_TreeView.SetSelection(selectedIDs, true);
                }
                if (commandName == "ObjectSelectorClosed")
                {
                    this.m_DraggedMixers = null;
                }
            }
        }

        public void OnGUI(Rect rect)
        {
            Rect rect2;
            Rect rect3;
            int controlID = GUIUtility.GetControlID(FocusType.Keyboard);
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            this.m_TreeView.OnEvent();
            AudioMixerDrawUtils.DrawRegionBg(rect, out rect2, out rect3);
            AudioMixerDrawUtils.HeaderLabel(rect2, s_Styles.header, s_Styles.audioMixerIcon);
            if (GUI.Button(new Rect(rect2.xMax - 15f, rect2.y + 3f, 15f, 15f), s_Styles.addText, EditorStyles.label))
            {
                (this.m_TreeView.gui as AudioMixersTreeViewGUI).BeginCreateNewMixer();
            }
            this.m_TreeView.OnGUI(rect3, controlID);
            if (this.m_TreeView.data.rowCount == 0)
            {
                EditorGUI.BeginDisabledGroup(true);
                GUI.Label(new RectOffset(-20, 0, -2, 0).Add(rect3), "No mixers found");
                EditorGUI.EndDisabledGroup();
            }
            AudioMixerDrawUtils.DrawScrollDropShadow(rect3, this.m_TreeView.state.scrollPos.y, this.m_TreeView.gui.GetTotalSize().y);
            this.HandleCommandEvents(controlID);
            this.HandleObjectSelectorResult();
        }

        public void OnMixerControllerChanged(AudioMixerController controller)
        {
            if (controller != null)
            {
                int[] selectedIDs = new int[] { controller.GetInstanceID() };
                this.m_TreeView.SetSelection(selectedIDs, true);
            }
        }

        private void OnMixersDroppedOnMixerCallback(List<AudioMixerController> draggedMixers, AudioMixerController droppedUponMixer)
        {
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = i => i.GetInstanceID();
            }
            int[] selectedIDs = draggedMixers.Select<AudioMixerController, int>(<>f__am$cache4).ToArray<int>();
            this.m_TreeView.SetSelection(selectedIDs, true);
            Selection.instanceIDs = selectedIDs;
            if (droppedUponMixer == null)
            {
                Undo.RecordObjects(draggedMixers.ToArray(), "Set output group for mixer" + ((draggedMixers.Count <= 1) ? string.Empty : "s"));
                foreach (AudioMixerController controller in draggedMixers)
                {
                    controller.outputAudioMixerGroup = null;
                }
                this.ReloadTree();
            }
            else
            {
                this.m_DraggedMixers = draggedMixers;
                Object obj2 = (draggedMixers.Count != 1) ? null : draggedMixers[0].outputAudioMixerGroup;
                ObjectSelector.get.Show(obj2, typeof(AudioMixerGroup), null, false, new List<int> { droppedUponMixer.GetInstanceID() });
                ObjectSelector.get.objectSelectorID = 0x4bc;
                ObjectSelector.get.titleContent = new GUIContent("Select Output Audio Mixer Group");
                GUIUtility.ExitGUI();
            }
        }

        public void OnTreeSelectionChanged(int[] selection)
        {
            Selection.instanceIDs = selection;
        }

        public void OnTreeViewContextClick(int index)
        {
            AudioMixerItem item = (AudioMixerItem) this.m_TreeView.FindNode(index);
            if (item != null)
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Delete AudioMixer"), false, new GenericMenu.MenuFunction2(this.DeleteAudioMixerCallback), item.mixer);
                menu.ShowAsContext();
            }
        }

        public void OnUndoRedoPerformed()
        {
            this.ReloadTree();
        }

        public void ReloadTree()
        {
            this.m_TreeView.ReloadData();
            this.m_TreeView.Repaint();
        }

        private class Styles
        {
            public GUIContent addText = new GUIContent("+", "Add mixer asset. The asset will be saved in the same folder as the current selected mixer or if none is selected saved in the Assets folder.");
            public Texture2D audioMixerIcon = EditorGUIUtility.FindTexture("AudioMixerController Icon");
            public GUIContent header = new GUIContent("Mixers", "All mixers in the project are shown here. By default a mixer outputs to the AudioListener but mixers can also route their output to other mixers. Each mixer shows where it outputs (in parenthesis). To reroute a mixer simply drag the mixer upon another mixer and select a group from the popup.");
            public GUIStyle optionsButton = "PaneOptions";
        }
    }
}

