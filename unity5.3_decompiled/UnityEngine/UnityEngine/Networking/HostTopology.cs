namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class HostTopology
    {
        [SerializeField]
        private ConnectionConfig m_DefConfig;
        [SerializeField]
        private int m_MaxDefConnections;
        [SerializeField]
        private float m_MessagePoolSizeGrowthFactor;
        [SerializeField]
        private ushort m_ReceivedMessagePoolSize;
        [SerializeField]
        private ushort m_SentMessagePoolSize;
        [SerializeField]
        private List<ConnectionConfig> m_SpecialConnections;

        private HostTopology()
        {
            this.m_SpecialConnections = new List<ConnectionConfig>();
            this.m_ReceivedMessagePoolSize = 0x80;
            this.m_SentMessagePoolSize = 0x80;
            this.m_MessagePoolSizeGrowthFactor = 0.75f;
        }

        public HostTopology(ConnectionConfig defaultConfig, int maxDefaultConnections)
        {
            this.m_SpecialConnections = new List<ConnectionConfig>();
            this.m_ReceivedMessagePoolSize = 0x80;
            this.m_SentMessagePoolSize = 0x80;
            this.m_MessagePoolSizeGrowthFactor = 0.75f;
            if (defaultConfig == null)
            {
                throw new NullReferenceException("config is not defined");
            }
            if (maxDefaultConnections <= 0)
            {
                throw new ArgumentOutOfRangeException("maxDefaultConnections", "count connection should be > 0");
            }
            if (maxDefaultConnections > 0xffff)
            {
                throw new ArgumentOutOfRangeException("maxDefaultConnections", "count connection should be < 65535");
            }
            ConnectionConfig.Validate(defaultConfig);
            this.m_DefConfig = new ConnectionConfig(defaultConfig);
            this.m_MaxDefConnections = maxDefaultConnections;
        }

        public int AddSpecialConnectionConfig(ConnectionConfig config)
        {
            this.m_SpecialConnections.Add(new ConnectionConfig(config));
            return (this.m_SpecialConnections.Count - 1);
        }

        public ConnectionConfig GetSpecialConnectionConfig(int i)
        {
            if ((i > this.m_SpecialConnections.Count) || (i == 0))
            {
                throw new ArgumentException("special configuration index is out of valid range");
            }
            return this.m_SpecialConnections[i - 1];
        }

        public ConnectionConfig DefaultConfig
        {
            get
            {
                return this.m_DefConfig;
            }
        }

        public int MaxDefaultConnections
        {
            get
            {
                return this.m_MaxDefConnections;
            }
        }

        public float MessagePoolSizeGrowthFactor
        {
            get
            {
                return this.m_MessagePoolSizeGrowthFactor;
            }
            set
            {
                if ((value <= 0.5) || (value > 1.0))
                {
                    throw new ArgumentException("pool growth factor should be varied between 0.5 and 1.0");
                }
                this.m_MessagePoolSizeGrowthFactor = value;
            }
        }

        public ushort ReceivedMessagePoolSize
        {
            get
            {
                return this.m_ReceivedMessagePoolSize;
            }
            set
            {
                this.m_ReceivedMessagePoolSize = value;
            }
        }

        public ushort SentMessagePoolSize
        {
            get
            {
                return this.m_SentMessagePoolSize;
            }
            set
            {
                this.m_SentMessagePoolSize = value;
            }
        }

        public List<ConnectionConfig> SpecialConnectionConfigs
        {
            get
            {
                return this.m_SpecialConnections;
            }
        }

        public int SpecialConnectionConfigsCount
        {
            get
            {
                return this.m_SpecialConnections.Count;
            }
        }
    }
}

