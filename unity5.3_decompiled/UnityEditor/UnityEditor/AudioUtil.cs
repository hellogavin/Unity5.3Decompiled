namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Internal;

    internal sealed class AudioUtil
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ClearWaveForm(AudioClip clip);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetBitRate(AudioClip clip);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetBitsPerSample(AudioClip clip);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetChannelCount(AudioClip clip);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern float GetClipPosition(AudioClip clip);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetClipSamplePosition(AudioClip clip);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetCustomFilterChannelCount(MonoBehaviour behaviour);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern float GetCustomFilterMaxIn(MonoBehaviour behaviour, int channel);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern float GetCustomFilterMaxOut(MonoBehaviour behaviour, int channel);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetCustomFilterProcessTime(MonoBehaviour behaviour);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern double GetDuration(AudioClip clip);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern float GetFMODCPUUsage();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetFMODMemoryAllocated();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetFrequency(AudioClip clip);
        public static Vector3 GetListenerPos()
        {
            Vector3 vector;
            INTERNAL_CALL_GetListenerPos(out vector);
            return vector;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern AnimationCurve GetLowpassCurve(AudioLowPassFilter lowPassFilter);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetMusicChannelCount(AudioClip clip);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetSampleCount(AudioClip clip);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern AudioCompressionFormat GetSoundCompressionFormat(AudioClip clip);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetSoundSize(AudioClip clip);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string[] GetSpatializerPluginNames();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern AudioCompressionFormat GetTargetPlatformSoundCompressionFormat(AudioClip clip);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Texture2D GetWaveForm(AudioClip clip, AssetImporter importer, int channel, float width, float height);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Texture2D GetWaveFormFast(AudioClip clip, int channel, int fromSample, int toSample, float width, float height);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool HasPreview(AudioClip clip);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool HaveAudioCallback(MonoBehaviour behaviour);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetListenerPos(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsClipPlaying(AudioClip clip);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsMovieAudio(AudioClip clip);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsTrackerFile(AudioClip clip);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void LoopClip(AudioClip clip, bool on);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void PauseClip(AudioClip clip);
        [ExcludeFromDocs]
        public static void PlayClip(AudioClip clip)
        {
            bool loop = false;
            int startSample = 0;
            PlayClip(clip, startSample, loop);
        }

        [ExcludeFromDocs]
        public static void PlayClip(AudioClip clip, int startSample)
        {
            bool loop = false;
            PlayClip(clip, startSample, loop);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void PlayClip(AudioClip clip, [DefaultValue("0")] int startSample, [DefaultValue("false")] bool loop);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ResumeClip(AudioClip clip);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetClipSamplePosition(AudioClip clip, int iSamplePosition);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetListenerTransform(Transform t);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void StopAllClips();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void StopClip(AudioClip clip);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void UpdateAudio();

        public static bool canUseSpatializerEffect { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool resetAllAudioClipPlayCountsOnPlay { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

