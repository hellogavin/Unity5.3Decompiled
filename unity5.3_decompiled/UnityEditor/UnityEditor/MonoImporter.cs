namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class MonoImporter : AssetImporter
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void CopyMonoScriptIconToImporters(MonoScript script);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern MonoScript[] GetAllRuntimeMonoScripts();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern Object GetDefaultReference(string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetExecutionOrder(MonoScript script);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern MonoScript GetScript();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetDefaultReferences(string[] name, Object[] target);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetExecutionOrder(MonoScript script, int order);
    }
}

