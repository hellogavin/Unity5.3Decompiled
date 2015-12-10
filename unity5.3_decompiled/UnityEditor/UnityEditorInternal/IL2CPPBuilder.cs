namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Scripting.Compilers;

    internal class IL2CPPBuilder
    {
        [CompilerGenerated]
        private static Predicate<string> <>f__am$cache7;
        [CompilerGenerated]
        private static Func<string, string> <>f__am$cache8;
        [CompilerGenerated]
        private static Func<string, string> <>f__am$cache9;
        [CompilerGenerated]
        private static Func<string, string, string> <>f__am$cacheA;
        private readonly bool m_DevelopmentBuild;
        private readonly LinkXmlReader m_linkXmlReader = new LinkXmlReader();
        private readonly Action<string> m_ModifyOutputBeforeCompile;
        private readonly IIl2CppPlatformProvider m_PlatformProvider;
        private readonly RuntimeClassRegistry m_RuntimeClassRegistry;
        private readonly string m_StagingAreaData;
        private readonly string m_TempFolder;

        public IL2CPPBuilder(string tempFolder, string stagingAreaData, IIl2CppPlatformProvider platformProvider, Action<string> modifyOutputBeforeCompile, RuntimeClassRegistry runtimeClassRegistry, bool developmentBuild)
        {
            this.m_TempFolder = tempFolder;
            this.m_StagingAreaData = stagingAreaData;
            this.m_PlatformProvider = platformProvider;
            this.m_ModifyOutputBeforeCompile = modifyOutputBeforeCompile;
            this.m_RuntimeClassRegistry = runtimeClassRegistry;
            this.m_DevelopmentBuild = developmentBuild;
        }

        private void ConvertPlayerDlltoCpp(ICollection<string> userAssemblies, string outputDirectory, string workingDirectory)
        {
            FileUtil.CreateOrCleanDirectory(outputDirectory);
            if (userAssemblies.Count != 0)
            {
                if (<>f__am$cache8 == null)
                {
                    <>f__am$cache8 = s => Path.Combine(Directory.GetCurrentDirectory(), s);
                }
                string[] strArray = Directory.GetFiles("Assets", "il2cpp_extra_types.txt", SearchOption.AllDirectories).Select<string, string>(<>f__am$cache8).ToArray<string>();
                string exe = this.GetIl2CppExe();
                List<string> source = new List<string> { "--convert-to-cpp", "--copy-level=None" };
                if (this.m_PlatformProvider.emitNullChecks)
                {
                    source.Add("--emit-null-checks");
                }
                if (this.m_PlatformProvider.enableStackTraces)
                {
                    source.Add("--enable-stacktrace");
                }
                if (this.m_PlatformProvider.enableArrayBoundsCheck)
                {
                    source.Add("--enable-array-bounds-check");
                }
                if (this.m_PlatformProvider.compactMode)
                {
                    source.Add("--output-format=Compact");
                }
                if (this.m_PlatformProvider.loadSymbols)
                {
                    source.Add("--enable-symbol-loading");
                }
                if (this.m_PlatformProvider.developmentMode)
                {
                    source.Add("--development-mode");
                }
                if (strArray.Length > 0)
                {
                    foreach (string str2 in strArray)
                    {
                        source.Add(string.Format("--extra-types.file=\"{0}\"", str2));
                    }
                }
                string path = Path.Combine(this.m_PlatformProvider.il2CppFolder, "il2cpp_default_extra_types.txt");
                if (File.Exists(path))
                {
                    source.Add(string.Format("--extra-types.file=\"{0}\"", path));
                }
                string str4 = string.Empty;
                if (PlayerSettings.GetPropertyOptionalString("additionalIl2CppArgs", ref str4))
                {
                    source.Add(str4);
                }
                List<string> list2 = new List<string>(userAssemblies);
                if (<>f__am$cache9 == null)
                {
                    <>f__am$cache9 = arg => "--assembly=\"" + Path.GetFullPath(arg) + "\"";
                }
                source.AddRange(list2.Select<string, string>(<>f__am$cache9));
                source.Add(string.Format("--generatedcppdir=\"{0}\"", Path.GetFullPath(outputDirectory)));
                if (<>f__am$cacheA == null)
                {
                    <>f__am$cacheA = (current, arg) => current + arg + " ";
                }
                string args = source.Aggregate<string, string>(string.Empty, <>f__am$cacheA);
                Console.WriteLine("Invoking il2cpp with arguments: " + args);
                if (EditorUtility.DisplayCancelableProgressBar("Building Player", "Converting managed assemblies to C++", 0.3f))
                {
                    throw new OperationCanceledException();
                }
                Runner.RunManagedProgram(exe, args, workingDirectory, new Il2CppOutputParser());
            }
        }

        private IEnumerable<string> FilterUserAssemblies(IEnumerable<string> assemblies, Predicate<string> isUsed, string managedDir)
        {
            <FilterUserAssemblies>c__AnonStorey6F storeyf = new <FilterUserAssemblies>c__AnonStorey6F {
                isUsed = isUsed,
                managedDir = managedDir
            };
            return assemblies.Where<string>(new Func<string, bool>(storeyf.<>m__F8)).Select<string, string>(new Func<string, string>(storeyf.<>m__F9));
        }

        public string GetCppOutputDirectoryInStagingArea()
        {
            return GetCppOutputPath(this.m_TempFolder);
        }

        public static string GetCppOutputPath(string tempFolder)
        {
            return Path.Combine(tempFolder, "il2cppOutput");
        }

        private string GetIl2CppExe()
        {
            return (this.m_PlatformProvider.il2CppFolder + "/build/il2cpp.exe");
        }

        private HashSet<string> GetUserAssemblies(string managedDir)
        {
            HashSet<string> set = new HashSet<string>();
            set.UnionWith(this.FilterUserAssemblies(this.m_RuntimeClassRegistry.GetUserAssemblies(), new Predicate<string>(this.m_RuntimeClassRegistry.IsDLLUsed), managedDir));
            set.UnionWith(this.FilterUserAssemblies(Directory.GetFiles(managedDir, "*.dll", SearchOption.TopDirectoryOnly), new Predicate<string>(this.m_linkXmlReader.IsDLLUsed), managedDir));
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = assembly => true;
            }
            set.UnionWith(this.FilterUserAssemblies(Directory.GetFiles(managedDir, "I18N*.dll", SearchOption.TopDirectoryOnly), <>f__am$cache7, managedDir));
            return set;
        }

        internal List<string> GetUserAssembliesToConvert(string managedDir)
        {
            HashSet<string> userAssemblies = this.GetUserAssemblies(managedDir);
            userAssemblies.Add(Directory.GetFiles(managedDir, "UnityEngine.dll", SearchOption.TopDirectoryOnly).Single<string>());
            return userAssemblies.ToList<string>();
        }

        public void Run()
        {
            string cppOutputDirectoryInStagingArea = this.GetCppOutputDirectoryInStagingArea();
            string fullPath = Path.GetFullPath(Path.Combine(this.m_StagingAreaData, "Managed"));
            foreach (string str3 in Directory.GetFiles(fullPath))
            {
                FileInfo info = new FileInfo(str3) {
                    IsReadOnly = false
                };
            }
            AssemblyStripper.StripAssemblies(this.m_StagingAreaData, this.m_PlatformProvider, this.m_RuntimeClassRegistry, this.m_DevelopmentBuild);
            this.ConvertPlayerDlltoCpp(this.GetUserAssembliesToConvert(fullPath), cppOutputDirectoryInStagingArea, fullPath);
            if (this.m_ModifyOutputBeforeCompile != null)
            {
                this.m_ModifyOutputBeforeCompile(cppOutputDirectoryInStagingArea);
            }
            if (this.m_PlatformProvider.CreateNativeCompiler() != null)
            {
                string path = Path.Combine(this.m_StagingAreaData, "Native");
                Directory.CreateDirectory(path);
                path = Path.Combine(path, this.m_PlatformProvider.nativeLibraryFileName);
                List<string> includePaths = new List<string>(this.m_PlatformProvider.includePaths) {
                    cppOutputDirectoryInStagingArea
                };
                this.m_PlatformProvider.CreateNativeCompiler().CompileDynamicLibrary(path, NativeCompiler.AllSourceFilesIn(cppOutputDirectoryInStagingArea), includePaths, this.m_PlatformProvider.libraryPaths, new string[0]);
            }
        }

        [CompilerGenerated]
        private sealed class <FilterUserAssemblies>c__AnonStorey6F
        {
            internal Predicate<string> isUsed;
            internal string managedDir;

            internal bool <>m__F8(string assembly)
            {
                return this.isUsed(assembly);
            }

            internal string <>m__F9(string usedAssembly)
            {
                return Path.Combine(this.managedDir, usedAssembly);
            }
        }
    }
}

