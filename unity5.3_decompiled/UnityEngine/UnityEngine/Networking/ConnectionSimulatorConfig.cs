namespace UnityEngine.Networking
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class ConnectionSimulatorConfig : IDisposable
    {
        internal IntPtr m_Ptr;

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern ConnectionSimulatorConfig(int outMinDelay, int outAvgDelay, int inMinDelay, int inAvgDelay, float packetLossPercentage);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Dispose();
        ~ConnectionSimulatorConfig()
        {
            this.Dispose();
        }
    }
}

