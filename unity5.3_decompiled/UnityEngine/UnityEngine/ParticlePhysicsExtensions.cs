namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public static class ParticlePhysicsExtensions
    {
        public static int GetCollisionEvents(this ParticleSystem ps, GameObject go, ParticleCollisionEvent[] collisionEvents)
        {
            return ParticleSystemExtensionsImpl.GetCollisionEvents(ps, go, collisionEvents);
        }

        public static int GetSafeCollisionEventSize(this ParticleSystem ps)
        {
            return ParticleSystemExtensionsImpl.GetSafeCollisionEventSize(ps);
        }
    }
}

