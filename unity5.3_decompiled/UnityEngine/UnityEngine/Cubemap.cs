namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;

    public sealed class Cubemap : Texture
    {
        public Cubemap(int size, TextureFormat format, bool mipmap)
        {
            Internal_Create(this, size, format, mipmap);
        }

        [ExcludeFromDocs]
        public void Apply()
        {
            bool makeNoLongerReadable = false;
            bool updateMipmaps = true;
            this.Apply(updateMipmaps, makeNoLongerReadable);
        }

        [ExcludeFromDocs]
        public void Apply(bool updateMipmaps)
        {
            bool makeNoLongerReadable = false;
            this.Apply(updateMipmaps, makeNoLongerReadable);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Apply([DefaultValue("true")] bool updateMipmaps, [DefaultValue("false")] bool makeNoLongerReadable);
        public Color GetPixel(CubemapFace face, int x, int y)
        {
            Color color;
            INTERNAL_CALL_GetPixel(this, face, x, y, out color);
            return color;
        }

        [ExcludeFromDocs]
        public Color[] GetPixels(CubemapFace face)
        {
            int miplevel = 0;
            return this.GetPixels(face, miplevel);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern Color[] GetPixels(CubemapFace face, [DefaultValue("0")] int miplevel);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetPixel(Cubemap self, CubemapFace face, int x, int y, out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetPixel(Cubemap self, CubemapFace face, int x, int y, ref Color color);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_Create([Writable] Cubemap mono, int size, TextureFormat format, bool mipmap);
        public void SetPixel(CubemapFace face, int x, int y, Color color)
        {
            INTERNAL_CALL_SetPixel(this, face, x, y, ref color);
        }

        [ExcludeFromDocs]
        public void SetPixels(Color[] colors, CubemapFace face)
        {
            int miplevel = 0;
            this.SetPixels(colors, face, miplevel);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetPixels(Color[] colors, CubemapFace face, [DefaultValue("0")] int miplevel);
        [ExcludeFromDocs]
        public void SmoothEdges()
        {
            int smoothRegionWidthInPixels = 1;
            this.SmoothEdges(smoothRegionWidthInPixels);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SmoothEdges([DefaultValue("1")] int smoothRegionWidthInPixels);

        public TextureFormat format { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int mipmapCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

