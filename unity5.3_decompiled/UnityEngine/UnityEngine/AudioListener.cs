namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class AudioListener : Behaviour
    {
        [Obsolete("GetOutputData returning a float[] is deprecated, use GetOutputData and pass a pre allocated array instead.")]
        public static float[] GetOutputData(int numSamples, int channel)
        {
            float[] samples = new float[numSamples];
            GetOutputDataHelper(samples, channel);
            return samples;
        }

        public static void GetOutputData(float[] samples, int channel)
        {
            GetOutputDataHelper(samples, channel);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void GetOutputDataHelper(float[] samples, int channel);
        [Obsolete("GetSpectrumData returning a float[] is deprecated, use GetOutputData and pass a pre allocated array instead.")]
        public static float[] GetSpectrumData(int numSamples, int channel, FFTWindow window)
        {
            float[] samples = new float[numSamples];
            GetSpectrumDataHelper(samples, channel, window);
            return samples;
        }

        public static void GetSpectrumData(float[] samples, int channel, FFTWindow window)
        {
            GetSpectrumDataHelper(samples, channel, window);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void GetSpectrumDataHelper(float[] samples, int channel, FFTWindow window);

        public static bool pause { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public AudioVelocityUpdateMode velocityUpdateMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static float volume { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

