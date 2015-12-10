namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public sealed class Compass
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_rawVector(out Vector3 value);

        public bool enabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float headingAccuracy { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public float magneticHeading { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public Vector3 rawVector
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_rawVector(out vector);
                return vector;
            }
        }

        public double timestamp { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public float trueHeading { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

