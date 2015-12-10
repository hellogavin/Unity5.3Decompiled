namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class EdgeCollider2D : Collider2D
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Reset();

        public int edgeCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int pointCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public Vector2[] points { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

