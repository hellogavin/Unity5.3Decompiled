namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class TerrainCollider : Collider
    {
        public TerrainData terrainData { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

