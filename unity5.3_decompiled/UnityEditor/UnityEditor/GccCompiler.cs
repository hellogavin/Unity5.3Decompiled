namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    internal class GccCompiler : NativeCompiler
    {
        [CompilerGenerated]
        private static Func<string, string, string> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<string, string, string> <>f__am$cache2;
        private readonly ICompilerSettings m_Settings;

        public GccCompiler(ICompilerSettings settings)
        {
            this.m_Settings = settings;
        }

        private void Compile(string file, string includePaths)
        {
            object[] args = new object[] { this.m_Settings.MachineSpecification, includePaths, file, base.ObjectFileFor(file) };
            string arguments = string.Format(" -c {0} -O0 -Wno-unused-value -Wno-invalid-offsetof -fvisibility=hidden -fno-rtti {1} {2} -o {3}", args);
            base.Execute(arguments, this.m_Settings.CompilerPath);
        }

        public override void CompileDynamicLibrary(string outFile, IEnumerable<string> sources, IEnumerable<string> includePaths, IEnumerable<string> libraries, IEnumerable<string> libraryPaths)
        {
            <CompileDynamicLibrary>c__AnonStorey6E storeye = new <CompileDynamicLibrary>c__AnonStorey6E {
                <>f__this = this
            };
            string[] strArray = sources.ToArray<string>();
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = (current, sourceDir) => current + "-I" + sourceDir + " ";
            }
            storeye.includeDirs = includePaths.Aggregate<string, string>(string.Empty, <>f__am$cache1);
            string str = string.Empty;
            string str2 = NativeCompiler.Aggregate(libraryPaths.Union<string>(this.m_Settings.LibPaths), "-L", " ");
            NativeCompiler.ParallelFor<string>(strArray, new Action<string>(storeye.<>m__F3));
            string[] arguments = new string[4];
            arguments[0] = string.Format("-shared {0} -o {1}", this.m_Settings.MachineSpecification, outFile);
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = (buff, s) => buff + " " + s;
            }
            arguments[1] = strArray.Where<string>(new Func<string, bool>(NativeCompiler.IsSourceFile)).Select<string, string>(new Func<string, string>(this.ObjectFileFor)).Aggregate<string>(<>f__am$cache2);
            arguments[2] = str2;
            arguments[3] = str;
            base.ExecuteCommand(this.m_Settings.LinkerPath, arguments);
        }

        protected override string objectFileExtension
        {
            get
            {
                return "o";
            }
        }

        [CompilerGenerated]
        private sealed class <CompileDynamicLibrary>c__AnonStorey6E
        {
            internal GccCompiler <>f__this;
            internal string includeDirs;

            internal void <>m__F3(string file)
            {
                this.<>f__this.Compile(file, this.includeDirs);
            }
        }
    }
}

