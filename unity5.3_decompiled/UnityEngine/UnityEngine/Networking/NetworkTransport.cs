namespace UnityEngine.Networking
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Internal;
    using UnityEngine.Networking.Types;

    public sealed class NetworkTransport
    {
        private NetworkTransport()
        {
        }

        [ExcludeFromDocs]
        public static int AddHost(HostTopology topology)
        {
            string ip = null;
            int port = 0;
            return AddHost(topology, port, ip);
        }

        [ExcludeFromDocs]
        public static int AddHost(HostTopology topology, int port)
        {
            string ip = null;
            return AddHost(topology, port, ip);
        }

        public static int AddHost(HostTopology topology, [DefaultValue("0")] int port, [DefaultValue("null")] string ip)
        {
            if (topology == null)
            {
                throw new NullReferenceException("topology is not defined");
            }
            if (ip == null)
            {
                return AddHostWrapperWithoutIp(new HostTopologyInternal(topology), port, 0, 0);
            }
            return AddHostWrapper(new HostTopologyInternal(topology), ip, port, 0, 0);
        }

        [ExcludeFromDocs]
        public static int AddHostWithSimulator(HostTopology topology, int minTimeout, int maxTimeout)
        {
            string ip = null;
            int port = 0;
            return AddHostWithSimulator(topology, minTimeout, maxTimeout, port, ip);
        }

        [ExcludeFromDocs]
        public static int AddHostWithSimulator(HostTopology topology, int minTimeout, int maxTimeout, int port)
        {
            string ip = null;
            return AddHostWithSimulator(topology, minTimeout, maxTimeout, port, ip);
        }

        public static int AddHostWithSimulator(HostTopology topology, int minTimeout, int maxTimeout, [DefaultValue("0")] int port, [DefaultValue("null")] string ip)
        {
            if (topology == null)
            {
                throw new NullReferenceException("topology is not defined");
            }
            if (ip == null)
            {
                return AddHostWrapperWithoutIp(new HostTopologyInternal(topology), port, minTimeout, maxTimeout);
            }
            return AddHostWrapper(new HostTopologyInternal(topology), ip, port, minTimeout, maxTimeout);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int AddHostWrapper(HostTopologyInternal topologyInt, string ip, int port, int minTimeout, int maxTimeout);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int AddHostWrapperWithoutIp(HostTopologyInternal topologyInt, int port, int minTimeout, int maxTimeout);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void AddSceneId(int id);
        [ExcludeFromDocs]
        public static int AddWebsocketHost(HostTopology topology, int port)
        {
            string ip = null;
            return AddWebsocketHost(topology, port, ip);
        }

        public static int AddWebsocketHost(HostTopology topology, int port, [DefaultValue("null")] string ip)
        {
            if (topology == null)
            {
                throw new NullReferenceException("topology is not defined");
            }
            if (ip == null)
            {
                return AddWsHostWrapperWithoutIp(new HostTopologyInternal(topology), port);
            }
            return AddWsHostWrapper(new HostTopologyInternal(topology), ip, port);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int AddWsHostWrapper(HostTopologyInternal topologyInt, string ip, int port);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int AddWsHostWrapperWithoutIp(HostTopologyInternal topologyInt, int port);
        private static void CheckTopology(HostTopology topology)
        {
            int maxPacketSize = GetMaxPacketSize();
            if (topology.DefaultConfig.PacketSize > maxPacketSize)
            {
                throw new ArgumentOutOfRangeException("Default config: packet size should be less than packet size defined in global config: " + maxPacketSize.ToString());
            }
            for (int i = 0; i < topology.SpecialConnectionConfigs.Count; i++)
            {
                if (topology.SpecialConnectionConfigs[i].PacketSize > maxPacketSize)
                {
                    throw new ArgumentOutOfRangeException("Special config " + i.ToString() + ": packet size should be less than packet size defined in global config: " + maxPacketSize.ToString());
                }
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int Connect(int hostId, string address, int port, int exeptionConnectionId, out byte error);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ConnectAsNetworkHost(int hostId, string address, int port, NetworkID network, SourceID source, NodeID node, out byte error);
        public static int ConnectEndPoint(int hostId, EndPoint endPoint, int exceptionConnectionId, out byte error)
        {
            error = 0;
            byte[] buffer = new byte[] { 0x5f, 0x24, 0x13, 0xf6 };
            if (endPoint == null)
            {
                throw new NullReferenceException("Null EndPoint provided");
            }
            if ((endPoint.GetType().FullName != "UnityEngine.XboxOne.XboxOneEndPoint") && (endPoint.GetType().FullName != "UnityEngine.PS4.SceEndPoint"))
            {
                throw new ArgumentException("Endpoint of type XboxOneEndPoint or SceEndPoint  required");
            }
            if (endPoint.GetType().FullName == "UnityEngine.XboxOne.XboxOneEndPoint")
            {
                EndPoint point = endPoint;
                if (point.AddressFamily != AddressFamily.InterNetworkV6)
                {
                    throw new ArgumentException("XboxOneEndPoint has an invalid family");
                }
                SocketAddress address = point.Serialize();
                if (address.Size != 14)
                {
                    throw new ArgumentException("XboxOneEndPoint has an invalid size");
                }
                if ((address[0] != 0) || (address[1] != 0))
                {
                    throw new ArgumentException("XboxOneEndPoint has an invalid family signature");
                }
                if (((address[2] != buffer[0]) || (address[3] != buffer[1])) || ((address[4] != buffer[2]) || (address[5] != buffer[3])))
                {
                    throw new ArgumentException("XboxOneEndPoint has an invalid signature");
                }
                byte[] buffer2 = new byte[8];
                for (int j = 0; j < buffer2.Length; j++)
                {
                    buffer2[j] = address[6 + j];
                }
                IntPtr ptr = new IntPtr(BitConverter.ToInt64(buffer2, 0));
                if (ptr == IntPtr.Zero)
                {
                    throw new ArgumentException("XboxOneEndPoint has an invalid SOCKET_STORAGE pointer");
                }
                byte[] buffer3 = new byte[2];
                Marshal.Copy(ptr, buffer3, 0, buffer3.Length);
                AddressFamily family = (AddressFamily) ((buffer3[1] << 8) + buffer3[0]);
                if (family != AddressFamily.InterNetworkV6)
                {
                    throw new ArgumentException("XboxOneEndPoint has corrupt or invalid SOCKET_STORAGE pointer");
                }
                return Internal_ConnectEndPoint(hostId, ptr, 0x80, exceptionConnectionId, out error);
            }
            SocketAddress address2 = endPoint.Serialize();
            if (address2.Size != 0x10)
            {
                throw new ArgumentException("EndPoint has an invalid size");
            }
            if (address2[0] != address2.Size)
            {
                throw new ArgumentException("EndPoint has an invalid size value");
            }
            if (address2[1] != 2)
            {
                throw new ArgumentException("EndPoint has an invalid family value");
            }
            byte[] source = new byte[0x10];
            for (int i = 0; i < source.Length; i++)
            {
                source[i] = address2[i];
            }
            IntPtr destination = Marshal.AllocHGlobal(source.Length);
            Marshal.Copy(source, 0, destination, source.Length);
            int num9 = Internal_ConnectEndPoint(hostId, destination, 0x10, exceptionConnectionId, out error);
            Marshal.FreeHGlobal(destination);
            return num9;
        }

        public static int ConnectToNetworkPeer(int hostId, string address, int port, int exceptionConnectionId, int relaySlotId, NetworkID network, SourceID source, NodeID node, out byte error)
        {
            return ConnectToNetworkPeer(hostId, address, port, exceptionConnectionId, relaySlotId, network, source, node, 0, 0f, out error);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int ConnectToNetworkPeer(int hostId, string address, int port, int exceptionConnectionId, int relaySlotId, NetworkID network, SourceID source, NodeID node, int bytesPerSec, float bucketSizeFactor, out byte error);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int ConnectWithSimulator(int hostId, string address, int port, int exeptionConnectionId, out byte error, ConnectionSimulatorConfig conf);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool Disconnect(int hostId, int connectionId, out byte error);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void DisconnectNetworkHost(int hostId, out byte error);
        internal static bool DoesEndPointUsePlatformProtocols(EndPoint endPoint)
        {
            if (endPoint.GetType().FullName == "UnityEngine.PS4.SceEndPoint")
            {
                SocketAddress address = endPoint.Serialize();
                if ((address[8] != 0) || (address[9] != 0))
                {
                    return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool FinishSendMulticast(int hostId, out byte error);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetAssetId(GameObject go);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetBroadcastConnectionInfo(int hostId, out int port, out byte error);
        public static void GetBroadcastConnectionInfo(int hostId, out string address, out int port, out byte error)
        {
            address = GetBroadcastConnectionInfo(hostId, out port, out error);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void GetBroadcastConnectionMessage(int hostId, byte[] buffer, int bufferSize, out int receivedSize, out byte error);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetConnectionInfo(int hostId, int connectionId, out int port, out ulong network, out ushort dstNode, out byte error);
        public static void GetConnectionInfo(int hostId, int connectionId, out string address, out int port, out NetworkID network, out NodeID dstNode, out byte error)
        {
            ulong num;
            ushort num2;
            address = GetConnectionInfo(hostId, connectionId, out port, out num, out num2, out error);
            network = (NetworkID) num;
            dstNode = (NodeID) num2;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetCurrentIncomingMessageAmount();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetCurrentOutgoingMessageAmount();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetCurrentRtt(int hostId, int connectionId, out byte error);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int GetMaxPacketSize();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetNetIOTimeuS();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetNetworkLostPacketNum(int hostId, int connectionId, out byte error);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetNetworkTimestamp();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetNextSceneId();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetPacketReceivedRate(int hostId, int connectionId, out byte error);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetPacketSentRate(int hostId, int connectionId, out byte error);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetRemoteDelayTimeMS(int hostId, int connectionId, int remoteTime, out byte error);
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("GetRemotePacketReceivedRate has been made obsolete. Please do not use this function."), WrapperlessIcall]
        public static extern int GetRemotePacketReceivedRate(int hostId, int connectionId, out byte error);
        public static void Init()
        {
            InitWithNoParameters();
        }

        public static void Init(GlobalConfig config)
        {
            InitWithParameters(new GlobalConfigInternal(config));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void InitWithNoParameters();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void InitWithParameters(GlobalConfigInternal config);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int Internal_ConnectEndPoint(int hostId, IntPtr sockAddrStorage, int sockAddrStorageLen, int exceptionConnectionId, out byte error);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsBroadcastDiscoveryRunning();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern NetworkEventType Receive(out int hostId, out int connectionId, out int channelId, byte[] buffer, int bufferSize, out int receivedSize, out byte error);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern NetworkEventType ReceiveFromHost(int hostId, out int connectionId, out int channelId, byte[] buffer, int bufferSize, out int receivedSize, out byte error);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern NetworkEventType ReceiveRelayEventFromHost(int hostId, out byte error);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool RemoveHost(int hostId);
        public static bool Send(int hostId, int connectionId, int channelId, byte[] buffer, int size, out byte error)
        {
            if (buffer == null)
            {
                throw new NullReferenceException("send buffer is not initialized");
            }
            return SendWrapper(hostId, connectionId, channelId, buffer, size, out error);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool SendMulticast(int hostId, int connectionId, out byte error);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool SendWrapper(int hostId, int connectionId, int channelId, byte[] buffer, int size, out byte error);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetBroadcastCredentials(int hostId, int key, int version, int subversion, out byte error);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetPacketStat(int direction, int packetStatId, int numMsgs, int numBytes);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Shutdown();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool StartBroadcastDiscovery(int hostId, int broadcastPort, int key, int version, int subversion, byte[] buffer, int size, int timeout, out byte error);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool StartSendMulticast(int hostId, int channelId, byte[] buffer, int size, out byte error);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void StopBroadcastDiscovery();

        public static bool IsStarted { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

