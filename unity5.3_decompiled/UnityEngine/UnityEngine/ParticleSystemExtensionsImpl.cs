namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    internal sealed class ParticleSystemExtensionsImpl
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern int GetCollisionEvents(ParticleSystem ps, GameObject go, ParticleCollisionEvent[] collisionEvents);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern int GetSafeCollisionEventSize(ParticleSystem ps);
    }
}

