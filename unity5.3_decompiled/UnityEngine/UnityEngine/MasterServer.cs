namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Internal;

    public sealed class MasterServer
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ClearHostList();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern HostData[] PollHostList();
        [ExcludeFromDocs]
        public static void RegisterHost(string gameTypeName, string gameName)
        {
            string comment = string.Empty;
            RegisterHost(gameTypeName, gameName, comment);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void RegisterHost(string gameTypeName, string gameName, [DefaultValue("\"\"")] string comment);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void RequestHostList(string gameTypeName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void UnregisterHost();

        public static bool dedicatedServer { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static string ipAddress { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static int port { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static int updateRate { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

