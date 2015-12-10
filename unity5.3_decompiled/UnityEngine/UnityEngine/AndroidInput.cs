namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class AndroidInput
    {
        private AndroidInput()
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Touch GetSecondaryTouch(int index);

        public static bool secondaryTouchEnabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int secondaryTouchHeight { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int secondaryTouchWidth { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int touchCountSecondary { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

