namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public class ScriptableObject : Object
    {
        public ScriptableObject()
        {
            Internal_CreateScriptableObject(this);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_CreateScriptableObject([Writable] ScriptableObject self);
        [Obsolete("Use EditorUtility.SetDirty instead")]
        public void SetDirty()
        {
            INTERNAL_CALL_SetDirty(this);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetDirty(ScriptableObject self);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern ScriptableObject CreateInstance(string className);
        public static ScriptableObject CreateInstance(Type type)
        {
            return CreateInstanceFromType(type);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern ScriptableObject CreateInstanceFromType(Type type);
        public static T CreateInstance<T>() where T: ScriptableObject
        {
            return (T) CreateInstance(typeof(T));
        }
    }
}

