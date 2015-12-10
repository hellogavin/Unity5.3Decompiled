namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class ProceduralTexture : Texture
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern Color32[] GetPixels32(int x, int y, int blockWidth, int blockHeight);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern ProceduralMaterial GetProceduralMaterial();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern ProceduralOutputType GetProceduralOutputType();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern bool HasBeenGenerated();

        public TextureFormat format { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool hasAlpha { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

