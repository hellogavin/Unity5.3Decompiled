namespace UnityEngine
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.Text;
    using UnityEngine.Internal;
    using UnityEngine.SceneManagement;
    using UnityEngine.Scripting;

    public sealed class Application
    {
        internal static AdvertisingIdentifierCallback OnAdvertisingIdentifierCallback;
        private static LogCallback s_LogCallbackHandler;
        private static LogCallback s_LogCallbackHandlerThreaded;
        private static volatile LogCallback s_RegisterLogCallbackDeprecated;

        public static  event LogCallback logMessageReceived
        {
            add
            {
                s_LogCallbackHandler = (LogCallback) Delegate.Combine(s_LogCallbackHandler, value);
                SetLogCallbackDefined(true);
            }
            remove
            {
                s_LogCallbackHandler = (LogCallback) Delegate.Remove(s_LogCallbackHandler, value);
            }
        }

        public static  event LogCallback logMessageReceivedThreaded
        {
            add
            {
                s_LogCallbackHandlerThreaded = (LogCallback) Delegate.Combine(s_LogCallbackHandlerThreaded, value);
                SetLogCallbackDefined(true);
            }
            remove
            {
                s_LogCallbackHandlerThreaded = (LogCallback) Delegate.Remove(s_LogCallbackHandlerThreaded, value);
            }
        }

        private static string BuildInvocationForArguments(string functionName, params object[] args)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(functionName);
            builder.Append('(');
            int length = args.Length;
            for (int i = 0; i < length; i++)
            {
                if (i != 0)
                {
                    builder.Append(", ");
                }
                builder.Append(ObjectToJSString(args[i]));
            }
            builder.Append(')');
            builder.Append(';');
            return builder.ToString();
        }

        [RequiredByNativeCode]
        private static void CallLogCallback(string logString, string stackTrace, LogType type, bool invokedOnMainThread)
        {
            if (invokedOnMainThread)
            {
                LogCallback callback = s_LogCallbackHandler;
                if (callback != null)
                {
                    callback(logString, stackTrace, type);
                }
            }
            LogCallback callback2 = s_LogCallbackHandlerThreaded;
            if (callback2 != null)
            {
                callback2(logString, stackTrace, type);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void CancelQuit();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool CanStreamedLevelBeLoaded(int levelIndex);
        public static bool CanStreamedLevelBeLoaded(string levelName)
        {
            return CanStreamedLevelBeLoadedByName(levelName);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool CanStreamedLevelBeLoadedByName(string levelName);
        [ExcludeFromDocs]
        public static void CaptureScreenshot(string filename)
        {
            int superSize = 0;
            CaptureScreenshot(filename, superSize);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void CaptureScreenshot(string filename, [DefaultValue("0")] int superSize);
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("Use Object.DontDestroyOnLoad instead"), WrapperlessIcall]
        public static extern void DontDestroyOnLoad(Object mono);
        public static void ExternalCall(string functionName, params object[] args)
        {
            Internal_ExternalCall(BuildInvocationForArguments(functionName, args));
        }

        public static void ExternalEval(string script)
        {
            if ((script.Length > 0) && (script[script.Length - 1] != ';'))
            {
                script = script + ';';
            }
            Internal_ExternalCall(script);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, Obsolete("For internal use only")]
        public static extern void ForceCrash(int mode);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern int GetBuildUnityVersion();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern int GetNumericUnityVersion(string version);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern float GetStreamProgressForLevel(int levelIndex);
        public static float GetStreamProgressForLevel(string levelName)
        {
            return GetStreamProgressForLevelByName(levelName);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern float GetStreamProgressForLevelByName(string levelName);
        internal static UserAuthorization GetUserAuthorizationRequestMode()
        {
            return (UserAuthorization) GetUserAuthorizationRequestMode_Internal();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int GetUserAuthorizationRequestMode_Internal();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string GetValueForARGV(string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool HasAdvancedLicense();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool HasARGV(string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool HasProLicense();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool HasUserAuthorization(UserAuthorization mode);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_ExternalCall(string script);
        internal static void InvokeOnAdvertisingIdentifierCallback(string advertisingId, bool trackingEnabled)
        {
            if (OnAdvertisingIdentifierCallback != null)
            {
                OnAdvertisingIdentifierCallback(advertisingId, trackingEnabled);
            }
        }

        [Obsolete("Use SceneManager.LoadScene")]
        public static void LoadLevel(int index)
        {
            SceneManager.LoadScene(index, LoadSceneMode.Single);
        }

        [Obsolete("Use SceneManager.LoadScene")]
        public static void LoadLevel(string name)
        {
            SceneManager.LoadScene(name, LoadSceneMode.Single);
        }

        [Obsolete("Use SceneManager.LoadScene")]
        public static void LoadLevelAdditive(int index)
        {
            SceneManager.LoadScene(index, LoadSceneMode.Additive);
        }

        [Obsolete("Use SceneManager.LoadScene")]
        public static void LoadLevelAdditive(string name)
        {
            SceneManager.LoadScene(name, LoadSceneMode.Additive);
        }

        [Obsolete("Use SceneManager.LoadSceneAsync")]
        public static AsyncOperation LoadLevelAdditiveAsync(int index)
        {
            return SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
        }

        [Obsolete("Use SceneManager.LoadSceneAsync")]
        public static AsyncOperation LoadLevelAdditiveAsync(string levelName)
        {
            return SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        }

        [Obsolete("Use SceneManager.LoadSceneAsync")]
        public static AsyncOperation LoadLevelAsync(int index)
        {
            return SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
        }

        [Obsolete("Use SceneManager.LoadSceneAsync")]
        public static AsyncOperation LoadLevelAsync(string levelName)
        {
            return SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
        }

        private static string ObjectToJSString(object o)
        {
            if (o == null)
            {
                return "null";
            }
            if (o is string)
            {
                string str = o.ToString().Replace(@"\", @"\\").Replace("\"", "\\\"").Replace("\n", @"\n").Replace("\r", @"\r").Replace("\0", string.Empty).Replace("\u2028", string.Empty).Replace("\u2029", string.Empty);
                return ('"' + str + '"');
            }
            if (((o is int) || (o is short)) || (((o is uint) || (o is ushort)) || (o is byte)))
            {
                return o.ToString();
            }
            if (o is float)
            {
                NumberFormatInfo numberFormat = CultureInfo.InvariantCulture.NumberFormat;
                float num3 = (float) o;
                return num3.ToString(numberFormat);
            }
            if (o is double)
            {
                NumberFormatInfo provider = CultureInfo.InvariantCulture.NumberFormat;
                double num4 = (double) o;
                return num4.ToString(provider);
            }
            if (o is char)
            {
                if (((char) o) == '"')
                {
                    return "\"\\\"\"";
                }
                return ('"' + o.ToString() + '"');
            }
            if (!(o is IList))
            {
                return ObjectToJSString(o.ToString());
            }
            IList list = (IList) o;
            StringBuilder builder = new StringBuilder();
            builder.Append("new Array(");
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                if (i != 0)
                {
                    builder.Append(", ");
                }
                builder.Append(ObjectToJSString(list[i]));
            }
            builder.Append(")");
            return builder.ToString();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void OpenURL(string url);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Quit();
        [Obsolete("Application.RegisterLogCallback is deprecated. Use Application.logMessageReceived instead.")]
        public static void RegisterLogCallback(LogCallback handler)
        {
            RegisterLogCallback(handler, false);
        }

        private static void RegisterLogCallback(LogCallback handler, bool threaded)
        {
            if (s_RegisterLogCallbackDeprecated != null)
            {
                logMessageReceived -= s_RegisterLogCallbackDeprecated;
                logMessageReceivedThreaded -= s_RegisterLogCallbackDeprecated;
            }
            s_RegisterLogCallbackDeprecated = handler;
            if (handler != null)
            {
                if (threaded)
                {
                    logMessageReceivedThreaded += handler;
                }
                else
                {
                    logMessageReceived += handler;
                }
            }
        }

        [Obsolete("Application.RegisterLogCallbackThreaded is deprecated. Use Application.logMessageReceivedThreaded instead.")]
        public static void RegisterLogCallbackThreaded(LogCallback handler)
        {
            RegisterLogCallback(handler, true);
        }

        [ExcludeFromDocs]
        internal static void ReplyToUserAuthorizationRequest(bool reply)
        {
            bool remember = false;
            ReplyToUserAuthorizationRequest(reply, remember);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void ReplyToUserAuthorizationRequest(bool reply, [DefaultValue("false")] bool remember);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern AsyncOperation RequestUserAuthorization(UserAuthorization mode);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void SetLogCallbackDefined(bool defined);
        [Obsolete("Use SceneManager.UnloadScene")]
        public static bool UnloadLevel(int index)
        {
            return SceneManager.UnloadScene(index);
        }

        [Obsolete("Use SceneManager.UnloadScene")]
        public static bool UnloadLevel(string scenePath)
        {
            return SceneManager.UnloadScene(scenePath);
        }

        [Obsolete("absoluteUrl is deprecated. Please use absoluteURL instead (UnityUpgradable) -> absoluteURL", true)]
        public static string absoluteUrl
        {
            get
            {
                return absoluteURL;
            }
        }

        public static string absoluteURL { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static ThreadPriority backgroundLoadingPriority { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static string bundleIdentifier { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string cloudProjectId { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string companyName { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string dataPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool genuine { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool genuineCheckAvailable { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static ApplicationInstallMode installMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static NetworkReachability internetReachability { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        internal static bool isBatchmode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool isConsolePlatform
        {
            get
            {
                RuntimePlatform platform = Application.platform;
                return ((((platform == RuntimePlatform.PS3) || (platform == RuntimePlatform.PS4)) || (platform == RuntimePlatform.XBOX360)) || (platform == RuntimePlatform.XboxOne));
            }
        }

        public static bool isEditor { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        internal static bool isHumanControllingUs { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        [Obsolete("This property is deprecated, please use LoadLevelAsync to detect if a specific scene is currently loading.")]
        public static bool isLoadingLevel { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool isMobilePlatform
        {
            get
            {
                switch (platform)
                {
                    case RuntimePlatform.IPhonePlayer:
                    case RuntimePlatform.Android:
                    case RuntimePlatform.MetroPlayerX86:
                    case RuntimePlatform.MetroPlayerX64:
                    case RuntimePlatform.MetroPlayerARM:
                    case RuntimePlatform.WP8Player:
                    case RuntimePlatform.BB10Player:
                    case RuntimePlatform.TizenPlayer:
                        return true;
                }
                return false;
            }
        }

        [Obsolete("use Application.isEditor instead")]
        public static bool isPlayer
        {
            get
            {
                return !isEditor;
            }
        }

        public static bool isPlaying { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        internal static bool isRunningUnitTests { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool isShowingSplashScreen { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool isWebPlayer { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        [Obsolete("Use SceneManager.sceneCountInBuildSettings")]
        public static int levelCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        [Obsolete("Use SceneManager to determine what scenes have been loaded")]
        public static int loadedLevel
        {
            get
            {
                return SceneManager.GetActiveScene().buildIndex;
            }
        }

        [Obsolete("Use SceneManager to determine what scenes have been loaded")]
        public static string loadedLevelName
        {
            get
            {
                return SceneManager.GetActiveScene().name;
            }
        }

        [SecurityCritical]
        public static string persistentDataPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static RuntimePlatform platform { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string productName { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool runInBackground { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static ApplicationSandboxType sandboxType { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string srcValue { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static StackTraceLogType stackTraceLogType { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static int streamedBytes { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string streamingAssetsPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        internal static bool submitAnalytics { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static SystemLanguage systemLanguage { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int targetFrameRate { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static string temporaryCachePath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string unityVersion { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string version { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool webSecurityEnabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string webSecurityHostUrl { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public delegate void LogCallback(string condition, string stackTrace, LogType type);
    }
}

