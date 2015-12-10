namespace UnityEngine
{
    using System;

    public interface ILogger : ILogHandler
    {
        bool IsLogTypeAllowed(LogType logType);
        void Log(object message);
        void Log(string tag, object message);
        void Log(LogType logType, object message);
        void Log(string tag, object message, Object context);
        void Log(LogType logType, object message, Object context);
        void Log(LogType logType, string tag, object message);
        void Log(LogType logType, string tag, object message, Object context);
        void LogError(string tag, object message);
        void LogError(string tag, object message, Object context);
        void LogException(Exception exception);
        void LogFormat(LogType logType, string format, params object[] args);
        void LogWarning(string tag, object message);
        void LogWarning(string tag, object message, Object context);

        LogType filterLogType { get; set; }

        bool logEnabled { get; set; }

        ILogHandler logHandler { get; set; }
    }
}

