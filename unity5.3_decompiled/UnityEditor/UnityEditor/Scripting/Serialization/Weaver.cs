namespace UnityEditor.Scripting.Serialization
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Unity.UNetWeaver;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEditor.Scripting;
    using UnityEditor.Utils;
    using UnityEditorInternal;
    using UnityEngine;

    internal static class Weaver
    {
        [CompilerGenerated]
        private static Func<string, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<MonoIsland, bool> <>f__am$cache1;

        private static ICompilationExtension GetCompilationExtension()
        {
            return ModuleManager.GetCompilationExtension(ModuleManager.GetTargetStringFromBuildTarget(EditorUserBuildSettings.activeBuildTarget));
        }

        public static string[] GetReferences(MonoIsland island, string projectDirectory)
        {
            List<string> list = new List<string>();
            List<string> first = new List<string>();
            IEnumerator<string> enumerator = first.Union<string>(island._references).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    string current = enumerator.Current;
                    string fileName = Path.GetFileName(current);
                    if (string.IsNullOrEmpty(fileName) || (!fileName.Contains("UnityEditor.dll") && !fileName.Contains("UnityEngine.dll")))
                    {
                        string file = !Path.IsPathRooted(current) ? Path.Combine(projectDirectory, current) : current;
                        if (AssemblyHelper.IsManagedAssembly(file) && !AssemblyHelper.IsInternalAssembly(file))
                        {
                            list.Add(file);
                        }
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
            return list.ToArray();
        }

        private static ManagedProgram ManagedProgramFor(string exe, string arguments)
        {
            return new ManagedProgram(MonoInstallationFinder.GetMonoInstallation("MonoBleedingEdge"), "4.0", exe, arguments);
        }

        private static void QueryAssemblyPathsAndResolver(ICompilationExtension compilationExtension, string file, bool editor, out string[] assemblyPaths, out IAssemblyResolver assemblyResolver)
        {
            assemblyResolver = compilationExtension.GetAssemblyResolver(editor, file, null);
            assemblyPaths = compilationExtension.GetCompilerExtraAssemblyPaths(editor, file).ToArray<string>();
        }

        private static ManagedProgram SerializationWeaverProgramWith(string arguments, string playerPackage)
        {
            return ManagedProgramFor(playerPackage + "/SerializationWeaver/SerializationWeaver.exe", arguments);
        }

        public static bool ShouldWeave(string name)
        {
            if (name.Contains("Boo."))
            {
                return false;
            }
            if (name.Contains("Mono."))
            {
                return false;
            }
            if (name.Contains("System"))
            {
                return false;
            }
            if (!name.EndsWith(".dll"))
            {
                return false;
            }
            return true;
        }

        public static void WeaveAssembliesInFolder(string folder, string playerPackage)
        {
            ICompilationExtension compilationExtension = GetCompilationExtension();
            string unityEngine = Path.Combine(folder, "UnityEngine.dll");
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = f => ShouldWeave(Path.GetFileName(f));
            }
            IEnumerator<string> enumerator = Directory.GetFiles(folder).Where<string>(<>f__am$cache0).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    IAssemblyResolver resolver;
                    string[] strArray;
                    string current = enumerator.Current;
                    QueryAssemblyPathsAndResolver(compilationExtension, current, false, out strArray, out resolver);
                    WeaveInto(current, current, unityEngine, playerPackage, strArray, resolver);
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

        public static bool WeaveInto(string unityUNet, string destPath, string unityEngine, string assemblyPath, string[] extraAssemblyPaths, IAssemblyResolver assemblyResolver)
        {
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = i => 0 < i._files.Length;
            }
            IEnumerable<MonoIsland> enumerable = InternalEditorUtility.GetMonoIslands().Where<MonoIsland>(<>f__am$cache1);
            string fullName = Directory.GetParent(Application.dataPath).FullName;
            string[] references = null;
            IEnumerator<MonoIsland> enumerator = enumerable.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    MonoIsland current = enumerator.Current;
                    if (destPath.Equals(current._output))
                    {
                        references = GetReferences(current, fullName);
                        goto Label_008C;
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
        Label_008C:
            if (references == null)
            {
                Debug.LogError("Weaver failure: unable to locate assemblies (no matching project) for: " + destPath);
                return false;
            }
            List<string> list = new List<string>();
            foreach (string str2 in references)
            {
                list.Add(Path.GetDirectoryName(str2));
            }
            if (extraAssemblyPaths != null)
            {
                list.AddRange(extraAssemblyPaths);
            }
            try
            {
                string[] assemblies = new string[] { assemblyPath };
                if (!Program.Process(unityEngine, unityUNet, Path.GetDirectoryName(destPath), assemblies, list.ToArray(), assemblyResolver, new Action<string>(Debug.LogWarning), new Action<string>(Debug.LogError)))
                {
                    Debug.LogError("Failure generating network code.");
                    return false;
                }
            }
            catch (Exception exception)
            {
                Debug.LogError("Exception generating network code: " + exception.ToString() + " " + exception.StackTrace);
            }
            return true;
        }

        public static bool WeaveUnetFromEditor(string assemblyPath, string destPath, string unityEngine, string unityUNet, bool buildingForEditor)
        {
            IAssemblyResolver resolver;
            string[] strArray;
            Console.WriteLine("WeaveUnetFromEditor " + assemblyPath);
            QueryAssemblyPathsAndResolver(GetCompilationExtension(), assemblyPath, buildingForEditor, out strArray, out resolver);
            return WeaveInto(unityUNet, destPath, unityEngine, assemblyPath, strArray, resolver);
        }
    }
}

