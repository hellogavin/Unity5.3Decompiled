namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public sealed class ColorUtility
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool DoTryParseHtmlColor(string htmlString, out Color32 color);
        public static string ToHtmlStringRGB(Color color)
        {
            Color32 color2 = color;
            return string.Format("{0:X2}{1:X2}{2:X2}", color2.r, color2.g, color2.b);
        }

        public static string ToHtmlStringRGBA(Color color)
        {
            Color32 color2 = color;
            object[] args = new object[] { color2.r, color2.g, color2.b, color2.a };
            return string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", args);
        }

        public static bool TryParseHtmlString(string htmlString, out Color color)
        {
            Color32 color2;
            bool flag = DoTryParseHtmlColor(htmlString, out color2);
            color = (Color) color2;
            return flag;
        }
    }
}

