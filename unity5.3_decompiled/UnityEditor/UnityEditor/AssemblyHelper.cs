namespace UnityEditor
{
    using Mono.Cecil;
    using Mono.Collections.Generic;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using UnityEditor.Modules;
    using UnityEditorInternal;
    using UnityEngine;

    internal class AssemblyHelper
    {
        [CompilerGenerated]
        private static Func<PluginImporter, string> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<FileInfo, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<FileInfo, string> <>f__am$cache2;
        private const int kDefaultDepth = 10;

        private static void AddReferencedAssembliesRecurse(string assemblyPath, List<string> alreadyFoundAssemblies, string[] allAssemblyPaths, string[] foldersToSearch, Dictionary<string, AssemblyDefinition> cache, BuildTarget target)
        {
            <AddReferencedAssembliesRecurse>c__AnonStorey2C storeyc = new <AddReferencedAssembliesRecurse>c__AnonStorey2C {
                target = target
            };
            if (!IgnoreAssembly(assemblyPath, storeyc.target))
            {
                AssemblyDefinition assemblyDefinitionCached = GetAssemblyDefinitionCached(assemblyPath, cache);
                if (assemblyDefinitionCached == null)
                {
                    throw new ArgumentException("Referenced Assembly " + Path.GetFileName(assemblyPath) + " could not be found!");
                }
                if (alreadyFoundAssemblies.IndexOf(assemblyPath) == -1)
                {
                    alreadyFoundAssemblies.Add(assemblyPath);
                    if (<>f__am$cache0 == null)
                    {
                        <>f__am$cache0 = i => Path.GetFileName(i.assetPath);
                    }
                    IEnumerable<string> source = PluginImporter.GetImporters(storeyc.target).Where<PluginImporter>(new Func<PluginImporter, bool>(storeyc.<>m__40)).Select<PluginImporter, string>(<>f__am$cache0).Distinct<string>();
                    <AddReferencedAssembliesRecurse>c__AnonStorey2D storeyd = new <AddReferencedAssembliesRecurse>c__AnonStorey2D();
                    using (Collection<AssemblyNameReference>.Enumerator enumerator = assemblyDefinitionCached.MainModule.AssemblyReferences.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            storeyd.referencedAssembly = enumerator.Current;
                            if (((storeyd.referencedAssembly.Name != "BridgeInterface") && (storeyd.referencedAssembly.Name != "WinRTBridge")) && ((storeyd.referencedAssembly.Name != "UnityEngineProxy") && !IgnoreAssembly(storeyd.referencedAssembly.Name + ".dll", storeyc.target)))
                            {
                                string str = FindAssemblyName(storeyd.referencedAssembly.FullName, storeyd.referencedAssembly.Name, allAssemblyPaths, foldersToSearch, cache);
                                if (str == string.Empty)
                                {
                                    bool flag = false;
                                    <AddReferencedAssembliesRecurse>c__AnonStorey2E storeye = new <AddReferencedAssembliesRecurse>c__AnonStorey2E {
                                        <>f__ref$45 = storeyd
                                    };
                                    string[] strArray = new string[] { ".dll", ".winmd" };
                                    for (int j = 0; j < strArray.Length; j++)
                                    {
                                        storeye.extension = strArray[j];
                                        if (source.Any<string>(new Func<string, bool>(storeye.<>m__42)))
                                        {
                                            flag = true;
                                            break;
                                        }
                                    }
                                    if (flag)
                                    {
                                        continue;
                                    }
                                    throw new ArgumentException(string.Format("The Assembly {0} is referenced by {1} ('{2}'). But the dll is not allowed to be included or could not be found.", storeyd.referencedAssembly.Name, assemblyDefinitionCached.MainModule.Assembly.Name.Name, assemblyPath));
                                }
                                AddReferencedAssembliesRecurse(str, alreadyFoundAssemblies, allAssemblyPaths, foldersToSearch, cache, storeyc.target);
                            }
                        }
                    }
                }
            }
        }

        public static void CheckForAssemblyFileNameMismatch(string assemblyPath)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(assemblyPath);
            string str2 = ExtractInternalAssemblyName(assemblyPath);
            if (fileNameWithoutExtension != str2)
            {
                Debug.LogWarning("Assembly '" + str2 + "' has non matching file name: '" + Path.GetFileName(assemblyPath) + "'. This can cause build issues on some platforms.");
            }
        }

        public static void ExtractAllClassesThatInheritMonoBehaviourAndScriptableObject(string path, out string[] classNamesArray, out string[] classNameSpacesArray)
        {
            List<string> list = new List<string>();
            List<string> list2 = new List<string>();
            ReaderParameters parameters = new ReaderParameters();
            DefaultAssemblyResolver resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(Path.GetDirectoryName(path));
            parameters.AssemblyResolver = resolver;
            AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(path, parameters);
            foreach (ModuleDefinition definition2 in assembly.Modules)
            {
                foreach (TypeDefinition definition3 in definition2.Types)
                {
                    TypeReference baseType = definition3.BaseType;
                    try
                    {
                        if (IsTypeMonoBehaviourOrScriptableObject(assembly, baseType))
                        {
                            list.Add(definition3.Name);
                            list2.Add(definition3.Namespace);
                        }
                    }
                    catch (Exception)
                    {
                        Debug.LogError("Failed to extract " + definition3.FullName + " class of base type " + baseType.FullName + " when inspecting " + path);
                    }
                }
            }
            classNamesArray = list.ToArray();
            classNameSpacesArray = list2.ToArray();
        }

        public static AssemblyTypeInfoGenerator.ClassInfo[] ExtractAssemblyTypeInfo(BuildTarget targetPlatform, bool isEditor, string assemblyPathName, string[] searchDirs)
        {
            AssemblyTypeInfoGenerator.ClassInfo[] infoArray;
            try
            {
                AssemblyTypeInfoGenerator generator;
                ICompilationExtension compilationExtension = ModuleManager.GetCompilationExtension(ModuleManager.GetTargetStringFromBuildTarget(targetPlatform));
                string[] compilerExtraAssemblyPaths = compilationExtension.GetCompilerExtraAssemblyPaths(isEditor, assemblyPathName);
                if ((compilerExtraAssemblyPaths != null) && (compilerExtraAssemblyPaths.Length > 0))
                {
                    List<string> list = new List<string>(searchDirs);
                    list.AddRange(compilerExtraAssemblyPaths);
                    searchDirs = list.ToArray();
                }
                IAssemblyResolver resolver = compilationExtension.GetAssemblyResolver(isEditor, assemblyPathName, searchDirs);
                if (resolver == null)
                {
                    generator = new AssemblyTypeInfoGenerator(assemblyPathName, searchDirs);
                }
                else
                {
                    generator = new AssemblyTypeInfoGenerator(assemblyPathName, resolver);
                }
                infoArray = generator.GatherClassInfo();
            }
            catch (Exception exception)
            {
                object[] objArray1 = new object[] { "ExtractAssemblyTypeInfo: Failed to process ", assemblyPathName, ", ", exception };
                throw new Exception(string.Concat(objArray1));
            }
            return infoArray;
        }

        public static string ExtractInternalAssemblyName(string path)
        {
            return AssemblyDefinition.ReadAssembly(path).Name.Name;
        }

        internal static ICollection<string> FindAssemblies(string basePath)
        {
            return FindAssemblies(basePath, 10);
        }

        internal static ICollection<string> FindAssemblies(string basePath, int maxDepth)
        {
            List<string> list = new List<string>();
            if (maxDepth != 0)
            {
                try
                {
                    DirectoryInfo info = new DirectoryInfo(basePath);
                    if (<>f__am$cache1 == null)
                    {
                        <>f__am$cache1 = file => IsManagedAssembly(file.FullName);
                    }
                    if (<>f__am$cache2 == null)
                    {
                        <>f__am$cache2 = file => file.FullName;
                    }
                    list.AddRange(info.GetFiles().Where<FileInfo>(<>f__am$cache1).Select<FileInfo, string>(<>f__am$cache2));
                    foreach (DirectoryInfo info2 in info.GetDirectories())
                    {
                        list.AddRange(FindAssemblies(info2.FullName, maxDepth - 1));
                    }
                }
                catch (Exception)
                {
                }
            }
            return list;
        }

        public static string[] FindAssembliesReferencedBy(string[] paths, string[] foldersToSearch, BuildTarget target)
        {
            List<string> alreadyFoundAssemblies = new List<string>();
            string[] allAssemblyPaths = paths;
            Dictionary<string, AssemblyDefinition> cache = new Dictionary<string, AssemblyDefinition>();
            for (int i = 0; i < paths.Length; i++)
            {
                AddReferencedAssembliesRecurse(paths[i], alreadyFoundAssemblies, allAssemblyPaths, foldersToSearch, cache, target);
            }
            for (int j = 0; j < paths.Length; j++)
            {
                alreadyFoundAssemblies.Remove(paths[j]);
            }
            return alreadyFoundAssemblies.ToArray();
        }

        public static string[] FindAssembliesReferencedBy(string path, string[] foldersToSearch, BuildTarget target)
        {
            return FindAssembliesReferencedBy(new string[] { path }, foldersToSearch, target);
        }

        private static string FindAssemblyName(string fullName, string name, string[] allAssemblyPaths, string[] foldersToSearch, Dictionary<string, AssemblyDefinition> cache)
        {
            for (int i = 0; i < allAssemblyPaths.Length; i++)
            {
                if (GetAssemblyDefinitionCached(allAssemblyPaths[i], cache).MainModule.Assembly.Name.Name == name)
                {
                    return allAssemblyPaths[i];
                }
            }
            foreach (string str in foldersToSearch)
            {
                string path = Path.Combine(str, name + ".dll");
                if (File.Exists(path))
                {
                    return path;
                }
            }
            return string.Empty;
        }

        [DebuggerHidden]
        internal static IEnumerable<T> FindImplementors<T>(Assembly assembly) where T: class
        {
            return new <FindImplementors>c__Iterator2<T> { assembly = assembly, <$>assembly = assembly, $PC = -2 };
        }

        public static Assembly FindLoadedAssemblyWithName(string s)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    if (assembly.Location.Contains(s))
                    {
                        return assembly;
                    }
                }
                catch (NotSupportedException)
                {
                }
            }
            return null;
        }

        private static AssemblyDefinition GetAssemblyDefinitionCached(string path, Dictionary<string, AssemblyDefinition> cache)
        {
            if (cache.ContainsKey(path))
            {
                return cache[path];
            }
            AssemblyDefinition definition = AssemblyDefinition.ReadAssembly(path);
            cache[path] = definition;
            return definition;
        }

        public static string[] GetNamesOfAssembliesLoadedInCurrentDomain()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            List<string> list = new List<string>();
            foreach (Assembly assembly in assemblies)
            {
                try
                {
                    list.Add(assembly.Location);
                }
                catch (NotSupportedException)
                {
                }
            }
            return list.ToArray();
        }

        internal static Type[] GetTypesFromAssembly(Assembly assembly)
        {
            if (assembly == null)
            {
                return new Type[0];
            }
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException)
            {
                return new Type[0];
            }
        }

        private static bool IgnoreAssembly(string assemblyPath, BuildTarget target)
        {
            BuildTarget target2 = target;
            return ((((target2 == BuildTarget.WSAPlayer) || (target2 == BuildTarget.WP8Player)) && ((((assemblyPath.IndexOf("mscorlib.dll") != -1) || (assemblyPath.IndexOf("System.") != -1)) || ((assemblyPath.IndexOf("Windows.dll") != -1) || (assemblyPath.IndexOf("Microsoft.") != -1))) || (((assemblyPath.IndexOf("Windows.") != -1) || (assemblyPath.IndexOf("WinRTLegacy.dll") != -1)) || (assemblyPath.IndexOf("platform.dll") != -1)))) || IsInternalAssembly(assemblyPath));
        }

        public static bool IsInternalAssembly(string file)
        {
            return ModuleManager.IsRegisteredModule(file);
        }

        public static bool IsManagedAssembly(string file)
        {
            DllType type = InternalEditorUtility.DetectDotNetDll(file);
            return ((type != DllType.Unknown) && (type != DllType.Native));
        }

        private static bool IsTypeMonoBehaviourOrScriptableObject(AssemblyDefinition assembly, TypeReference type)
        {
            if (type == null)
            {
                return false;
            }
            if (type.FullName == "System.Object")
            {
                return false;
            }
            Assembly assembly2 = null;
            if (type.Scope.Name == "UnityEngine")
            {
                assembly2 = typeof(MonoBehaviour).Assembly;
            }
            else if (type.Scope.Name == "UnityEditor")
            {
                assembly2 = typeof(EditorWindow).Assembly;
            }
            else if (type.Scope.Name == "UnityEngine.UI")
            {
                assembly2 = FindLoadedAssemblyWithName("UnityEngine.UI");
            }
            if (assembly2 != null)
            {
                string name = !type.IsGenericInstance ? type.FullName : (type.Namespace + "." + type.Name);
                Type type2 = assembly2.GetType(name);
                if ((type2 == typeof(MonoBehaviour)) || type2.IsSubclassOf(typeof(MonoBehaviour)))
                {
                    return true;
                }
                if ((type2 == typeof(ScriptableObject)) || type2.IsSubclassOf(typeof(ScriptableObject)))
                {
                    return true;
                }
            }
            TypeDefinition definition = null;
            try
            {
                definition = type.Resolve();
            }
            catch (AssemblyResolutionException)
            {
            }
            return ((definition != null) && IsTypeMonoBehaviourOrScriptableObject(assembly, definition.BaseType));
        }

        public static bool IsWindowsRuntimeAssembly(string assemblyPath)
        {
            using (FileStream stream = File.Open(assemblyPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                return AssemblyDefinition.ReadAssembly(stream).Name.IsWindowsRuntime;
            }
        }

        [CompilerGenerated]
        private sealed class <AddReferencedAssembliesRecurse>c__AnonStorey2C
        {
            internal BuildTarget target;

            internal bool <>m__40(PluginImporter i)
            {
                string platformData = i.GetPlatformData(this.target, "CPU");
                return (!string.IsNullOrEmpty(platformData) && !string.Equals(platformData, "AnyCPU", StringComparison.InvariantCultureIgnoreCase));
            }
        }

        [CompilerGenerated]
        private sealed class <AddReferencedAssembliesRecurse>c__AnonStorey2D
        {
            internal AssemblyNameReference referencedAssembly;
        }

        [CompilerGenerated]
        private sealed class <AddReferencedAssembliesRecurse>c__AnonStorey2E
        {
            internal AssemblyHelper.<AddReferencedAssembliesRecurse>c__AnonStorey2D <>f__ref$45;
            internal string extension;

            internal bool <>m__42(string p)
            {
                return string.Equals(p, this.<>f__ref$45.referencedAssembly.Name + this.extension, StringComparison.InvariantCultureIgnoreCase);
            }
        }

        [CompilerGenerated]
        private sealed class <FindImplementors>c__Iterator2<T> : IDisposable, IEnumerator, IEnumerable, IEnumerable<T>, IEnumerator<T> where T: class
        {
            internal T $current;
            internal int $PC;
            internal Assembly <$>assembly;
            internal Type[] <$s_239>__1;
            internal int <$s_240>__2;
            internal Type <interfaze>__0;
            internal T <module>__4;
            internal Type <type>__3;
            internal Assembly assembly;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<interfaze>__0 = typeof(T);
                        this.<$s_239>__1 = AssemblyHelper.GetTypesFromAssembly(this.assembly);
                        this.<$s_240>__2 = 0;
                        goto Label_0138;

                    case 1:
                        break;

                    default:
                        goto Label_0152;
                }
            Label_012A:
                this.<$s_240>__2++;
            Label_0138:
                if (this.<$s_240>__2 < this.<$s_239>__1.Length)
                {
                    this.<type>__3 = this.<$s_239>__1[this.<$s_240>__2];
                    if ((!this.<type>__3.IsInterface && !this.<type>__3.IsAbstract) && this.<interfaze>__0.IsAssignableFrom(this.<type>__3))
                    {
                        this.<module>__4 = null;
                        if (typeof(ScriptableObject).IsAssignableFrom(this.<type>__3))
                        {
                            this.<module>__4 = ScriptableObject.CreateInstance(this.<type>__3) as T;
                        }
                        else
                        {
                            this.<module>__4 = Activator.CreateInstance(this.<type>__3) as T;
                        }
                        if (this.<module>__4 != null)
                        {
                            this.$current = this.<module>__4;
                            this.$PC = 1;
                            return true;
                        }
                    }
                    goto Label_012A;
                }
                this.$PC = -1;
            Label_0152:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new AssemblyHelper.<FindImplementors>c__Iterator2<T> { assembly = this.<$>assembly };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<T>.GetEnumerator();
            }

            T IEnumerator<T>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }
    }
}

