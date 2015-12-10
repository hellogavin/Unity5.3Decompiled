namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class Joint : Component
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_anchor(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_axis(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_connectedAnchor(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_anchor(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_axis(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_connectedAnchor(ref Vector3 value);

        public Vector3 anchor
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_anchor(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_anchor(ref value);
            }
        }

        public bool autoConfigureConnectedAnchor { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector3 axis
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_axis(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_axis(ref value);
            }
        }

        public float breakForce { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float breakTorque { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector3 connectedAnchor
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_connectedAnchor(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_connectedAnchor(ref value);
            }
        }

        public Rigidbody connectedBody { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool enableCollision { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool enablePreprocessing { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

