namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class BillboardRenderer : Renderer
    {
        public BillboardAsset billboard { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

