namespace UnityEngine
{
    using System;

    internal sealed class UnityString
    {
        public static string Format(string fmt, params object[] args)
        {
            return string.Format(fmt, args);
        }
    }
}

