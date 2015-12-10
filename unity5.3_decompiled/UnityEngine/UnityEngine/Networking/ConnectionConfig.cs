namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class ConnectionConfig
    {
        private const int g_MinPacketSize = 0x80;
        [SerializeField]
        private uint m_AckDelay;
        [SerializeField]
        private uint m_AllCostTimeout;
        [SerializeField]
        internal List<ChannelQOS> m_Channels;
        [SerializeField]
        private uint m_ConnectTimeout;
        [SerializeField]
        private uint m_DisconnectTimeout;
        [SerializeField]
        private ushort m_FragmentSize;
        [SerializeField]
        private bool m_IsAcksLong;
        [SerializeField]
        private ushort m_MaxCombinedReliableMessageCount;
        [SerializeField]
        private ushort m_MaxCombinedReliableMessageSize;
        [SerializeField]
        private byte m_MaxConnectionAttempt;
        [SerializeField]
        private ushort m_MaxSentMessageQueueSize;
        [SerializeField]
        private uint m_MinUpdateTimeout;
        [SerializeField]
        private byte m_NetworkDropThreshold;
        [SerializeField]
        private byte m_OverflowDropThreshold;
        [SerializeField]
        private ushort m_PacketSize;
        [SerializeField]
        private uint m_PingTimeout;
        [SerializeField]
        private uint m_ReducedPingTimeout;
        [SerializeField]
        private uint m_ResendTimeout;
        [SerializeField]
        private bool m_UsePlatformSpecificProtocols;

        public ConnectionConfig()
        {
            this.m_Channels = new List<ChannelQOS>();
            this.m_PacketSize = 0x5dc;
            this.m_FragmentSize = 500;
            this.m_ResendTimeout = 0x4b0;
            this.m_DisconnectTimeout = 0x7d0;
            this.m_ConnectTimeout = 0x7d0;
            this.m_MinUpdateTimeout = 10;
            this.m_PingTimeout = 500;
            this.m_ReducedPingTimeout = 100;
            this.m_AllCostTimeout = 20;
            this.m_NetworkDropThreshold = 5;
            this.m_OverflowDropThreshold = 5;
            this.m_MaxConnectionAttempt = 10;
            this.m_AckDelay = 0x21;
            this.m_MaxCombinedReliableMessageSize = 100;
            this.m_MaxCombinedReliableMessageCount = 10;
            this.m_MaxSentMessageQueueSize = 0x80;
            this.m_IsAcksLong = false;
            this.m_UsePlatformSpecificProtocols = false;
        }

        public ConnectionConfig(ConnectionConfig config)
        {
            this.m_Channels = new List<ChannelQOS>();
            if (config == null)
            {
                throw new NullReferenceException("config is not defined");
            }
            this.m_PacketSize = config.m_PacketSize;
            this.m_FragmentSize = config.m_FragmentSize;
            this.m_ResendTimeout = config.m_ResendTimeout;
            this.m_DisconnectTimeout = config.m_DisconnectTimeout;
            this.m_ConnectTimeout = config.m_ConnectTimeout;
            this.m_MinUpdateTimeout = config.m_MinUpdateTimeout;
            this.m_PingTimeout = config.m_PingTimeout;
            this.m_ReducedPingTimeout = config.m_ReducedPingTimeout;
            this.m_AllCostTimeout = config.m_AllCostTimeout;
            this.m_NetworkDropThreshold = config.m_NetworkDropThreshold;
            this.m_OverflowDropThreshold = config.m_OverflowDropThreshold;
            this.m_MaxConnectionAttempt = config.m_MaxConnectionAttempt;
            this.m_AckDelay = config.m_AckDelay;
            this.m_MaxCombinedReliableMessageSize = config.MaxCombinedReliableMessageSize;
            this.m_MaxCombinedReliableMessageCount = config.m_MaxCombinedReliableMessageCount;
            this.m_MaxSentMessageQueueSize = config.m_MaxSentMessageQueueSize;
            this.m_IsAcksLong = config.m_IsAcksLong;
            this.m_UsePlatformSpecificProtocols = config.m_UsePlatformSpecificProtocols;
            foreach (ChannelQOS lqos in config.m_Channels)
            {
                this.m_Channels.Add(new ChannelQOS(lqos));
            }
        }

        public byte AddChannel(QosType value)
        {
            if (this.m_Channels.Count > 0xff)
            {
                throw new ArgumentOutOfRangeException("Channels Count should be less than 256");
            }
            if (!Enum.IsDefined(typeof(QosType), value))
            {
                throw new ArgumentOutOfRangeException("requested qos type doesn't exist: " + ((int) value));
            }
            ChannelQOS item = new ChannelQOS(value);
            this.m_Channels.Add(item);
            return (byte) (this.m_Channels.Count - 1);
        }

        public QosType GetChannel(byte idx)
        {
            if (idx >= this.m_Channels.Count)
            {
                throw new ArgumentOutOfRangeException("requested index greater than maximum channels count");
            }
            return this.m_Channels[idx].QOS;
        }

        public static void Validate(ConnectionConfig config)
        {
            if (config.m_PacketSize < 0x80)
            {
                int num = 0x80;
                throw new ArgumentOutOfRangeException("PacketSize should be > " + num.ToString());
            }
            if (config.m_FragmentSize >= (config.m_PacketSize - 0x80))
            {
                int num2 = 0x80;
                throw new ArgumentOutOfRangeException("FragmentSize should be < PacketSize - " + num2.ToString());
            }
            if (config.m_Channels.Count > 0xff)
            {
                throw new ArgumentOutOfRangeException("Channels number should be less than 256");
            }
        }

        public uint AckDelay
        {
            get
            {
                return this.m_AckDelay;
            }
            set
            {
                this.m_AckDelay = value;
            }
        }

        public uint AllCostTimeout
        {
            get
            {
                return this.m_AllCostTimeout;
            }
            set
            {
                this.m_AllCostTimeout = value;
            }
        }

        public int ChannelCount
        {
            get
            {
                return this.m_Channels.Count;
            }
        }

        public List<ChannelQOS> Channels
        {
            get
            {
                return this.m_Channels;
            }
        }

        public uint ConnectTimeout
        {
            get
            {
                return this.m_ConnectTimeout;
            }
            set
            {
                this.m_ConnectTimeout = value;
            }
        }

        public uint DisconnectTimeout
        {
            get
            {
                return this.m_DisconnectTimeout;
            }
            set
            {
                this.m_DisconnectTimeout = value;
            }
        }

        public ushort FragmentSize
        {
            get
            {
                return this.m_FragmentSize;
            }
            set
            {
                this.m_FragmentSize = value;
            }
        }

        public bool IsAcksLong
        {
            get
            {
                return this.m_IsAcksLong;
            }
            set
            {
                this.m_IsAcksLong = value;
            }
        }

        public ushort MaxCombinedReliableMessageCount
        {
            get
            {
                return this.m_MaxCombinedReliableMessageCount;
            }
            set
            {
                this.m_MaxCombinedReliableMessageCount = value;
            }
        }

        public ushort MaxCombinedReliableMessageSize
        {
            get
            {
                return this.m_MaxCombinedReliableMessageSize;
            }
            set
            {
                this.m_MaxCombinedReliableMessageSize = value;
            }
        }

        public byte MaxConnectionAttempt
        {
            get
            {
                return this.m_MaxConnectionAttempt;
            }
            set
            {
                this.m_MaxConnectionAttempt = value;
            }
        }

        public ushort MaxSentMessageQueueSize
        {
            get
            {
                return this.m_MaxSentMessageQueueSize;
            }
            set
            {
                this.m_MaxSentMessageQueueSize = value;
            }
        }

        public uint MinUpdateTimeout
        {
            get
            {
                return this.m_MinUpdateTimeout;
            }
            set
            {
                if (value == 0)
                {
                    throw new ArgumentOutOfRangeException("Minimal update timeout should be > 0");
                }
                this.m_MinUpdateTimeout = value;
            }
        }

        public byte NetworkDropThreshold
        {
            get
            {
                return this.m_NetworkDropThreshold;
            }
            set
            {
                this.m_NetworkDropThreshold = value;
            }
        }

        public byte OverflowDropThreshold
        {
            get
            {
                return this.m_OverflowDropThreshold;
            }
            set
            {
                this.m_OverflowDropThreshold = value;
            }
        }

        public ushort PacketSize
        {
            get
            {
                return this.m_PacketSize;
            }
            set
            {
                this.m_PacketSize = value;
            }
        }

        public uint PingTimeout
        {
            get
            {
                return this.m_PingTimeout;
            }
            set
            {
                this.m_PingTimeout = value;
            }
        }

        public uint ReducedPingTimeout
        {
            get
            {
                return this.m_ReducedPingTimeout;
            }
            set
            {
                this.m_ReducedPingTimeout = value;
            }
        }

        public uint ResendTimeout
        {
            get
            {
                return this.m_ResendTimeout;
            }
            set
            {
                this.m_ResendTimeout = value;
            }
        }

        public bool UsePlatformSpecificProtocols
        {
            get
            {
                return this.m_UsePlatformSpecificProtocols;
            }
            set
            {
                if (value && (Application.platform != RuntimePlatform.PS4))
                {
                    throw new ArgumentOutOfRangeException("Platform specific protocols are not supported on this platform");
                }
                this.m_UsePlatformSpecificProtocols = value;
            }
        }
    }
}

