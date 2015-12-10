namespace UnityEditor.Macros
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityScript.Scripting;

    public static class MacroEvaluator
    {
        [CompilerGenerated]
        private static Func<Type, bool> <>f__am$cache1;
        private static readonly EvaluationContext EditorEvaluationContext = new EvaluationContext(new EditorEvaluationDomainProvider());

        public static string Eval(string macro)
        {
            if (macro.StartsWith("ExecuteMethod: "))
            {
                return ExecuteMethodThroughReflection(macro);
            }
            object obj2 = Evaluator.Eval(EditorEvaluationContext, macro);
            return ((obj2 != null) ? obj2.ToString() : "Null");
        }

        private static string ExecuteMethodThroughReflection(string macro)
        {
            <ExecuteMethodThroughReflection>c__AnonStorey9C storeyc = new <ExecuteMethodThroughReflection>c__AnonStorey9C();
            Match match = new Regex(@"ExecuteMethod: (?<type>.*)\.(?<method>.*)").Match(macro);
            storeyc.typename = match.Groups["type"].ToString();
            string name = match.Groups["method"].ToString();
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = t => t != null;
            }
            MethodInfo method = EditorAssemblies.loadedAssemblies.Select<Assembly, Type>(new Func<Assembly, Type>(storeyc.<>m__1CA)).Where<Type>(<>f__am$cache1).First<Type>().GetMethod(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            if (method == null)
            {
                throw new ArgumentException(string.Format("cannot find method {0} in type {1}", name, storeyc.typename));
            }
            if (method.GetParameters().Length > 0)
            {
                throw new ArgumentException("You can only invoke static methods with no arguments");
            }
            object obj2 = method.Invoke(null, new object[0]);
            return ((obj2 != null) ? obj2.ToString() : "Null");
        }

        [CompilerGenerated]
        private sealed class <ExecuteMethodThroughReflection>c__AnonStorey9C
        {
            internal string typename;

            internal Type <>m__1CA(Assembly a)
            {
                return a.GetType(this.typename, false);
            }
        }

        private class EditorEvaluationDomainProvider : SimpleEvaluationDomainProvider
        {
            [CompilerGenerated]
            private static Func<Assembly, IEnumerable<AssemblyName>> <>f__am$cache1;
            [CompilerGenerated]
            private static Func<AssemblyName, Assembly> <>f__am$cache2;
            [CompilerGenerated]
            private static Func<Assembly, bool> <>f__am$cache3;
            private static readonly string[] DefaultImports = new string[] { "UnityEditor", "UnityEngine" };

            public EditorEvaluationDomainProvider() : base(DefaultImports)
            {
            }

            public override Assembly[] GetAssemblyReferences()
            {
                Assembly[] loadedAssemblies = EditorAssemblies.loadedAssemblies;
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = (Func<Assembly, IEnumerable<AssemblyName>>) (a => a.GetReferencedAssemblies());
                }
                if (<>f__am$cache2 == null)
                {
                    <>f__am$cache2 = a => TryToLoad(a);
                }
                if (<>f__am$cache3 == null)
                {
                    <>f__am$cache3 = a => a != null;
                }
                IEnumerable<Assembly> second = loadedAssemblies.SelectMany<Assembly, AssemblyName>(<>f__am$cache1).Select<AssemblyName, Assembly>(<>f__am$cache2).Where<Assembly>(<>f__am$cache3);
                return loadedAssemblies.Concat<Assembly>(second).ToArray<Assembly>();
            }

            private static Assembly TryToLoad(AssemblyName a)
            {
                try
                {
                    return Assembly.Load(a);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}

