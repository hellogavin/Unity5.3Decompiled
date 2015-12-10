namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public struct GUID
    {
        private uint m_Value0;
        private uint m_Value1;
        private uint m_Value2;
        private uint m_Value3;
        public GUID(string hexRepresentation)
        {
            this.m_Value0 = 0;
            this.m_Value1 = 0;
            this.m_Value2 = 0;
            this.m_Value3 = 0;
            this.ParseExact(hexRepresentation);
        }

        public override bool Equals(object obj)
        {
            GUID guid = (GUID) obj;
            return (guid == this);
        }

        public override int GetHashCode()
        {
            return this.m_Value0.GetHashCode();
        }

        public bool Empty()
        {
            return ((((this.m_Value0 == 0) && (this.m_Value1 == 0)) && (this.m_Value2 == 0)) && (this.m_Value3 == 0));
        }

        public bool ParseExact(string hex)
        {
            this.HexToGUIDInternal(hex, ref this);
            return !this.Empty();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void HexToGUIDInternal(string hex, ref GUID guid);
        public static bool operator ==(GUID x, GUID y)
        {
            return ((((x.m_Value0 == y.m_Value0) && (x.m_Value1 == y.m_Value1)) && (x.m_Value2 == y.m_Value2)) && (x.m_Value3 == y.m_Value3));
        }

        public static bool operator !=(GUID x, GUID y)
        {
            return !(x == y);
        }
    }
}

