namespace UnityEditor.Scripting.Compilers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEditor;
    using UnityEditor.Scripting;
    using UnityEditor.Utils;

    internal class MonoCSharpCompiler : MonoScriptCompilerBase
    {
        [CompilerGenerated]
        private static Func<CompilerMessage, string> <>f__am$cache0;

        public MonoCSharpCompiler(MonoIsland island, bool runUpdater) : base(island, runUpdater)
        {
        }

        public static string[] Compile(string[] sources, string[] references, string[] defines, string outputFile)
        {
            MonoIsland island = new MonoIsland(BuildTarget.StandaloneWindows, "unity", sources, references, defines, outputFile);
            using (MonoCSharpCompiler compiler = new MonoCSharpCompiler(island, false))
            {
                compiler.BeginCompiling();
                while (!compiler.Poll())
                {
                    Thread.Sleep(50);
                }
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = cm => cm.message;
                }
                return compiler.GetCompilerMessages().Select<CompilerMessage, string>(<>f__am$cache0).ToArray<string>();
            }
        }

        protected override CompilerOutputParserBase CreateOutputParser()
        {
            return new MonoCSharpCompilerOutputParser();
        }

        private string[] GetAdditionalReferences()
        {
            return new string[] { "System.Runtime.Serialization.dll", "System.Xml.Linq.dll" };
        }

        private string GetCompilerPath(List<string> arguments)
        {
            string profileDirectory = base.GetProfileDirectory();
            string[] textArray1 = new string[] { "smcs", "gmcs", "mcs" };
            foreach (string str2 in textArray1)
            {
                string path = Path.Combine(profileDirectory, str2 + ".exe");
                if (File.Exists(path))
                {
                    return path;
                }
            }
            throw new ApplicationException("Unable to find csharp compiler in " + profileDirectory);
        }

        protected override Program StartCompiler()
        {
            List<string> arguments = new List<string> {
                "-debug",
                "-target:library",
                "-nowarn:0169",
                "-out:" + ScriptCompilerBase.PrepareFileName(this._island._output)
            };
            foreach (string str in this._island._references)
            {
                arguments.Add("-r:" + ScriptCompilerBase.PrepareFileName(str));
            }
            IEnumerator<string> enumerator = this._island._defines.Distinct<string>().GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    arguments.Add("-define:" + enumerator.Current);
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
                arguments.Add(ScriptCompilerBase.PrepareFileName(str3));
            }
            foreach (string str4 in this.GetAdditionalReferences())
            {
                string path = Path.Combine(base.GetProfileDirectory(), str4);
                if (File.Exists(path))
                {
                    arguments.Add("-r:" + ScriptCompilerBase.PrepareFileName(path));
                }
            }
            return base.StartCompiler(this._island._target, this.GetCompilerPath(arguments), arguments);
        }
    }
}

