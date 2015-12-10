namespace UnityEditor
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using UnityEditorInternal;

    internal class CodeStrippingUtils
    {
        public static readonly string[] NativeClassBlackList = new string[] { "PreloadData", "Material", "Cubemap", "Texture3D", "RenderTexture", "Mesh" };
        private static readonly string[] s_UserAssemblies = new string[] { "Assembly-CSharp.dll", "Assembly-CSharp-firstpass.dll", "Assembly-UnityScript.dll", "Assembly-UnityScript-firstpass.dll", "UnityEngine.Analytics.dll" };

        private static HashSet<string> CollectManagedTypeReferencesFromRoots(string directory, string[] rootAssemblies)
        {
            HashSet<string> set = new HashSet<string>();
            AssemblyReferenceChecker checker = new AssemblyReferenceChecker();
            bool withMethods = false;
            bool ignoreSystemDlls = false;
            checker.CollectReferencesFromRoots(directory, rootAssemblies, withMethods, 0f, ignoreSystemDlls);
            string[] assemblyFileNames = checker.GetAssemblyFileNames();
            AssemblyDefinition[] assemblyDefinitions = checker.GetAssemblyDefinitions();
            foreach (AssemblyDefinition definition in assemblyDefinitions)
            {
                foreach (TypeDefinition definition2 in definition.MainModule.Types)
                {
                    if (definition2.Namespace.StartsWith("UnityEngine") && (((definition2.Fields.Count > 0) || (definition2.Methods.Count > 0)) || (definition2.Properties.Count > 0)))
                    {
                        string name = definition2.Name;
                        set.Add(name);
                    }
                }
            }
            AssemblyDefinition definition3 = null;
            for (int i = 0; i < assemblyFileNames.Length; i++)
            {
                if (assemblyFileNames[i] == "UnityEngine.dll")
                {
                    definition3 = assemblyDefinitions[i];
                }
            }
            foreach (AssemblyDefinition definition4 in assemblyDefinitions)
            {
                if (definition4 != definition3)
                {
                    IEnumerator<TypeReference> enumerator = definition4.MainModule.GetTypeReferences().GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            TypeReference current = enumerator.Current;
                            if (current.Namespace.StartsWith("UnityEngine"))
                            {
                                string item = current.Name;
                                set.Add(item);
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
            return set;
        }

        private static HashSet<string> CollectNativeClassListFromRoots(string directory, string[] rootAssemblies)
        {
            HashSet<string> set = new HashSet<string>();
            foreach (string str in CollectManagedTypeReferencesFromRoots(directory, rootAssemblies))
            {
                int iD = BaseObjectTools.StringToClassID(str);
                if ((iD != -1) && !BaseObjectTools.IsBaseObject(iD))
                {
                    set.Add(str);
                }
            }
            return set;
        }

        private static void ExcludeModuleManagers(ref HashSet<string> nativeClasses)
        {
            string[] moduleNames = ModuleMetadata.GetModuleNames();
            int derivedFromClassID = BaseObjectTools.StringToClassID("GlobalGameManager");
            foreach (string str in moduleNames)
            {
                if (ModuleMetadata.GetModuleStrippable(str))
                {
                    int[] moduleClasses = ModuleMetadata.GetModuleClasses(str);
                    HashSet<int> set = new HashSet<int>();
                    HashSet<string> other = new HashSet<string>();
                    foreach (int num3 in moduleClasses)
                    {
                        if (BaseObjectTools.IsDerivedFromClassID(num3, derivedFromClassID))
                        {
                            set.Add(num3);
                        }
                        else
                        {
                            other.Add(BaseObjectTools.ClassIDToString(num3));
                        }
                    }
                    if ((other.Count != 0) && !nativeClasses.Overlaps(other))
                    {
                        foreach (int num5 in set)
                        {
                            nativeClasses.Remove(BaseObjectTools.ClassIDToString(num5));
                        }
                    }
                }
            }
        }

        public static void GenerateDependencies(string strippedAssemblyDir, string icallsListFile, RuntimeClassRegistry rcr, out HashSet<string> nativeClasses, out HashSet<string> nativeModules)
        {
            string[] userAssemblies = GetUserAssemblies(strippedAssemblyDir);
            nativeClasses = !PlayerSettings.stripEngineCode ? null : GenerateNativeClassList(rcr, strippedAssemblyDir, userAssemblies);
            if (nativeClasses != null)
            {
                ExcludeModuleManagers(ref nativeClasses);
            }
            nativeModules = GetNativeModulesToRegister(nativeClasses);
            if ((nativeClasses != null) && (icallsListFile != null))
            {
                HashSet<string> modulesFromICalls = GetModulesFromICalls(icallsListFile);
                int derivedFromClassID = BaseObjectTools.StringToClassID("GlobalGameManager");
                foreach (string str in modulesFromICalls)
                {
                    foreach (int num2 in ModuleMetadata.GetModuleClasses(str))
                    {
                        if (BaseObjectTools.IsDerivedFromClassID(num2, derivedFromClassID))
                        {
                            nativeClasses.Add(BaseObjectTools.ClassIDToString(num2));
                        }
                    }
                }
                nativeModules.UnionWith(modulesFromICalls);
            }
            new AssemblyReferenceChecker().CollectReferencesFromRoots(strippedAssemblyDir, userAssemblies, true, 0f, true);
        }

        private static HashSet<string> GenerateNativeClassList(RuntimeClassRegistry rcr, string directory, string[] rootAssemblies)
        {
            HashSet<string> set = CollectNativeClassListFromRoots(directory, rootAssemblies);
            foreach (string str in NativeClassBlackList)
            {
                set.Add(str);
            }
            foreach (string str2 in rcr.GetAllNativeClassesIncludingManagersAsString())
            {
                int iD = BaseObjectTools.StringToClassID(str2);
                if ((iD != -1) && !BaseObjectTools.IsBaseObject(iD))
                {
                    set.Add(str2);
                }
            }
            HashSet<string> set2 = new HashSet<string>();
            foreach (string str3 in set)
            {
                for (int i = BaseObjectTools.StringToClassID(str3); !BaseObjectTools.IsBaseObject(i); i = BaseObjectTools.GetSuperClassID(i))
                {
                    set2.Add(BaseObjectTools.ClassIDToString(i));
                }
            }
            return set2;
        }

        private static HashSet<string> GetAllStrippableModules()
        {
            HashSet<string> set = new HashSet<string>();
            foreach (string str in ModuleMetadata.GetModuleNames())
            {
                if (ModuleMetadata.GetModuleStrippable(str))
                {
                    set.Add(str);
                }
            }
            return set;
        }

        private static IEnumerable<string> GetAssembliesInDirectory(string strippedAssemblyDir, string assemblyName)
        {
            return Directory.GetFiles(strippedAssemblyDir, assemblyName, SearchOption.TopDirectoryOnly);
        }

        private static HashSet<string> GetClassNames(IEnumerable<int> classIds)
        {
            HashSet<string> set = new HashSet<string>();
            IEnumerator<int> enumerator = classIds.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    int current = enumerator.Current;
                    set.Add(BaseObjectTools.ClassIDToString(current));
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            return set;
        }

        public static HashSet<string> GetModulesFromICalls(string icallsListFile)
        {
            string[] strArray = File.ReadAllLines(icallsListFile);
            HashSet<string> set = new HashSet<string>();
            foreach (string str in strArray)
            {
                string iCallModule = ModuleMetadata.GetICallModule(str);
                if (!string.IsNullOrEmpty(iCallModule))
                {
                    set.Add(iCallModule);
                }
            }
            return set;
        }

        public static HashSet<string> GetNativeModulesToRegister(HashSet<string> nativeClasses)
        {
            return ((nativeClasses != null) ? GetRequiredStrippableModules(nativeClasses) : GetAllStrippableModules());
        }

        private static HashSet<string> GetRequiredStrippableModules(HashSet<string> nativeClasses)
        {
            HashSet<string> set = new HashSet<string>();
            foreach (string str in ModuleMetadata.GetModuleNames())
            {
                if (ModuleMetadata.GetModuleStrippable(str))
                {
                    HashSet<string> classNames = GetClassNames(ModuleMetadata.GetModuleClasses(str));
                    if (nativeClasses.Overlaps(classNames))
                    {
                        set.Add(str);
                    }
                }
            }
            return set;
        }

        private static string[] GetUserAssemblies(string strippedAssemblyDir)
        {
            List<string> list = new List<string>();
            foreach (string str in s_UserAssemblies)
            {
                list.AddRange(GetAssembliesInDirectory(strippedAssemblyDir, str));
            }
            return list.ToArray();
        }

        private static void WriteModuleAndClassRegistrationFile(string file, HashSet<string> nativeModules, HashSet<string> nativeClasses, HashSet<string> classesToSkip)
        {
            using (TextWriter writer = new StreamWriter(file))
            {
                WriteStaticallyLinkedModuleRegistration(writer, nativeModules, nativeClasses);
                writer.WriteLine();
                writer.WriteLine("void RegisterAllClasses()");
                writer.WriteLine("{");
                if (nativeClasses == null)
                {
                    writer.WriteLine("\tvoid RegisterAllClassesGranular();");
                    writer.WriteLine("\tRegisterAllClassesGranular();");
                }
                else
                {
                    writer.WriteLine("\t//Total: {0} classes", nativeClasses.Count);
                    int num = 0;
                    foreach (string str in nativeClasses)
                    {
                        writer.WriteLine("\t//{0}. {1}", num, str);
                        if (classesToSkip.Contains(str))
                        {
                            writer.WriteLine("\t//Skipping {0}", str);
                        }
                        else
                        {
                            writer.WriteLine("\tvoid RegisterClass_{0}();", str);
                            writer.WriteLine("\tRegisterClass_{0}();", str);
                        }
                        writer.WriteLine();
                        num++;
                    }
                }
                writer.WriteLine("}");
                writer.Close();
            }
        }

        public static void WriteModuleAndClassRegistrationFile(string strippedAssemblyDir, string icallsListFile, string outputDir, RuntimeClassRegistry rcr, IEnumerable<string> classesToSkip)
        {
            HashSet<string> set;
            HashSet<string> set2;
            GenerateDependencies(strippedAssemblyDir, icallsListFile, rcr, out set, out set2);
            WriteModuleAndClassRegistrationFile(Path.Combine(outputDir, "UnityClassRegistration.cpp"), set2, set, new HashSet<string>(classesToSkip));
        }

        private static void WriteStaticallyLinkedModuleRegistration(TextWriter w, HashSet<string> nativeModules, HashSet<string> nativeClasses)
        {
            w.WriteLine("struct ClassRegistrationContext;");
            w.WriteLine("void InvokeRegisterStaticallyLinkedModuleClasses(ClassRegistrationContext& context)");
            w.WriteLine("{");
            if (nativeClasses == null)
            {
                w.WriteLine("\tvoid RegisterStaticallyLinkedModuleClasses(ClassRegistrationContext&);");
                w.WriteLine("\tRegisterStaticallyLinkedModuleClasses(context);");
            }
            else
            {
                w.WriteLine("\t// Do nothing (we're in stripping mode)");
            }
            w.WriteLine("}");
            w.WriteLine();
            w.WriteLine("void RegisterStaticallyLinkedModulesGranular()");
            w.WriteLine("{");
            foreach (string str in nativeModules)
            {
                w.WriteLine("\tvoid RegisterModule_" + str + "();");
                w.WriteLine("\tRegisterModule_" + str + "();");
                w.WriteLine();
            }
            w.WriteLine("}");
        }

        public static string[] UserAssemblies
        {
            get
            {
                return s_UserAssemblies;
            }
        }
    }
}

