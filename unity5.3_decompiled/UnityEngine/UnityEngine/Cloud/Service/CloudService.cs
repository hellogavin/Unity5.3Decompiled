namespace UnityEngine.Cloud.Service
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    internal sealed class CloudService : IDisposable
    {
        [NonSerialized]
        internal IntPtr m_Ptr;
        public CloudService(CloudServiceType serviceType)
        {
            this.InternalCreate(serviceType);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void InternalCreate(CloudServiceType serviceType);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void InternalDestroy();
        ~CloudService()
        {
            this.InternalDestroy();
        }

        public void Dispose()
        {
            this.InternalDestroy();
            GC.SuppressFinalize(this);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool Initialize(string projectId);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool StartEventHandler(string sessionInfo, int maxNumberOfEventInQueue, int maxEventTimeoutInSec);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool PauseEventHandler(bool flushEvents);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool StopEventHandler();
        public bool StartEventDispatcher(CloudServiceConfig serviceConfig, Dictionary<string, string> headers)
        {
            return this.InternalStartEventDispatcher(serviceConfig, FlattenedHeadersFrom(headers));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern bool InternalStartEventDispatcher(CloudServiceConfig serviceConfig, string[] headers);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool PauseEventDispatcher();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool StopEventDispatcher();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void ResetNetworkRetryIndex();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool QueueEvent(string eventData, CloudEventFlags flags);
        public bool SaveFileFromServer(string fileName, string url, Dictionary<string, string> headers, object d, string methodName)
        {
            if (methodName == null)
            {
                methodName = string.Empty;
            }
            return this.InternalSaveFileFromServer(fileName, url, FlattenedHeadersFrom(headers), d, methodName);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern bool InternalSaveFileFromServer(string fileName, string url, string[] headers, object d, string methodName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool SaveFile(string fileName, string data);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string RestoreFile(string fileName);
        public string serviceFolderName { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        private static string[] FlattenedHeadersFrom(Dictionary<string, string> headers)
        {
            if (headers == null)
            {
                return null;
            }
            string[] strArray = new string[headers.Count * 2];
            int num = 0;
            foreach (KeyValuePair<string, string> pair in headers)
            {
                strArray[num++] = pair.Key.ToString();
                strArray[num++] = pair.Value.ToString();
            }
            return strArray;
        }
    }
}

