namespace UnityEditorInternal
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class ComponentUtility
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool CopyComponent(Component component);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool MoveComponentDown(Component component);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool MoveComponentUp(Component component);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool PasteComponentAsNew(GameObject go);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool PasteComponentValues(Component component);
    }
}

