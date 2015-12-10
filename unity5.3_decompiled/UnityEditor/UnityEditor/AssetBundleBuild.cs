namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct AssetBundleBuild
    {
        public string assetBundleName;
        public string assetBundleVariant;
        public string[] assetNames;
    }
}

