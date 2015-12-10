namespace UnityEditor
{
    using System;
    using UnityEditor.Audio;

    [CanEditMultipleObjects, CustomEditor(typeof(AudioMixerSnapshotController))]
    internal class AudioMixerSnapshotControllerInspector : Editor
    {
        public override void OnInspectorGUI()
        {
        }
    }
}

