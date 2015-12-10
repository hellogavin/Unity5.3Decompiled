namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;
    using UnityEngineInternal;

    public sealed class Network
    {
        public static NetworkViewID AllocateViewID()
        {
            NetworkViewID wid;
            Internal_AllocateViewID(out wid);
            return wid;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void CloseConnection(NetworkPlayer target, bool sendDisconnectionNotification);
        [ExcludeFromDocs]
        public static NetworkConnectionError Connect(string GUID)
        {
            string password = string.Empty;
            return Connect(GUID, password);
        }

        [ExcludeFromDocs]
        public static NetworkConnectionError Connect(HostData hostData)
        {
            string password = string.Empty;
            return Connect(hostData, password);
        }

        [ExcludeFromDocs]
        public static NetworkConnectionError Connect(string IP, int remotePort)
        {
            string password = string.Empty;
            return Connect(IP, remotePort, password);
        }

        [ExcludeFromDocs]
        public static NetworkConnectionError Connect(string[] IPs, int remotePort)
        {
            string password = string.Empty;
            return Connect(IPs, remotePort, password);
        }

        public static NetworkConnectionError Connect(string GUID, [DefaultValue("\"\"")] string password)
        {
            return Internal_ConnectToGuid(GUID, password);
        }

        public static NetworkConnectionError Connect(HostData hostData, [DefaultValue("\"\"")] string password)
        {
            if (hostData == null)
            {
                throw new NullReferenceException();
            }
            if ((hostData.guid.Length > 0) && hostData.useNat)
            {
                return Connect(hostData.guid, password);
            }
            return Connect(hostData.ip, hostData.port, password);
        }

        public static NetworkConnectionError Connect(string IP, int remotePort, [DefaultValue("\"\"")] string password)
        {
            return Internal_ConnectToSingleIP(IP, remotePort, 0, password);
        }

        public static NetworkConnectionError Connect(string[] IPs, int remotePort, [DefaultValue("\"\"")] string password)
        {
            return Internal_ConnectToIPs(IPs, remotePort, 0, password);
        }

        public static void Destroy(GameObject gameObject)
        {
            if (gameObject != null)
            {
                NetworkView component = gameObject.GetComponent<NetworkView>();
                if (component != null)
                {
                    Destroy(component.viewID);
                }
                else
                {
                    Debug.LogError("Couldn't destroy game object because no network view is attached to it.", gameObject);
                }
            }
        }

        public static void Destroy(NetworkViewID viewID)
        {
            INTERNAL_CALL_Destroy(ref viewID);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void DestroyPlayerObjects(NetworkPlayer playerID);
        [ExcludeFromDocs]
        public static void Disconnect()
        {
            int timeout = 200;
            Disconnect(timeout);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Disconnect([DefaultValue("200")] int timeout);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetAveragePing(NetworkPlayer player);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetLastPing(NetworkPlayer player);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool HavePublicAddress();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void InitializeSecurity();
        [Obsolete("Use the IntializeServer(connections, listenPort, useNat) function instead")]
        public static NetworkConnectionError InitializeServer(int connections, int listenPort)
        {
            return Internal_InitializeServerDeprecated(connections, listenPort);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern NetworkConnectionError InitializeServer(int connections, int listenPort, bool useNat);
        [TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
        public static Object Instantiate(Object prefab, Vector3 position, Quaternion rotation, int group)
        {
            return INTERNAL_CALL_Instantiate(prefab, ref position, ref rotation, group);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_AllocateViewID(out NetworkViewID viewID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Destroy(ref NetworkViewID viewID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Object INTERNAL_CALL_Instantiate(Object prefab, ref Vector3 position, ref Quaternion rotation, int group);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Internal_RemoveRPCs(NetworkPlayer playerID, ref NetworkViewID viewID, uint channelMask);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern NetworkConnectionError Internal_ConnectToGuid(string guid, string password);
        [ExcludeFromDocs]
        private static NetworkConnectionError Internal_ConnectToIPs(string[] IP, int remotePort, int localPort)
        {
            string password = string.Empty;
            return Internal_ConnectToIPs(IP, remotePort, localPort, password);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern NetworkConnectionError Internal_ConnectToIPs(string[] IP, int remotePort, int localPort, [DefaultValue("\"\"")] string password);
        [ExcludeFromDocs]
        private static NetworkConnectionError Internal_ConnectToSingleIP(string IP, int remotePort, int localPort)
        {
            string password = string.Empty;
            return Internal_ConnectToSingleIP(IP, remotePort, localPort, password);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern NetworkConnectionError Internal_ConnectToSingleIP(string IP, int remotePort, int localPort, [DefaultValue("\"\"")] string password);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int Internal_GetPlayer();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_GetTime(out double t);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern NetworkConnectionError Internal_InitializeServerDeprecated(int connections, int listenPort);
        private static void Internal_RemoveRPCs(NetworkPlayer playerID, NetworkViewID viewID, uint channelMask)
        {
            INTERNAL_CALL_Internal_RemoveRPCs(playerID, ref viewID, channelMask);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_SetSendingGlobal(int group, bool enabled);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_SetSendingSpecific(NetworkPlayer player, int group, bool enabled);
        public static void RemoveRPCs(NetworkPlayer playerID)
        {
            Internal_RemoveRPCs(playerID, NetworkViewID.unassigned, uint.MaxValue);
        }

        public static void RemoveRPCs(NetworkViewID viewID)
        {
            Internal_RemoveRPCs(NetworkPlayer.unassigned, viewID, uint.MaxValue);
        }

        public static void RemoveRPCs(NetworkPlayer playerID, int group)
        {
            Internal_RemoveRPCs(playerID, NetworkViewID.unassigned, ((uint) 1) << group);
        }

        public static void RemoveRPCsInGroup(int group)
        {
            Internal_RemoveRPCs(NetworkPlayer.unassigned, NetworkViewID.unassigned, ((uint) 1) << group);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetLevelPrefix(int prefix);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetReceivingEnabled(NetworkPlayer player, int group, bool enabled);
        public static void SetSendingEnabled(int group, bool enabled)
        {
            Internal_SetSendingGlobal(group, enabled);
        }

        public static void SetSendingEnabled(NetworkPlayer player, int group, bool enabled)
        {
            Internal_SetSendingSpecific(player, group, enabled);
        }

        [ExcludeFromDocs]
        public static ConnectionTesterStatus TestConnection()
        {
            bool forceTest = false;
            return TestConnection(forceTest);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern ConnectionTesterStatus TestConnection([DefaultValue("false")] bool forceTest);
        [ExcludeFromDocs]
        public static ConnectionTesterStatus TestConnectionNAT()
        {
            bool forceTest = false;
            return TestConnectionNAT(forceTest);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern ConnectionTesterStatus TestConnectionNAT([DefaultValue("false")] bool forceTest);

        public static NetworkPlayer[] connections { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string connectionTesterIP { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static int connectionTesterPort { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static string incomingPassword { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool isClient { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool isMessageQueueRunning { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool isServer { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static NetworkLogLevel logLevel { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static int maxConnections { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static int minimumAllocatableViewIDs { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static string natFacilitatorIP { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static int natFacilitatorPort { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static NetworkPeerType peerType { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static NetworkPlayer player
        {
            get
            {
                NetworkPlayer player;
                player.index = Internal_GetPlayer();
                return player;
            }
        }

        public static string proxyIP { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static string proxyPassword { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static int proxyPort { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static float sendRate { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static double time
        {
            get
            {
                double num;
                Internal_GetTime(out num);
                return num;
            }
        }

        [Obsolete("No longer needed. This is now explicitly set in the InitializeServer function call. It is implicitly set when calling Connect depending on if an IP/port combination is used (useNat=false) or a GUID is used(useNat=true).")]
        public static bool useNat { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool useProxy { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

