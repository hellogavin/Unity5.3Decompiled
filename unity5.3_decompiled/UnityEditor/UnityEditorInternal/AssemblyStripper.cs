namespace UnityEditorInternal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Xml;
    using UnityEditor;
    using UnityEditor.Utils;
    using UnityEngine;

    internal class AssemblyStripper
    {
        [CompilerGenerated]
        private static Func<string, string> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<string, string> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<string, string> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<string, string, string> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<string, string> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<RuntimeClassRegistry.MethodDescription, string> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<RuntimeClassRegistry.MethodDescription, string> <>f__am$cache6;

        private static bool AddWhiteListsForModules(IEnumerable<string> nativeModules, ref IEnumerable<string> blacklists, string moduleStrippingInformationFolder)
        {
            bool flag = false;
            IEnumerator<string> enumerator = nativeModules.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    string current = enumerator.Current;
                    string moduleWhitelist = GetModuleWhitelist(current, moduleStrippingInformationFolder);
                    if (File.Exists(moduleWhitelist))
                    {
                        if (!blacklists.Contains<string>(moduleWhitelist))
                        {
                            string[] second = new string[] { moduleWhitelist };
                            blacklists = blacklists.Concat<string>(second);
                            flag = true;
                        }
                        flag = flag || AddWhiteListsForModules(GetDependentModules(moduleWhitelist), ref blacklists, moduleStrippingInformationFolder);
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
            return flag;
        }

        private static List<string> GetDependentModules(string moduleXml)
        {
            XmlDocument document = new XmlDocument();
            document.Load(moduleXml);
            List<string> list = new List<string>();
            IEnumerator enumerator = document.DocumentElement.SelectNodes("/linker/dependencies/module").GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    XmlNode current = (XmlNode) enumerator.Current;
                    list.Add(current.Attributes["name"].Value);
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
            return list;
        }

        private static string GetMethodPreserveBlacklistContents(RuntimeClassRegistry rcr)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<linker>");
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = m => m.assembly;
            }
            IEnumerator<IGrouping<string, RuntimeClassRegistry.MethodDescription>> enumerator = rcr.GetMethodsToPreserve().GroupBy<RuntimeClassRegistry.MethodDescription, string>(<>f__am$cache5).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    IGrouping<string, RuntimeClassRegistry.MethodDescription> current = enumerator.Current;
                    builder.AppendLine(string.Format("\t<assembly fullname=\"{0}\">", current.Key));
                    if (<>f__am$cache6 == null)
                    {
                        <>f__am$cache6 = m => m.fullTypeName;
                    }
                    IEnumerator<IGrouping<string, RuntimeClassRegistry.MethodDescription>> enumerator2 = current.GroupBy<RuntimeClassRegistry.MethodDescription, string>(<>f__am$cache6).GetEnumerator();
                    try
                    {
                        while (enumerator2.MoveNext())
                        {
                            IGrouping<string, RuntimeClassRegistry.MethodDescription> grouping2 = enumerator2.Current;
                            builder.AppendLine(string.Format("\t\t<type fullname=\"{0}\">", grouping2.Key));
                            IEnumerator<RuntimeClassRegistry.MethodDescription> enumerator3 = grouping2.GetEnumerator();
                            try
                            {
                                while (enumerator3.MoveNext())
                                {
                                    RuntimeClassRegistry.MethodDescription description = enumerator3.Current;
                                    builder.AppendLine(string.Format("\t\t\t<method name=\"{0}\"/>", description.methodName));
                                }
                            }
                            finally
                            {
                                if (enumerator3 == null)
                                {
                                }
                                enumerator3.Dispose();
                            }
                            builder.AppendLine("\t\t</type>");
                        }
                    }
                    finally
                    {
                        if (enumerator2 == null)
                        {
                        }
                        enumerator2.Dispose();
                    }
                    builder.AppendLine("\t</assembly>");
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            builder.AppendLine("</linker>");
            return builder.ToString();
        }

        private static string GetModuleWhitelist(string module, string moduleStrippingInformationFolder)
        {
            string[] components = new string[] { moduleStrippingInformationFolder, module + ".xml" };
            return Paths.Combine(components);
        }

        private static List<string> GetUserAssemblies(RuntimeClassRegistry rcr, string managedDir)
        {
            <GetUserAssemblies>c__AnonStorey63 storey = new <GetUserAssemblies>c__AnonStorey63 {
                rcr = rcr,
                managedDir = managedDir
            };
            return storey.rcr.GetUserAssemblies().Where<string>(new Func<string, bool>(storey.<>m__D3)).Select<string, string>(new Func<string, string>(storey.<>m__D4)).ToList<string>();
        }

        internal static IEnumerable<string> GetUserBlacklistFiles()
        {
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = s => Path.Combine(Directory.GetCurrentDirectory(), s);
            }
            return Directory.GetFiles("Assets", "link.xml", SearchOption.AllDirectories).Select<string, string>(<>f__am$cache4);
        }

        private static bool RunAssemblyLinker(IEnumerable<string> args, out string @out, out string err, string linkerPath, string workingDirectory)
        {
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = (buff, s) => buff + " " + s;
            }
            string str = args.Aggregate<string>(<>f__am$cache3);
            Console.WriteLine("Invoking UnusedByteCodeStripper2 with arguments: " + str);
            Runner.RunManagedProgram(linkerPath, str, workingDirectory, null);
            @out = string.Empty;
            err = string.Empty;
            return true;
        }

        private static void RunAssemblyStripper(string stagingAreaData, IEnumerable assemblies, string managedAssemblyFolderPath, string[] assembliesToStrip, string[] searchDirs, string monoLinkerPath, IIl2CppPlatformProvider platformProvider, RuntimeClassRegistry rcr, bool developmentBuild)
        {
            bool flag2;
            bool flag = ((rcr != null) && PlayerSettings.stripEngineCode) && platformProvider.supportsEngineStripping;
            IEnumerable<string> first = Il2CppBlacklistPaths;
            if (rcr != null)
            {
                string[] second = new string[] { WriteMethodsToPreserveBlackList(stagingAreaData, rcr) };
                first = first.Concat<string>(second);
            }
            if (!flag)
            {
                foreach (string str3 in Directory.GetFiles(platformProvider.moduleStrippingInformationFolder, "*.xml"))
                {
                    string[] textArray2 = new string[] { str3 };
                    first = first.Concat<string>(textArray2);
                }
            }
            string fullPath = Path.GetFullPath(Path.Combine(managedAssemblyFolderPath, "tempStrip"));
            do
            {
                string str;
                string str2;
                flag2 = false;
                if (EditorUtility.DisplayCancelableProgressBar("Building Player", "Stripping assemblies", 0f))
                {
                    throw new OperationCanceledException();
                }
                if (!StripAssembliesTo(assembliesToStrip, searchDirs, fullPath, managedAssemblyFolderPath, out str, out str2, monoLinkerPath, platformProvider, first, developmentBuild))
                {
                    object[] objArray1 = new object[] { "Error in stripping assemblies: ", assemblies, ", ", str2 };
                    throw new Exception(string.Concat(objArray1));
                }
                string str5 = Path.Combine(managedAssemblyFolderPath, "ICallSummary.txt");
                string exe = Path.Combine(MonoInstallationFinder.GetFrameWorksFolder(), "Tools/InternalCallRegistrationWriter/InternalCallRegistrationWriter.exe");
                string args = string.Format("-assembly=\"{0}\" -output=\"{1}\" -summary=\"{2}\"", Path.Combine(fullPath, "UnityEngine.dll"), Path.Combine(managedAssemblyFolderPath, "UnityICallRegistration.cpp"), str5);
                Runner.RunManagedProgram(exe, args);
                if (flag)
                {
                    HashSet<string> set;
                    HashSet<string> set2;
                    CodeStrippingUtils.GenerateDependencies(fullPath, str5, rcr, out set, out set2);
                    flag2 = AddWhiteListsForModules(set2, ref first, platformProvider.moduleStrippingInformationFolder);
                }
            }
            while (flag2);
            string path = Path.GetFullPath(Path.Combine(managedAssemblyFolderPath, "tempUnstripped"));
            Directory.CreateDirectory(path);
            foreach (string str9 in Directory.GetFiles(managedAssemblyFolderPath))
            {
                string extension = Path.GetExtension(str9);
                if (string.Equals(extension, ".dll", StringComparison.InvariantCultureIgnoreCase) || string.Equals(extension, ".mdb", StringComparison.InvariantCultureIgnoreCase))
                {
                    File.Move(str9, Path.Combine(path, Path.GetFileName(str9)));
                }
            }
            foreach (string str11 in Directory.GetFiles(fullPath))
            {
                File.Move(str11, Path.Combine(managedAssemblyFolderPath, Path.GetFileName(str11)));
            }
            Directory.Delete(fullPath);
        }

        internal static void StripAssemblies(string stagingAreaData, IIl2CppPlatformProvider platformProvider, RuntimeClassRegistry rcr, bool developmentBuild)
        {
            string fullPath = Path.GetFullPath(Path.Combine(stagingAreaData, "Managed"));
            List<string> userAssemblies = GetUserAssemblies(rcr, fullPath);
            string[] assembliesToStrip = userAssemblies.ToArray();
            string[] searchDirs = new string[] { fullPath };
            RunAssemblyStripper(stagingAreaData, userAssemblies, fullPath, assembliesToStrip, searchDirs, MonoLinker2Path, platformProvider, rcr, developmentBuild);
        }

        private static bool StripAssembliesTo(string[] assemblies, string[] searchDirs, string outputFolder, string workingDirectory, out string output, out string error, string linkerPath, IIl2CppPlatformProvider platformProvider, IEnumerable<string> additionalBlacklist, bool developmentBuild)
        {
            <StripAssembliesTo>c__AnonStorey62 storey = new <StripAssembliesTo>c__AnonStorey62 {
                workingDirectory = workingDirectory
            };
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
            additionalBlacklist = additionalBlacklist.Select<string, string>(new Func<string, string>(storey.<>m__CE)).Where<string>(new Func<string, bool>(File.Exists));
            IEnumerable<string> userBlacklistFiles = GetUserBlacklistFiles();
            IEnumerator<string> enumerator = userBlacklistFiles.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Console.WriteLine("UserBlackList: " + enumerator.Current);
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            additionalBlacklist = additionalBlacklist.Concat<string>(userBlacklistFiles);
            List<string> args = new List<string> {
                "-out \"" + outputFolder + "\"",
                "-l none",
                "-c link",
                "-b " + developmentBuild,
                "-x \"" + GetModuleWhitelist("Core", platformProvider.moduleStrippingInformationFolder) + "\"",
                "-f \"" + Path.Combine(platformProvider.il2CppFolder, "LinkerDescriptors") + "\""
            };
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = path => "-x \"" + path + "\"";
            }
            args.AddRange(additionalBlacklist.Select<string, string>(<>f__am$cache0));
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = d => "-d \"" + d + "\"";
            }
            args.AddRange(searchDirs.Select<string, string>(<>f__am$cache1));
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = assembly => "-a  \"" + Path.GetFullPath(assembly) + "\"";
            }
            args.AddRange(assemblies.Select<string, string>(<>f__am$cache2));
            return RunAssemblyLinker(args, out output, out error, linkerPath, storey.workingDirectory);
        }

        private static string WriteMethodsToPreserveBlackList(string stagingAreaData, RuntimeClassRegistry rcr)
        {
            string path = !Path.IsPathRooted(stagingAreaData) ? (Directory.GetCurrentDirectory() + "/") : string.Empty;
            path = path + stagingAreaData + "/methods_pointedto_by_uievents.xml";
            File.WriteAllText(path, GetMethodPreserveBlacklistContents(rcr));
            return path;
        }

        private static string BlacklistPath
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(MonoLinkerPath), "Core.xml");
            }
        }

        private static string[] Il2CppBlacklistPaths
        {
            get
            {
                return new string[] { Path.Combine("..", "platform_native_link.xml") };
            }
        }

        private static string ModulesWhitelistPath
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(MonoLinkerPath), "ModuleStrippingInformation");
            }
        }

        private static string MonoLinker2Path
        {
            get
            {
                return Path.Combine(MonoInstallationFinder.GetFrameWorksFolder(), "Tools/UnusedByteCodeStripper2/UnusedBytecodeStripper2.exe");
            }
        }

        private static string MonoLinkerPath
        {
            get
            {
                if (Application.platform != RuntimePlatform.WindowsEditor)
                {
                    return Path.Combine(MonoInstallationFinder.GetFrameWorksFolder(), "Tools/UnusedByteCodeStripper/UnusedBytecodeStripper.exe");
                }
                return Path.Combine(MonoInstallationFinder.GetFrameWorksFolder(), "Tools/UnusedBytecodeStripper.exe");
            }
        }

        [CompilerGenerated]
        private sealed class <GetUserAssemblies>c__AnonStorey63
        {
            internal string managedDir;
            internal RuntimeClassRegistry rcr;

            internal bool <>m__D3(string s)
            {
                return this.rcr.IsDLLUsed(s);
            }

            internal string <>m__D4(string s)
            {
                return Path.Combine(this.managedDir, s);
            }
        }

        [CompilerGenerated]
        private sealed class <StripAssembliesTo>c__AnonStorey62
        {
            internal string workingDirectory;

            internal string <>m__CE(string s)
            {
                return (!Path.IsPathRooted(s) ? Path.Combine(this.workingDirectory, s) : s);
            }
        }
    }
}

