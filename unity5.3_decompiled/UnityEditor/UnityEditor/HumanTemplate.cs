namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class HumanTemplate : Object
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern HumanTemplate();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void ClearTemplate();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string Find(string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Insert(string name, string templateName);
    }
}

