namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class ShaderImporter : AssetImporter
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern Texture GetDefaultTexture(string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern Shader GetShader();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetDefaultTextures(string[] name, Texture[] textures);
    }
}

