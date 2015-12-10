namespace UnityEditor.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using Unity.DataContract;
    using UnityEditor;
    using UnityEditor.Hardware;
    using UnityEditor.Utils;
    using UnityEditorInternal;
    using UnityEngine;

    internal static class ModuleManager
    {
        [CompilerGenerated]
        private static Func<KeyValuePair<string, PackageFileData>, bool> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<Assembly, bool> <>f__am$cache6;
        [CompilerGenerated]
        private static Func<Assembly, bool> <>f__am$cache7;
        [CompilerGenerated]
        private static Func<Assembly, Type> <>f__am$cache8;
        [CompilerGenerated]
        private static Func<KeyValuePair<string, PackageFileData>, bool> <>f__am$cache9;
        [CompilerGenerated]
        private static Func<KeyValuePair<string, PackageFileData>, string> <>f__am$cacheA;
        [CompilerGenerated]
        private static Func<KeyValuePair<string, PackageFileData>, bool> <>f__am$cacheB;
        [NonSerialized]
        private static IPlatformSupportModule s_ActivePlatformModule;
        [NonSerialized]
        private static List<IEditorModule> s_EditorModules;
        [NonSerialized]
        private static IPackageManagerModule s_PackageManager;
        [NonSerialized]
        private static List<IPlatformSupportModule> s_PlatformModules;
        [NonSerialized]
        private static bool s_PlatformModulesInitialized;

        static ModuleManager()
        {
            EditorUserBuildSettings.activeBuildTargetChanged = (Action) Delegate.Combine(EditorUserBuildSettings.activeBuildTargetChanged, new Action(ModuleManager.OnActiveBuildTargetChanged));
        }

        private static void ChangeActivePlatformModuleTo(string target)
        {
            DeactivateActivePlatformModule();
            IEnumerator<IPlatformSupportModule> enumerator = platformSupportModules.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    IPlatformSupportModule current = enumerator.Current;
                    if (current.TargetName == target)
                    {
                        s_ActivePlatformModule = current;
                        current.OnActivate();
                        return;
                    }
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
        }

        private static string CombinePaths(params string[] paths)
        {
            if (paths == null)
            {
                throw new ArgumentNullException("paths");
            }
            if (paths.Length == 1)
            {
                return paths[0];
            }
            StringBuilder builder = new StringBuilder(paths[0]);
            for (int i = 1; i < paths.Length; i++)
            {
                builder.AppendFormat("{0}{1}", "/", paths[i]);
            }
            return builder.ToString();
        }

        private static void DeactivateActivePlatformModule()
        {
            if (s_ActivePlatformModule != null)
            {
                s_ActivePlatformModule.OnDeactivate();
                s_ActivePlatformModule = null;
            }
        }

        private static IPlatformSupportModule FindPlatformSupportModule(string moduleName)
        {
            IEnumerator<IPlatformSupportModule> enumerator = platformSupportModules.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    IPlatformSupportModule current = enumerator.Current;
                    if (current.TargetName == moduleName)
                    {
                        return current;
                    }
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            return null;
        }

        internal static IBuildPostprocessor GetBuildPostProcessor(string target)
        {
            if (target != null)
            {
                IEnumerator<IPlatformSupportModule> enumerator = platformSupportModules.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        IPlatformSupportModule current = enumerator.Current;
                        if (current.TargetName == target)
                        {
                            return current.CreateBuildPostprocessor();
                        }
                    }
                }
                finally
                {
                    if (enumerator == null)
                    {
                    }
                    enumerator.Dispose();
                }
            }
            return null;
        }

        internal static IBuildPostprocessor GetBuildPostProcessor(BuildTarget target)
        {
            return GetBuildPostProcessor(GetTargetStringFromBuildTarget(target));
        }

        internal static IBuildWindowExtension GetBuildWindowExtension(string target)
        {
            if (!string.IsNullOrEmpty(target))
            {
                IEnumerator<IPlatformSupportModule> enumerator = platformSupportModules.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        IPlatformSupportModule current = enumerator.Current;
                        if (current.TargetName == target)
                        {
                            return current.CreateBuildWindowExtension();
                        }
                    }
                }
                finally
                {
                    if (enumerator == null)
                    {
                    }
                    enumerator.Dispose();
                }
            }
            return null;
        }

        internal static ICompilationExtension GetCompilationExtension(string target)
        {
            IEnumerator<IPlatformSupportModule> enumerator = platformSupportModules.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    IPlatformSupportModule current = enumerator.Current;
                    if (current.TargetName == target)
                    {
                        return current.CreateCompilationExtension();
                    }
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            return new DefaultCompilationExtension();
        }

        internal static IDevice GetDevice(string deviceId)
        {
            DevDevice device;
            if (!DevDeviceList.FindDevice(deviceId, out device))
            {
                throw new ApplicationException("Couldn't create device API for device: " + deviceId);
            }
            IPlatformSupportModule module = FindPlatformSupportModule(device.module);
            if (module == null)
            {
                throw new ApplicationException("Couldn't find module for target: " + device.module);
            }
            return module.CreateDevice(deviceId);
        }

        internal static GUIContent[] GetDisplayNames(string target)
        {
            if (!string.IsNullOrEmpty(target))
            {
                IEnumerator<IPlatformSupportModule> enumerator = platformSupportModules.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        IPlatformSupportModule current = enumerator.Current;
                        if (current.TargetName == target)
                        {
                            return current.GetDisplayNames();
                        }
                    }
                }
                finally
                {
                    if (enumerator == null)
                    {
                    }
                    enumerator.Dispose();
                }
            }
            return null;
        }

        internal static ISettingEditorExtension GetEditorSettingsExtension(string target)
        {
            if (!string.IsNullOrEmpty(target))
            {
                IEnumerator<IPlatformSupportModule> enumerator = platformSupportModules.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        IPlatformSupportModule current = enumerator.Current;
                        if (current.TargetName == target)
                        {
                            return current.CreateSettingsEditorExtension();
                        }
                    }
                }
                finally
                {
                    if (enumerator == null)
                    {
                    }
                    enumerator.Dispose();
                }
            }
            return null;
        }

        internal static string GetExtensionVersion(string target)
        {
            if (!string.IsNullOrEmpty(target))
            {
                IEnumerator<IPlatformSupportModule> enumerator = platformSupportModules.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        IPlatformSupportModule current = enumerator.Current;
                        if (current.TargetName == target)
                        {
                            return current.ExtensionVersion;
                        }
                    }
                }
                finally
                {
                    if (enumerator == null)
                    {
                    }
                    enumerator.Dispose();
                }
            }
            return null;
        }

        internal static List<string> GetJamTargets()
        {
            List<string> list = new List<string>();
            IEnumerator<IPlatformSupportModule> enumerator = platformSupportModules.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    IPlatformSupportModule current = enumerator.Current;
                    list.Add(current.JamTarget);
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            return list;
        }

        internal static IPluginImporterExtension GetPluginImporterExtension(string target)
        {
            if (target != null)
            {
                IEnumerator<IPlatformSupportModule> enumerator = platformSupportModules.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        IPlatformSupportModule current = enumerator.Current;
                        if (current.TargetName == target)
                        {
                            return current.CreatePluginImporterExtension();
                        }
                    }
                }
                finally
                {
                    if (enumerator == null)
                    {
                    }
                    enumerator.Dispose();
                }
            }
            return null;
        }

        internal static IPluginImporterExtension GetPluginImporterExtension(BuildTarget target)
        {
            return GetPluginImporterExtension(GetTargetStringFromBuildTarget(target));
        }

        internal static IPluginImporterExtension GetPluginImporterExtension(BuildTargetGroup target)
        {
            return GetPluginImporterExtension(GetTargetStringFromBuildTargetGroup(target));
        }

        internal static List<IPreferenceWindowExtension> GetPreferenceWindowExtensions()
        {
            List<IPreferenceWindowExtension> list = new List<IPreferenceWindowExtension>();
            IEnumerator<IPlatformSupportModule> enumerator = platformSupportModules.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    IPreferenceWindowExtension item = enumerator.Current.CreatePreferenceWindowExtension();
                    if (item != null)
                    {
                        list.Add(item);
                    }
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            return list;
        }

        private static IScriptingImplementations GetScriptingImplementations(string target)
        {
            if (!string.IsNullOrEmpty(target))
            {
                IEnumerator<IPlatformSupportModule> enumerator = platformSupportModules.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        IPlatformSupportModule current = enumerator.Current;
                        if (current.TargetName == target)
                        {
                            return current.CreateScriptingImplementations();
                        }
                    }
                }
                finally
                {
                    if (enumerator == null)
                    {
                    }
                    enumerator.Dispose();
                }
            }
            return null;
        }

        internal static IScriptingImplementations GetScriptingImplementations(BuildTargetGroup target)
        {
            if (target == BuildTargetGroup.Standalone)
            {
                return new DesktopStandalonePostProcessor.ScriptingImplementations();
            }
            return GetScriptingImplementations(GetTargetStringFromBuildTargetGroup(target));
        }

        internal static string GetTargetStringFromBuildTarget(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.StandaloneOSXUniversal:
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                    return "OSXStandalone";

                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneGLESEmu:
                case BuildTarget.StandaloneWindows64:
                    return "WindowsStandalone";

                case BuildTarget.iOS:
                    return "iOS";

                case BuildTarget.PS3:
                    return "PS3";

                case BuildTarget.XBOX360:
                    return "Xbox360";

                case BuildTarget.Android:
                    return "Android";

                case BuildTarget.StandaloneLinux:
                case BuildTarget.StandaloneLinux64:
                case BuildTarget.StandaloneLinuxUniversal:
                    return "LinuxStandalone";

                case BuildTarget.WebGL:
                    return "WebGL";

                case BuildTarget.WSAPlayer:
                    return "Metro";

                case BuildTarget.WP8Player:
                    return "WP8";

                case BuildTarget.BlackBerry:
                    return "BlackBerry";

                case BuildTarget.Tizen:
                    return "Tizen";

                case BuildTarget.PSP2:
                    return "PSP2";

                case BuildTarget.PS4:
                    return "PS4";

                case BuildTarget.PSM:
                    return "PSM";

                case BuildTarget.XboxOne:
                    return "XboxOne";

                case BuildTarget.SamsungTV:
                    return "SamsungTV";

                case BuildTarget.Nintendo3DS:
                    return "N3DS";

                case BuildTarget.WiiU:
                    return "WiiU";

                case BuildTarget.tvOS:
                    return "tvOS";
            }
            return null;
        }

        internal static string GetTargetStringFromBuildTargetGroup(BuildTargetGroup target)
        {
            switch (target)
            {
                case BuildTargetGroup.iPhone:
                    return "iOS";

                case BuildTargetGroup.PS3:
                    return "PS3";

                case BuildTargetGroup.XBOX360:
                    return "Xbox360";

                case BuildTargetGroup.Android:
                    return "Android";

                case BuildTargetGroup.WebGL:
                    return "WebGL";

                case BuildTargetGroup.Metro:
                    return "Metro";

                case BuildTargetGroup.WP8:
                    return "WP8";

                case BuildTargetGroup.BlackBerry:
                    return "BlackBerry";

                case BuildTargetGroup.Tizen:
                    return "Tizen";

                case BuildTargetGroup.PSP2:
                    return "PSP2";

                case BuildTargetGroup.PS4:
                    return "PS4";

                case BuildTargetGroup.PSM:
                    return "PSM";

                case BuildTargetGroup.XboxOne:
                    return "XboxOne";

                case BuildTargetGroup.SamsungTV:
                    return "SamsungTV";

                case BuildTargetGroup.Nintendo3DS:
                    return "N3DS";

                case BuildTargetGroup.WiiU:
                    return "WiiU";

                case BuildTargetGroup.tvOS:
                    return "tvOS";
            }
            return null;
        }

        internal static IUserAssembliesValidator GetUserAssembliesValidator(string target)
        {
            if (target != null)
            {
                IEnumerator<IPlatformSupportModule> enumerator = platformSupportModules.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        IPlatformSupportModule current = enumerator.Current;
                        if (current.TargetName == target)
                        {
                            return current.CreateUserAssembliesValidatorExtension();
                        }
                    }
                }
                finally
                {
                    if (enumerator == null)
                    {
                    }
                    enumerator.Dispose();
                }
            }
            return null;
        }

        internal static bool HaveLicenseForBuildTarget(string targetString)
        {
            BuildTarget standaloneWindows = BuildTarget.StandaloneWindows;
            if (!TryParseBuildTarget(targetString, out standaloneWindows))
            {
                return false;
            }
            return BuildPipeline.LicenseCheck(standaloneWindows);
        }

        internal static void Initialize()
        {
            if (s_PackageManager == null)
            {
                RegisterPackageManager();
                if (s_PackageManager != null)
                {
                    LoadUnityExtensions();
                }
                else
                {
                    Debug.LogError("Failed to load package manager");
                }
            }
        }

        private static bool InitializePackageManager(PackageInfo package)
        {
            if (<>f__am$cache9 == null)
            {
                <>f__am$cache9 = x => x.Value.type == PackageFileType.Dll;
            }
            if (<>f__am$cacheA == null)
            {
                <>f__am$cacheA = x => x.Key;
            }
            string str = package.files.Where<KeyValuePair<string, PackageFileData>>(<>f__am$cache9).Select<KeyValuePair<string, PackageFileData>, string>(<>f__am$cacheA).FirstOrDefault<string>();
            if ((str == null) || !File.Exists(Path.Combine(package.basePath, str)))
            {
                return false;
            }
            InternalEditorUtility.SetPlatformPath(package.basePath);
            return InitializePackageManager(InternalEditorUtility.LoadAssemblyWrapper(Path.GetFileName(str), Path.Combine(package.basePath, str)), package);
        }

        private static bool InitializePackageManager(Assembly assembly, PackageInfo package)
        {
            s_PackageManager = AssemblyHelper.FindImplementors<IPackageManagerModule>(assembly).FirstOrDefault<IPackageManagerModule>();
            if (s_PackageManager == null)
            {
                return false;
            }
            string location = assembly.Location;
            if (package != null)
            {
                InternalEditorUtility.SetupCustomDll(Path.GetFileName(location), location);
            }
            else
            {
                PackageInfo info2 = new PackageInfo {
                    basePath = Path.GetDirectoryName(location)
                };
                package = info2;
            }
            s_PackageManager.moduleInfo = package;
            s_PackageManager.editorInstallPath = EditorApplication.applicationContentsPath;
            s_PackageManager.unityVersion = (string) new PackageVersion(Application.unityVersion);
            s_PackageManager.Initialize();
            IEnumerator<PackageInfo> enumerator = s_PackageManager.playbackEngines.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    PackageInfo current = enumerator.Current;
                    BuildTarget standaloneWindows = BuildTarget.StandaloneWindows;
                    if (TryParseBuildTarget(current.name, out standaloneWindows))
                    {
                        object[] arg = new object[] { standaloneWindows, current.version, current.unityVersion, current.basePath };
                        Console.WriteLine("Setting {0} v{1} for Unity v{2} to {3}", arg);
                        if (<>f__am$cacheB == null)
                        {
                            <>f__am$cacheB = f => f.Value.type == PackageFileType.Dll;
                        }
                        IEnumerator<KeyValuePair<string, PackageFileData>> enumerator2 = current.files.Where<KeyValuePair<string, PackageFileData>>(<>f__am$cacheB).GetEnumerator();
                        try
                        {
                            while (enumerator2.MoveNext())
                            {
                                KeyValuePair<string, PackageFileData> pair = enumerator2.Current;
                                if (!File.Exists(Path.Combine(current.basePath, pair.Key).NormalizePath()))
                                {
                                    object[] args = new object[] { current.basePath, current.name };
                                    Debug.LogWarningFormat("Missing assembly \t{0} for {1}. Player support may be incomplete.", args);
                                }
                                else
                                {
                                    InternalEditorUtility.SetupCustomDll(Path.GetFileName(location), location);
                                }
                            }
                        }
                        finally
                        {
                            if (enumerator2 == null)
                            {
                            }
                            enumerator2.Dispose();
                        }
                        BuildPipeline.SetPlaybackEngineDirectory(standaloneWindows, BuildOptions.CompressTextures, current.basePath);
                        InternalEditorUtility.SetPlatformPath(current.basePath);
                        s_PackageManager.LoadPackage(current);
                    }
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            return true;
        }

        internal static void InitializePlatformSupportModules()
        {
            if (s_PlatformModulesInitialized)
            {
                Console.WriteLine("Platform modules already initialized, skipping");
            }
            else
            {
                Initialize();
                RegisterPlatformSupportModules();
                IEnumerator<IPlatformSupportModule> enumerator = platformSupportModules.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        IPlatformSupportModule current = enumerator.Current;
                        EditorUtility.LoadPlatformSupportModuleNativeDllInternal(current.TargetName);
                        current.OnLoad();
                    }
                }
                finally
                {
                    if (enumerator == null)
                    {
                    }
                    enumerator.Dispose();
                }
                OnActiveBuildTargetChanged();
                s_PlatformModulesInitialized = true;
            }
        }

        internal static bool IsPlatformSupported(BuildTarget target)
        {
            return (GetTargetStringFromBuildTarget(target) != null);
        }

        internal static bool IsPlatformSupportLoaded(string target)
        {
            IEnumerator<IPlatformSupportModule> enumerator = platformSupportModules.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    IPlatformSupportModule current = enumerator.Current;
                    if (current.TargetName == target)
                    {
                        return true;
                    }
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            return false;
        }

        internal static bool IsRegisteredModule(string file)
        {
            return ((s_PackageManager != null) && (s_PackageManager.GetType().Assembly.Location.NormalizePath() == file.NormalizePath()));
        }

        private static void LoadUnityExtensions()
        {
            IEnumerator<PackageInfo> enumerator = s_PackageManager.unityExtensions.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    PackageInfo current = enumerator.Current;
                    object[] arg = new object[] { current.name, current.version, current.unityVersion, current.basePath };
                    Console.WriteLine("Setting {0} v{1} for Unity v{2} to {3}", arg);
                    if (<>f__am$cache5 == null)
                    {
                        <>f__am$cache5 = f => f.Value.type == PackageFileType.Dll;
                    }
                    IEnumerator<KeyValuePair<string, PackageFileData>> enumerator2 = current.files.Where<KeyValuePair<string, PackageFileData>>(<>f__am$cache5).GetEnumerator();
                    try
                    {
                        while (enumerator2.MoveNext())
                        {
                            KeyValuePair<string, PackageFileData> pair = enumerator2.Current;
                            string path = Path.Combine(current.basePath, pair.Key).NormalizePath();
                            if (!File.Exists(path))
                            {
                                object[] args = new object[] { pair.Key, current.name };
                                Debug.LogWarningFormat("Missing assembly \t{0} for {1}. Extension support may be incomplete.", args);
                            }
                            else
                            {
                                bool flag = !string.IsNullOrEmpty(pair.Value.guid);
                                Console.WriteLine("  {0} ({1}) GUID: {2}", pair.Key, !flag ? "Custom" : "Extension", pair.Value.guid);
                                if (flag)
                                {
                                    InternalEditorUtility.RegisterExtensionDll(path.Replace('\\', '/'), pair.Value.guid);
                                    continue;
                                }
                                InternalEditorUtility.SetupCustomDll(Path.GetFileName(path), path);
                            }
                        }
                    }
                    finally
                    {
                        if (enumerator2 == null)
                        {
                        }
                        enumerator2.Dispose();
                    }
                    s_PackageManager.LoadPackage(current);
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
        }

        private static void OnActiveBuildTargetChanged()
        {
            ChangeActivePlatformModuleTo(GetTargetStringFromBuildTarget(EditorUserBuildSettings.activeBuildTarget));
        }

        private static IEnumerable<IEditorModule> RegisterEditorModulesFromAssembly(Assembly assembly)
        {
            return AssemblyHelper.FindImplementors<IEditorModule>(assembly);
        }

        private static IEnumerable<T> RegisterModulesFromLoadedAssemblies<T>(Func<Assembly, IEnumerable<T>> processAssembly)
        {
            <RegisterModulesFromLoadedAssemblies>c__AnonStorey9D<T> storeyd = new <RegisterModulesFromLoadedAssemblies>c__AnonStorey9D<T> {
                processAssembly = processAssembly
            };
            if (storeyd.processAssembly == null)
            {
                throw new ArgumentNullException("processAssembly");
            }
            return AppDomain.CurrentDomain.GetAssemblies().Aggregate<Assembly, List<T>>(new List<T>(), new Func<List<T>, Assembly, List<T>>(storeyd.<>m__1D7));
        }

        private static void RegisterPackageManager()
        {
            PackageInfo info;
            s_EditorModules = new List<IEditorModule>();
            try
            {
                if (<>f__am$cache6 == null)
                {
                    <>f__am$cache6 = a => null != a.GetType("Unity.PackageManager.PackageManager");
                }
                Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault<Assembly>(<>f__am$cache6);
                if ((assembly != null) && InitializePackageManager(assembly, null))
                {
                    return;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error enumerating assemblies looking for package manager. {0}", exception);
            }
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = a => a.GetName().Name == "Unity.Locator";
            }
            if (<>f__am$cache8 == null)
            {
                <>f__am$cache8 = a => a.GetType("Unity.PackageManager.Locator");
            }
            Type type = AppDomain.CurrentDomain.GetAssemblies().Where<Assembly>(<>f__am$cache7).Select<Assembly, Type>(<>f__am$cache8).FirstOrDefault<Type>();
            try
            {
                object[] args = new object[2];
                string[] textArray1 = new string[] { FileUtil.NiceWinPath(EditorApplication.applicationContentsPath) };
                args[0] = textArray1;
                args[1] = Application.unityVersion;
                type.InvokeMember("Scan", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, args);
            }
            catch (Exception exception2)
            {
                Console.WriteLine("Error scanning for packages. {0}", exception2);
                return;
            }
            try
            {
                string[] textArray2 = new string[] { Application.unityVersion };
                info = type.InvokeMember("GetPackageManager", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, textArray2) as PackageInfo;
                if (info == null)
                {
                    Console.WriteLine("No package manager found!");
                    return;
                }
            }
            catch (Exception exception3)
            {
                Console.WriteLine("Error scanning for packages. {0}", exception3);
                return;
            }
            try
            {
                InitializePackageManager(info);
            }
            catch (Exception exception4)
            {
                Console.WriteLine("Error initializing package manager. {0}", exception4);
            }
            if (s_PackageManager != null)
            {
                s_PackageManager.CheckForUpdates();
            }
        }

        private static void RegisterPlatformSupportModules()
        {
            if (s_PlatformModules != null)
            {
                Console.WriteLine("Modules already registered, not loading");
            }
            else
            {
                Console.WriteLine("Registering platform support modules:");
                Stopwatch stopwatch = Stopwatch.StartNew();
                s_PlatformModules = RegisterModulesFromLoadedAssemblies<IPlatformSupportModule>(new Func<Assembly, IEnumerable<IPlatformSupportModule>>(ModuleManager.RegisterPlatformSupportModulesFromAssembly)).ToList<IPlatformSupportModule>();
                stopwatch.Stop();
                Console.WriteLine("Registered platform support modules in: " + stopwatch.Elapsed.TotalSeconds + "s.");
            }
        }

        internal static IEnumerable<IPlatformSupportModule> RegisterPlatformSupportModulesFromAssembly(Assembly assembly)
        {
            return AssemblyHelper.FindImplementors<IPlatformSupportModule>(assembly);
        }

        public static string RemapDllLocation(string dllLocation)
        {
            string fileName = Path.GetFileName(dllLocation);
            string directoryName = Path.GetDirectoryName(dllLocation);
            string[] paths = new string[] { directoryName, "Standalone", fileName };
            string path = CombinePaths(paths);
            if (File.Exists(path))
            {
                return path;
            }
            return dllLocation;
        }

        internal static void Shutdown()
        {
            if (s_PackageManager != null)
            {
                s_PackageManager.Shutdown(true);
            }
            s_PackageManager = null;
            s_PlatformModules = null;
            s_EditorModules = null;
        }

        internal static void ShutdownPlatformSupportModules()
        {
            DeactivateActivePlatformModule();
            foreach (IPlatformSupportModule module in s_PlatformModules)
            {
                module.OnUnload();
            }
        }

        private static bool TryParseBuildTarget(string targetString, out BuildTarget target)
        {
            target = BuildTarget.StandaloneWindows;
            try
            {
                target = (BuildTarget) ((int) Enum.Parse(typeof(BuildTarget), targetString));
                return true;
            }
            catch
            {
                Debug.LogWarning(string.Format("Couldn't find build target for {0}", targetString));
            }
            return false;
        }

        private static List<IEditorModule> editorModules
        {
            get
            {
                if (s_EditorModules == null)
                {
                    return new List<IEditorModule>();
                }
                return s_EditorModules;
            }
        }

        internal static IPackageManagerModule packageManager
        {
            get
            {
                Initialize();
                return s_PackageManager;
            }
        }

        internal static IEnumerable<IPlatformSupportModule> platformSupportModules
        {
            get
            {
                Initialize();
                if (s_PlatformModules == null)
                {
                    RegisterPlatformSupportModules();
                }
                return s_PlatformModules;
            }
        }

        [CompilerGenerated]
        private sealed class <RegisterModulesFromLoadedAssemblies>c__AnonStorey9D<T>
        {
            internal Func<Assembly, IEnumerable<T>> processAssembly;

            internal List<T> <>m__1D7(List<T> list, Assembly assembly)
            {
                try
                {
                    IEnumerable<T> source = this.processAssembly(assembly);
                    if ((source != null) && source.Any<T>())
                    {
                        list.AddRange(source);
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Error while registering modules from " + assembly.FullName + ": " + exception.Message);
                }
                return list;
            }
        }
    }
}

