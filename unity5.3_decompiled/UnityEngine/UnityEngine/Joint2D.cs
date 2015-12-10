namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class Joint2D : Behaviour
    {
        public Vector2 GetReactionForce(float timeStep)
        {
            Vector2 vector;
            Joint2D_CUSTOM_INTERNAL_GetReactionForce(this, timeStep, out vector);
            return vector;
        }

        public float GetReactionTorque(float timeStep)
        {
            return INTERNAL_CALL_GetReactionTorque(this, timeStep);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern float INTERNAL_CALL_GetReactionTorque(Joint2D self, float timeStep);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Joint2D_CUSTOM_INTERNAL_GetReactionForce(Joint2D joint, float timeStep, out Vector2 value);

        public float breakForce { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float breakTorque { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("Joint2D.collideConnected has been deprecated. Use Joint2D.enableCollision instead (UnityUpgradable) -> enableCollision", true)]
        public bool collideConnected
        {
            get
            {
                return this.enableCollision;
            }
            set
            {
                this.enableCollision = value;
            }
        }

        public Rigidbody2D connectedBody { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool enableCollision { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector2 reactionForce
        {
            get
            {
                return this.GetReactionForce(Time.fixedDeltaTime);
            }
        }

        public float reactionTorque
        {
            get
            {
                return this.GetReactionTorque(Time.fixedDeltaTime);
            }
        }
    }
}

