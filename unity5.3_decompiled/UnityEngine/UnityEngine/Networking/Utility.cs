namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Networking.Types;

    public sealed class Utility
    {
        private static Dictionary<NetworkID, NetworkAccessToken> s_dictTokens = new Dictionary<NetworkID, NetworkAccessToken>();
        private static AppID s_programAppID = AppID.Invalid;
        private static Random s_randomGenerator = new Random(Environment.TickCount);
        private static int s_randomSourceComponent = 0;
        private static bool s_useRandomSourceID = false;

        private Utility()
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void FetchNetworkingId(string projectId);
        public static NetworkAccessToken GetAccessTokenForNetwork(NetworkID netId)
        {
            NetworkAccessToken token;
            if (!s_dictTokens.TryGetValue(netId, out token))
            {
                token = new NetworkAccessToken();
            }
            return token;
        }

        public static AppID GetAppID()
        {
            return s_programAppID;
        }

        public static SourceID GetSourceID()
        {
            return (SourceID) (SystemInfo.deviceUniqueIdentifier + s_randomSourceComponent).GetHashCode();
        }

        public static void SetAccessTokenForNetwork(NetworkID netId, NetworkAccessToken accessToken)
        {
            s_dictTokens.Add(netId, accessToken);
        }

        public static void SetAppID(AppID newAppID)
        {
            s_programAppID = newAppID;
        }

        private static void SetUseRandomSourceID(bool useRandomSourceID)
        {
            if (useRandomSourceID && !s_useRandomSourceID)
            {
                s_randomSourceComponent = s_randomGenerator.Next(0x7fffffff);
            }
            else if (!useRandomSourceID && s_useRandomSourceID)
            {
                s_randomSourceComponent = 0;
            }
            s_useRandomSourceID = useRandomSourceID;
        }

        public static bool useRandomSourceID
        {
            get
            {
                return s_useRandomSourceID;
            }
            set
            {
                SetUseRandomSourceID(value);
            }
        }
    }
}

