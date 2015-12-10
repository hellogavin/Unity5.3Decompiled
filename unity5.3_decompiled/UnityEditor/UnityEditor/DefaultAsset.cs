namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class DefaultAsset : Object
    {
        internal bool isWarning { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        internal string message { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

