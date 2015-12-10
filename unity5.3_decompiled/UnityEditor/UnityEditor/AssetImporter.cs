namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class AssetImporter : Object
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern AssetImporter GetAtPath(string path);
        public void SaveAndReimport()
        {
            AssetDatabase.ImportAsset(this.assetPath);
        }

        public string assetBundleName { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public string assetBundleVariant { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public string assetPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public ulong assetTimeStamp { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public string userData { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

