namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public class TrackedReference
    {
        internal IntPtr m_Ptr;
        protected TrackedReference()
        {
        }

        public override bool Equals(object o)
        {
            return ((o as TrackedReference) == this);
        }

        public override int GetHashCode()
        {
            return (int) this.m_Ptr;
        }

        public static bool operator ==(TrackedReference x, TrackedReference y)
        {
            object obj2 = x;
            object obj3 = y;
            if ((obj3 == null) && (obj2 == null))
            {
                return true;
            }
            if (obj3 == null)
            {
                return (x.m_Ptr == IntPtr.Zero);
            }
            if (obj2 == null)
            {
                return (y.m_Ptr == IntPtr.Zero);
            }
            return (x.m_Ptr == y.m_Ptr);
        }

        public static bool operator !=(TrackedReference x, TrackedReference y)
        {
            return !(x == y);
        }

        public static implicit operator bool(TrackedReference exists)
        {
            return (exists != null);
        }
    }
}

