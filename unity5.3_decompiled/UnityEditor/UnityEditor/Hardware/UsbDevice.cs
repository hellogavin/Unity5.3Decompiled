namespace UnityEditor.Hardware
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct UsbDevice
    {
        public readonly int vendorId;
        public readonly int productId;
        public readonly int revision;
        public readonly string udid;
        public readonly string name;
        public override string ToString()
        {
            string[] textArray1 = new string[] { this.name, " (udid:", this.udid, ", vid: ", this.vendorId.ToString("X4"), ", pid: ", this.productId.ToString("X4"), ", rev: ", this.revision.ToString("X4"), ")" };
            return string.Concat(textArray1);
        }
    }
}

