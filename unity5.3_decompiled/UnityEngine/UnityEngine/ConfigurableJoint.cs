namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public sealed class ConfigurableJoint : Joint
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_angularXDrive(out JointDrive value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_angularXLimitSpring(out SoftJointLimitSpring value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_angularYLimit(out SoftJointLimit value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_angularYZDrive(out JointDrive value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_angularYZLimitSpring(out SoftJointLimitSpring value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_angularZLimit(out SoftJointLimit value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_highAngularXLimit(out SoftJointLimit value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_linearLimit(out SoftJointLimit value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_linearLimitSpring(out SoftJointLimitSpring value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_lowAngularXLimit(out SoftJointLimit value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_secondaryAxis(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_slerpDrive(out JointDrive value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_targetAngularVelocity(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_targetPosition(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_targetRotation(out Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_targetVelocity(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_xDrive(out JointDrive value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_yDrive(out JointDrive value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_zDrive(out JointDrive value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_angularXDrive(ref JointDrive value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_angularXLimitSpring(ref SoftJointLimitSpring value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_angularYLimit(ref SoftJointLimit value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_angularYZDrive(ref JointDrive value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_angularYZLimitSpring(ref SoftJointLimitSpring value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_angularZLimit(ref SoftJointLimit value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_highAngularXLimit(ref SoftJointLimit value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_linearLimit(ref SoftJointLimit value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_linearLimitSpring(ref SoftJointLimitSpring value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_lowAngularXLimit(ref SoftJointLimit value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_secondaryAxis(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_slerpDrive(ref JointDrive value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_targetAngularVelocity(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_targetPosition(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_targetRotation(ref Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_targetVelocity(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_xDrive(ref JointDrive value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_yDrive(ref JointDrive value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_zDrive(ref JointDrive value);

        public JointDrive angularXDrive
        {
            get
            {
                JointDrive drive;
                this.INTERNAL_get_angularXDrive(out drive);
                return drive;
            }
            set
            {
                this.INTERNAL_set_angularXDrive(ref value);
            }
        }

        public SoftJointLimitSpring angularXLimitSpring
        {
            get
            {
                SoftJointLimitSpring spring;
                this.INTERNAL_get_angularXLimitSpring(out spring);
                return spring;
            }
            set
            {
                this.INTERNAL_set_angularXLimitSpring(ref value);
            }
        }

        public ConfigurableJointMotion angularXMotion { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public SoftJointLimit angularYLimit
        {
            get
            {
                SoftJointLimit limit;
                this.INTERNAL_get_angularYLimit(out limit);
                return limit;
            }
            set
            {
                this.INTERNAL_set_angularYLimit(ref value);
            }
        }

        public ConfigurableJointMotion angularYMotion { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public JointDrive angularYZDrive
        {
            get
            {
                JointDrive drive;
                this.INTERNAL_get_angularYZDrive(out drive);
                return drive;
            }
            set
            {
                this.INTERNAL_set_angularYZDrive(ref value);
            }
        }

        public SoftJointLimitSpring angularYZLimitSpring
        {
            get
            {
                SoftJointLimitSpring spring;
                this.INTERNAL_get_angularYZLimitSpring(out spring);
                return spring;
            }
            set
            {
                this.INTERNAL_set_angularYZLimitSpring(ref value);
            }
        }

        public SoftJointLimit angularZLimit
        {
            get
            {
                SoftJointLimit limit;
                this.INTERNAL_get_angularZLimit(out limit);
                return limit;
            }
            set
            {
                this.INTERNAL_set_angularZLimit(ref value);
            }
        }

        public ConfigurableJointMotion angularZMotion { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool configuredInWorldSpace { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public SoftJointLimit highAngularXLimit
        {
            get
            {
                SoftJointLimit limit;
                this.INTERNAL_get_highAngularXLimit(out limit);
                return limit;
            }
            set
            {
                this.INTERNAL_set_highAngularXLimit(ref value);
            }
        }

        public SoftJointLimit linearLimit
        {
            get
            {
                SoftJointLimit limit;
                this.INTERNAL_get_linearLimit(out limit);
                return limit;
            }
            set
            {
                this.INTERNAL_set_linearLimit(ref value);
            }
        }

        public SoftJointLimitSpring linearLimitSpring
        {
            get
            {
                SoftJointLimitSpring spring;
                this.INTERNAL_get_linearLimitSpring(out spring);
                return spring;
            }
            set
            {
                this.INTERNAL_set_linearLimitSpring(ref value);
            }
        }

        public SoftJointLimit lowAngularXLimit
        {
            get
            {
                SoftJointLimit limit;
                this.INTERNAL_get_lowAngularXLimit(out limit);
                return limit;
            }
            set
            {
                this.INTERNAL_set_lowAngularXLimit(ref value);
            }
        }

        public float projectionAngle { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float projectionDistance { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public JointProjectionMode projectionMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public RotationDriveMode rotationDriveMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector3 secondaryAxis
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_secondaryAxis(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_secondaryAxis(ref value);
            }
        }

        public JointDrive slerpDrive
        {
            get
            {
                JointDrive drive;
                this.INTERNAL_get_slerpDrive(out drive);
                return drive;
            }
            set
            {
                this.INTERNAL_set_slerpDrive(ref value);
            }
        }

        public bool swapBodies { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector3 targetAngularVelocity
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_targetAngularVelocity(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_targetAngularVelocity(ref value);
            }
        }

        public Vector3 targetPosition
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_targetPosition(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_targetPosition(ref value);
            }
        }

        public Quaternion targetRotation
        {
            get
            {
                Quaternion quaternion;
                this.INTERNAL_get_targetRotation(out quaternion);
                return quaternion;
            }
            set
            {
                this.INTERNAL_set_targetRotation(ref value);
            }
        }

        public Vector3 targetVelocity
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_targetVelocity(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_targetVelocity(ref value);
            }
        }

        public JointDrive xDrive
        {
            get
            {
                JointDrive drive;
                this.INTERNAL_get_xDrive(out drive);
                return drive;
            }
            set
            {
                this.INTERNAL_set_xDrive(ref value);
            }
        }

        public ConfigurableJointMotion xMotion { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public JointDrive yDrive
        {
            get
            {
                JointDrive drive;
                this.INTERNAL_get_yDrive(out drive);
                return drive;
            }
            set
            {
                this.INTERNAL_set_yDrive(ref value);
            }
        }

        public ConfigurableJointMotion yMotion { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public JointDrive zDrive
        {
            get
            {
                JointDrive drive;
                this.INTERNAL_get_zDrive(out drive);
                return drive;
            }
            set
            {
                this.INTERNAL_set_zDrive(ref value);
            }
        }

        public ConfigurableJointMotion zMotion { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

