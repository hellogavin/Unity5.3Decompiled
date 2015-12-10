namespace UnityEditor
{
    using Microsoft.Win32;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor.Utils;
    using UnityEditor.VisualStudioIntegration;
    using UnityEditorInternal;

    [InitializeOnLoad]
    internal class SyncVS : AssetPostprocessor
    {
        [CompilerGenerated]
        private static Func<KeyValuePair<VisualStudioVersion, string>, VisualStudioVersion> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<KeyValuePair<VisualStudioVersion, string>, string> <>f__am$cache4;
        private static bool s_AlreadySyncedThisDomainReload;
        private static readonly SolutionSynchronizer Synchronizer = new SolutionSynchronizer(Directory.GetParent(Application.dataPath).FullName, new SolutionSynchronizationSettings());

        static SyncVS()
        {
            EditorUserBuildSettings.activeBuildTargetChanged = (Action) Delegate.Combine(EditorUserBuildSettings.activeBuildTargetChanged, new Action(SyncVS.SyncVisualStudioProjectIfItAlreadyExists));
            try
            {
                InstalledVisualStudios = GetInstalledVisualStudios() as Dictionary<VisualStudioVersion, string>;
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error detecting Visual Studio installations: {0}{1}{2}", exception.Message, Environment.NewLine, exception.StackTrace);
                InstalledVisualStudios = new Dictionary<VisualStudioVersion, string>();
            }
            SetVisualStudioAsEditorIfNoEditorWasSet();
            UnityVSSupport.Initialize();
        }

        internal static bool CheckVisualStudioVersion(int major, int minor, int build)
        {
            int num = -1;
            int num2 = -1;
            if (major != 11)
            {
                return false;
            }
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\DevDiv\vc\Servicing");
            if (key == null)
            {
                return false;
            }
            foreach (string str in key.GetSubKeyNames())
            {
                if (str.StartsWith("11.") && (str.Length > 3))
                {
                    try
                    {
                        int num4 = Convert.ToInt32(str.Substring(3));
                        if (num4 > num)
                        {
                            num = num4;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            if (num < 0)
            {
                return false;
            }
            RegistryKey key2 = key.OpenSubKey(string.Format(@"11.{0}\RuntimeDebug", num));
            if (key2 == null)
            {
                return false;
            }
            string str2 = key2.GetValue("Version", null) as string;
            if (str2 == null)
            {
                return false;
            }
            char[] separator = new char[] { '.' };
            string[] strArray2 = str2.Split(separator);
            if ((strArray2 == null) || (strArray2.Length < 3))
            {
                return false;
            }
            try
            {
                num2 = Convert.ToInt32(strArray2[2]);
            }
            catch (Exception)
            {
                return false;
            }
            return ((num > minor) || ((num == minor) && (num2 >= build)));
        }

        public static void CreateIfDoesntExist()
        {
            if (!Synchronizer.SolutionExists())
            {
                Synchronizer.Sync();
            }
        }

        private static string DeriveProgramFilesSentinel()
        {
            char[] separator = new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
            string str = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles).Split(separator).LastOrDefault<string>();
            if (string.IsNullOrEmpty(str))
            {
                return "Program Files";
            }
            int startIndex = str.LastIndexOf("(x86)");
            if (0 <= startIndex)
            {
                str = str.Remove(startIndex);
            }
            return str.TrimEnd(new char[0]);
        }

        private static string DeriveVisualStudioPath(string debuggerPath)
        {
            string a = DeriveProgramFilesSentinel();
            string str2 = "Common7";
            bool flag = false;
            char[] separator = new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
            string[] strArray = debuggerPath.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            foreach (string str4 in strArray)
            {
                if (!flag && string.Equals(a, str4, StringComparison.OrdinalIgnoreCase))
                {
                    flag = true;
                }
                else if (flag)
                {
                    folderPath = Path.Combine(folderPath, str4);
                    if (string.Equals(str2, str4, StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
            string[] components = new string[] { folderPath, "IDE", "devenv.exe" };
            return Paths.Combine(components);
        }

        public static string FindBestVisualStudio()
        {
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = kvp => kvp.Key;
            }
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = kvp2 => kvp2.Value;
            }
            return InstalledVisualStudios.OrderByDescending<KeyValuePair<VisualStudioVersion, string>, VisualStudioVersion>(<>f__am$cache3).Select<KeyValuePair<VisualStudioVersion, string>, string>(<>f__am$cache4).FirstOrDefault<string>();
        }

        private static IDictionary<VisualStudioVersion, string> GetInstalledVisualStudios()
        {
            Dictionary<VisualStudioVersion, string> dictionary = new Dictionary<VisualStudioVersion, string>();
            if (SolutionSynchronizationSettings.IsWindows)
            {
                IEnumerator enumerator = Enum.GetValues(typeof(VisualStudioVersion)).GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        VisualStudioVersion current = (VisualStudioVersion) ((int) enumerator.Current);
                        try
                        {
                            string environmentVariable = Environment.GetEnvironmentVariable(string.Format("VS{0}0COMNTOOLS", (int) current));
                            if (!string.IsNullOrEmpty(environmentVariable))
                            {
                                string[] components = new string[] { environmentVariable, "..", "IDE", "devenv.exe" };
                                string path = Paths.Combine(components);
                                if (File.Exists(path))
                                {
                                    dictionary[current] = path;
                                    continue;
                                }
                            }
                            environmentVariable = Registry.GetValue(string.Format(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\{0}.0\Debugger", (int) current), "FEQARuntimeImplDll", null) as string;
                            if (!string.IsNullOrEmpty(environmentVariable))
                            {
                                string str3 = DeriveVisualStudioPath(environmentVariable);
                                if (!string.IsNullOrEmpty(str3) && File.Exists(str3))
                                {
                                    dictionary[current] = DeriveVisualStudioPath(environmentVariable);
                                }
                            }
                            continue;
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable == null)
                    {
                    }
                    disposable.Dispose();
                }
            }
            return dictionary;
        }

        private static void OpenProjectFileUnlessInBatchMode()
        {
            if (!InternalEditorUtility.inBatchMode)
            {
                InternalEditorUtility.OpenFileAtLineExternal(string.Empty, -1);
            }
        }

        private static bool PathsAreEquivalent(string aPath, string zPath)
        {
            if ((aPath == null) && (zPath == null))
            {
                return true;
            }
            if (string.IsNullOrEmpty(aPath) || string.IsNullOrEmpty(zPath))
            {
                return false;
            }
            aPath = Path.GetFullPath(aPath);
            zPath = Path.GetFullPath(zPath);
            StringComparison ordinalIgnoreCase = StringComparison.OrdinalIgnoreCase;
            if (!SolutionSynchronizationSettings.IsOSX && !SolutionSynchronizationSettings.IsWindows)
            {
                ordinalIgnoreCase = StringComparison.Ordinal;
            }
            aPath = aPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            zPath = zPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            return string.Equals(aPath, zPath, ordinalIgnoreCase);
        }

        public static void PostprocessSyncProject(string[] importedAssets, string[] addedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            Synchronizer.SyncIfNeeded(addedAssets.Union<string>(deletedAssets.Union<string>(movedAssets.Union<string>(movedFromAssetPaths))));
        }

        public static bool ProjectExists()
        {
            return Synchronizer.SolutionExists();
        }

        private static void SetVisualStudioAsEditorIfNoEditorWasSet()
        {
            string str = EditorPrefs.GetString("kScriptsDefaultApp");
            string str2 = FindBestVisualStudio();
            if ((str == string.Empty) && (str2 != null))
            {
                EditorPrefs.SetString("kScriptsDefaultApp", str2);
            }
        }

        [MenuItem("Assets/Open C# Project")]
        private static void SyncAndOpenSolution()
        {
            SyncSolution();
            OpenProjectFileUnlessInBatchMode();
        }

        public static void SyncIfFirstFileOpenSinceDomainLoad()
        {
            if (!s_AlreadySyncedThisDomainReload)
            {
                s_AlreadySyncedThisDomainReload = true;
                Synchronizer.Sync();
            }
        }

        public static void SyncSolution()
        {
            AssetDatabase.Refresh();
            Synchronizer.Sync();
        }

        public static void SyncVisualStudioProjectIfItAlreadyExists()
        {
            if (Synchronizer.SolutionExists())
            {
                Synchronizer.Sync();
            }
        }

        internal static Dictionary<VisualStudioVersion, string> InstalledVisualStudios
        {
            [CompilerGenerated]
            get
            {
                return <InstalledVisualStudios>k__BackingField;
            }
            [CompilerGenerated]
            private set
            {
                <InstalledVisualStudios>k__BackingField = value;
            }
        }

        private class SolutionSynchronizationSettings : DefaultSolutionSynchronizationSettings
        {
            protected override string FrameworksPath()
            {
                string applicationContentsPath = EditorApplication.applicationContentsPath;
                return (!IsOSX ? applicationContentsPath : (applicationContentsPath + "/Frameworks"));
            }

            public override string GetProjectFooterTemplate(ScriptingLanguage language)
            {
                return EditorPrefs.GetString("VSProjectFooter", base.GetProjectFooterTemplate(language));
            }

            public override string GetProjectHeaderTemplate(ScriptingLanguage language)
            {
                return EditorPrefs.GetString("VSProjectHeader", base.GetProjectHeaderTemplate(language));
            }

            public override string[] Defines
            {
                get
                {
                    return EditorUserBuildSettings.activeScriptCompilationDefines;
                }
            }

            public override string EditorAssemblyPath
            {
                get
                {
                    return InternalEditorUtility.GetEditorAssemblyPath();
                }
            }

            public override string EngineAssemblyPath
            {
                get
                {
                    return InternalEditorUtility.GetEngineAssemblyPath();
                }
            }

            internal static bool IsOSX
            {
                get
                {
                    return (Environment.OSVersion.Platform == PlatformID.Unix);
                }
            }

            internal static bool IsWindows
            {
                get
                {
                    return ((!IsOSX && (Path.DirectorySeparatorChar == '\\')) && (Environment.NewLine == "\r\n"));
                }
            }

            public override string SolutionTemplate
            {
                get
                {
                    return EditorPrefs.GetString("VSSolutionText", base.SolutionTemplate);
                }
            }

            public override int VisualStudioVersion
            {
                get
                {
                    string externalScriptEditor = InternalEditorUtility.GetExternalScriptEditor();
                    if ((SyncVS.InstalledVisualStudios.ContainsKey(UnityEditor.VisualStudioVersion.VisualStudio2008) && (externalScriptEditor != string.Empty)) && SyncVS.PathsAreEquivalent(SyncVS.InstalledVisualStudios[UnityEditor.VisualStudioVersion.VisualStudio2008], externalScriptEditor))
                    {
                        return 9;
                    }
                    return 10;
                }
            }
        }
    }
}

