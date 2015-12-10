namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public sealed class SliderJoint2D : AnchoredJoint2D
    {
        public float GetMotorForce(float timeStep)
        {
            return INTERNAL_CALL_GetMotorForce(this, timeStep);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern float INTERNAL_CALL_GetMotorForce(SliderJoint2D self, float timeStep);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_limits(out JointTranslationLimits2D value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_motor(out JointMotor2D value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_limits(ref JointTranslationLimits2D value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_motor(ref JointMotor2D value);

        public float angle { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool autoConfigureAngle { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float jointSpeed { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public float jointTranslation { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public JointTranslationLimits2D limits
        {
            get
            {
                JointTranslationLimits2D limitsd;
                this.INTERNAL_get_limits(out limitsd);
                return limitsd;
            }
            set
            {
                this.INTERNAL_set_limits(ref value);
            }
        }

        public JointLimitState2D limitState { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public JointMotor2D motor
        {
            get
            {
                JointMotor2D motord;
                this.INTERNAL_get_motor(out motord);
                return motord;
            }
            set
            {
                this.INTERNAL_set_motor(ref value);
            }
        }

        public float referenceAngle { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool useLimits { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool useMotor { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

