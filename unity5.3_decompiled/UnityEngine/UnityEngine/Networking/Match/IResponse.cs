namespace UnityEngine.Networking.Match
{
    using System;

    public interface IResponse
    {
        void SetFailure(string info);
        void SetSuccess();
    }
}

