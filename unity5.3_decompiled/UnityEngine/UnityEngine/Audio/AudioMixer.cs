namespace UnityEngine.Audio
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class AudioMixer : Object
    {
        internal AudioMixer()
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool ClearFloat(string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern AudioMixerGroup[] FindMatchingGroups(string subPath);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern AudioMixerSnapshot FindSnapshot(string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool GetFloat(string name, out float value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool SetFloat(string name, float value);
        private void TransitionToSnapshot(AudioMixerSnapshot snapshot, float timeToReach)
        {
            if (snapshot == null)
            {
                throw new ArgumentException("null Snapshot passed to AudioMixer.TransitionToSnapshot of AudioMixer '" + base.name + "'");
            }
            if (snapshot.audioMixer != this)
            {
                string[] textArray1 = new string[] { "Snapshot '", snapshot.name, "' passed to AudioMixer.TransitionToSnapshot is not a snapshot from AudioMixer '", base.name, "'" };
                throw new ArgumentException(string.Concat(textArray1));
            }
            snapshot.TransitionTo(timeToReach);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void TransitionToSnapshots(AudioMixerSnapshot[] snapshots, float[] weights, float timeToReach);

        public AudioMixerGroup outputAudioMixerGroup { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

