namespace UnityEngine.Rendering
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class GraphicsSettings : Object
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Shader GetCustomShader(BuiltinShaderType type);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern BuiltinShaderMode GetShaderMode(BuiltinShaderType type);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetCustomShader(BuiltinShaderType type, Shader shader);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetShaderMode(BuiltinShaderType type, BuiltinShaderMode mode);
    }
}

