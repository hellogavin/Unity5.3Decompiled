namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public sealed class WheelCollider : Collider
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void ConfigureVehicleSubsteps(float speedThreshold, int stepsBelowThreshold, int stepsAboveThreshold);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool GetGroundHit(out WheelHit hit);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void GetWorldPose(out Vector3 pos, out Quaternion quat);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_center(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_forwardFriction(out WheelFrictionCurve value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_sidewaysFriction(out WheelFrictionCurve value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_suspensionSpring(out JointSpring value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_center(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_forwardFriction(ref WheelFrictionCurve value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_sidewaysFriction(ref WheelFrictionCurve value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_suspensionSpring(ref JointSpring value);

        public float brakeTorque { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector3 center
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_center(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_center(ref value);
            }
        }

        public float forceAppPointDistance { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public WheelFrictionCurve forwardFriction
        {
            get
            {
                WheelFrictionCurve curve;
                this.INTERNAL_get_forwardFriction(out curve);
                return curve;
            }
            set
            {
                this.INTERNAL_set_forwardFriction(ref value);
            }
        }

        public bool isGrounded { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public float mass { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float motorTorque { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float radius { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float rpm { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public WheelFrictionCurve sidewaysFriction
        {
            get
            {
                WheelFrictionCurve curve;
                this.INTERNAL_get_sidewaysFriction(out curve);
                return curve;
            }
            set
            {
                this.INTERNAL_set_sidewaysFriction(ref value);
            }
        }

        public float sprungMass { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public float steerAngle { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float suspensionDistance { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public JointSpring suspensionSpring
        {
            get
            {
                JointSpring spring;
                this.INTERNAL_get_suspensionSpring(out spring);
                return spring;
            }
            set
            {
                this.INTERNAL_set_suspensionSpring(ref value);
            }
        }

        public float wheelDampingRate { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

