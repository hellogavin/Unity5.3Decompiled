namespace UnityEditor
{
    using System;

    public enum TextureImporterGenerateCubemap
    {
        AutoCubemap = 6,
        Cylindrical = 2,
        FullCubemap = 5,
        [Obsolete("Obscure shperemap modes are not supported any longer (use TextureImporterGenerateCubemap.Spheremap instead).")]
        NiceSpheremap = 4,
        None = 0,
        [Obsolete("Obscure shperemap modes are not supported any longer (use TextureImporterGenerateCubemap.Spheremap instead).")]
        SimpleSpheremap = 3,
        Spheremap = 1
    }
}

