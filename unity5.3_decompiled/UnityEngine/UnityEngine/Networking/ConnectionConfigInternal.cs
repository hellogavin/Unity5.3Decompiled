namespace UnityEngine.Networking
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal sealed class ConnectionConfigInternal : IDisposable
    {
        internal IntPtr m_Ptr;

        private ConnectionConfigInternal()
        {
        }

        public ConnectionConfigInternal(ConnectionConfig config)
        {
            if (config == null)
            {
                throw new NullReferenceException("config is not defined");
            }
            this.InitWrapper();
            this.InitPacketSize(config.PacketSize);
            this.InitFragmentSize(config.FragmentSize);
            this.InitResendTimeout(config.ResendTimeout);
            this.InitDisconnectTimeout(config.DisconnectTimeout);
            this.InitConnectTimeout(config.ConnectTimeout);
            this.InitMinUpdateTimeout(config.MinUpdateTimeout);
            this.InitPingTimeout(config.PingTimeout);
            this.InitReducedPingTimeout(config.ReducedPingTimeout);
            this.InitAllCostTimeout(config.AllCostTimeout);
            this.InitNetworkDropThreshold(config.NetworkDropThreshold);
            this.InitOverflowDropThreshold(config.OverflowDropThreshold);
            this.InitMaxConnectionAttempt(config.MaxConnectionAttempt);
            this.InitAckDelay(config.AckDelay);
            this.InitMaxCombinedReliableMessageSize(config.MaxCombinedReliableMessageSize);
            this.InitMaxCombinedReliableMessageCount(config.MaxCombinedReliableMessageCount);
            this.InitMaxSentMessageQueueSize(config.MaxSentMessageQueueSize);
            this.InitIsAcksLong(config.IsAcksLong);
            this.InitUsePlatformSpecificProtocols(config.UsePlatformSpecificProtocols);
            for (byte i = 0; i < config.ChannelCount; i = (byte) (i + 1))
            {
                this.AddChannel(config.GetChannel(i));
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern byte AddChannel(QosType value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Dispose();
        ~ConnectionConfigInternal()
        {
            this.Dispose();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern QosType GetChannel(int i);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void InitAckDelay(uint value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void InitAllCostTimeout(uint value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void InitConnectTimeout(uint value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void InitDisconnectTimeout(uint value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void InitFragmentSize(ushort value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void InitIsAcksLong(bool value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void InitMaxCombinedReliableMessageCount(ushort value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void InitMaxCombinedReliableMessageSize(ushort value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void InitMaxConnectionAttempt(byte value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void InitMaxSentMessageQueueSize(ushort value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void InitMinUpdateTimeout(uint value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void InitNetworkDropThreshold(byte value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void InitOverflowDropThreshold(byte value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void InitPacketSize(ushort value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void InitPingTimeout(uint value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void InitReducedPingTimeout(uint value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void InitResendTimeout(uint value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void InitUsePlatformSpecificProtocols(bool value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void InitWrapper();

        public int ChannelSize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

