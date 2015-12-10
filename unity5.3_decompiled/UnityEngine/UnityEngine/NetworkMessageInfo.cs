namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public struct NetworkMessageInfo
    {
        private double m_TimeStamp;
        private NetworkPlayer m_Sender;
        private NetworkViewID m_ViewID;
        public double timestamp
        {
            get
            {
                return this.m_TimeStamp;
            }
        }
        public NetworkPlayer sender
        {
            get
            {
                return this.m_Sender;
            }
        }
        public NetworkView networkView
        {
            get
            {
                if (this.m_ViewID == NetworkViewID.unassigned)
                {
                    Debug.LogError("No NetworkView is assigned to this NetworkMessageInfo object. Note that this is expected in OnNetworkInstantiate().");
                    return this.NullNetworkView();
                }
                return NetworkView.Find(this.m_ViewID);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern NetworkView NullNetworkView();
    }
}

