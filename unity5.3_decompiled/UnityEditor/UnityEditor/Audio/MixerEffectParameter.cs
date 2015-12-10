namespace UnityEditor.Audio
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    internal struct MixerEffectParameter
    {
        public string parameterName;
        public UnityEditor.GUID GUID;
    }
}

