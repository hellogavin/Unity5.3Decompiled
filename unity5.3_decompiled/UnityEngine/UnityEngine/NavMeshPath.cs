namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public sealed class NavMeshPath
    {
        internal IntPtr m_Ptr;
        internal Vector3[] m_corners;
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern NavMeshPath();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void DestroyNavMeshPath();
        ~NavMeshPath()
        {
            this.DestroyNavMeshPath();
            this.m_Ptr = IntPtr.Zero;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern int GetCornersNonAlloc(Vector3[] results);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern Vector3[] CalculateCornersInternal();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void ClearCornersInternal();
        public void ClearCorners()
        {
            this.ClearCornersInternal();
            this.m_corners = null;
        }

        private void CalculateCorners()
        {
            if (this.m_corners == null)
            {
                this.m_corners = this.CalculateCornersInternal();
            }
        }

        public Vector3[] corners
        {
            get
            {
                this.CalculateCorners();
                return this.m_corners;
            }
        }
        public NavMeshPathStatus status { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

