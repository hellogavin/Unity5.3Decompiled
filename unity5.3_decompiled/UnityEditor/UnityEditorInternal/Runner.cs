namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using UnityEditor.Scripting;
    using UnityEditor.Scripting.Compilers;
    using UnityEditor.Utils;
    using UnityEngine;

    internal class Runner
    {
        internal static void RunManagedProgram(string exe, string args)
        {
            RunManagedProgram(exe, args, Application.dataPath + "/..", null);
        }

        internal static void RunManagedProgram(string exe, string args, string workingDirectory, CompilerOutputParserBase parser)
        {
            Program program;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                ProcessStartInfo si = new ProcessStartInfo {
                    Arguments = args,
                    CreateNoWindow = true,
                    FileName = exe
                };
                program = new Program(si);
            }
            else
            {
                program = new ManagedProgram(MonoInstallationFinder.GetMonoInstallation("MonoBleedingEdge"), "4.0", exe, args);
            }
            using (program)
            {
                program.GetProcessStartInfo().WorkingDirectory = workingDirectory;
                program.Start();
                program.WaitForExit();
                stopwatch.Stop();
                Console.WriteLine("{0} exited after {1} ms.", exe, stopwatch.ElapsedMilliseconds);
                if (program.ExitCode != 0)
                {
                    if (parser != null)
                    {
                        string[] errorOutput = program.GetErrorOutput();
                        string[] standardOutput = program.GetStandardOutput();
                        IEnumerator<CompilerMessage> enumerator = parser.Parse(errorOutput, standardOutput, true).GetEnumerator();
                        try
                        {
                            while (enumerator.MoveNext())
                            {
                                CompilerMessage current = enumerator.Current;
                                Debug.LogPlayerBuildError(current.message, current.file, current.line, current.column);
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
                    Debug.LogError("Failed running " + exe + " " + args + "\n\n" + program.GetAllOutput());
                    throw new Exception(string.Format("{0} did not run properly!", exe));
                }
            }
        }

        public static void RunNativeProgram(string exe, string args)
        {
            using (NativeProgram program = new NativeProgram(exe, args))
            {
                program.Start();
                program.WaitForExit();
                if (program.ExitCode != 0)
                {
                    Debug.LogError("Failed running " + exe + " " + args + "\n\n" + program.GetAllOutput());
                    throw new Exception(string.Format("{0} did not run properly!", exe));
                }
            }
        }
    }
}

