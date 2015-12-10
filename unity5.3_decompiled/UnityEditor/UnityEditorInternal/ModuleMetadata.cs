namespace UnityEditorInternal
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal sealed class ModuleMetadata
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetICallModule(string icall);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int[] GetModuleClasses(string moduleName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string[] GetModuleNames();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool GetModuleStrippable(string moduleName);
    }
}

