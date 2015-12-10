namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public sealed class CharacterJoint : Joint
    {
        [Obsolete("RotationDrive not in use for Unity 5 and assumed disabled.", true)]
        public JointDrive rotationDrive;
        [Obsolete("TargetAngularVelocity not in use for Unity 5 and assumed disabled.", true)]
        public Vector3 targetAngularVelocity;
        [Obsolete("TargetRotation not in use for Unity 5 and assumed disabled.", true)]
        public Quaternion targetRotation;

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_highTwistLimit(out SoftJointLimit value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_lowTwistLimit(out SoftJointLimit value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_swing1Limit(out SoftJointLimit value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_swing2Limit(out SoftJointLimit value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_swingAxis(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_swingLimitSpring(out SoftJointLimitSpring value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_twistLimitSpring(out SoftJointLimitSpring value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_highTwistLimit(ref SoftJointLimit value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_lowTwistLimit(ref SoftJointLimit value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_swing1Limit(ref SoftJointLimit value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_swing2Limit(ref SoftJointLimit value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_swingAxis(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_swingLimitSpring(ref SoftJointLimitSpring value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_twistLimitSpring(ref SoftJointLimitSpring value);

        public bool enableProjection { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public SoftJointLimit highTwistLimit
        {
            get
            {
                SoftJointLimit limit;
                this.INTERNAL_get_highTwistLimit(out limit);
                return limit;
            }
            set
            {
                this.INTERNAL_set_highTwistLimit(ref value);
            }
        }

        public SoftJointLimit lowTwistLimit
        {
            get
            {
                SoftJointLimit limit;
                this.INTERNAL_get_lowTwistLimit(out limit);
                return limit;
            }
            set
            {
                this.INTERNAL_set_lowTwistLimit(ref value);
            }
        }

        public float projectionAngle { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float projectionDistance { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public SoftJointLimit swing1Limit
        {
            get
            {
                SoftJointLimit limit;
                this.INTERNAL_get_swing1Limit(out limit);
                return limit;
            }
            set
            {
                this.INTERNAL_set_swing1Limit(ref value);
            }
        }

        public SoftJointLimit swing2Limit
        {
            get
            {
                SoftJointLimit limit;
                this.INTERNAL_get_swing2Limit(out limit);
                return limit;
            }
            set
            {
                this.INTERNAL_set_swing2Limit(ref value);
            }
        }

        public Vector3 swingAxis
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_swingAxis(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_swingAxis(ref value);
            }
        }

        public SoftJointLimitSpring swingLimitSpring
        {
            get
            {
                SoftJointLimitSpring spring;
                this.INTERNAL_get_swingLimitSpring(out spring);
                return spring;
            }
            set
            {
                this.INTERNAL_set_swingLimitSpring(ref value);
            }
        }

        public SoftJointLimitSpring twistLimitSpring
        {
            get
            {
                SoftJointLimitSpring spring;
                this.INTERNAL_get_twistLimitSpring(out spring);
                return spring;
            }
            set
            {
                this.INTERNAL_set_twistLimitSpring(ref value);
            }
        }
    }
}

