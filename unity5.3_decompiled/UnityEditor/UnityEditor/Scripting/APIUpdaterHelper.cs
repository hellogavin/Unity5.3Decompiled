namespace UnityEditor.Scripting
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using Unity.DataContract;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEditor.Scripting.Compilers;
    using UnityEditor.Utils;
    using UnityEngine;

    internal class APIUpdaterHelper
    {
        [CompilerGenerated]
        private static Func<Assembly, IEnumerable<Type>> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<string, Exception, string> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<KeyValuePair<string, PackageFileData>, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<KeyValuePair<string, PackageFileData>, string> <>f__am$cache3;

        private static string APIVersionArgument()
        {
            return (" --api-version " + Application.unityVersion + " ");
        }

        private static string AssemblySearchPathArgument()
        {
            string[] textArray1 = new string[] { " -s ", CommandLineFormatter.PrepareFileName(Path.Combine(MonoInstallationFinder.GetFrameWorksFolder(), "Managed")), ",+", CommandLineFormatter.PrepareFileName(Path.Combine(EditorApplication.applicationContentsPath, "UnityExtensions/Unity")), ",+", CommandLineFormatter.PrepareFileName(Application.dataPath) };
            return string.Concat(textArray1);
        }

        private static string ConfigurationProviderAssembliesPathArgument()
        {
            StringBuilder builder = new StringBuilder();
            IEnumerator<PackageInfo> enumerator = ModuleManager.packageManager.unityExtensions.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    PackageInfo current = enumerator.Current;
                    if (<>f__am$cache2 == null)
                    {
                        <>f__am$cache2 = f => f.Value.type == PackageFileType.Dll;
                    }
                    if (<>f__am$cache3 == null)
                    {
                        <>f__am$cache3 = pi => pi.Key;
                    }
                    IEnumerator<string> enumerator2 = current.files.Where<KeyValuePair<string, PackageFileData>>(<>f__am$cache2).Select<KeyValuePair<string, PackageFileData>, string>(<>f__am$cache3).GetEnumerator();
                    try
                    {
                        while (enumerator2.MoveNext())
                        {
                            string str = enumerator2.Current;
                            builder.AppendFormat(" {0}", CommandLineFormatter.PrepareFileName(Path.Combine(current.basePath, str)));
                        }
                        continue;
                    }
                    finally
                    {
                        if (enumerator2 == null)
                        {
                        }
                        enumerator2.Dispose();
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
            string unityEditorManagedPath = GetUnityEditorManagedPath();
            builder.AppendFormat(" {0}", CommandLineFormatter.PrepareFileName(Path.Combine(unityEditorManagedPath, "UnityEngine.dll")));
            builder.AppendFormat(" {0}", CommandLineFormatter.PrepareFileName(Path.Combine(unityEditorManagedPath, "UnityEditor.dll")));
            return builder.ToString();
        }

        public static bool DoesAssemblyRequireUpgrade(string assetFullPath)
        {
            if (File.Exists(assetFullPath))
            {
                string str;
                string str2;
                if (!AssemblyHelper.IsManagedAssembly(assetFullPath))
                {
                    return false;
                }
                int num = RunUpdatingProgram("AssemblyUpdater.exe", TimeStampArgument() + APIVersionArgument() + "--check-update-required -a " + CommandLineFormatter.PrepareFileName(assetFullPath) + AssemblySearchPathArgument() + ConfigurationProviderAssembliesPathArgument(), out str, out str2);
                Console.WriteLine("{0}{1}", str, str2);
                switch (num)
                {
                    case 0:
                    case 1:
                        return false;

                    case 2:
                        return true;
                }
                Debug.LogError(str + Environment.NewLine + str2);
            }
            return false;
        }

        private static string GetUnityEditorManagedPath()
        {
            return Path.Combine(MonoInstallationFinder.GetFrameWorksFolder(), "Managed");
        }

        public static bool IsReferenceToMissingObsoleteMember(string namespaceName, string className)
        {
            bool flag;
            <IsReferenceToMissingObsoleteMember>c__AnonStoreyA9 ya = new <IsReferenceToMissingObsoleteMember>c__AnonStoreyA9 {
                className = className,
                namespaceName = namespaceName
            };
            try
            {
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = (Func<Assembly, IEnumerable<Type>>) (a => a.GetTypes());
                }
                flag = AppDomain.CurrentDomain.GetAssemblies().SelectMany<Assembly, Type>(<>f__am$cache0).FirstOrDefault<Type>(new Func<Type, bool>(ya.<>m__1F4)) != null;
            }
            catch (ReflectionTypeLoadException exception)
            {
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = (acc, curr) => acc + "\r\n\t" + curr.Message;
                }
                throw new Exception(exception.Message + exception.LoaderExceptions.Aggregate<Exception, string>(string.Empty, <>f__am$cache1));
            }
            return flag;
        }

        private static bool IsUpdateable(Type type)
        {
            object[] customAttributes = type.GetCustomAttributes(typeof(ObsoleteAttribute), false);
            if (customAttributes.Length != 1)
            {
                return false;
            }
            ObsoleteAttribute attribute = (ObsoleteAttribute) customAttributes[0];
            return attribute.Message.Contains("UnityUpgradable");
        }

        private static string ResolveAssemblyPath(string assemblyPath)
        {
            return CommandLineFormatter.PrepareFileName(assemblyPath);
        }

        public static void Run(string commaSeparatedListOfAssemblies)
        {
            char[] separator = new char[] { ',' };
            string[] source = commaSeparatedListOfAssemblies.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            object[] args = new object[] { source.Count<string>() };
            APIUpdaterLogger.WriteToFile("Started to update {0} assemblie(s)", args);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            foreach (string str in source)
            {
                if (AssemblyHelper.IsManagedAssembly(str))
                {
                    string str2;
                    string str3;
                    string str4 = ResolveAssemblyPath(str);
                    int num2 = RunUpdatingProgram("AssemblyUpdater.exe", "-u -a " + str4 + APIVersionArgument() + AssemblySearchPathArgument() + ConfigurationProviderAssembliesPathArgument(), out str2, out str3);
                    if (str2.Length > 0)
                    {
                        object[] objArray2 = new object[] { str4, str2 };
                        APIUpdaterLogger.WriteToFile("Assembly update output ({0})\r\n{1}", objArray2);
                    }
                    if (num2 < 0)
                    {
                        object[] objArray3 = new object[] { num2, str3 };
                        APIUpdaterLogger.WriteErrorToConsole("Error {0} running AssemblyUpdater. Its output is: `{1}`", objArray3);
                    }
                }
            }
            object[] objArray4 = new object[] { stopwatch.Elapsed.TotalSeconds };
            APIUpdaterLogger.WriteToFile("Update finished in {0}s", objArray4);
        }

        private static int RunUpdatingProgram(string executable, string arguments, out string stdOut, out string stdErr)
        {
            string str = EditorApplication.applicationContentsPath + "/Tools/ScriptUpdater/" + executable;
            ManagedProgram program = new ManagedProgram(MonoInstallationFinder.GetMonoInstallation("MonoBleedingEdge"), "4.0", str, arguments);
            program.LogProcessStartInfo();
            program.Start();
            program.WaitForExit();
            stdOut = program.GetStandardOutputAsString();
            stdErr = string.Join("\r\n", program.GetErrorOutput());
            return program.ExitCode;
        }

        private static string TimeStampArgument()
        {
            return (" --timestamp " + DateTime.Now.Ticks + " ");
        }

        [CompilerGenerated]
        private sealed class <IsReferenceToMissingObsoleteMember>c__AnonStoreyA9
        {
            internal string className;
            internal string namespaceName;

            internal bool <>m__1F4(Type t)
            {
                return (((t.Name == this.className) && (t.Namespace == this.namespaceName)) && APIUpdaterHelper.IsUpdateable(t));
            }
        }
    }
}

