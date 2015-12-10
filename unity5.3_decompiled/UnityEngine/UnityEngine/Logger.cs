namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public class Logger : ILogger, ILogHandler
    {
        private const string kNoTagFormat = "{0}";
        private const string kTagFormat = "{0}: {1}";

        private Logger()
        {
        }

        public Logger(ILogHandler logHandler)
        {
            this.logHandler = logHandler;
            this.logEnabled = true;
            this.filterLogType = LogType.Log;
        }

        private static string GetString(object message)
        {
            return ((message == null) ? "Null" : message.ToString());
        }

        public bool IsLogTypeAllowed(LogType logType)
        {
            return (this.logEnabled && ((logType <= this.filterLogType) || (logType == LogType.Exception)));
        }

        public void Log(object message)
        {
            if (this.IsLogTypeAllowed(LogType.Log))
            {
                object[] args = new object[] { GetString(message) };
                this.logHandler.LogFormat(LogType.Log, null, "{0}", args);
            }
        }

        public void Log(string tag, object message)
        {
            if (this.IsLogTypeAllowed(LogType.Log))
            {
                object[] args = new object[] { tag, GetString(message) };
                this.logHandler.LogFormat(LogType.Log, null, "{0}: {1}", args);
            }
        }

        public void Log(LogType logType, object message)
        {
            if (this.IsLogTypeAllowed(logType))
            {
                object[] args = new object[] { GetString(message) };
                this.logHandler.LogFormat(logType, null, "{0}", args);
            }
        }

        public void Log(string tag, object message, Object context)
        {
            if (this.IsLogTypeAllowed(LogType.Log))
            {
                object[] args = new object[] { tag, GetString(message) };
                this.logHandler.LogFormat(LogType.Log, context, "{0}: {1}", args);
            }
        }

        public void Log(LogType logType, object message, Object context)
        {
            if (this.IsLogTypeAllowed(logType))
            {
                object[] args = new object[] { GetString(message) };
                this.logHandler.LogFormat(logType, context, "{0}", args);
            }
        }

        public void Log(LogType logType, string tag, object message)
        {
            if (this.IsLogTypeAllowed(logType))
            {
                object[] args = new object[] { tag, GetString(message) };
                this.logHandler.LogFormat(logType, null, "{0}: {1}", args);
            }
        }

        public void Log(LogType logType, string tag, object message, Object context)
        {
            if (this.IsLogTypeAllowed(logType))
            {
                object[] args = new object[] { tag, GetString(message) };
                this.logHandler.LogFormat(logType, context, "{0}: {1}", args);
            }
        }

        public void LogError(string tag, object message)
        {
            if (this.IsLogTypeAllowed(LogType.Error))
            {
                object[] args = new object[] { tag, GetString(message) };
                this.logHandler.LogFormat(LogType.Error, null, "{0}: {1}", args);
            }
        }

        public void LogError(string tag, object message, Object context)
        {
            if (this.IsLogTypeAllowed(LogType.Error))
            {
                object[] args = new object[] { tag, GetString(message) };
                this.logHandler.LogFormat(LogType.Error, context, "{0}: {1}", args);
            }
        }

        public void LogException(Exception exception)
        {
            if (this.logEnabled)
            {
                this.logHandler.LogException(exception, null);
            }
        }

        public void LogException(Exception exception, Object context)
        {
            if (this.logEnabled)
            {
                this.logHandler.LogException(exception, context);
            }
        }

        public void LogFormat(LogType logType, string format, params object[] args)
        {
            if (this.IsLogTypeAllowed(logType))
            {
                this.logHandler.LogFormat(logType, null, format, args);
            }
        }

        public void LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            if (this.IsLogTypeAllowed(logType))
            {
                this.logHandler.LogFormat(logType, context, format, args);
            }
        }

        public void LogWarning(string tag, object message)
        {
            if (this.IsLogTypeAllowed(LogType.Warning))
            {
                object[] args = new object[] { tag, GetString(message) };
                this.logHandler.LogFormat(LogType.Warning, null, "{0}: {1}", args);
            }
        }

        public void LogWarning(string tag, object message, Object context)
        {
            if (this.IsLogTypeAllowed(LogType.Warning))
            {
                object[] args = new object[] { tag, GetString(message) };
                this.logHandler.LogFormat(LogType.Warning, context, "{0}: {1}", args);
            }
        }

        public LogType filterLogType { get; set; }

        public bool logEnabled { get; set; }

        public ILogHandler logHandler { get; set; }
    }
}

