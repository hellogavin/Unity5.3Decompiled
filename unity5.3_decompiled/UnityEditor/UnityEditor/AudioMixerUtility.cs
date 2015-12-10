namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEditor.Audio;

    internal class AudioMixerUtility
    {
        public static void RepaintAudioMixerAndInspectors()
        {
            InspectorWindow.RepaintAllInspectors();
            AudioMixerWindow.RepaintAudioMixerWindow();
        }

        public static void VisitGroupsRecursivly(AudioMixerGroupController group, Action<AudioMixerGroupController> visitorCallback)
        {
            foreach (AudioMixerGroupController controller in group.children)
            {
                VisitGroupsRecursivly(controller, visitorCallback);
            }
            if (visitorCallback != null)
            {
                visitorCallback(group);
            }
        }

        public class VisitorFetchInstanceIDs
        {
            public List<int> instanceIDs = new List<int>();

            public void Visitor(AudioMixerGroupController group)
            {
                this.instanceIDs.Add(group.GetInstanceID());
            }
        }
    }
}

