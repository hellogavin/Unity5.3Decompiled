namespace UnityEditor.Macros
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public static class MethodEvaluator
    {
        [CompilerGenerated]
        private static string <ToCommaSeparatedString`1>m__1CF<T>(T o)
        {
            return o.ToString();
        }

        public static object Eval(string assemblyFile, string typeName, string methodName, Type[] paramTypes, object[] args)
        {
            object obj2;
            AssemblyResolver resolver = new AssemblyResolver(Path.GetDirectoryName(assemblyFile));
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(resolver.AssemblyResolve);
            try
            {
                Assembly assembly = Assembly.LoadFrom(assemblyFile);
                MethodInfo info = assembly.GetType(typeName, true).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, paramTypes, null);
                if (info == null)
                {
                    object[] objArray1 = new object[] { typeName, methodName, ToCommaSeparatedString<Type>(paramTypes), assembly.FullName };
                    throw new ArgumentException(string.Format("Method {0}.{1}({2}) not found in assembly {3}!", objArray1));
                }
                obj2 = info.Invoke(null, args);
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(resolver.AssemblyResolve);
            }
            return obj2;
        }

        private static string ToCommaSeparatedString<T>(IEnumerable<T> items)
        {
            return string.Join(", ", items.Select<T, string>(new Func<T, string>(MethodEvaluator.<ToCommaSeparatedString`1>m__1CF<T>)).ToArray<string>());
        }

        public class AssemblyResolver
        {
            private readonly string _assemblyDirectory;

            public AssemblyResolver(string assemblyDirectory)
            {
                this._assemblyDirectory = assemblyDirectory;
            }

            public Assembly AssemblyResolve(object sender, ResolveEventArgs args)
            {
                char[] separator = new char[] { ',' };
                string path = Path.Combine(this._assemblyDirectory, args.Name.Split(separator)[0] + ".dll");
                if (File.Exists(path))
                {
                    return Assembly.LoadFrom(path);
                }
                return null;
            }
        }
    }
}

