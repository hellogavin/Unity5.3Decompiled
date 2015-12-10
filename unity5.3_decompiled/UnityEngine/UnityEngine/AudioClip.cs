namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    public sealed class AudioClip : Object
    {
        private event PCMReaderCallback m_PCMReaderCallback;

        private event PCMSetPositionCallback m_PCMSetPositionCallback;

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern AudioClip Construct_Internal();
        public static AudioClip Create(string name, int lengthSamples, int channels, int frequency, bool stream)
        {
            return Create(name, lengthSamples, channels, frequency, stream, (PCMReaderCallback) null, (PCMSetPositionCallback) null);
        }

        [Obsolete("The _3D argument of AudioClip is deprecated. Use the spatialBlend property of AudioSource instead to morph between 2D and 3D playback.")]
        public static AudioClip Create(string name, int lengthSamples, int channels, int frequency, bool _3D, bool stream)
        {
            return Create(name, lengthSamples, channels, frequency, stream);
        }

        public static AudioClip Create(string name, int lengthSamples, int channels, int frequency, bool stream, PCMReaderCallback pcmreadercallback)
        {
            return Create(name, lengthSamples, channels, frequency, stream, pcmreadercallback, null);
        }

        [Obsolete("The _3D argument of AudioClip is deprecated. Use the spatialBlend property of AudioSource instead to morph between 2D and 3D playback.")]
        public static AudioClip Create(string name, int lengthSamples, int channels, int frequency, bool _3D, bool stream, PCMReaderCallback pcmreadercallback)
        {
            return Create(name, lengthSamples, channels, frequency, stream, pcmreadercallback, null);
        }

        public static AudioClip Create(string name, int lengthSamples, int channels, int frequency, bool stream, PCMReaderCallback pcmreadercallback, PCMSetPositionCallback pcmsetpositioncallback)
        {
            if (name == null)
            {
                throw new NullReferenceException();
            }
            if (lengthSamples <= 0)
            {
                throw new ArgumentException("Length of created clip must be larger than 0");
            }
            if (channels <= 0)
            {
                throw new ArgumentException("Number of channels in created clip must be greater than 0");
            }
            if (frequency <= 0)
            {
                throw new ArgumentException("Frequency in created clip must be greater than 0");
            }
            AudioClip clip = Construct_Internal();
            if (pcmreadercallback != null)
            {
                clip.m_PCMReaderCallback = (PCMReaderCallback) Delegate.Combine(clip.m_PCMReaderCallback, pcmreadercallback);
            }
            if (pcmsetpositioncallback != null)
            {
                clip.m_PCMSetPositionCallback = (PCMSetPositionCallback) Delegate.Combine(clip.m_PCMSetPositionCallback, pcmsetpositioncallback);
            }
            clip.Init_Internal(name, lengthSamples, channels, frequency, stream);
            return clip;
        }

        [Obsolete("The _3D argument of AudioClip is deprecated. Use the spatialBlend property of AudioSource instead to morph between 2D and 3D playback.")]
        public static AudioClip Create(string name, int lengthSamples, int channels, int frequency, bool _3D, bool stream, PCMReaderCallback pcmreadercallback, PCMSetPositionCallback pcmsetpositioncallback)
        {
            return Create(name, lengthSamples, channels, frequency, stream, pcmreadercallback, pcmsetpositioncallback);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool GetData(float[] data, int offsetSamples);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Init_Internal(string name, int lengthSamples, int channels, int frequency, bool stream);
        [RequiredByNativeCode]
        private void InvokePCMReaderCallback_Internal(float[] data)
        {
            if (this.m_PCMReaderCallback != null)
            {
                this.m_PCMReaderCallback(data);
            }
        }

        [RequiredByNativeCode]
        private void InvokePCMSetPositionCallback_Internal(int position)
        {
            if (this.m_PCMSetPositionCallback != null)
            {
                this.m_PCMSetPositionCallback(position);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool LoadAudioData();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool SetData(float[] data, int offsetSamples);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool UnloadAudioData();

        public int channels { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int frequency { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        [Obsolete("Use AudioClip.loadState instead to get more detailed information about the loading process.")]
        public bool isReadyToPlay { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public float length { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool loadInBackground { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public AudioDataLoadState loadState { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public AudioClipLoadType loadType { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool preloadAudioData { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int samples { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public delegate void PCMReaderCallback(float[] data);

        public delegate void PCMSetPositionCallback(int position);
    }
}

