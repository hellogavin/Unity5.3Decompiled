namespace UnityEngine
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine.Internal;

    public sealed class Debug
    {
        internal static Logger s_Logger = new Logger(new DebugLogHandler());

        [Conditional("UNITY_ASSERTIONS")]
        public static void Assert(bool condition)
        {
            if (!condition)
            {
                logger.Log(LogType.Assert, "Assertion failed");
            }
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void Assert(bool condition, object message)
        {
            if (!condition)
            {
                logger.Log(LogType.Assert, message);
            }
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                logger.Log(LogType.Assert, message);
            }
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void Assert(bool condition, Object context)
        {
            if (!condition)
            {
                logger.Log(LogType.Assert, "Assertion failed", context);
            }
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void Assert(bool condition, object message, Object context)
        {
            if (!condition)
            {
                logger.Log(LogType.Assert, message, context);
            }
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void Assert(bool condition, string message, Object context)
        {
            if (!condition)
            {
                logger.Log(LogType.Assert, message, context);
            }
        }

        [Conditional("UNITY_ASSERTIONS"), Obsolete("Assert(bool, string, params object[]) is obsolete. Use AssertFormat(bool, string, params object[]) (UnityUpgradable) -> AssertFormat(*)", true)]
        public static void Assert(bool condition, string format, params object[] args)
        {
            if (!condition)
            {
                logger.LogFormat(LogType.Assert, format, args);
            }
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AssertFormat(bool condition, string format, params object[] args)
        {
            if (!condition)
            {
                logger.LogFormat(LogType.Assert, format, args);
            }
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void AssertFormat(bool condition, Object context, string format, params object[] args)
        {
            if (!condition)
            {
                logger.LogFormat(LogType.Assert, context, format, args);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Break();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ClearDeveloperConsole();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void DebugBreak();
        [ExcludeFromDocs]
        public static void DrawLine(Vector3 start, Vector3 end)
        {
            bool depthTest = true;
            float duration = 0f;
            Color white = Color.white;
            INTERNAL_CALL_DrawLine(ref start, ref end, ref white, duration, depthTest);
        }

        [ExcludeFromDocs]
        public static void DrawLine(Vector3 start, Vector3 end, Color color)
        {
            bool depthTest = true;
            float duration = 0f;
            INTERNAL_CALL_DrawLine(ref start, ref end, ref color, duration, depthTest);
        }

        [ExcludeFromDocs]
        public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
        {
            bool depthTest = true;
            INTERNAL_CALL_DrawLine(ref start, ref end, ref color, duration, depthTest);
        }

        public static void DrawLine(Vector3 start, Vector3 end, [DefaultValue("Color.white")] Color color, [DefaultValue("0.0f")] float duration, [DefaultValue("true")] bool depthTest)
        {
            INTERNAL_CALL_DrawLine(ref start, ref end, ref color, duration, depthTest);
        }

        [ExcludeFromDocs]
        public static void DrawRay(Vector3 start, Vector3 dir)
        {
            bool depthTest = true;
            float duration = 0f;
            Color white = Color.white;
            DrawRay(start, dir, white, duration, depthTest);
        }

        [ExcludeFromDocs]
        public static void DrawRay(Vector3 start, Vector3 dir, Color color)
        {
            bool depthTest = true;
            float duration = 0f;
            DrawRay(start, dir, color, duration, depthTest);
        }

        [ExcludeFromDocs]
        public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration)
        {
            bool depthTest = true;
            DrawRay(start, dir, color, duration, depthTest);
        }

        public static void DrawRay(Vector3 start, Vector3 dir, [DefaultValue("Color.white")] Color color, [DefaultValue("0.0f")] float duration, [DefaultValue("true")] bool depthTest)
        {
            DrawLine(start, start + dir, color, duration, depthTest);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_DrawLine(ref Vector3 start, ref Vector3 end, ref Color color, float duration, bool depthTest);
        public static void Log(object message)
        {
            logger.Log(LogType.Log, message);
        }

        public static void Log(object message, Object context)
        {
            logger.Log(LogType.Log, message, context);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void LogAssertion(object message)
        {
            logger.Log(LogType.Assert, message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void LogAssertion(object message, Object context)
        {
            logger.Log(LogType.Assert, message, context);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void LogAssertionFormat(string format, params object[] args)
        {
            logger.LogFormat(LogType.Assert, format, args);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void LogAssertionFormat(Object context, string format, params object[] args)
        {
            logger.LogFormat(LogType.Assert, context, format, args);
        }

        public static void LogError(object message)
        {
            logger.Log(LogType.Error, message);
        }

        public static void LogError(object message, Object context)
        {
            logger.Log(LogType.Error, message, context);
        }

        public static void LogErrorFormat(string format, params object[] args)
        {
            logger.LogFormat(LogType.Error, format, args);
        }

        public static void LogErrorFormat(Object context, string format, params object[] args)
        {
            logger.LogFormat(LogType.Error, context, format, args);
        }

        public static void LogException(Exception exception)
        {
            logger.LogException(exception, null);
        }

        public static void LogException(Exception exception, Object context)
        {
            logger.LogException(exception, context);
        }

        public static void LogFormat(string format, params object[] args)
        {
            logger.LogFormat(LogType.Log, format, args);
        }

        public static void LogFormat(Object context, string format, params object[] args)
        {
            logger.LogFormat(LogType.Log, context, format, args);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void LogPlayerBuildError(string message, string file, int line, int column);
        public static void LogWarning(object message)
        {
            logger.Log(LogType.Warning, message);
        }

        public static void LogWarning(object message, Object context)
        {
            logger.Log(LogType.Warning, message, context);
        }

        public static void LogWarningFormat(string format, params object[] args)
        {
            logger.LogFormat(LogType.Warning, format, args);
        }

        public static void LogWarningFormat(Object context, string format, params object[] args)
        {
            logger.LogFormat(LogType.Warning, context, format, args);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void OpenConsoleFile();

        public static bool developerConsoleVisible { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool isDebugBuild { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static ILogger logger
        {
            get
            {
                return s_Logger;
            }
        }
    }
}

