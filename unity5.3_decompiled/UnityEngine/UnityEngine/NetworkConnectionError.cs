namespace UnityEngine
{
    using System;

    public enum NetworkConnectionError
    {
        AlreadyConnectedToAnotherServer = -1,
        AlreadyConnectedToServer = 0x10,
        ConnectionBanned = 0x16,
        ConnectionFailed = 15,
        CreateSocketOrThreadFailure = -2,
        EmptyConnectTarget = -4,
        IncorrectParameters = -3,
        InternalDirectConnectFailed = -5,
        InvalidPassword = 0x17,
        NATPunchthroughFailed = 0x49,
        NATTargetConnectionLost = 0x47,
        NATTargetNotConnected = 0x45,
        NoError = 0,
        RSAPublicKeyMismatch = 0x15,
        TooManyConnectedPlayers = 0x12
    }
}

