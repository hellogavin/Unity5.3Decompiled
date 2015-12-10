namespace UnityEditor
{
    using System;

    public enum TextureImporterType
    {
        Advanced = 5,
        Bump = 1,
        Cookie = 4,
        Cubemap = 3,
        Cursor = 7,
        GUI = 2,
        Image = 0,
        Lightmap = 6,
        [Obsolete("Use Cubemap")]
        Reflection = 3,
        Sprite = 8
    }
}

