namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Internal;

    public sealed class Texture3D : Texture
    {
        public Texture3D(int width, int height, int depth, TextureFormat format, bool mipmap)
        {
            Internal_Create(this, width, height, depth, format, mipmap);
        }

        [ExcludeFromDocs]
        public void Apply()
        {
            bool updateMipmaps = true;
            this.Apply(updateMipmaps);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Apply([DefaultValue("true")] bool updateMipmaps);
        [ExcludeFromDocs]
        public Color[] GetPixels()
        {
            int miplevel = 0;
            return this.GetPixels(miplevel);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern Color[] GetPixels([DefaultValue("0")] int miplevel);
        [ExcludeFromDocs]
        public Color32[] GetPixels32()
        {
            int miplevel = 0;
            return this.GetPixels32(miplevel);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern Color32[] GetPixels32([DefaultValue("0")] int miplevel);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_Create([Writable] Texture3D mono, int width, int height, int depth, TextureFormat format, bool mipmap);
        [ExcludeFromDocs]
        public void SetPixels(Color[] colors)
        {
            int miplevel = 0;
            this.SetPixels(colors, miplevel);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetPixels(Color[] colors, [DefaultValue("0")] int miplevel);
        [ExcludeFromDocs]
        public void SetPixels32(Color32[] colors)
        {
            int miplevel = 0;
            this.SetPixels32(colors, miplevel);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetPixels32(Color32[] colors, [DefaultValue("0")] int miplevel);

        public int depth { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public TextureFormat format { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

