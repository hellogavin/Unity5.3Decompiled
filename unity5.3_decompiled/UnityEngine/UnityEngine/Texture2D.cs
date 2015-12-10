namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;

    public sealed class Texture2D : Texture
    {
        public Texture2D(int width, int height)
        {
            Internal_Create(this, width, height, TextureFormat.ARGB32, true, false, IntPtr.Zero);
        }

        public Texture2D(int width, int height, TextureFormat format, bool mipmap)
        {
            Internal_Create(this, width, height, format, mipmap, false, IntPtr.Zero);
        }

        public Texture2D(int width, int height, TextureFormat format, bool mipmap, bool linear)
        {
            Internal_Create(this, width, height, format, mipmap, linear, IntPtr.Zero);
        }

        internal Texture2D(int width, int height, TextureFormat format, bool mipmap, bool linear, IntPtr nativeTex)
        {
            Internal_Create(this, width, height, format, mipmap, linear, nativeTex);
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
        public void Compress(bool highQuality)
        {
            INTERNAL_CALL_Compress(this, highQuality);
        }

        public static Texture2D CreateExternalTexture(int width, int height, TextureFormat format, bool mipmap, bool linear, IntPtr nativeTex)
        {
            return new Texture2D(width, height, format, mipmap, linear, nativeTex);
        }

        public byte[] EncodeToJPG()
        {
            return this.EncodeToJPG(0x4b);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern byte[] EncodeToJPG(int quality);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern byte[] EncodeToPNG();
        public Color GetPixel(int x, int y)
        {
            Color color;
            INTERNAL_CALL_GetPixel(this, x, y, out color);
            return color;
        }

        public Color GetPixelBilinear(float u, float v)
        {
            Color color;
            INTERNAL_CALL_GetPixelBilinear(this, u, v, out color);
            return color;
        }

        [ExcludeFromDocs]
        public Color[] GetPixels()
        {
            int miplevel = 0;
            return this.GetPixels(miplevel);
        }

        public Color[] GetPixels([DefaultValue("0")] int miplevel)
        {
            int blockWidth = this.width >> miplevel;
            if (blockWidth < 1)
            {
                blockWidth = 1;
            }
            int blockHeight = this.height >> miplevel;
            if (blockHeight < 1)
            {
                blockHeight = 1;
            }
            return this.GetPixels(0, 0, blockWidth, blockHeight, miplevel);
        }

        [ExcludeFromDocs]
        public Color[] GetPixels(int x, int y, int blockWidth, int blockHeight)
        {
            int miplevel = 0;
            return this.GetPixels(x, y, blockWidth, blockHeight, miplevel);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern Color[] GetPixels(int x, int y, int blockWidth, int blockHeight, [DefaultValue("0")] int miplevel);
        [ExcludeFromDocs]
        public Color32[] GetPixels32()
        {
            int miplevel = 0;
            return this.GetPixels32(miplevel);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern Color32[] GetPixels32([DefaultValue("0")] int miplevel);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern byte[] GetRawTextureData();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Compress(Texture2D self, bool highQuality);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetPixel(Texture2D self, int x, int y, out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetPixelBilinear(Texture2D self, float u, float v, out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_ReadPixels(Texture2D self, ref Rect source, int destX, int destY, bool recalculateMipMaps);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetPixel(Texture2D self, int x, int y, ref Color color);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_Create([Writable] Texture2D mono, int width, int height, TextureFormat format, bool mipmap, bool linear, IntPtr nativeTex);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern bool Internal_ResizeWH(int width, int height);
        [ExcludeFromDocs]
        public bool LoadImage(byte[] data)
        {
            bool markNonReadable = false;
            return this.LoadImage(data, markNonReadable);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool LoadImage(byte[] data, [DefaultValue("false")] bool markNonReadable);
        public void LoadRawTextureData(byte[] data)
        {
            this.LoadRawTextureData_ImplArray(data);
        }

        public void LoadRawTextureData(IntPtr data, int size)
        {
            this.LoadRawTextureData_ImplPointer(data, size);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void LoadRawTextureData_ImplArray(byte[] data);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void LoadRawTextureData_ImplPointer(IntPtr data, int size);
        [ExcludeFromDocs]
        public Rect[] PackTextures(Texture2D[] textures, int padding)
        {
            bool makeNoLongerReadable = false;
            int maximumAtlasSize = 0x800;
            return this.PackTextures(textures, padding, maximumAtlasSize, makeNoLongerReadable);
        }

        [ExcludeFromDocs]
        public Rect[] PackTextures(Texture2D[] textures, int padding, int maximumAtlasSize)
        {
            bool makeNoLongerReadable = false;
            return this.PackTextures(textures, padding, maximumAtlasSize, makeNoLongerReadable);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern Rect[] PackTextures(Texture2D[] textures, int padding, [DefaultValue("2048")] int maximumAtlasSize, [DefaultValue("false")] bool makeNoLongerReadable);
        [ExcludeFromDocs]
        public void ReadPixels(Rect source, int destX, int destY)
        {
            bool recalculateMipMaps = true;
            INTERNAL_CALL_ReadPixels(this, ref source, destX, destY, recalculateMipMaps);
        }

        public void ReadPixels(Rect source, int destX, int destY, [DefaultValue("true")] bool recalculateMipMaps)
        {
            INTERNAL_CALL_ReadPixels(this, ref source, destX, destY, recalculateMipMaps);
        }

        public bool Resize(int width, int height)
        {
            return this.Internal_ResizeWH(width, height);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool Resize(int width, int height, TextureFormat format, bool hasMipMap);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void SetAllPixels32(Color32[] colors, int miplevel);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void SetBlockOfPixels32(int x, int y, int blockWidth, int blockHeight, Color32[] colors, int miplevel);
        public void SetPixel(int x, int y, Color color)
        {
            INTERNAL_CALL_SetPixel(this, x, y, ref color);
        }

        [ExcludeFromDocs]
        public void SetPixels(Color[] colors)
        {
            int miplevel = 0;
            this.SetPixels(colors, miplevel);
        }

        public void SetPixels(Color[] colors, [DefaultValue("0")] int miplevel)
        {
            int blockWidth = this.width >> miplevel;
            if (blockWidth < 1)
            {
                blockWidth = 1;
            }
            int blockHeight = this.height >> miplevel;
            if (blockHeight < 1)
            {
                blockHeight = 1;
            }
            this.SetPixels(0, 0, blockWidth, blockHeight, colors, miplevel);
        }

        [ExcludeFromDocs]
        public void SetPixels(int x, int y, int blockWidth, int blockHeight, Color[] colors)
        {
            int miplevel = 0;
            this.SetPixels(x, y, blockWidth, blockHeight, colors, miplevel);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetPixels(int x, int y, int blockWidth, int blockHeight, Color[] colors, [DefaultValue("0")] int miplevel);
        [ExcludeFromDocs]
        public void SetPixels32(Color32[] colors)
        {
            int miplevel = 0;
            this.SetPixels32(colors, miplevel);
        }

        public void SetPixels32(Color32[] colors, [DefaultValue("0")] int miplevel)
        {
            this.SetAllPixels32(colors, miplevel);
        }

        [ExcludeFromDocs]
        public void SetPixels32(int x, int y, int blockWidth, int blockHeight, Color32[] colors)
        {
            int miplevel = 0;
            this.SetPixels32(x, y, blockWidth, blockHeight, colors, miplevel);
        }

        public void SetPixels32(int x, int y, int blockWidth, int blockHeight, Color32[] colors, [DefaultValue("0")] int miplevel)
        {
            this.SetBlockOfPixels32(x, y, blockWidth, blockHeight, colors, miplevel);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void UpdateExternalTexture(IntPtr nativeTex);

        public bool alphaIsTransparency { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static Texture2D blackTexture { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public TextureFormat format { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int mipmapCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static Texture2D whiteTexture { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

