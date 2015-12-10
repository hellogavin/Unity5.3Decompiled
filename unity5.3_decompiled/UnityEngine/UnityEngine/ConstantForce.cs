namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public sealed class ConstantForce : Behaviour
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_force(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_relativeForce(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_relativeTorque(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_torque(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_force(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_relativeForce(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_relativeTorque(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_torque(ref Vector3 value);

        public Vector3 force
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_force(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_force(ref value);
            }
        }

        public Vector3 relativeForce
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_relativeForce(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_relativeForce(ref value);
            }
        }

        public Vector3 relativeTorque
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_relativeTorque(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_relativeTorque(ref value);
            }
        }

        public Vector3 torque
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_torque(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_torque(ref value);
            }
        }
    }
}

