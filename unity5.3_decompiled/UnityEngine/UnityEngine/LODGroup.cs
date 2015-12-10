namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [SelectionBase]
    public sealed class LODGroup : Component
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void ForceLOD(int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern LOD[] GetLODs();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_localReferencePoint(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_localReferencePoint(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void RecalculateBounds();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetLODs(LOD[] lods);
        [Obsolete("Use SetLODs instead.")]
        public void SetLODS(LOD[] lods)
        {
            this.SetLODs(lods);
        }

        public bool animateCrossFading { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static float crossFadeAnimationDuration { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool enabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public LODFadeMode fadeMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector3 localReferencePoint
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_localReferencePoint(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_localReferencePoint(ref value);
            }
        }

        public int lodCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public float size { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

