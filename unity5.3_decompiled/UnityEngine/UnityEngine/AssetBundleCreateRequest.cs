namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Scripting;

    [RequiredByNativeCode]
    public sealed class AssetBundleCreateRequest : AsyncOperation
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void DisableCompatibilityChecks();

        public AssetBundle assetBundle { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

