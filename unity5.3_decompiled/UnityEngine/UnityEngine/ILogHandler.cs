namespace UnityEngine
{
    using System;

    public interface ILogHandler
    {
        void LogException(Exception exception, Object context);
        void LogFormat(LogType logType, Object context, string format, params object[] args);
    }
}

