namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public sealed class HingeJoint : Joint
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_limits(out JointLimits value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_motor(out JointMotor value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_spring(out JointSpring value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_limits(ref JointLimits value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_motor(ref JointMotor value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_spring(ref JointSpring value);

        public float angle { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public JointLimits limits
        {
            get
            {
                JointLimits limits;
                this.INTERNAL_get_limits(out limits);
                return limits;
            }
            set
            {
                this.INTERNAL_set_limits(ref value);
            }
        }

        public JointMotor motor
        {
            get
            {
                JointMotor motor;
                this.INTERNAL_get_motor(out motor);
                return motor;
            }
            set
            {
                this.INTERNAL_set_motor(ref value);
            }
        }

        public JointSpring spring
        {
            get
            {
                JointSpring spring;
                this.INTERNAL_get_spring(out spring);
                return spring;
            }
            set
            {
                this.INTERNAL_set_spring(ref value);
            }
        }

        public bool useLimits { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool useMotor { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool useSpring { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float velocity { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

