namespace UnityEditor.Hardware
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class Usb
    {
        public static  event OnDevicesChangedHandler DevicesChanged;

        public static void OnDevicesChanged(UsbDevice[] devices)
        {
            if ((DevicesChanged != null) && (devices != null))
            {
                DevicesChanged(devices);
            }
        }

        public delegate void OnDevicesChangedHandler(UsbDevice[] devices);
    }
}

