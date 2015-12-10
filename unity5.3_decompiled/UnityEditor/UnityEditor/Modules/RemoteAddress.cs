namespace UnityEditor.Modules
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct RemoteAddress
    {
        public string ip;
        public int port;
        public RemoteAddress(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
        }
    }
}

