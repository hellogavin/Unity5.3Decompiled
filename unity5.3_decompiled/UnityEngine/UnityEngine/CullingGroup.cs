namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential)]
    public sealed class CullingGroup : IDisposable
    {
        internal IntPtr m_Ptr;
        private StateChanged m_OnStateChanged;
        public CullingGroup()
        {
            this.Init();
        }

        ~CullingGroup()
        {
            if (this.m_Ptr != IntPtr.Zero)
            {
                this.FinalizerFailure();
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Dispose();
        public StateChanged onStateChanged
        {
            get
            {
                return this.m_OnStateChanged;
            }
            set
            {
                this.m_OnStateChanged = value;
            }
        }
        public bool enabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        public Camera targetCamera { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetBoundingSpheres(BoundingSphere[] array);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetBoundingSphereCount(int count);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void EraseSwapBack(int index);
        public static void EraseSwapBack<T>(int index, T[] myArray, ref int size)
        {
            size--;
            myArray[index] = myArray[size];
        }

        public int QueryIndices(bool visible, int[] result, int firstIndex)
        {
            return this.QueryIndices(visible, -1, CullingQueryOptions.IgnoreDistance, result, firstIndex);
        }

        public int QueryIndices(int distanceIndex, int[] result, int firstIndex)
        {
            return this.QueryIndices(false, distanceIndex, CullingQueryOptions.IgnoreVisibility, result, firstIndex);
        }

        public int QueryIndices(bool visible, int distanceIndex, int[] result, int firstIndex)
        {
            return this.QueryIndices(visible, distanceIndex, CullingQueryOptions.Normal, result, firstIndex);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern int QueryIndices(bool visible, int distanceIndex, CullingQueryOptions options, int[] result, int firstIndex);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool IsVisible(int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern int GetDistance(int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetBoundingDistances(float[] distances);
        public void SetDistanceReferencePoint(Vector3 point)
        {
            INTERNAL_CALL_SetDistanceReferencePoint(this, ref point);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetDistanceReferencePoint(CullingGroup self, ref Vector3 point);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetDistanceReferencePoint(Transform transform);
        [RequiredByNativeCode, SecuritySafeCritical]
        private static unsafe void SendEvents(CullingGroup cullingGroup, IntPtr eventsPtr, int count)
        {
            CullingGroupEvent* eventPtr = (CullingGroupEvent*) eventsPtr.ToPointer();
            if (cullingGroup.m_OnStateChanged != null)
            {
                for (int i = 0; i < count; i++)
                {
                    cullingGroup.m_OnStateChanged(eventPtr[i]);
                }
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Init();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void FinalizerFailure();
        public delegate void StateChanged(CullingGroupEvent sphere);
    }
}

