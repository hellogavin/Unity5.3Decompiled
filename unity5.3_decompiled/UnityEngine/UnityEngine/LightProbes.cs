namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Rendering;

    public sealed class LightProbes : Object
    {
        [Obsolete("GetInterpolatedLightProbe has been deprecated. Please use the static GetInterpolatedProbe instead.", true)]
        public void GetInterpolatedLightProbe(Vector3 position, Renderer renderer, float[] coefficients)
        {
        }

        public static void GetInterpolatedProbe(Vector3 position, Renderer renderer, out SphericalHarmonicsL2 probe)
        {
            INTERNAL_CALL_GetInterpolatedProbe(ref position, renderer, out probe);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetInterpolatedProbe(ref Vector3 position, Renderer renderer, out SphericalHarmonicsL2 probe);

        public SphericalHarmonicsL2[] bakedProbes { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int cellCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        [Obsolete("coefficients property has been deprecated. Please use bakedProbes instead.", true)]
        public float[] coefficients
        {
            get
            {
                return new float[0];
            }
            set
            {
            }
        }

        public int count { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public Vector3[] positions { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

