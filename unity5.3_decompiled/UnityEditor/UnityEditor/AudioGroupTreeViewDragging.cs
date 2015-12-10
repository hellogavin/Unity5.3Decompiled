namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.Audio;

    internal class AudioGroupTreeViewDragging : AssetsTreeViewDragging
    {
        [CompilerGenerated]
        private static Func<AudioMixerGroupController, int> <>f__am$cache1;
        private AudioMixerGroupTreeView m_owner;

        public AudioGroupTreeViewDragging(TreeView treeView, AudioMixerGroupTreeView owner) : base(treeView)
        {
            this.m_owner = owner;
        }

        public override DragAndDropVisualMode DoDrag(TreeViewItem parentNode, TreeViewItem targetNode, bool perform, TreeViewDragging.DropPosition dragPos)
        {
            AudioMixerTreeViewNode node = targetNode as AudioMixerTreeViewNode;
            AudioMixerTreeViewNode node2 = parentNode as AudioMixerTreeViewNode;
            List<AudioMixerGroupController> source = new List<Object>(DragAndDrop.objectReferences).OfType<AudioMixerGroupController>().ToList<AudioMixerGroupController>();
            if ((node2 == null) || (source.Count <= 0))
            {
                return DragAndDropVisualMode.None;
            }
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = i => i.GetInstanceID();
            }
            List<int> draggedInstanceIDs = source.Select<AudioMixerGroupController, int>(<>f__am$cache1).ToList<int>();
            bool flag = this.ValidDrag(parentNode, draggedInstanceIDs) && !AudioMixerController.WillModificationOfTopologyCauseFeedback(this.m_owner.Controller.GetAllAudioGroupsSlow(), source, node2.group, null);
            if (perform && flag)
            {
                AudioMixerGroupController group = node2.group;
                this.m_owner.Controller.ReparentSelection(group, node.group, source);
                this.m_owner.ReloadTree();
                base.m_TreeView.SetSelection(draggedInstanceIDs.ToArray(), true);
            }
            return (!flag ? DragAndDropVisualMode.Rejected : DragAndDropVisualMode.Move);
        }

        public override void StartDrag(TreeViewItem draggedItem, List<int> draggedItemIDs)
        {
            if (!EditorApplication.isPlaying)
            {
                base.StartDrag(draggedItem, draggedItemIDs);
            }
        }

        private bool ValidDrag(TreeViewItem parent, List<int> draggedInstanceIDs)
        {
            for (TreeViewItem item = parent; item != null; item = item.parent)
            {
                if (draggedInstanceIDs.Contains(item.id))
                {
                    return false;
                }
            }
            return true;
        }
    }
}

