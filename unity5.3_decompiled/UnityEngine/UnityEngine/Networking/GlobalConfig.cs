namespace UnityEngine.Networking
{
    using System;
    using UnityEngine;

    [Serializable]
    public class GlobalConfig
    {
        [SerializeField]
        private ushort m_MaxPacketSize = 0x7d0;
        [SerializeField]
        private ushort m_ReactorMaximumReceivedMessages = 0x400;
        [SerializeField]
        private ushort m_ReactorMaximumSentMessages = 0x400;
        [SerializeField]
        private UnityEngine.Networking.ReactorModel m_ReactorModel = UnityEngine.Networking.ReactorModel.SelectReactor;
        [SerializeField]
        private uint m_ThreadAwakeTimeout = 1;

        public ushort MaxPacketSize
        {
            get
            {
                return this.m_MaxPacketSize;
            }
            set
            {
                this.m_MaxPacketSize = value;
            }
        }

        public ushort ReactorMaximumReceivedMessages
        {
            get
            {
                return this.m_ReactorMaximumReceivedMessages;
            }
            set
            {
                this.m_ReactorMaximumReceivedMessages = value;
            }
        }

        public ushort ReactorMaximumSentMessages
        {
            get
            {
                return this.m_ReactorMaximumSentMessages;
            }
            set
            {
                this.m_ReactorMaximumSentMessages = value;
            }
        }

        public UnityEngine.Networking.ReactorModel ReactorModel
        {
            get
            {
                return this.m_ReactorModel;
            }
            set
            {
                this.m_ReactorModel = value;
            }
        }

        public uint ThreadAwakeTimeout
        {
            get
            {
                return this.m_ThreadAwakeTimeout;
            }
            set
            {
                if (value == 0)
                {
                    throw new ArgumentOutOfRangeException("Minimal thread awake timeout should be > 0");
                }
                this.m_ThreadAwakeTimeout = value;
            }
        }
    }
}

