namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class TrueTypeFontImporter : AssetImporter
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern Font GenerateEditableFont(string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern bool IsFormatSupported();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern Font[] LookupFallbackFontReferences(string[] _names);

        public int characterPadding { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int characterSpacing { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public string customCharacters { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public string[] fontNames { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Font[] fontReferences { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public FontRenderingMode fontRenderingMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("FontRenderModes are no longer supported.", true)]
        private int fontRenderMode
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        public int fontSize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public FontTextureCase fontTextureCase { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public string fontTTFName { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool includeFontData { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("Per-Font styles are no longer supported. Set the style in the rendering component, or import a styled version of the font.", true)]
        private FontStyle style
        {
            get
            {
                return FontStyle.Normal;
            }
            set
            {
            }
        }

        [Obsolete("use2xBehaviour is deprecated and will be removed in a future release.")]
        public bool use2xBehaviour { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

