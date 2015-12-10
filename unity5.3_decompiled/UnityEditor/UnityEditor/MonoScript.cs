namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class MonoScript : TextAsset
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern MonoScript();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern MonoScript FromMonoBehaviour(MonoBehaviour behaviour);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern MonoScript FromScriptableObject(ScriptableObject scriptableObject);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern Type GetClass();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern bool GetScriptTypeWasJustCreatedFromComponentMenu();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void Init(string scriptContents, string className, string nameSpace, string assemblyName, bool isEditorScript);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void SetScriptTypeWasJustCreatedFromComponentMenu();
    }
}

