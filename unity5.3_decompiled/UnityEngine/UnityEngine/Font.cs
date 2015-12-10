namespace UnityEngine
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    public sealed class Font : Object
    {
        private event FontTextureRebuildCallback m_FontTextureRebuildCallback;

        public static  event Action<Font> textureRebuilt;

        public Font()
        {
            Internal_CreateFont(this, null);
        }

        public Font(string name)
        {
            Internal_CreateFont(this, name);
        }

        private Font(string[] names, int size)
        {
            Internal_CreateDynamicFont(this, names, size);
        }

        public static Font CreateDynamicFontFromOSFont(string fontname, int size)
        {
            return new Font(new string[] { fontname }, size);
        }

        public static Font CreateDynamicFontFromOSFont(string[] fontnames, int size)
        {
            return new Font(fontnames, size);
        }

        [ExcludeFromDocs]
        public bool GetCharacterInfo(char ch, out CharacterInfo info)
        {
            FontStyle normal = FontStyle.Normal;
            int size = 0;
            return this.GetCharacterInfo(ch, out info, size, normal);
        }

        [ExcludeFromDocs]
        public bool GetCharacterInfo(char ch, out CharacterInfo info, int size)
        {
            FontStyle normal = FontStyle.Normal;
            return this.GetCharacterInfo(ch, out info, size, normal);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool GetCharacterInfo(char ch, out CharacterInfo info, [DefaultValue("0")] int size, [DefaultValue("FontStyle.Normal")] FontStyle style);
        public static int GetMaxVertsForString(string str)
        {
            return ((str.Length * 4) + 4);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string[] GetOSInstalledFontNames();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool HasCharacter(char c);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_CreateDynamicFont([Writable] Font _font, string[] _names, int size);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_CreateFont([Writable] Font _font, string name);
        [RequiredByNativeCode]
        private static void InvokeTextureRebuilt_Internal(Font font)
        {
            Action<Font> textureRebuilt = Font.textureRebuilt;
            if (textureRebuilt != null)
            {
                textureRebuilt(font);
            }
            if (font.m_FontTextureRebuildCallback != null)
            {
                font.m_FontTextureRebuildCallback();
            }
        }

        [ExcludeFromDocs]
        public void RequestCharactersInTexture(string characters)
        {
            FontStyle normal = FontStyle.Normal;
            int size = 0;
            this.RequestCharactersInTexture(characters, size, normal);
        }

        [ExcludeFromDocs]
        public void RequestCharactersInTexture(string characters, int size)
        {
            FontStyle normal = FontStyle.Normal;
            this.RequestCharactersInTexture(characters, size, normal);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void RequestCharactersInTexture(string characters, [DefaultValue("0")] int size, [DefaultValue("FontStyle.Normal")] FontStyle style);

        public int ascent { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public CharacterInfo[] characterInfo { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool dynamic { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public string[] fontNames { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int fontSize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int lineHeight { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public Material material { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Font.textureRebuildCallback has been deprecated. Use Font.textureRebuilt instead.")]
        public FontTextureRebuildCallback textureRebuildCallback
        {
            get
            {
                return this.m_FontTextureRebuildCallback;
            }
            set
            {
                this.m_FontTextureRebuildCallback = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public delegate void FontTextureRebuildCallback();
    }
}

