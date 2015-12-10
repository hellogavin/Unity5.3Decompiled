namespace UnityEditor
{
    using Mono.Cecil;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;

    internal class MonoAOTRegistration
    {
        private static ArrayList BuildNativeMethodList(AssemblyDefinition[] assemblies)
        {
            ArrayList res = new ArrayList();
            foreach (AssemblyDefinition definition in assemblies)
            {
                if (!"System".Equals(definition.Name.Name))
                {
                    ExtractNativeMethodsFromTypes(definition.MainModule.Types, res);
                }
            }
            return res;
        }

        public static HashSet<string> BuildReferencedTypeList(AssemblyDefinition[] assemblies)
        {
            HashSet<string> set = new HashSet<string>();
            foreach (AssemblyDefinition definition in assemblies)
            {
                if (!definition.Name.Name.StartsWith("System") && !definition.Name.Name.Equals("UnityEngine"))
                {
                    IEnumerator<TypeReference> enumerator = definition.MainModule.GetTypeReferences().GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            TypeReference current = enumerator.Current;
                            set.Add(current.FullName);
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

        private static void ExtractNativeMethodsFromTypes(ICollection<TypeDefinition> types, ArrayList res)
        {
            IEnumerator<TypeDefinition> enumerator = types.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    TypeDefinition current = enumerator.Current;
                    foreach (MethodDefinition definition2 in current.Methods)
                    {
                        if ((definition2.IsStatic && definition2.IsPInvokeImpl) && definition2.PInvokeInfo.Module.Name.Equals("__Internal"))
                        {
                            if (res.Contains(definition2.Name))
                            {
                                throw new SystemException("Duplicate native method found : " + definition2.Name + ". Please check your source carefully.");
                            }
                            res.Add(definition2.Name);
                        }
                    }
                    if (current.HasNestedTypes)
                    {
                        ExtractNativeMethodsFromTypes(current.NestedTypes, res);
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

        private static void GenerateInternalCallMethod(TypeDefinition typeDefinition, MethodDefinition method, TextWriter output)
        {
            if (method.IsInternalCall)
            {
                string str = string.Format("\tvoid Register_{0}_{1}_{2} ();", typeDefinition.Namespace, typeDefinition.Name, method.Name);
                string str2 = string.Format("\tRegister_{0}_{1}_{2} ();", typeDefinition.Namespace, typeDefinition.Name, method.Name).Replace('.', '_');
                str = str.Replace('.', '_');
                if (!str2.Contains("UnityEngine.Serialization"))
                {
                    output.WriteLine(str);
                    output.WriteLine(str2);
                }
            }
        }

        public static void GenerateRegisterClasses(RuntimeClassRegistry allClasses, TextWriter output)
        {
            output.Write("void RegisterAllClasses() \n{\n");
            output.Write("void RegisterAllClassesIPhone();\nRegisterAllClassesIPhone();\n");
            output.Write("\n}\n");
        }

        public static void GenerateRegisterClassesForStripping(RuntimeClassRegistry allClasses, TextWriter output)
        {
            output.Write("void RegisterAllClasses() \n{\n");
            allClasses.SynchronizeClasses();
            foreach (string str in allClasses.GetAllNativeClassesAsString())
            {
                output.WriteLine(string.Format("extern int RegisterClass_{0}();\nRegisterClass_{0}();", str));
            }
            output.Write("\n}\n");
        }

        public static void GenerateRegisterInternalCalls(AssemblyDefinition[] assemblies, TextWriter output)
        {
            output.Write("void RegisterAllStrippedInternalCalls ()\n{\n");
            foreach (AssemblyDefinition definition in assemblies)
            {
                foreach (TypeDefinition definition2 in definition.MainModule.Types)
                {
                    foreach (MethodDefinition definition3 in definition2.Methods)
                    {
                        GenerateInternalCallMethod(definition2, definition3, output);
                    }
                }
            }
            output.Write("\n}\n");
        }

        public static void GenerateRegisterModules(RuntimeClassRegistry allClasses, TextWriter output, bool strippingEnabled)
        {
            allClasses.SynchronizeClasses();
            HashSet<string> nativeClasses = !strippingEnabled ? null : new HashSet<string>(allClasses.GetAllNativeClassesAsString());
            HashSet<string> nativeModulesToRegister = CodeStrippingUtils.GetNativeModulesToRegister(nativeClasses);
            nativeModulesToRegister.Add("IMGUI");
            foreach (string str in nativeModulesToRegister)
            {
                output.WriteLine("\textern \"C\" void RegisterModule_" + str + "();");
            }
            output.WriteLine("void RegisterStaticallyLinkedModules()");
            output.WriteLine("{");
            foreach (string str2 in nativeModulesToRegister)
            {
                output.WriteLine("\tRegisterModule_" + str2 + "();");
            }
            output.WriteLine("}");
        }

        public static void ResolveDefinedNativeClassesFromMono(AssemblyDefinition[] assemblies, RuntimeClassRegistry res)
        {
            if (res != null)
            {
                foreach (AssemblyDefinition definition in assemblies)
                {
                    foreach (TypeDefinition definition2 in definition.MainModule.Types)
                    {
                        if (((definition2.Fields.Count > 0) || (definition2.Methods.Count > 0)) || (definition2.Properties.Count > 0))
                        {
                            string name = definition2.Name;
                            res.AddMonoClass(name);
                        }
                    }
                }
            }
        }

        public static void ResolveReferencedUnityEngineClassesFromMono(AssemblyDefinition[] assemblies, AssemblyDefinition unityEngine, RuntimeClassRegistry res)
        {
            if (res != null)
            {
                foreach (AssemblyDefinition definition in assemblies)
                {
                    if (definition != unityEngine)
                    {
                        IEnumerator<TypeReference> enumerator = definition.MainModule.GetTypeReferences().GetEnumerator();
                        try
                        {
                            while (enumerator.MoveNext())
                            {
                                TypeReference current = enumerator.Current;
                                if (current.Namespace.StartsWith("UnityEngine"))
                                {
                                    string name = current.Name;
                                    res.AddMonoClass(name);
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
            }
        }

        public static void WriteCPlusPlusFileForStaticAOTModuleRegistration(BuildTarget buildTarget, string file, CrossCompileOptions crossCompileOptions, bool advancedLic, string targetDevice, bool stripping, RuntimeClassRegistry usedClassRegistry, AssemblyReferenceChecker checker)
        {
            using (TextWriter writer = new StreamWriter(file))
            {
                string[] assemblyFileNames = checker.GetAssemblyFileNames();
                AssemblyDefinition[] assemblyDefinitions = checker.GetAssemblyDefinitions();
                bool flag = (crossCompileOptions & CrossCompileOptions.FastICall) != CrossCompileOptions.Dynamic;
                ArrayList list = BuildNativeMethodList(assemblyDefinitions);
                if (buildTarget == BuildTarget.iOS)
                {
                    writer.WriteLine("#include \"RegisterMonoModules.h\"");
                    writer.WriteLine("#include <stdio.h>");
                }
                writer.WriteLine(string.Empty);
                writer.WriteLine("#if defined(TARGET_IPHONE_SIMULATOR) && TARGET_IPHONE_SIMULATOR");
                writer.WriteLine("    #define DECL_USER_FUNC(f) void f() __attribute__((weak_import))");
                writer.WriteLine(@"    #define REGISTER_USER_FUNC(f)\");
                writer.WriteLine(@"        do {\");
                writer.WriteLine(@"        if(f != NULL)\");
                writer.WriteLine(@"            mono_dl_register_symbol(#f, (void*)f);\");
                writer.WriteLine(@"        else\");
                writer.WriteLine("            ::printf_console(\"Symbol '%s' not found. Maybe missing implementation for Simulator?\\n\", #f);\\");
                writer.WriteLine("        }while(0)");
                writer.WriteLine("#else");
                writer.WriteLine("    #define DECL_USER_FUNC(f) void f() ");
                writer.WriteLine("    #if !defined(__arm64__)");
                writer.WriteLine("    #define REGISTER_USER_FUNC(f) mono_dl_register_symbol(#f, (void*)&f)");
                writer.WriteLine("    #else");
                writer.WriteLine("        #define REGISTER_USER_FUNC(f)");
                writer.WriteLine("    #endif");
                writer.WriteLine("#endif");
                writer.WriteLine("extern \"C\"\n{");
                writer.WriteLine("    typedef void* gpointer;");
                writer.WriteLine("    typedef int gboolean;");
                if (buildTarget == BuildTarget.iOS)
                {
                    writer.WriteLine("    const char*         UnityIPhoneRuntimeVersion = \"{0}\";", Application.unityVersion);
                    writer.WriteLine("    void                mono_dl_register_symbol (const char* name, void *addr);");
                    writer.WriteLine("#if !defined(__arm64__)");
                    writer.WriteLine("    extern int          mono_ficall_flag;");
                    writer.WriteLine("#endif");
                }
                writer.WriteLine("    void                mono_aot_register_module(gpointer *aot_info);");
                writer.WriteLine("#if __ORBIS__ || SN_TARGET_PSP2");
                writer.WriteLine("#define DLL_EXPORT __declspec(dllexport)");
                writer.WriteLine("#else");
                writer.WriteLine("#define DLL_EXPORT");
                writer.WriteLine("#endif");
                writer.WriteLine("#if !(TARGET_IPHONE_SIMULATOR)");
                writer.WriteLine("    extern gboolean     mono_aot_only;");
                for (int i = 0; i < assemblyFileNames.Length; i++)
                {
                    string str = assemblyFileNames[i];
                    string str2 = assemblyDefinitions[i].Name.Name.Replace(".", "_").Replace("-", "_").Replace(" ", "_");
                    writer.WriteLine("    extern gpointer*    mono_aot_module_{0}_info; // {1}", str2, str);
                }
                writer.WriteLine("#endif // !(TARGET_IPHONE_SIMULATOR)");
                IEnumerator enumerator = list.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        string current = (string) enumerator.Current;
                        writer.WriteLine("    DECL_USER_FUNC({0});", current);
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
                writer.WriteLine("}");
                writer.WriteLine("DLL_EXPORT void RegisterMonoModules()");
                writer.WriteLine("{");
                writer.WriteLine("#if !(TARGET_IPHONE_SIMULATOR) && !defined(__arm64__)");
                writer.WriteLine("    mono_aot_only = true;");
                if (buildTarget == BuildTarget.iOS)
                {
                    writer.WriteLine("    mono_ficall_flag = {0};", !flag ? "false" : "true");
                }
                foreach (AssemblyDefinition definition in assemblyDefinitions)
                {
                    string str4 = definition.Name.Name.Replace(".", "_").Replace("-", "_").Replace(" ", "_");
                    writer.WriteLine("    mono_aot_register_module(mono_aot_module_{0}_info);", str4);
                }
                writer.WriteLine("#endif // !(TARGET_IPHONE_SIMULATOR) && !defined(__arm64__)");
                writer.WriteLine(string.Empty);
                if (buildTarget == BuildTarget.iOS)
                {
                    IEnumerator enumerator2 = list.GetEnumerator();
                    try
                    {
                        while (enumerator2.MoveNext())
                        {
                            string str5 = (string) enumerator2.Current;
                            writer.WriteLine("    REGISTER_USER_FUNC({0});", str5);
                        }
                    }
                    finally
                    {
                        IDisposable disposable2 = enumerator2 as IDisposable;
                        if (disposable2 == null)
                        {
                        }
                        disposable2.Dispose();
                    }
                }
                writer.WriteLine("}");
                writer.WriteLine(string.Empty);
                AssemblyDefinition unityEngine = null;
                for (int j = 0; j < assemblyFileNames.Length; j++)
                {
                    if (assemblyFileNames[j] == "UnityEngine.dll")
                    {
                        unityEngine = assemblyDefinitions[j];
                    }
                }
                if (buildTarget == BuildTarget.iOS)
                {
                    AssemblyDefinition[] assemblies = new AssemblyDefinition[] { unityEngine };
                    GenerateRegisterInternalCalls(assemblies, writer);
                    ResolveDefinedNativeClassesFromMono(assemblies, usedClassRegistry);
                    ResolveReferencedUnityEngineClassesFromMono(assemblyDefinitions, unityEngine, usedClassRegistry);
                    GenerateRegisterModules(usedClassRegistry, writer, stripping);
                    if (stripping && (usedClassRegistry != null))
                    {
                        GenerateRegisterClassesForStripping(usedClassRegistry, writer);
                    }
                    else
                    {
                        GenerateRegisterClasses(usedClassRegistry, writer);
                    }
                }
                writer.Close();
            }
        }
    }
}

