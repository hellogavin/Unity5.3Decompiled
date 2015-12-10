namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.Audio;
    using UnityEngine;

    internal class AudioMixerTreeViewDragging : TreeViewDragging
    {
        [CompilerGenerated]
        private static Func<AudioMixerItem, AudioMixerGroupController> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<AudioMixerItem, int> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<AudioMixerItem, AudioMixerController> <>f__am$cache3;
        private const string k_AudioMixerDraggingID = "AudioMixerDragging";
        private Action<List<AudioMixerController>, AudioMixerController> m_MixersDroppedOnMixerCallback;

        public AudioMixerTreeViewDragging(TreeView treeView, Action<List<AudioMixerController>, AudioMixerController> mixerDroppedOnMixerCallback) : base(treeView)
        {
            this.m_MixersDroppedOnMixerCallback = mixerDroppedOnMixerCallback;
        }

        public override DragAndDropVisualMode DoDrag(TreeViewItem parentNode, TreeViewItem targetNode, bool perform, TreeViewDragging.DropPosition dragPos)
        {
            DragData genericData = DragAndDrop.GetGenericData("AudioMixerDragging") as DragData;
            if (genericData == null)
            {
                return DragAndDropVisualMode.None;
            }
            List<AudioMixerItem> draggedItems = genericData.m_DraggedItems;
            AudioMixerItem item = parentNode as AudioMixerItem;
            if ((item == null) || (genericData == null))
            {
                return DragAndDropVisualMode.None;
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = i => i.mixer.masterGroup;
            }
            List<AudioMixerGroupController> groupsToBeMoved = draggedItems.Select<AudioMixerItem, AudioMixerGroupController>(<>f__am$cache1).ToList<AudioMixerGroupController>();
            bool flag = AudioMixerController.WillModificationOfTopologyCauseFeedback(item.mixer.GetAllAudioGroupsSlow(), groupsToBeMoved, item.mixer.masterGroup, null);
            bool flag2 = this.ValidDrag(parentNode, draggedItems) && !flag;
            if ((perform && flag2) && (this.m_MixersDroppedOnMixerCallback != null))
            {
                this.m_MixersDroppedOnMixerCallback(this.GetAudioMixersFromItems(draggedItems), item.mixer);
            }
            return (!flag2 ? DragAndDropVisualMode.Rejected : DragAndDropVisualMode.Move);
        }

        public override bool DragElement(TreeViewItem targetItem, Rect targetItemRect, bool firstItem)
        {
            DragData genericData = DragAndDrop.GetGenericData("AudioMixerDragging") as DragData;
            if (genericData == null)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.None;
                return false;
            }
            if ((targetItem != null) || !base.m_TreeView.GetTotalRect().Contains(Event.current.mousePosition))
            {
                return base.DragElement(targetItem, targetItemRect, firstItem);
            }
            if (base.m_DropData != null)
            {
                base.m_DropData.dropTargetControlID = 0;
                base.m_DropData.rowMarkerControlID = 0;
            }
            if (Event.current.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                if (this.m_MixersDroppedOnMixerCallback != null)
                {
                    this.m_MixersDroppedOnMixerCallback(this.GetAudioMixersFromItems(genericData.m_DraggedItems), null);
                }
            }
            DragAndDrop.visualMode = DragAndDropVisualMode.Move;
            Event.current.Use();
            return false;
        }

        private List<AudioMixerItem> GetAudioMixerItemsFromIDs(List<int> draggedMixers)
        {
            return TreeViewUtility.FindItemsInList(draggedMixers, base.m_TreeView.data.GetRows()).OfType<AudioMixerItem>().ToList<AudioMixerItem>();
        }

        private List<AudioMixerController> GetAudioMixersFromItems(List<AudioMixerItem> draggedItems)
        {
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = i => i.mixer;
            }
            return draggedItems.Select<AudioMixerItem, AudioMixerController>(<>f__am$cache3).ToList<AudioMixerController>();
        }

        public override void StartDrag(TreeViewItem draggedNode, List<int> draggedNodes)
        {
            if (!EditorApplication.isPlaying)
            {
                List<AudioMixerItem> audioMixerItemsFromIDs = this.GetAudioMixerItemsFromIDs(draggedNodes);
                DragAndDrop.PrepareStartDrag();
                DragAndDrop.SetGenericData("AudioMixerDragging", new DragData(audioMixerItemsFromIDs));
                DragAndDrop.objectReferences = new Object[0];
                DragAndDrop.StartDrag(draggedNodes.Count + " AudioMixer" + ((draggedNodes.Count <= 1) ? string.Empty : "s"));
            }
        }

        private bool ValidDrag(TreeViewItem parent, List<AudioMixerItem> draggedItems)
        {
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = n => n.id;
            }
            List<int> list = draggedItems.Select<AudioMixerItem, int>(<>f__am$cache2).ToList<int>();
            for (TreeViewItem item = parent; item != null; item = item.parent)
            {
                if (list.Contains(item.id))
                {
                    return false;
                }
            }
            return true;
        }

        private class DragData
        {
            public List<AudioMixerItem> m_DraggedItems;

            public DragData(List<AudioMixerItem> draggedItems)
            {
                this.m_DraggedItems = draggedItems;
            }
        }
    }
}

