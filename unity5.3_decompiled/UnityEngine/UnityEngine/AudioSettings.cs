namespace UnityEngine
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    public sealed class AudioSettings
    {
        public static  event AudioConfigurationChangeHandler OnAudioConfigurationChanged;

        public static AudioConfiguration GetConfiguration()
        {
            AudioConfiguration configuration;
            INTERNAL_CALL_GetConfiguration(out configuration);
            return configuration;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void GetDSPBufferSize(out int bufferLength, out int numBuffers);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetConfiguration(out AudioConfiguration value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_Reset(ref AudioConfiguration config);
        [RequiredByNativeCode]
        internal static void InvokeOnAudioConfigurationChanged(bool deviceWasChanged)
        {
            if (OnAudioConfigurationChanged != null)
            {
                OnAudioConfigurationChanged(deviceWasChanged);
            }
        }

        public static bool Reset(AudioConfiguration config)
        {
            return INTERNAL_CALL_Reset(ref config);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, Obsolete("AudioSettings.SetDSPBufferSize is deprecated and has been replaced by audio project settings and the AudioSettings.GetConfiguration/AudioSettings.Reset API.")]
        public static extern void SetDSPBufferSize(int bufferLength, int numBuffers);

        public static AudioSpeakerMode driverCapabilities { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("AudioSettings.driverCaps is obsolete. Use driverCapabilities instead (UnityUpgradable) -> driverCapabilities", true)]
        public static AudioSpeakerMode driverCaps
        {
            get
            {
                return driverCapabilities;
            }
        }

        public static double dspTime { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        internal static bool editingInPlaymode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static int outputSampleRate { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static AudioSpeakerMode speakerMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public delegate void AudioConfigurationChangeHandler(bool deviceWasChanged);
    }
}

