namespace UnityEngine
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;
    using UnityEngine.Scripting;

    public class StackTraceUtility
    {
        private static string projectFolder = string.Empty;

        [SecuritySafeCritical]
        internal static string ExtractFormattedStackTrace(StackTrace stackTrace)
        {
            StringBuilder builder = new StringBuilder(0xff);
            for (int i = 0; i < stackTrace.FrameCount; i++)
            {
                StackFrame frame = stackTrace.GetFrame(i);
                MethodBase method = frame.GetMethod();
                if (method != null)
                {
                    Type declaringType = method.DeclaringType;
                    if (declaringType != null)
                    {
                        string str = declaringType.Namespace;
                        if ((str != null) && (str.Length != 0))
                        {
                            builder.Append(str);
                            builder.Append(".");
                        }
                        builder.Append(declaringType.Name);
                        builder.Append(":");
                        builder.Append(method.Name);
                        builder.Append("(");
                        int index = 0;
                        ParameterInfo[] parameters = method.GetParameters();
                        bool flag = true;
                        while (index < parameters.Length)
                        {
                            if (!flag)
                            {
                                builder.Append(", ");
                            }
                            else
                            {
                                flag = false;
                            }
                            builder.Append(parameters[index].ParameterType.Name);
                            index++;
                        }
                        builder.Append(")");
                        string fileName = frame.GetFileName();
                        if ((fileName != null) && (((((declaringType.Name != "Debug") || (declaringType.Namespace != "UnityEngine")) && ((declaringType.Name != "Logger") || (declaringType.Namespace != "UnityEngine"))) && ((declaringType.Name != "DebugLogHandler") || (declaringType.Namespace != "UnityEngine"))) && ((declaringType.Name != "Assert") || (declaringType.Namespace != "UnityEngine.Assertions"))))
                        {
                            builder.Append(" (at ");
                            if (fileName.StartsWith(projectFolder))
                            {
                                fileName = fileName.Substring(projectFolder.Length, fileName.Length - projectFolder.Length);
                            }
                            builder.Append(fileName);
                            builder.Append(":");
                            builder.Append(frame.GetFileLineNumber().ToString());
                            builder.Append(")");
                        }
                        builder.Append("\n");
                    }
                }
            }
            return builder.ToString();
        }

        [RequiredByNativeCode, SecuritySafeCritical]
        public static string ExtractStackTrace()
        {
            StackTrace stackTrace = new StackTrace(1, true);
            return ExtractFormattedStackTrace(stackTrace).ToString();
        }

        public static string ExtractStringFromException(object exception)
        {
            string message = string.Empty;
            string stackTrace = string.Empty;
            ExtractStringFromExceptionInternal(exception, out message, out stackTrace);
            return (message + "\n" + stackTrace);
        }

        [SecuritySafeCritical, RequiredByNativeCode]
        internal static void ExtractStringFromExceptionInternal(object exceptiono, out string message, out string stackTrace)
        {
            if (exceptiono == null)
            {
                throw new ArgumentException("ExtractStringFromExceptionInternal called with null exception");
            }
            Exception innerException = exceptiono as Exception;
            if (innerException == null)
            {
                throw new ArgumentException("ExtractStringFromExceptionInternal called with an exceptoin that was not of type System.Exception");
            }
            StringBuilder builder = new StringBuilder((innerException.StackTrace != null) ? (innerException.StackTrace.Length * 2) : 0x200);
            message = string.Empty;
            string str = string.Empty;
            while (innerException != null)
            {
                if (str.Length == 0)
                {
                    str = innerException.StackTrace;
                }
                else
                {
                    str = innerException.StackTrace + "\n" + str;
                }
                string name = innerException.GetType().Name;
                string str3 = string.Empty;
                if (innerException.Message != null)
                {
                    str3 = innerException.Message;
                }
                if (str3.Trim().Length != 0)
                {
                    name = name + ": " + str3;
                }
                message = name;
                if (innerException.InnerException != null)
                {
                    str = "Rethrow as " + name + "\n" + str;
                }
                innerException = innerException.InnerException;
            }
            builder.Append(str + "\n");
            StackTrace trace = new StackTrace(1, true);
            builder.Append(ExtractFormattedStackTrace(trace));
            stackTrace = builder.ToString();
        }

        private static bool IsSystemStacktraceType(object name)
        {
            string str = (string) name;
            return ((((str.StartsWith("UnityEditor.") || str.StartsWith("UnityEngine.")) || (str.StartsWith("System.") || str.StartsWith("UnityScript.Lang."))) || str.StartsWith("Boo.Lang.")) || str.StartsWith("UnityEngine.SetupCoroutine"));
        }

        [RequiredByNativeCode]
        internal static string PostprocessStacktrace(string oldString, bool stripEngineInternalInformation)
        {
            if (oldString == null)
            {
                return string.Empty;
            }
            char[] separator = new char[] { '\n' };
            string[] strArray = oldString.Split(separator);
            StringBuilder builder = new StringBuilder(oldString.Length);
            for (int i = 0; i < strArray.Length; i++)
            {
                strArray[i] = strArray[i].Trim();
            }
            for (int j = 0; j < strArray.Length; j++)
            {
                string name = strArray[j];
                if (((name.Length != 0) && (name[0] != '\n')) && !name.StartsWith("in (unmanaged)"))
                {
                    if (stripEngineInternalInformation && name.StartsWith("UnityEditor.EditorGUIUtility:RenderGameViewCameras"))
                    {
                        break;
                    }
                    if ((stripEngineInternalInformation && (j < (strArray.Length - 1))) && IsSystemStacktraceType(name))
                    {
                        if (IsSystemStacktraceType(strArray[j + 1]))
                        {
                            continue;
                        }
                        int index = name.IndexOf(" (at");
                        if (index != -1)
                        {
                            name = name.Substring(0, index);
                        }
                    }
                    if (((name.IndexOf("(wrapper managed-to-native)") == -1) && (name.IndexOf("(wrapper delegate-invoke)") == -1)) && ((name.IndexOf("at <0x00000> <unknown method>") == -1) && ((!stripEngineInternalInformation || !name.StartsWith("[")) || !name.EndsWith("]"))))
                    {
                        if (name.StartsWith("at "))
                        {
                            name = name.Remove(0, 3);
                        }
                        int startIndex = name.IndexOf("[0x");
                        int num5 = -1;
                        if (startIndex != -1)
                        {
                            num5 = name.IndexOf("]", startIndex);
                        }
                        if ((startIndex != -1) && (num5 > startIndex))
                        {
                            name = name.Remove(startIndex, (num5 - startIndex) + 1);
                        }
                        name = name.Replace("  in <filename unknown>:0", string.Empty).Replace(projectFolder, string.Empty).Replace('\\', '/');
                        int num6 = name.LastIndexOf("  in ");
                        if (num6 != -1)
                        {
                            name = name.Remove(num6, 5).Insert(num6, " (at ");
                            name = name.Insert(name.Length, ")");
                        }
                        builder.Append(name + "\n");
                    }
                }
            }
            return builder.ToString();
        }

        [RequiredByNativeCode]
        internal static void SetProjectFolder(string folder)
        {
            projectFolder = folder;
        }
    }
}

