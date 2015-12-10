namespace UnityEditorInternal
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    internal sealed class GradientPreviewCache
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ClearCache();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Texture2D GetGradientPreview(Gradient curve);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Texture2D GetPropertyPreview(SerializedProperty property);
    }
}

