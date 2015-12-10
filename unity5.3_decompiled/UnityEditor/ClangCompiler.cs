using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor.Utils;

internal class ClangCompiler : NativeCompiler
{
    [CompilerGenerated]
    private static Func<string, string, string> <>f__am$cache1;
    [CompilerGenerated]
    private static Func<string, string, string> <>f__am$cache2;
    private readonly ICompilerSettings m_Settings;

    public ClangCompiler(ICompilerSettings settings)
    {
        this.m_Settings = settings;
    }

    private void Compile(string file, string includePaths)
    {
        object[] args = new object[] { this.m_Settings.MachineSpecification, includePaths, file, base.ObjectFileFor(file) };
        string arguments = string.Format(" -c -arch {0} -stdlib=libstdc++ -O0 -Wno-unused-value -Wno-invalid-offsetof -fvisibility=hidden -fno-rtti -I/Applications/Xcode.app/Contents/Developer/Toolchains/XcodeDefault.xctoolchain/usr/include {1} -isysroot /Applications/Xcode.app/Contents/Developer/Platforms/MacOSX.platform/Developer/SDKs/MacOSX10.8.sdk -mmacosx-version-min=10.6 -single_module -compatibility_version 1 -current_version 1 {2} -o {3}", args);
        base.Execute(arguments, this.m_Settings.CompilerPath);
    }

    public override void CompileDynamicLibrary(string outFile, IEnumerable<string> sources, IEnumerable<string> includePaths, IEnumerable<string> libraries, IEnumerable<string> libraryPaths)
    {
        <CompileDynamicLibrary>c__AnonStorey6C storeyc = new <CompileDynamicLibrary>c__AnonStorey6C {
            <>f__this = this
        };
        string[] strArray = sources.ToArray<string>();
        if (<>f__am$cache1 == null)
        {
            <>f__am$cache1 = (current, sourceDir) => current + "-I" + sourceDir + " ";
        }
        storeyc.includeDirs = includePaths.Aggregate<string, string>(string.Empty, <>f__am$cache1);
        string str = NativeCompiler.Aggregate(libraries, "-force_load ", " ");
        string str2 = NativeCompiler.Aggregate(libraryPaths.Union<string>(this.m_Settings.LibPaths), "-L", " ");
        NativeCompiler.ParallelFor<string>(strArray, new Action<string>(storeyc.<>m__EE));
        string str3 = "\"" + Path.GetFullPath(Path.Combine(Path.GetDirectoryName(outFile), Path.GetFileNameWithoutExtension(outFile) + ".map")) + "\"";
        string[] arguments = new string[11];
        arguments[0] = "-dylib";
        arguments[1] = "-arch " + this.m_Settings.MachineSpecification;
        arguments[2] = "-macosx_version_min 10.6";
        arguments[3] = "-lSystem";
        arguments[4] = "-lstdc++";
        arguments[5] = "-map";
        arguments[6] = str3;
        arguments[7] = "-o " + outFile;
        if (<>f__am$cache2 == null)
        {
            <>f__am$cache2 = (buff, s) => buff + " " + s;
        }
        arguments[8] = strArray.Select<string, string>(new Func<string, string>(this.ObjectFileFor)).Aggregate<string>(<>f__am$cache2);
        arguments[9] = str2;
        arguments[10] = str;
        base.ExecuteCommand("ld", arguments);
        string command = Path.Combine(MonoInstallationFinder.GetFrameWorksFolder(), "Tools/MapFileParser/MapFileParser");
        string str5 = "\"" + Path.GetFullPath(Path.Combine(Path.GetDirectoryName(outFile), "SymbolMap")) + "\"";
        string[] textArray2 = new string[] { "-format=Clang", str3, str5 };
        base.ExecuteCommand(command, textArray2);
    }

    protected override void SetupProcessStartInfo(ProcessStartInfo startInfo)
    {
    }

    protected override string objectFileExtension
    {
        get
        {
            return "o";
        }
    }

    [CompilerGenerated]
    private sealed class <CompileDynamicLibrary>c__AnonStorey6C
    {
        internal ClangCompiler <>f__this;
        internal string includeDirs;

        internal void <>m__EE(string file)
        {
            this.<>f__this.Compile(file, this.includeDirs);
        }
    }
}

