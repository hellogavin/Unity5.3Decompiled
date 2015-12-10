namespace UnityEngine
{
    using System;

    public sealed class PlayerPrefsException : Exception
    {
        public PlayerPrefsException(string error) : base(error)
        {
        }
    }
}

