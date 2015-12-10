namespace UnityEditor.Modules
{
    using System;

    internal interface IDevice
    {
        RemoteAddress StartPlayerConnectionSupport();
        RemoteAddress StartRemoteSupport();
        void StopPlayerConnectionSupport();
        void StopRemoteSupport();
    }
}

