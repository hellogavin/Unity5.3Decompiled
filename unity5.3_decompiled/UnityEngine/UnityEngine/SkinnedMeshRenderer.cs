namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class SkinnedMeshRenderer : Renderer
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void BakeMesh(Mesh mesh);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern float GetBlendShapeWeight(int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_localBounds(out Bounds value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_localBounds(ref Bounds value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetBlendShapeWeight(int index, float value);

        internal Transform actualRootBone { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public Transform[] bones { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Bounds localBounds
        {
            get
            {
                Bounds bounds;
                this.INTERNAL_get_localBounds(out bounds);
                return bounds;
            }
            set
            {
                this.INTERNAL_set_localBounds(ref value);
            }
        }

        public SkinQuality quality { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Transform rootBone { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Mesh sharedMesh { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool updateWhenOffscreen { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

