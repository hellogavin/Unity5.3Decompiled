namespace UnityEngine.Networking
{
    using System;

    public enum QosType
    {
        Unreliable,
        UnreliableFragmented,
        UnreliableSequenced,
        Reliable,
        ReliableFragmented,
        ReliableSequenced,
        StateUpdate,
        ReliableStateUpdate,
        AllCostDelivery
    }
}

