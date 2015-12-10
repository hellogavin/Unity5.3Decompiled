namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;

    public sealed class TouchScreenKeyboard
    {
        [NonSerialized]
        internal IntPtr m_Ptr;

        public TouchScreenKeyboard(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure, bool alert, string textPlaceholder)
        {
            TouchScreenKeyboard_InternalConstructorHelperArguments arguments = new TouchScreenKeyboard_InternalConstructorHelperArguments {
                keyboardType = Convert.ToUInt32(keyboardType),
                autocorrection = Convert.ToUInt32(autocorrection),
                multiline = Convert.ToUInt32(multiline),
                secure = Convert.ToUInt32(secure),
                alert = Convert.ToUInt32(alert)
            };
            this.TouchScreenKeyboard_InternalConstructorHelper(ref arguments, text, textPlaceholder);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Destroy();
        ~TouchScreenKeyboard()
        {
            this.Destroy();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_get_area(out Rect value);
        [ExcludeFromDocs]
        public static TouchScreenKeyboard Open(string text)
        {
            string textPlaceholder = string.Empty;
            bool alert = false;
            bool secure = false;
            bool multiline = false;
            bool autocorrection = true;
            TouchScreenKeyboardType keyboardType = TouchScreenKeyboardType.Default;
            return Open(text, keyboardType, autocorrection, multiline, secure, alert, textPlaceholder);
        }

        [ExcludeFromDocs]
        public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType)
        {
            string textPlaceholder = string.Empty;
            bool alert = false;
            bool secure = false;
            bool multiline = false;
            bool autocorrection = true;
            return Open(text, keyboardType, autocorrection, multiline, secure, alert, textPlaceholder);
        }

        [ExcludeFromDocs]
        public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection)
        {
            string textPlaceholder = string.Empty;
            bool alert = false;
            bool secure = false;
            bool multiline = false;
            return Open(text, keyboardType, autocorrection, multiline, secure, alert, textPlaceholder);
        }

        [ExcludeFromDocs]
        public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline)
        {
            string textPlaceholder = string.Empty;
            bool alert = false;
            bool secure = false;
            return Open(text, keyboardType, autocorrection, multiline, secure, alert, textPlaceholder);
        }

        [ExcludeFromDocs]
        public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure)
        {
            string textPlaceholder = string.Empty;
            bool alert = false;
            return Open(text, keyboardType, autocorrection, multiline, secure, alert, textPlaceholder);
        }

        [ExcludeFromDocs]
        public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure, bool alert)
        {
            string textPlaceholder = string.Empty;
            return Open(text, keyboardType, autocorrection, multiline, secure, alert, textPlaceholder);
        }

        public static TouchScreenKeyboard Open(string text, [DefaultValue("TouchScreenKeyboardType.Default")] TouchScreenKeyboardType keyboardType, [DefaultValue("true")] bool autocorrection, [DefaultValue("false")] bool multiline, [DefaultValue("false")] bool secure, [DefaultValue("false")] bool alert, [DefaultValue("\"\"")] string textPlaceholder)
        {
            return new TouchScreenKeyboard(text, keyboardType, autocorrection, multiline, secure, alert, textPlaceholder);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void TouchScreenKeyboard_InternalConstructorHelper(ref TouchScreenKeyboard_InternalConstructorHelperArguments arguments, string text, string textPlaceholder);

        public bool active { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static Rect area
        {
            get
            {
                Rect rect;
                INTERNAL_get_area(out rect);
                return rect;
            }
        }

        public bool done { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool hideInput { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool isSupported
        {
            get
            {
                RuntimePlatform platform = Application.platform;
                switch (platform)
                {
                    case RuntimePlatform.MetroPlayerX86:
                    case RuntimePlatform.MetroPlayerX64:
                    case RuntimePlatform.MetroPlayerARM:
                        return false;

                    case RuntimePlatform.WP8Player:
                    case RuntimePlatform.BB10Player:
                    case RuntimePlatform.TizenPlayer:
                    case RuntimePlatform.PSM:
                    case RuntimePlatform.WiiU:
                        break;

                    default:
                        switch (platform)
                        {
                            case RuntimePlatform.IPhonePlayer:
                            case RuntimePlatform.Android:
                                break;

                            case RuntimePlatform.PS3:
                            case RuntimePlatform.XBOX360:
                                goto Label_0066;

                            default:
                                goto Label_0066;
                        }
                        break;
                }
                return true;
            Label_0066:
                return false;
            }
        }

        public int targetDisplay { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public string text { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool visible { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool wasCanceled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

