namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class PreviewHelpers
    {
        internal static void AdjustWidthAndHeightForStaticPreview(int textureWidth, int textureHeight, ref int width, ref int height)
        {
            int max = width;
            int num2 = height;
            if ((textureWidth <= width) && (textureHeight <= height))
            {
                width = textureWidth;
                height = textureHeight;
            }
            else
            {
                float b = ((float) height) / ((float) textureWidth);
                float a = ((float) width) / ((float) textureHeight);
                float num5 = Mathf.Min(a, b);
                width = Mathf.RoundToInt(textureWidth * num5);
                height = Mathf.RoundToInt(textureHeight * num5);
            }
            width = Mathf.Clamp(width, 2, max);
            height = Mathf.Clamp(height, 2, num2);
        }
    }
}

