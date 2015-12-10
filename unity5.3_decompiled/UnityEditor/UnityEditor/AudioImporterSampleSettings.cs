namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct AudioImporterSampleSettings
    {
        public AudioClipLoadType loadType;
        public AudioSampleRateSetting sampleRateSetting;
        public uint sampleRateOverride;
        public AudioCompressionFormat compressionFormat;
        public float quality;
        public int conversionMode;
    }
}

