namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public sealed class AssetBundleManifest : Object
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string[] GetAllAssetBundles();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string[] GetAllAssetBundlesWithVariant();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string[] GetAllDependencies(string assetBundleName);
        public Hash128 GetAssetBundleHash(string assetBundleName)
        {
            Hash128 hash;
            INTERNAL_CALL_GetAssetBundleHash(this, assetBundleName, out hash);
            return hash;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string[] GetDirectDependencies(string assetBundleName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetAssetBundleHash(AssetBundleManifest self, string assetBundleName, out Hash128 value);
    }
}

