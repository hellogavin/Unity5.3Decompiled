namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class CircleCollider2D : Collider2D
    {
        [Obsolete("CircleCollider2D.center has been deprecated. Use CircleCollider2D.offset instead (UnityUpgradable) -> offset", true)]
        public Vector2 center
        {
            get
            {
                return Vector2.zero;
            }
            set
            {
            }
        }

        public float radius { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

