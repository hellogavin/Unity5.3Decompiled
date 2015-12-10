namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.Audio;
    using UnityEngine;

    internal class AudioMixerSelection
    {
        private AudioMixerController m_Controller;

        public AudioMixerSelection(AudioMixerController controller)
        {
            this.m_Controller = controller;
            this.ChannelStripSelection = new List<AudioMixerGroupController>();
            this.SyncToUnitySelection();
        }

        public void ClearChannelStrips()
        {
            Selection.objects = new Object[0];
        }

        public bool HasSingleChannelStripSelection()
        {
            return (this.ChannelStripSelection.Count == 1);
        }

        private void RefreshCachedChannelStripSelection()
        {
            Object[] filtered = Selection.GetFiltered(typeof(AudioMixerGroupController), SelectionMode.Deep);
            this.ChannelStripSelection = new List<AudioMixerGroupController>();
            foreach (AudioMixerGroupController controller in this.m_Controller.GetAllAudioGroupsSlow())
            {
                if (filtered.Contains<Object>(controller))
                {
                    this.ChannelStripSelection.Add(controller);
                }
            }
        }

        public void Sanitize()
        {
            this.RefreshCachedChannelStripSelection();
        }

        public void SetChannelStrips(List<AudioMixerGroupController> newSelection)
        {
            Selection.objects = newSelection.ToArray();
        }

        public void SetSingleChannelStrip(AudioMixerGroupController group)
        {
            AudioMixerGroupController[] controllerArray1 = new AudioMixerGroupController[] { group };
            Selection.objects = controllerArray1;
        }

        public void SyncToUnitySelection()
        {
            if (this.m_Controller != null)
            {
                this.RefreshCachedChannelStripSelection();
            }
        }

        public void ToggleChannelStrip(AudioMixerGroupController group)
        {
            List<Object> list = new List<Object>(Selection.objects);
            if (list.Contains(group))
            {
                list.Remove(group);
            }
            else
            {
                list.Add(group);
            }
            Selection.objects = list.ToArray();
        }

        public List<AudioMixerGroupController> ChannelStripSelection { get; private set; }
    }
}

