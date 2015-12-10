namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Internal;

    public sealed class TextureImporter : AssetImporter
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void ClearPlatformTextureSettings(string platform);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool DoesSourceTextureHaveAlpha();
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("DoesSourceTextureHaveColor always returns true in Unity."), WrapperlessIcall]
        public extern bool DoesSourceTextureHaveColor();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern TextureImporterFormat FullToSimpleTextureFormat(TextureImporterFormat format);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool GetAllowsAlphaSplitting();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern string GetImportWarnings();
        public bool GetPlatformTextureSettings(string platform, out int maxTextureSize, out TextureImporterFormat textureFormat)
        {
            int compressionQuality = 0;
            return this.GetPlatformTextureSettings(platform, out maxTextureSize, out textureFormat, out compressionQuality);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool GetPlatformTextureSettings(string platform, out int maxTextureSize, out TextureImporterFormat textureFormat, out int compressionQuality);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void GetWidthAndHeight(ref int width, ref int height);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_spriteBorder(out Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_spritePivot(out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_spriteBorder(ref Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_spritePivot(ref Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern bool IsSourceTextureHDR();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void ReadTextureImportInstructions(BuildTarget target, out TextureFormat desiredFormat, out ColorSpace colorSpace, out int compressionQuality);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void ReadTextureSettings(TextureImporterSettings dest);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetAllowsAlphaSplitting(bool flag);
        [ExcludeFromDocs]
        public void SetPlatformTextureSettings(string platform, int maxTextureSize, TextureImporterFormat textureFormat)
        {
            bool allowsAlphaSplit = false;
            this.SetPlatformTextureSettings(platform, maxTextureSize, textureFormat, allowsAlphaSplit);
        }

        public void SetPlatformTextureSettings(string platform, int maxTextureSize, TextureImporterFormat textureFormat, [DefaultValue("false")] bool allowsAlphaSplit)
        {
            this.SetPlatformTextureSettings(platform, maxTextureSize, textureFormat, 50, allowsAlphaSplit);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetPlatformTextureSettings(string platform, int maxTextureSize, TextureImporterFormat textureFormat, int compressionQuality, bool allowsAlphaSplit);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetTextureSettings(TextureImporterSettings src);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern TextureImporterFormat SimpleToFullTextureFormat2(TextureImporterFormat simpleFormat, TextureImporterType tType, TextureImporterSettings settings, bool doesTextureContainAlpha, bool sourceWasHDR, BuildTarget destinationPlatform);

        public bool alphaIsTransparency { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int anisoLevel { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool borderMipmap { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int compressionQuality { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool convertToNormalmap { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("correctGamma Property deprecated. Use generateMipsInLinearSpace instead.")]
        public bool correctGamma { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool fadeout { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public FilterMode filterMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public TextureImporterGenerateCubemap generateCubemap { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool generateMipsInLinearSpace { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool grayscaleToAlpha { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float heightmapScale { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool isReadable { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool lightmap { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool linearTexture { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int maxTextureSize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float mipMapBias { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool mipmapEnabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int mipmapFadeDistanceEnd { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int mipmapFadeDistanceStart { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public TextureImporterMipFilter mipmapFilter { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool normalmap { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public TextureImporterNormalFilter normalmapFilter { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public TextureImporterNPOTScale npotScale { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool qualifiesForSpritePacking { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public Vector4 spriteBorder
        {
            get
            {
                Vector4 vector;
                this.INTERNAL_get_spriteBorder(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_spriteBorder(ref value);
            }
        }

        public SpriteImportMode spriteImportMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public string spritePackingTag { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector2 spritePivot
        {
            get
            {
                Vector2 vector;
                this.INTERNAL_get_spritePivot(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_spritePivot(ref value);
            }
        }

        public float spritePixelsPerUnit { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("Use spritePixelsPerUnit property instead.")]
        public float spritePixelsToUnits { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public SpriteMetaData[] spritesheet { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public TextureImporterFormat textureFormat { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public TextureImporterType textureType { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public TextureWrapMode wrapMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

