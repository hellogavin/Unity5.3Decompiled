namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public struct NetworkPlayer
    {
        internal int index;
        public NetworkPlayer(string ip, int port)
        {
            Debug.LogError("Not yet implemented");
            this.index = 0;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern string Internal_GetIPAddress(int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int Internal_GetPort(int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern string Internal_GetExternalIP();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int Internal_GetExternalPort();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern string Internal_GetLocalIP();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int Internal_GetLocalPort();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int Internal_GetPlayerIndex();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern string Internal_GetGUID(int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern string Internal_GetLocalGUID();
        public override int GetHashCode()
        {
            return this.index.GetHashCode();
        }

        public override bool Equals(object other)
        {
            if (!(other is NetworkPlayer))
            {
                return false;
            }
            NetworkPlayer player = (NetworkPlayer) other;
            return (player.index == this.index);
        }

        public string ipAddress
        {
            get
            {
                if (this.index == Internal_GetPlayerIndex())
                {
                    return Internal_GetLocalIP();
                }
                return Internal_GetIPAddress(this.index);
            }
        }
        public int port
        {
            get
            {
                if (this.index == Internal_GetPlayerIndex())
                {
                    return Internal_GetLocalPort();
                }
                return Internal_GetPort(this.index);
            }
        }
        public string guid
        {
            get
            {
                if (this.index == Internal_GetPlayerIndex())
                {
                    return Internal_GetLocalGUID();
                }
                return Internal_GetGUID(this.index);
            }
        }
        public override string ToString()
        {
            return this.index.ToString();
        }

        public string externalIP
        {
            get
            {
                return Internal_GetExternalIP();
            }
        }
        public int externalPort
        {
            get
            {
                return Internal_GetExternalPort();
            }
        }
        internal static NetworkPlayer unassigned
        {
            get
            {
                NetworkPlayer player;
                player.index = -1;
                return player;
            }
        }
        public static bool operator ==(NetworkPlayer lhs, NetworkPlayer rhs)
        {
            return (lhs.index == rhs.index);
        }

        public static bool operator !=(NetworkPlayer lhs, NetworkPlayer rhs)
        {
            return (lhs.index != rhs.index);
        }
    }
}

