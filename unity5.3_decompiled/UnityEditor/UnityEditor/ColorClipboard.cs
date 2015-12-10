namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal static class ColorClipboard
    {
        public static bool HasColor()
        {
            Color color;
            return TryGetColor(false, out color);
        }

        public static void SetColor(Color color)
        {
            EditorGUIUtility.systemCopyBuffer = string.Empty;
            EditorGUIUtility.SetPasteboardColor(color);
        }

        public static bool TryGetColor(bool allowHDR, out Color color)
        {
            bool flag = false;
            if (ColorUtility.TryParseHtmlString(EditorGUIUtility.systemCopyBuffer, out color))
            {
                flag = true;
            }
            else if (EditorGUIUtility.HasPasteboardColor())
            {
                color = EditorGUIUtility.GetPasteboardColor();
                flag = true;
            }
            if (!flag)
            {
                return false;
            }
            if (!allowHDR && (color.maxColorComponent > 1f))
            {
                color = color.RGBMultiplied((float) (1f / color.maxColorComponent));
            }
            return true;
        }
    }
}

