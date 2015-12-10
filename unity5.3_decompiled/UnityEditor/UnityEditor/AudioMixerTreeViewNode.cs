namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor.Audio;

    internal class AudioMixerTreeViewNode : TreeViewItem
    {
        public AudioMixerTreeViewNode(int instanceID, int depth, TreeViewItem parent, string displayName, AudioMixerGroupController group) : base(instanceID, depth, parent, displayName)
        {
            this.group = group;
        }

        public AudioMixerGroupController group { get; set; }
    }
}

