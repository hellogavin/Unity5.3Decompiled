namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal sealed class PackageUtility
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern ExportPackageItem[] BuildExportPackageItemsList(string[] guids, bool dependencies);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ExportPackage(string[] guids, string fileName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern ImportPackageItem[] ExtractAndPrepareAssetList(string packagePath, out string packageIconPath, out bool canPerformReInstall);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ImportPackageAssets(ImportPackageItem[] items, bool performReInstall);
    }
}

