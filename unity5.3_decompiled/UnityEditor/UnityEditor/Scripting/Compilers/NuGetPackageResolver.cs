namespace UnityEditor.Scripting.Compilers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Utils;

    internal sealed class NuGetPackageResolver
    {
        public NuGetPackageResolver()
        {
            this.TargetMoniker = "UAP,Version=v10.0";
        }

        private string ConvertToWindowsPath(string path)
        {
            return path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        }

        public bool EnsureProjectLockFile(string projectFile)
        {
            string directoryName = Path.GetDirectoryName(this.ProjectLockFile);
            string path = FileUtil.NiceWinPath(Path.Combine(directoryName, Path.GetFileName(projectFile)));
            Console.WriteLine("Restoring NuGet packages from \"{0}\".", Path.GetFullPath(path));
            if (File.Exists(this.ProjectLockFile))
            {
                Console.WriteLine("Done. Reusing existing \"{0}\" file.", Path.GetFullPath(this.ProjectLockFile));
                return true;
            }
            if (!string.IsNullOrEmpty(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            File.Copy(projectFile, path, true);
            string str4 = FileUtil.NiceWinPath(Path.Combine(BuildPipeline.GetBuildToolsDirectory(BuildTarget.WSAPlayer), "nuget.exe"));
            ProcessStartInfo si = new ProcessStartInfo {
                Arguments = string.Format("restore \"{0}\" -NonInteractive -Source https://api.nuget.org/v3/index.json", path),
                CreateNoWindow = true,
                FileName = str4
            };
            Program program = new Program(si);
            using (program)
            {
                program.Start();
                for (int i = 0; i < 15; i++)
                {
                    if (!program.WaitForExit(0xea60))
                    {
                        Console.WriteLine("Still restoring NuGet packages.");
                    }
                }
                if (!program.HasExited)
                {
                    throw new Exception(string.Format("Failed to restore NuGet packages:{0}Time out.", Environment.NewLine));
                }
                if (program.ExitCode != 0)
                {
                    throw new Exception(string.Format("Failed to restore NuGet packages:{0}{1}", Environment.NewLine, program.GetAllOutput()));
                }
            }
            Console.WriteLine("Done.");
            return false;
        }

        private string GetPackagesPath()
        {
            string packagesDirectory = this.PackagesDirectory;
            if (!string.IsNullOrEmpty(packagesDirectory))
            {
                return packagesDirectory;
            }
            packagesDirectory = Environment.GetEnvironmentVariable("NUGET_PACKAGES");
            if (!string.IsNullOrEmpty(packagesDirectory))
            {
                return packagesDirectory;
            }
            return Path.Combine(Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), ".nuget"), "packages");
        }

        public void Resolve()
        {
            Dictionary<string, object> dictionary = (Dictionary<string, object>) Json.Deserialize(File.ReadAllText(this.ProjectLockFile));
            Dictionary<string, object> dictionary2 = (Dictionary<string, object>) dictionary["targets"];
            Dictionary<string, object> dictionary3 = (Dictionary<string, object>) dictionary2[this.TargetMoniker];
            List<string> list = new List<string>();
            string str2 = this.ConvertToWindowsPath(this.GetPackagesPath());
            foreach (KeyValuePair<string, object> pair in dictionary3)
            {
                object obj2;
                Dictionary<string, object> dictionary4 = (Dictionary<string, object>) pair.Value;
                if (dictionary4.TryGetValue("compile", out obj2))
                {
                    Dictionary<string, object> dictionary5 = (Dictionary<string, object>) obj2;
                    char[] separator = new char[] { '/' };
                    string[] strArray = pair.Key.Split(separator);
                    string str3 = strArray[0];
                    string str4 = strArray[1];
                    string path = Path.Combine(Path.Combine(str2, str3), str4);
                    if (!Directory.Exists(path))
                    {
                        throw new Exception(string.Format("Package directory not found: \"{0}\".", path));
                    }
                    foreach (string str6 in dictionary5.Keys)
                    {
                        if (!string.Equals(Path.GetFileName(str6), "_._", StringComparison.InvariantCultureIgnoreCase))
                        {
                            string str8 = Path.Combine(path, this.ConvertToWindowsPath(str6));
                            if (!File.Exists(str8))
                            {
                                throw new Exception(string.Format("Reference not found: \"{0}\".", str8));
                            }
                            list.Add(str8);
                        }
                    }
                    if (dictionary4.ContainsKey("frameworkAssemblies"))
                    {
                        throw new NotImplementedException("Support for \"frameworkAssemblies\" property has not been implemented yet.");
                    }
                }
            }
            this.ResolvedReferences = list.ToArray();
        }

        public string PackagesDirectory { get; set; }

        public string ProjectLockFile { get; set; }

        public string[] ResolvedReferences { get; private set; }

        public string TargetMoniker { get; set; }
    }
}

