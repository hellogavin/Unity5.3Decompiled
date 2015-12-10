namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class EditorMaterialUtility
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsBackgroundMaterial(Material material);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ResetDefaultTextures(Material material, bool overrideSetTextures);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetShaderDefaults(Shader shader, string[] name, Texture[] textures);
    }
}

