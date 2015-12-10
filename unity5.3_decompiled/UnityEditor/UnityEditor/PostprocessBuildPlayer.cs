namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using UnityEditor.Modules;
    using UnityEditor.Utils;
    using UnityEngine;

    internal class PostprocessBuildPlayer
    {
        internal const string StreamingAssets = "Assets/StreamingAssets";

        private static void AddPluginSubdirIfExists(List<string> subdirs, string basedir, string subdir)
        {
            if (Directory.Exists(Path.Combine(basedir, subdir)))
            {
                subdirs.Add(subdir);
            }
            else
            {
                subdirs.Add(string.Empty);
            }
        }

        internal static string ExecuteSystemProcess(string command, string args, string workingdir)
        {
            ProcessStartInfo si = new ProcessStartInfo {
                FileName = command,
                Arguments = args,
                WorkingDirectory = workingdir,
                CreateNoWindow = true
            };
            Program program = new Program(si);
            program.Start();
            while (!program.WaitForExit(100))
            {
            }
            string standardOutputAsString = program.GetStandardOutputAsString();
            program.Dispose();
            return standardOutputAsString;
        }

        internal static string GenerateBundleIdentifier(string companyName, string productName)
        {
            return ("unity." + companyName + "." + productName);
        }

        internal static string GetArchitectureForTarget(BuildTarget target)
        {
            BuildTarget target2 = target;
            switch (target2)
            {
                case BuildTarget.StandaloneLinux:
                    goto Label_0040;

                case BuildTarget.StandaloneWindows64:
                    break;

                default:
                    switch (target2)
                    {
                        case BuildTarget.StandaloneOSXIntel:
                        case BuildTarget.StandaloneWindows:
                            goto Label_0040;

                        default:
                            switch (target2)
                            {
                                case BuildTarget.StandaloneLinuxUniversal:
                                    goto Label_0040;
                            }
                            return string.Empty;
                    }
                    break;
            }
            return "x86_64";
        Label_0040:
            return "x86";
        }

        public static string GetExtensionForBuildTarget(BuildTarget target, BuildOptions options)
        {
            IBuildPostprocessor buildPostProcessor = ModuleManager.GetBuildPostProcessor(target);
            if (buildPostProcessor == null)
            {
                return string.Empty;
            }
            return buildPostProcessor.GetExtension(target, options);
        }

        public static string GetScriptLayoutFileFromBuild(BuildOptions options, BuildTarget target, string installPath, string fileName)
        {
            IBuildPostprocessor buildPostProcessor = ModuleManager.GetBuildPostProcessor(target);
            if (buildPostProcessor != null)
            {
                return buildPostProcessor.GetScriptLayoutFileFromBuild(options, installPath, fileName);
            }
            return string.Empty;
        }

        internal static void InstallPlugins(string destPluginFolder, BuildTarget target)
        {
            string basePluginFolder = "Assets/Plugins";
            IBuildPostprocessor buildPostProcessor = ModuleManager.GetBuildPostProcessor(target);
            if (buildPostProcessor != null)
            {
                bool flag;
                string[] strArray = buildPostProcessor.FindPluginFilesToCopy(basePluginFolder, out flag);
                if (strArray != null)
                {
                    if ((strArray.Length > 0) && !Directory.Exists(destPluginFolder))
                    {
                        Directory.CreateDirectory(destPluginFolder);
                    }
                    foreach (string str2 in strArray)
                    {
                        if (Directory.Exists(str2))
                        {
                            string str3 = Path.Combine(destPluginFolder, str2);
                            FileUtil.CopyDirectoryRecursive(str2, str3);
                        }
                        else
                        {
                            string fileName = Path.GetFileName(str2);
                            if (flag)
                            {
                                string directoryName = Path.GetDirectoryName(str2.Substring(basePluginFolder.Length + 1));
                                string str6 = Path.Combine(destPluginFolder, directoryName);
                                string to = Path.Combine(str6, fileName);
                                if (!Directory.Exists(str6))
                                {
                                    Directory.CreateDirectory(str6);
                                }
                                FileUtil.UnityFileCopy(str2, to, true);
                            }
                            else
                            {
                                string str8 = Path.Combine(destPluginFolder, fileName);
                                FileUtil.UnityFileCopy(str2, str8, true);
                            }
                        }
                    }
                    return;
                }
            }
            bool flag2 = false;
            List<string> subdirs = new List<string>();
            bool flag3 = ((target == BuildTarget.StandaloneOSXIntel) || (target == BuildTarget.StandaloneOSXIntel64)) || (target == BuildTarget.StandaloneOSXUniversal);
            bool copyDirectories = flag3;
            string extension = string.Empty;
            string debugExtension = string.Empty;
            if (flag3)
            {
                extension = ".bundle";
                subdirs.Add(string.Empty);
            }
            else if (target == BuildTarget.StandaloneWindows)
            {
                extension = ".dll";
                debugExtension = ".pdb";
                AddPluginSubdirIfExists(subdirs, basePluginFolder, subDir32Bit);
            }
            else if (target == BuildTarget.StandaloneWindows64)
            {
                extension = ".dll";
                debugExtension = ".pdb";
                AddPluginSubdirIfExists(subdirs, basePluginFolder, subDir64Bit);
            }
            else if (target == BuildTarget.StandaloneGLESEmu)
            {
                extension = ".dll";
                debugExtension = ".pdb";
                subdirs.Add(string.Empty);
            }
            else if (target == BuildTarget.StandaloneLinux)
            {
                extension = ".so";
                AddPluginSubdirIfExists(subdirs, basePluginFolder, subDir32Bit);
            }
            else if (target == BuildTarget.StandaloneLinux64)
            {
                extension = ".so";
                AddPluginSubdirIfExists(subdirs, basePluginFolder, subDir64Bit);
            }
            else if (target == BuildTarget.StandaloneLinuxUniversal)
            {
                extension = ".so";
                subdirs.Add(subDir32Bit);
                subdirs.Add(subDir64Bit);
                flag2 = true;
            }
            else if (target == BuildTarget.PS3)
            {
                extension = ".sprx";
                subdirs.Add(string.Empty);
            }
            else if (target == BuildTarget.Android)
            {
                extension = ".so";
                subdirs.Add("Android");
            }
            else if (target == BuildTarget.BlackBerry)
            {
                extension = ".so";
                subdirs.Add("BlackBerry");
            }
            foreach (string str11 in subdirs)
            {
                if (flag2)
                {
                    InstallPluginsByExtension(Path.Combine(basePluginFolder, str11), extension, debugExtension, Path.Combine(destPluginFolder, str11), copyDirectories);
                }
                else
                {
                    InstallPluginsByExtension(Path.Combine(basePluginFolder, str11), extension, debugExtension, destPluginFolder, copyDirectories);
                }
            }
        }

        internal static bool InstallPluginsByExtension(string pluginSourceFolder, string extension, string debugExtension, string destPluginFolder, bool copyDirectories)
        {
            bool flag = false;
            if (Directory.Exists(pluginSourceFolder))
            {
                foreach (string str in Directory.GetFileSystemEntries(pluginSourceFolder))
                {
                    string fileName = Path.GetFileName(str);
                    string str3 = Path.GetExtension(str);
                    bool flag2 = str3.Equals(extension, StringComparison.OrdinalIgnoreCase) || fileName.Equals(extension, StringComparison.OrdinalIgnoreCase);
                    bool flag3 = ((debugExtension != null) && (debugExtension.Length != 0)) && (str3.Equals(debugExtension, StringComparison.OrdinalIgnoreCase) || fileName.Equals(debugExtension, StringComparison.OrdinalIgnoreCase));
                    if (flag2 || flag3)
                    {
                        if (!Directory.Exists(destPluginFolder))
                        {
                            Directory.CreateDirectory(destPluginFolder);
                        }
                        string target = Path.Combine(destPluginFolder, fileName);
                        if (copyDirectories)
                        {
                            FileUtil.CopyDirectoryRecursive(str, target);
                        }
                        else if (!Directory.Exists(str))
                        {
                            FileUtil.UnityFileCopy(str, target);
                        }
                        flag = true;
                    }
                }
            }
            return flag;
        }

        internal static void InstallStreamingAssets(string stagingAreaDataPath)
        {
            if (Directory.Exists("Assets/StreamingAssets"))
            {
                FileUtil.CopyDirectoryRecursiveForPostprocess("Assets/StreamingAssets", Path.Combine(stagingAreaDataPath, "StreamingAssets"), true);
            }
        }

        internal static bool IsPlugin(string path, string targetExtension)
        {
            return ((string.Compare(Path.GetExtension(path), targetExtension, true) == 0) || (string.Compare(Path.GetFileName(path), targetExtension, true) == 0));
        }

        public static void Launch(BuildTarget target, string path, string productName, BuildOptions options)
        {
            BuildLaunchPlayerArgs args;
            IBuildPostprocessor buildPostProcessor = ModuleManager.GetBuildPostProcessor(target);
            if (buildPostProcessor == null)
            {
                throw new UnityException(string.Format("Launching {0} build target via mono is not supported", target));
            }
            args.target = target;
            args.playerPackage = BuildPipeline.GetPlaybackEngineDirectory(target, options);
            args.installPath = path;
            args.productName = productName;
            args.options = options;
            buildPostProcessor.LaunchPlayer(args);
        }

        public static void Postprocess(BuildTarget target, string installPath, string companyName, string productName, int width, int height, string downloadWebplayerUrl, string manualDownloadWebplayerUrl, BuildOptions options, RuntimeClassRegistry usedClassRegistry)
        {
            string str = "Temp/StagingArea";
            string str2 = "Temp/StagingArea/Data";
            string str3 = "Temp/StagingArea/Data/Managed";
            string playbackEngineDirectory = BuildPipeline.GetPlaybackEngineDirectory(target, options);
            bool flag = ((options & BuildOptions.InstallInBuildFolder) != BuildOptions.CompressTextures) && SupportsInstallInBuildFolder(target);
            if ((installPath == string.Empty) && !flag)
            {
                throw new Exception(installPath + " must not be an empty string");
            }
            IBuildPostprocessor buildPostProcessor = ModuleManager.GetBuildPostProcessor(target);
            if (buildPostProcessor != null)
            {
                BuildPostProcessArgs args;
                args.target = target;
                args.stagingAreaData = str2;
                args.stagingArea = str;
                args.stagingAreaDataManaged = str3;
                args.playerPackage = playbackEngineDirectory;
                args.installPath = installPath;
                args.companyName = companyName;
                args.productName = productName;
                args.productGUID = PlayerSettings.productGUID;
                args.options = options;
                args.usedClassRegistry = usedClassRegistry;
                buildPostProcessor.PostProcess(args);
            }
            else
            {
                BuildTarget target2 = target;
                if ((target2 != BuildTarget.WebPlayer) && (target2 != BuildTarget.WebPlayerStreamed))
                {
                    throw new UnityException(string.Format("Build target '{0}' not supported", target));
                }
                PostProcessWebPlayer.PostProcess(options, installPath, downloadWebplayerUrl, width, height);
            }
        }

        public static bool SupportsInstallInBuildFolder(BuildTarget target)
        {
            IBuildPostprocessor buildPostProcessor = ModuleManager.GetBuildPostProcessor(target);
            if (buildPostProcessor != null)
            {
                return buildPostProcessor.SupportsInstallInBuildFolder();
            }
            BuildTarget target2 = target;
            switch (target2)
            {
                case BuildTarget.PS3:
                case BuildTarget.Android:
                case BuildTarget.PSP2:
                case BuildTarget.PSM:
                    break;

                default:
                    if ((target2 != BuildTarget.WSAPlayer) && (target2 != BuildTarget.WP8Player))
                    {
                        return false;
                    }
                    break;
            }
            return true;
        }

        public static bool SupportsScriptsOnlyBuild(BuildTarget target)
        {
            IBuildPostprocessor buildPostProcessor = ModuleManager.GetBuildPostProcessor(target);
            return ((buildPostProcessor != null) && buildPostProcessor.SupportsScriptsOnlyBuild());
        }

        public static string subDir32Bit
        {
            get
            {
                return "x86";
            }
        }

        public static string subDir64Bit
        {
            get
            {
                return "x86_64";
            }
        }
    }
}

