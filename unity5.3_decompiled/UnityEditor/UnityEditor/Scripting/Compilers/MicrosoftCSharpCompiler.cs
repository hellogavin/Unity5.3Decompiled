namespace UnityEditor.Scripting.Compilers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.Scripting;
    using UnityEditor.Utils;
    using UnityEditorInternal;
    using UnityEngine;

    internal class MicrosoftCSharpCompiler : ScriptCompilerBase
    {
        private static string[] _uwpReferences;

        public MicrosoftCSharpCompiler(MonoIsland island, bool runUpdater) : base(island)
        {
        }

        protected override CompilerOutputParserBase CreateOutputParser()
        {
            return new MicrosoftCSharpCompilerOutputParser();
        }

        internal static bool EnsureProjectLockFile(string projectLockFile)
        {
            NuGetPackageResolver resolver = new NuGetPackageResolver {
                ProjectLockFile = projectLockFile
            };
            return EnsureProjectLockFile(resolver);
        }

        private static bool EnsureProjectLockFile(NuGetPackageResolver resolver)
        {
            string projectFile = FileUtil.NiceWinPath(Path.Combine(BuildPipeline.GetBuildToolsDirectory(BuildTarget.WSAPlayer), "project.json"));
            return resolver.EnsureProjectLockFile(projectFile);
        }

        private void FillNETCoreCompilerOptions(WSASDK wsaSDK, List<string> arguments, ref string argsPrefix)
        {
            string str;
            argsPrefix = "/noconfig ";
            arguments.Add("/nostdlib+");
            if (GetCurrentScriptingBackend() != ScriptingImplementation.IL2CPP)
            {
                arguments.Add("/define:NETFX_CORE");
            }
            string platformAssemblyPath = GetPlatformAssemblyPath(wsaSDK);
            switch (wsaSDK)
            {
                case WSASDK.SDK80:
                    str = "8.0";
                    break;

                case WSASDK.SDK81:
                    str = "8.1";
                    break;

                case WSASDK.PhoneSDK81:
                    str = "Phone 8.1";
                    break;

                case WSASDK.UWP:
                    str = "UAP";
                    if (GetCurrentScriptingBackend() != ScriptingImplementation.IL2CPP)
                    {
                        arguments.Add("/define:WINDOWS_UWP");
                    }
                    break;

                default:
                    throw new Exception("Unknown Windows SDK: " + EditorUserBuildSettings.wsaSDK.ToString());
            }
            if (!File.Exists(platformAssemblyPath))
            {
                throw new Exception(string.Format("'{0}' not found, do you have Windows {1} SDK installed?", platformAssemblyPath, str));
            }
            arguments.Add("/reference:\"" + platformAssemblyPath + "\"");
            string[] additionalReferences = GetAdditionalReferences(wsaSDK);
            if (additionalReferences != null)
            {
                foreach (string str3 in additionalReferences)
                {
                    arguments.Add("/reference:\"" + str3 + "\"");
                }
            }
            foreach (string str4 in this.GetNETWSAAssemblies(wsaSDK))
            {
                arguments.Add("/reference:\"" + str4 + "\"");
            }
            if (GetCurrentScriptingBackend() != ScriptingImplementation.IL2CPP)
            {
                string str5;
                string netWSAAssemblyInfoUWP;
                string str7;
                switch (wsaSDK)
                {
                    case WSASDK.SDK80:
                        str5 = Path.Combine(Path.GetTempPath(), ".NETCore,Version=v4.5.AssemblyAttributes.cs");
                        netWSAAssemblyInfoUWP = this.GetNetWSAAssemblyInfoWindows80();
                        str7 = @"Managed\WinRTLegacy.dll";
                        break;

                    case WSASDK.SDK81:
                        str5 = Path.Combine(Path.GetTempPath(), ".NETCore,Version=v4.5.1.AssemblyAttributes.cs");
                        netWSAAssemblyInfoUWP = this.GetNetWSAAssemblyInfoWindows81();
                        str7 = @"Managed\WinRTLegacy.dll";
                        break;

                    case WSASDK.PhoneSDK81:
                        str5 = Path.Combine(Path.GetTempPath(), "WindowsPhoneApp,Version=v8.1.AssemblyAttributes.cs");
                        netWSAAssemblyInfoUWP = this.GetNetWSAAssemblyInfoWindowsPhone81();
                        str7 = @"Managed\Phone\WinRTLegacy.dll";
                        break;

                    case WSASDK.UWP:
                        str5 = Path.Combine(Path.GetTempPath(), ".NETCore,Version=v5.0.AssemblyAttributes.cs");
                        netWSAAssemblyInfoUWP = this.GetNetWSAAssemblyInfoUWP();
                        str7 = @"Managed\UAP\WinRTLegacy.dll";
                        break;

                    default:
                        throw new Exception("Unknown Windows SDK: " + EditorUserBuildSettings.wsaSDK.ToString());
                }
                str7 = Path.Combine(BuildPipeline.GetPlaybackEngineDirectory(this._island._target, BuildOptions.CompressTextures), str7);
                arguments.Add("/reference:\"" + str7.Replace('/', '\\') + "\"");
                if (File.Exists(str5))
                {
                    File.Delete(str5);
                }
                File.WriteAllText(str5, netWSAAssemblyInfoUWP);
                arguments.Add(str5);
            }
        }

        internal static string[] GetAdditionalReferences(WSASDK wsaSDK)
        {
            if (wsaSDK != WSASDK.UWP)
            {
                return null;
            }
            if (_uwpReferences == null)
            {
                _uwpReferences = UWPReferences.GetReferences();
            }
            return _uwpReferences;
        }

        private static ScriptingImplementation GetCurrentScriptingBackend()
        {
            int num = 0;
            if (!PlayerSettings.GetPropertyOptionalInt("ScriptingBackend", ref num, BuildTargetGroup.Metro))
            {
                num = 2;
            }
            return (ScriptingImplementation) num;
        }

        internal static string GetNETCoreFrameworkReferencesDirectory(WSASDK wsaSDK)
        {
            if (GetCurrentScriptingBackend() == ScriptingImplementation.IL2CPP)
            {
                return BuildPipeline.GetMonoLibDirectory(BuildTarget.WSAPlayer);
            }
            switch (wsaSDK)
            {
                case WSASDK.SDK80:
                    return (ProgramFilesDirectory + @"\Reference Assemblies\Microsoft\Framework\.NETCore\v4.5");

                case WSASDK.SDK81:
                    return (ProgramFilesDirectory + @"\Reference Assemblies\Microsoft\Framework\.NETCore\v4.5.1");

                case WSASDK.PhoneSDK81:
                    return (ProgramFilesDirectory + @"\Reference Assemblies\Microsoft\Framework\WindowsPhoneApp\v8.1");

                case WSASDK.UWP:
                    return null;
            }
            throw new Exception("Unknown Windows SDK: " + wsaSDK.ToString());
        }

        private string[] GetNETWSAAssemblies(WSASDK wsaSDK)
        {
            if ((wsaSDK != WSASDK.UWP) || (GetCurrentScriptingBackend() == ScriptingImplementation.IL2CPP))
            {
                return Directory.GetFiles(GetNETCoreFrameworkReferencesDirectory(wsaSDK), "*.dll");
            }
            NuGetPackageResolver resolver = new NuGetPackageResolver {
                ProjectLockFile = @"UWP\project.lock.json"
            };
            for (int i = !EnsureProjectLockFile(resolver) ? 1 : 2; i != 0; i--)
            {
                try
                {
                    resolver.Resolve();
                }
                catch (Exception)
                {
                    if (i <= 1)
                    {
                        throw;
                    }
                    Console.WriteLine("Failed to resolve NuGet packages. Deleting \"{0}\" and retrying.", Path.GetFullPath(resolver.ProjectLockFile));
                    File.Delete(resolver.ProjectLockFile);
                    EnsureProjectLockFile(resolver);
                }
            }
            return resolver.ResolvedReferences;
        }

        private string GetNetWSAAssemblyInfoUWP()
        {
            string[] textArray1 = new string[] { "using System;", "using System.Reflection;", "[assembly: global::System.Runtime.Versioning.TargetFrameworkAttribute(\".NETCore,Version=v5.0\", FrameworkDisplayName = \".NET for Windows Universal\")]" };
            return string.Join("\r\n", textArray1);
        }

        private string GetNetWSAAssemblyInfoWindows80()
        {
            string[] textArray1 = new string[] { "using System;", " using System.Reflection;", "[assembly: global::System.Runtime.Versioning.TargetFrameworkAttribute(\".NETCore,Version=v4.5\", FrameworkDisplayName = \".NET for Windows Store apps\")]" };
            return string.Join("\r\n", textArray1);
        }

        private string GetNetWSAAssemblyInfoWindows81()
        {
            string[] textArray1 = new string[] { "using System;", " using System.Reflection;", "[assembly: global::System.Runtime.Versioning.TargetFrameworkAttribute(\".NETCore,Version=v4.5.1\", FrameworkDisplayName = \".NET for Windows Store apps (Windows 8.1)\")]" };
            return string.Join("\r\n", textArray1);
        }

        private string GetNetWSAAssemblyInfoWindowsPhone81()
        {
            string[] textArray1 = new string[] { "using System;", " using System.Reflection;", "[assembly: global::System.Runtime.Versioning.TargetFrameworkAttribute(\"WindowsPhoneApp,Version=v8.1\", FrameworkDisplayName = \"Windows Phone 8.1\")]" };
            return string.Join("\r\n", textArray1);
        }

        protected static string GetPlatformAssemblyPath(WSASDK wsaSDK)
        {
            string windowsKitDirectory = GetWindowsKitDirectory(wsaSDK);
            if (wsaSDK == WSASDK.UWP)
            {
                return Path.Combine(windowsKitDirectory, @"UnionMetadata\Facade\Windows.winmd");
            }
            return Path.Combine(windowsKitDirectory, @"References\CommonConfiguration\Neutral\Windows.winmd");
        }

        protected override string[] GetStreamContainingCompilerMessages()
        {
            return base.GetStandardOutput();
        }

        internal static string GetWindowsKitDirectory(WSASDK wsaSDK)
        {
            string str;
            string str2;
            switch (wsaSDK)
            {
                case WSASDK.SDK80:
                    str2 = RegistryUtil.GetRegistryStringValue32(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v8.0", "InstallationFolder", null);
                    str = @"Windows Kits\8.0";
                    break;

                case WSASDK.SDK81:
                    str2 = RegistryUtil.GetRegistryStringValue32(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v8.1", "InstallationFolder", null);
                    str = @"Windows Kits\8.1";
                    break;

                case WSASDK.PhoneSDK81:
                    str2 = RegistryUtil.GetRegistryStringValue32(@"SOFTWARE\Microsoft\Microsoft SDKs\WindowsPhoneApp\v8.1", "InstallationFolder", null);
                    str = @"Windows Phone Kits\8.1";
                    break;

                case WSASDK.UWP:
                    str2 = RegistryUtil.GetRegistryStringValue32(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v10.0", "InstallationFolder", null);
                    str = @"Windows Kits\10.0";
                    break;

                default:
                    throw new Exception("Unknown Windows SDK: " + wsaSDK.ToString());
            }
            if (str2 == null)
            {
                str2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), str);
            }
            return str2;
        }

        protected override Program StartCompiler()
        {
            string str = ScriptCompilerBase.PrepareFileName(this._island._output);
            List<string> arguments = new List<string> {
                "/target:library",
                "/nowarn:0169",
                "/out:" + str
            };
            string[] collection = new string[] { "/debug:pdbonly", "/optimize+" };
            arguments.InsertRange(0, collection);
            string argsPrefix = string.Empty;
            if (base.CompilingForWSA() && ((PlayerSettings.WSA.compilationOverrides == PlayerSettings.WSACompilationOverrides.UseNetCore) || (PlayerSettings.WSA.compilationOverrides == PlayerSettings.WSACompilationOverrides.UseNetCorePartially)))
            {
                this.FillNETCoreCompilerOptions(EditorUserBuildSettings.wsaSDK, arguments, ref argsPrefix);
            }
            return this.StartCompilerImpl(arguments, argsPrefix, EditorUserBuildSettings.wsaSDK == WSASDK.UWP);
        }

        private Program StartCompilerImpl(List<string> arguments, string argsPrefix, bool msBuildCompiler)
        {
            string str4;
            foreach (string str in this._island._references)
            {
                arguments.Add("/reference:" + ScriptCompilerBase.PrepareFileName(str));
            }
            IEnumerator<string> enumerator = this._island._defines.Distinct<string>().GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    arguments.Add("/define:" + enumerator.Current);
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            foreach (string str3 in this._island._files)
            {
                arguments.Add(ScriptCompilerBase.PrepareFileName(str3).Replace('/', '\\'));
            }
            if (msBuildCompiler)
            {
                str4 = Path.Combine(ProgramFilesDirectory, @"MSBuild\14.0\Bin\csc.exe");
            }
            else
            {
                str4 = Path.Combine(WindowsDirectory, @"Microsoft.NET\Framework\v4.0.30319\Csc.exe");
            }
            if (!File.Exists(str4))
            {
                throw new Exception("'" + str4 + "' not found, either .NET 4.5 is not installed or your OS is not Windows 8/8.1.");
            }
            base.AddCustomResponseFileIfPresent(arguments, "csc.rsp");
            string str5 = CommandLineFormatter.GenerateResponseFile(arguments);
            ProcessStartInfo si = new ProcessStartInfo {
                Arguments = argsPrefix + "@" + str5,
                FileName = str4,
                CreateNoWindow = true
            };
            Program program = new Program(si);
            program.Start();
            return program;
        }

        internal static string ProgramFilesDirectory
        {
            get
            {
                string environmentVariable = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
                if (Directory.Exists(environmentVariable))
                {
                    return environmentVariable;
                }
                Debug.Log("Env variables ProgramFiles(x86) & ProgramFiles didn't exist, trying hard coded paths");
                string fullPath = Path.GetFullPath(WindowsDirectory + @"\..\..");
                string path = fullPath + "Program Files (x86)";
                string str5 = fullPath + "Program Files";
                if (Directory.Exists(path))
                {
                    return path;
                }
                if (Directory.Exists(str5))
                {
                    return str5;
                }
                string[] textArray1 = new string[] { "Path '", path, "' or '", str5, "' doesn't exist." };
                throw new Exception(string.Concat(textArray1));
            }
        }

        internal static string WindowsDirectory
        {
            get
            {
                return Environment.GetEnvironmentVariable("windir");
            }
        }
    }
}

