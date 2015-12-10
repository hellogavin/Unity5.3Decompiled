namespace UnityEditor
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using UnityEditor.Utils;
    using UnityEngine;

    internal class MonoAssemblyStripping
    {
        private static void CopyAllDlls(string fromDir, string toDir)
        {
            DirectoryInfo info = new DirectoryInfo(toDir);
            foreach (FileInfo info2 in info.GetFiles("*.dll"))
            {
                FileUtil.ReplaceFile(Path.Combine(toDir, info2.Name), Path.Combine(fromDir, info2.Name));
            }
        }

        private static void CopyFiles(IEnumerable<string> files, string fromDir, string toDir)
        {
            IEnumerator<string> enumerator = files.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    string current = enumerator.Current;
                    FileUtil.ReplaceFile(Path.Combine(fromDir, current), Path.Combine(toDir, current));
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

        private static void DeleteAllDllsFrom(string managedLibrariesDirectory)
        {
            DirectoryInfo info = new DirectoryInfo(managedLibrariesDirectory);
            foreach (FileInfo info2 in info.GetFiles("*.dll"))
            {
                FileUtil.DeleteFileOrDirectory(info2.FullName);
            }
        }

        private static bool DoesTypeEnheritFrom(TypeReference type, string typeName)
        {
            while (type != null)
            {
                if (type.FullName == typeName)
                {
                    return true;
                }
                type = type.Resolve().BaseType;
            }
            return false;
        }

        public static string GenerateBlackList(string librariesFolder, RuntimeClassRegistry usedClasses, string[] allAssemblies)
        {
            string str = "tmplink.xml";
            usedClasses.SynchronizeClasses();
            using (TextWriter writer = new StreamWriter(Path.Combine(librariesFolder, str)))
            {
                writer.WriteLine("<linker>");
                writer.WriteLine("<assembly fullname=\"UnityEngine\">");
                foreach (string str2 in usedClasses.GetAllManagedClassesAsString())
                {
                    writer.WriteLine(string.Format("<type fullname=\"UnityEngine.{0}\" preserve=\"{1}\"/>", str2, usedClasses.GetRetentionLevel(str2)));
                }
                writer.WriteLine("</assembly>");
                DefaultAssemblyResolver resolver = new DefaultAssemblyResolver();
                resolver.AddSearchDirectory(librariesFolder);
                foreach (string str3 in allAssemblies)
                {
                    ReaderParameters parameters = new ReaderParameters {
                        AssemblyResolver = resolver
                    };
                    AssemblyDefinition definition = resolver.Resolve(Path.GetFileNameWithoutExtension(str3), parameters);
                    writer.WriteLine("<assembly fullname=\"{0}\">", definition.Name.Name);
                    if (definition.Name.Name.StartsWith("UnityEngine."))
                    {
                        foreach (string str4 in usedClasses.GetAllManagedClassesAsString())
                        {
                            writer.WriteLine(string.Format("<type fullname=\"UnityEngine.{0}\" preserve=\"{1}\"/>", str4, usedClasses.GetRetentionLevel(str4)));
                        }
                    }
                    GenerateBlackListTypeXML(writer, definition.MainModule.Types, usedClasses.GetAllManagedBaseClassesAsString());
                    writer.WriteLine("</assembly>");
                }
                writer.WriteLine("</linker>");
            }
            return str;
        }

        private static void GenerateBlackListTypeXML(TextWriter w, IList<TypeDefinition> types, List<string> baseTypes)
        {
            if (types != null)
            {
                IEnumerator<TypeDefinition> enumerator = types.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        TypeDefinition current = enumerator.Current;
                        if (current != null)
                        {
                            foreach (string str in baseTypes)
                            {
                                if (DoesTypeEnheritFrom(current, str))
                                {
                                    w.WriteLine("<type fullname=\"{0}\" preserve=\"all\"/>", current.FullName);
                                    break;
                                }
                            }
                            GenerateBlackListTypeXML(w, current.NestedTypes, baseTypes);
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
        }

        public static void MonoCilStrip(BuildTarget buildTarget, string managedLibrariesDirectory, string[] fileNames)
        {
            string str2 = Path.Combine(BuildPipeline.GetBuildToolsDirectory(buildTarget), "mono-cil-strip.exe");
            foreach (string str3 in fileNames)
            {
                Process process = MonoProcessUtility.PrepareMonoProcess(buildTarget, managedLibrariesDirectory);
                string str4 = str3 + ".out";
                process.StartInfo.Arguments = "\"" + str2 + "\"";
                ProcessStartInfo startInfo = process.StartInfo;
                string arguments = startInfo.Arguments;
                string[] textArray1 = new string[] { arguments, " \"", str3, "\" \"", str3, ".out\"" };
                startInfo.Arguments = string.Concat(textArray1);
                MonoProcessUtility.RunMonoProcess(process, "byte code stripper", Path.Combine(managedLibrariesDirectory, str4));
                ReplaceFile(managedLibrariesDirectory + "/" + str4, managedLibrariesDirectory + "/" + str3);
                File.Delete(managedLibrariesDirectory + "/" + str4);
            }
        }

        public static void MonoLink(BuildTarget buildTarget, string managedLibrariesDirectory, string[] input, string[] allAssemblies, RuntimeClassRegistry usedClasses)
        {
            Process process = MonoProcessUtility.PrepareMonoProcess(buildTarget, managedLibrariesDirectory);
            string buildToolsDirectory = BuildPipeline.GetBuildToolsDirectory(buildTarget);
            string str2 = null;
            string path = Path.Combine(MonoInstallationFinder.GetFrameWorksFolder(), StripperExe());
            string str5 = Path.Combine(Path.GetDirectoryName(path), "link.xml");
            string str6 = Path.Combine(managedLibrariesDirectory, "output");
            Directory.CreateDirectory(str6);
            process.StartInfo.Arguments = "\"" + path + "\" -l none -c link";
            foreach (string str7 in input)
            {
                ProcessStartInfo info1 = process.StartInfo;
                info1.Arguments = info1.Arguments + " -a \"" + str7 + "\"";
            }
            ProcessStartInfo startInfo = process.StartInfo;
            string arguments = startInfo.Arguments;
            string[] textArray1 = new string[] { arguments, " -out output -x \"", str5, "\" -d \"", managedLibrariesDirectory, "\"" };
            startInfo.Arguments = string.Concat(textArray1);
            string str8 = Path.Combine(buildToolsDirectory, "link.xml");
            if (File.Exists(str8))
            {
                ProcessStartInfo info3 = process.StartInfo;
                info3.Arguments = info3.Arguments + " -x \"" + str8 + "\"";
            }
            string str9 = Path.Combine(Path.GetDirectoryName(path), "Core.xml");
            if (File.Exists(str9))
            {
                ProcessStartInfo info4 = process.StartInfo;
                info4.Arguments = info4.Arguments + " -x \"" + str9 + "\"";
            }
            foreach (string str10 in Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "Assets"), "link.xml", SearchOption.AllDirectories))
            {
                ProcessStartInfo info5 = process.StartInfo;
                info5.Arguments = info5.Arguments + " -x \"" + str10 + "\"";
            }
            if (usedClasses != null)
            {
                str2 = GenerateBlackList(managedLibrariesDirectory, usedClasses, allAssemblies);
                ProcessStartInfo info6 = process.StartInfo;
                info6.Arguments = info6.Arguments + " -x \"" + str2 + "\"";
            }
            MonoProcessUtility.RunMonoProcess(process, "assemblies stripper", Path.Combine(str6, "mscorlib.dll"));
            DeleteAllDllsFrom(managedLibrariesDirectory);
            CopyAllDlls(managedLibrariesDirectory, str6);
            foreach (string str11 in Directory.GetFiles(managedLibrariesDirectory))
            {
                if (str11.Contains(".mdb") && !File.Exists(str11.Replace(".mdb", string.Empty)))
                {
                    FileUtil.DeleteFileOrDirectory(str11);
                }
            }
            if (str2 != null)
            {
                FileUtil.DeleteFileOrDirectory(Path.Combine(managedLibrariesDirectory, str2));
            }
            FileUtil.DeleteFileOrDirectory(str6);
        }

        private static void ReplaceFile(string src, string dst)
        {
            if (File.Exists(dst))
            {
                FileUtil.DeleteFileOrDirectory(dst);
            }
            FileUtil.CopyFileOrDirectory(src, dst);
        }

        private static string StripperExe()
        {
            if (Application.platform != RuntimePlatform.WindowsEditor)
            {
                return "Tools/UnusedByteCodeStripper/UnusedBytecodeStripper.exe";
            }
            return "Tools/UnusedBytecodeStripper.exe";
        }
    }
}

