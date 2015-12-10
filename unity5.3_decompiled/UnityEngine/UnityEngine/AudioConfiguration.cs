namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct AudioConfiguration
    {
        public AudioSpeakerMode speakerMode;
        public int dspBufferSize;
        public int sampleRate;
        public int numRealVoices;
        public int numVirtualVoices;
    }
}

