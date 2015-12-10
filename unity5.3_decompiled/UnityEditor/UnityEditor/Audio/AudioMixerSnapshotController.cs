namespace UnityEditor.Audio
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Audio;

    internal sealed class AudioMixerSnapshotController : AudioMixerSnapshot
    {
        public AudioMixerSnapshotController(AudioMixer owner)
        {
            Internal_CreateAudioMixerSnapshotController(this, owner);
        }

        public void ClearTransitionTypeOverride(GUID guid)
        {
            INTERNAL_CALL_ClearTransitionTypeOverride(this, guid);
        }

        public bool GetTransitionTypeOverride(GUID guid, out ParameterTransitionType type)
        {
            return INTERNAL_CALL_GetTransitionTypeOverride(this, guid, out type);
        }

        public bool GetValue(GUID guid, out float value)
        {
            return INTERNAL_CALL_GetValue(this, guid, out value);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_ClearTransitionTypeOverride(AudioMixerSnapshotController self, GUID guid);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_GetTransitionTypeOverride(AudioMixerSnapshotController self, GUID guid, out ParameterTransitionType type);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_GetValue(AudioMixerSnapshotController self, GUID guid, out float value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetTransitionTypeOverride(AudioMixerSnapshotController self, GUID guid, ParameterTransitionType type);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetValue(AudioMixerSnapshotController self, GUID guid, float value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_CreateAudioMixerSnapshotController(AudioMixerSnapshotController mono, AudioMixer owner);
        public void SetTransitionTypeOverride(GUID guid, ParameterTransitionType type)
        {
            INTERNAL_CALL_SetTransitionTypeOverride(this, guid, type);
        }

        public void SetValue(GUID guid, float value)
        {
            INTERNAL_CALL_SetValue(this, guid, value);
        }

        public GUID snapshotID { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

