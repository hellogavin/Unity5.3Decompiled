namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal sealed class OSColorPicker
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Close();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_get_color(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_set_color(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Show(bool showAlpha);

        public static Color color
        {
            get
            {
                Color color;
                INTERNAL_get_color(out color);
                return color;
            }
            set
            {
                INTERNAL_set_color(ref value);
            }
        }

        public static bool visible { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

