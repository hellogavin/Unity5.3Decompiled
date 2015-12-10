namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;

    public class Collider2D : Behaviour
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_OverlapPoint(Collider2D self, ref Vector2 point);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_bounds(out Bounds value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_offset(out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_offset(ref Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool IsTouching(Collider2D collider);
        [ExcludeFromDocs]
        public bool IsTouchingLayers()
        {
            int layerMask = -1;
            return this.IsTouchingLayers(layerMask);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool IsTouchingLayers([DefaultValue("Physics2D.AllLayers")] int layerMask);
        public bool OverlapPoint(Vector2 point)
        {
            return INTERNAL_CALL_OverlapPoint(this, ref point);
        }

        public Rigidbody2D attachedRigidbody { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public Bounds bounds
        {
            get
            {
                Bounds bounds;
                this.INTERNAL_get_bounds(out bounds);
                return bounds;
            }
        }

        public float density { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal ColliderErrorState2D errorState { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isTrigger { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector2 offset
        {
            get
            {
                Vector2 vector;
                this.INTERNAL_get_offset(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_offset(ref value);
            }
        }

        public int shapeCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public PhysicsMaterial2D sharedMaterial { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool usedByEffector { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

