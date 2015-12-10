namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine.Networking;

    internal class NetworkDetailStats
    {
        private const int kPacketHistoryTicks = 20;
        private const float kPacketTickInterval = 0.5f;
        internal static Dictionary<short, NetworkOperationDetails> m_NetworkOperations = new Dictionary<short, NetworkOperationDetails>();
        private static float s_LastTickTime;

        public static void IncrementStat(NetworkDirection direction, short msgId, string entryName, int amount)
        {
            NetworkOperationDetails details;
            if (m_NetworkOperations.ContainsKey(msgId))
            {
                details = m_NetworkOperations[msgId];
            }
            else
            {
                details = new NetworkOperationDetails {
                    MsgId = msgId
                };
                m_NetworkOperations[msgId] = details;
            }
            details.IncrementStat(direction, entryName, amount);
        }

        public static void NewProfilerTick(float newTime)
        {
            if ((newTime - s_LastTickTime) > 0.5f)
            {
                s_LastTickTime = newTime;
                int tickId = ((int) s_LastTickTime) % 20;
                foreach (NetworkOperationDetails details in m_NetworkOperations.Values)
                {
                    details.NewProfilerTick(tickId);
                }
            }
        }

        public static void ResetAll()
        {
            foreach (NetworkOperationDetails details in m_NetworkOperations.Values)
            {
                NetworkTransport.SetPacketStat(0, details.MsgId, 0, 1);
                NetworkTransport.SetPacketStat(1, details.MsgId, 0, 1);
            }
            m_NetworkOperations.Clear();
        }

        public static void SetStat(NetworkDirection direction, short msgId, string entryName, int amount)
        {
            NetworkOperationDetails details;
            if (m_NetworkOperations.ContainsKey(msgId))
            {
                details = m_NetworkOperations[msgId];
            }
            else
            {
                details = new NetworkOperationDetails {
                    MsgId = msgId
                };
                m_NetworkOperations[msgId] = details;
            }
            details.SetStat(direction, entryName, amount);
        }

        public enum NetworkDirection
        {
            Incoming,
            Outgoing
        }

        internal class NetworkOperationDetails
        {
            public Dictionary<string, NetworkDetailStats.NetworkOperationEntryDetails> m_Entries = new Dictionary<string, NetworkDetailStats.NetworkOperationEntryDetails>();
            public short MsgId;
            public float totalIn;
            public float totalOut;

            public void Clear()
            {
                foreach (NetworkDetailStats.NetworkOperationEntryDetails details in this.m_Entries.Values)
                {
                    details.Clear();
                }
                this.totalIn = 0f;
                this.totalOut = 0f;
            }

            public void IncrementStat(NetworkDetailStats.NetworkDirection direction, string entryName, int amount)
            {
                NetworkDetailStats.NetworkOperationEntryDetails details;
                if (this.m_Entries.ContainsKey(entryName))
                {
                    details = this.m_Entries[entryName];
                }
                else
                {
                    details = new NetworkDetailStats.NetworkOperationEntryDetails {
                        m_EntryName = entryName
                    };
                    this.m_Entries[entryName] = details;
                }
                details.AddStat(direction, amount);
                switch (direction)
                {
                    case NetworkDetailStats.NetworkDirection.Incoming:
                        this.totalIn += amount;
                        break;

                    case NetworkDetailStats.NetworkDirection.Outgoing:
                        this.totalOut += amount;
                        break;
                }
            }

            public void NewProfilerTick(int tickId)
            {
                foreach (NetworkDetailStats.NetworkOperationEntryDetails details in this.m_Entries.Values)
                {
                    details.NewProfilerTick(tickId);
                }
                NetworkTransport.SetPacketStat(0, this.MsgId, (int) this.totalIn, 1);
                NetworkTransport.SetPacketStat(1, this.MsgId, (int) this.totalOut, 1);
                this.totalIn = 0f;
                this.totalOut = 0f;
            }

            public void SetStat(NetworkDetailStats.NetworkDirection direction, string entryName, int amount)
            {
                NetworkDetailStats.NetworkOperationEntryDetails details;
                if (this.m_Entries.ContainsKey(entryName))
                {
                    details = this.m_Entries[entryName];
                }
                else
                {
                    details = new NetworkDetailStats.NetworkOperationEntryDetails {
                        m_EntryName = entryName
                    };
                    this.m_Entries[entryName] = details;
                }
                details.AddStat(direction, amount);
                switch (direction)
                {
                    case NetworkDetailStats.NetworkDirection.Incoming:
                        this.totalIn = amount;
                        break;

                    case NetworkDetailStats.NetworkDirection.Outgoing:
                        this.totalOut = amount;
                        break;
                }
            }
        }

        internal class NetworkOperationEntryDetails
        {
            public string m_EntryName;
            public NetworkDetailStats.NetworkStatsSequence m_IncomingSequence = new NetworkDetailStats.NetworkStatsSequence();
            public int m_IncomingTotal;
            public NetworkDetailStats.NetworkStatsSequence m_OutgoingSequence = new NetworkDetailStats.NetworkStatsSequence();
            public int m_OutgoingTotal;

            public void AddStat(NetworkDetailStats.NetworkDirection direction, int amount)
            {
                int tick = ((int) NetworkDetailStats.s_LastTickTime) % 20;
                switch (direction)
                {
                    case NetworkDetailStats.NetworkDirection.Incoming:
                        this.m_IncomingTotal += amount;
                        this.m_IncomingSequence.Add(tick, amount);
                        break;

                    case NetworkDetailStats.NetworkDirection.Outgoing:
                        this.m_OutgoingTotal += amount;
                        this.m_OutgoingSequence.Add(tick, amount);
                        break;
                }
            }

            public void Clear()
            {
                this.m_IncomingTotal = 0;
                this.m_OutgoingTotal = 0;
            }

            public void NewProfilerTick(int tickId)
            {
                this.m_IncomingSequence.NewProfilerTick(tickId);
                this.m_OutgoingSequence.NewProfilerTick(tickId);
            }
        }

        internal class NetworkStatsSequence
        {
            private int[] m_MessagesPerTick = new int[20];
            public int MessageTotal;

            public void Add(int tick, int amount)
            {
                this.m_MessagesPerTick[tick] += amount;
                this.MessageTotal += amount;
            }

            public int GetFiveTick(int tick)
            {
                int num = 0;
                for (int i = 0; i < 5; i++)
                {
                    num += this.m_MessagesPerTick[((tick - i) + 20) % 20];
                }
                return (num / 5);
            }

            public int GetTenTick(int tick)
            {
                int num = 0;
                for (int i = 0; i < 10; i++)
                {
                    num += this.m_MessagesPerTick[((tick - i) + 20) % 20];
                }
                return (num / 10);
            }

            public void NewProfilerTick(int tick)
            {
                this.MessageTotal -= this.m_MessagesPerTick[tick];
                this.m_MessagesPerTick[tick] = 0;
            }
        }
    }
}

