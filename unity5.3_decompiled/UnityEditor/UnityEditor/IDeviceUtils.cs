namespace UnityEditor
{
    using System;
    using UnityEditor.Modules;

    internal static class IDeviceUtils
    {
        internal static RemoteAddress StartPlayerConnectionSupport(string deviceId)
        {
            return ModuleManager.GetDevice(deviceId).StartPlayerConnectionSupport();
        }

        internal static RemoteAddress StartRemoteSupport(string deviceId)
        {
            return ModuleManager.GetDevice(deviceId).StartRemoteSupport();
        }

        internal static void StopPlayerConnectionSupport(string deviceId)
        {
            ModuleManager.GetDevice(deviceId).StopPlayerConnectionSupport();
        }

        internal static void StopRemoteSupport(string deviceId)
        {
            ModuleManager.GetDevice(deviceId).StopRemoteSupport();
        }
    }
}

