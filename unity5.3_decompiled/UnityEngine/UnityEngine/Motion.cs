namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class Motion : Object
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_averageSpeed(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, Obsolete("ValidateIfRetargetable is not supported anymore. Use isHumanMotion instead.", true)]
        public extern bool ValidateIfRetargetable(bool val);

        public float apparentSpeed { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public float averageAngularSpeed { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public float averageDuration { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public Vector3 averageSpeed
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_averageSpeed(out vector);
                return vector;
            }
        }

        [Obsolete("isAnimatorMotion is not supported anymore. Use !legacy instead.", true)]
        public bool isAnimatorMotion { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isHumanMotion { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isLooping { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool legacy { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

