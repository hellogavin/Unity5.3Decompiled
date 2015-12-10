namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class MeshRenderer : Renderer
    {
        public Mesh additionalVertexStreams { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

