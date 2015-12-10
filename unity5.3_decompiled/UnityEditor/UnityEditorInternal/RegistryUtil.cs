namespace UnityEditorInternal
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class RegistryUtil
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetRegistryStringValue32(string subKey, string valueName, string defaultValue);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern uint GetRegistryUInt32Value32(string subKey, string valueName, uint defaultValue);
    }
}

