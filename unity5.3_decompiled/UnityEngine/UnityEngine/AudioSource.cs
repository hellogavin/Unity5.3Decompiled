namespace UnityEngine
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Audio;
    using UnityEngine.Internal;

    public sealed class AudioSource : Behaviour
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern AnimationCurve GetCustomCurve(AudioSourceCurveType type);
        [Obsolete("GetOutputData return a float[] is deprecated, use GetOutputData passing a pre allocated array instead.")]
        public float[] GetOutputData(int numSamples, int channel)
        {
            float[] samples = new float[numSamples];
            this.GetOutputDataHelper(samples, channel);
            return samples;
        }

        public void GetOutputData(float[] samples, int channel)
        {
            this.GetOutputDataHelper(samples, channel);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void GetOutputDataHelper(float[] samples, int channel);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool GetSpatializerFloat(int index, out float value);
        [Obsolete("GetSpectrumData returning a float[] is deprecated, use GetSpectrumData passing a pre allocated array instead.")]
        public float[] GetSpectrumData(int numSamples, int channel, FFTWindow window)
        {
            float[] samples = new float[numSamples];
            this.GetSpectrumDataHelper(samples, channel, window);
            return samples;
        }

        public void GetSpectrumData(float[] samples, int channel, FFTWindow window)
        {
            this.GetSpectrumDataHelper(samples, channel, window);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void GetSpectrumDataHelper(float[] samples, int channel, FFTWindow window);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Pause(AudioSource self);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_UnPause(AudioSource self);
        public void Pause()
        {
            INTERNAL_CALL_Pause(this);
        }

        [ExcludeFromDocs]
        public void Play()
        {
            ulong delay = 0L;
            this.Play(delay);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Play([DefaultValue("0")] ulong delay);
        [ExcludeFromDocs]
        public static void PlayClipAtPoint(AudioClip clip, Vector3 position)
        {
            float volume = 1f;
            PlayClipAtPoint(clip, position, volume);
        }

        public static void PlayClipAtPoint(AudioClip clip, Vector3 position, [DefaultValue("1.0F")] float volume)
        {
            GameObject obj2 = new GameObject("One shot audio") {
                transform = { position = position }
            };
            AudioSource source = (AudioSource) obj2.AddComponent(typeof(AudioSource));
            source.clip = clip;
            source.spatialBlend = 1f;
            source.volume = volume;
            source.Play();
            Object.Destroy(obj2, clip.length * ((Time.timeScale >= 0.01f) ? Time.timeScale : 0.01f));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void PlayDelayed(float delay);
        [ExcludeFromDocs]
        public void PlayOneShot(AudioClip clip)
        {
            float volumeScale = 1f;
            this.PlayOneShot(clip, volumeScale);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void PlayOneShot(AudioClip clip, [DefaultValue("1.0F")] float volumeScale);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void PlayScheduled(double time);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetCustomCurve(AudioSourceCurveType type, AnimationCurve curve);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetScheduledEndTime(double time);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetScheduledStartTime(double time);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool SetSpatializerFloat(int index, float value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Stop();
        public void UnPause()
        {
            INTERNAL_CALL_UnPause(this);
        }

        public bool bypassEffects { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool bypassListenerEffects { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool bypassReverbZones { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public AudioClip clip { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float dopplerLevel { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool ignoreListenerPause { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool ignoreListenerVolume { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool isPlaying { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool loop { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float maxDistance { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("maxVolume is not supported anymore. Use min-, maxDistance and rolloffMode instead.", true)]
        public float maxVolume { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float minDistance { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("minVolume is not supported anymore. Use min-, maxDistance and rolloffMode instead.", true)]
        public float minVolume { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool mute { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public AudioMixerGroup outputAudioMixerGroup { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("AudioSource.pan has been deprecated. Use AudioSource.panStereo instead (UnityUpgradable) -> panStereo", true)]
        public float pan
        {
            get
            {
                return this.panStereo;
            }
            set
            {
            }
        }

        [Obsolete("AudioSource.panLevel has been deprecated. Use AudioSource.spatialBlend instead (UnityUpgradable) -> spatialBlend", true), EditorBrowsable(EditorBrowsableState.Never)]
        public float panLevel
        {
            get
            {
                return this.spatialBlend;
            }
            set
            {
            }
        }

        public float panStereo { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float pitch { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool playOnAwake { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int priority { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float reverbZoneMix { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("rolloffFactor is not supported anymore. Use min-, maxDistance and rolloffMode instead.", true)]
        public float rolloffFactor { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public AudioRolloffMode rolloffMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float spatialBlend { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool spatialize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float spread { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float time { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int timeSamples { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public AudioVelocityUpdateMode velocityUpdateMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float volume { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

