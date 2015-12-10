namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    internal sealed class RuntimeUndo
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void RecordObject(Object objectToUndo, string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void RecordObjects(Object[] objectsToUndo, string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetTransformParent(Transform transform, Transform newParent, string name);
    }
}

