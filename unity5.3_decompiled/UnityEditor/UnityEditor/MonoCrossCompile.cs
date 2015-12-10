namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEngine;

    internal class MonoCrossCompile
    {
        [CompilerGenerated]
        private static Func<string, bool> <>f__am$cache1;
        public static string ArtifactsPath;

        private static void CrossCompileAOT(BuildTarget target, string crossCompilerAbsolutePath, string assembliesAbsoluteDirectory, CrossCompileOptions crossCompileOptions, string input, string output, string additionalOptions)
        {
            string str = string.Empty;
            if (!IsDebugableAssembly(input))
            {
                crossCompileOptions &= ~CrossCompileOptions.Debugging;
                crossCompileOptions &= ~CrossCompileOptions.LoadSymbols;
            }
            bool flag = (crossCompileOptions & CrossCompileOptions.Debugging) != CrossCompileOptions.Dynamic;
            bool flag2 = (crossCompileOptions & CrossCompileOptions.LoadSymbols) != CrossCompileOptions.Dynamic;
            bool flag3 = flag || flag2;
            if (flag3)
            {
                str = str + "--debug ";
            }
            if (flag)
            {
                str = str + "--optimize=-linears ";
            }
            str = str + "--aot=full,asmonly,";
            if (flag3)
            {
                str = str + "write-symbols,";
            }
            if ((crossCompileOptions & CrossCompileOptions.Debugging) != CrossCompileOptions.Dynamic)
            {
                str = str + "soft-debug,";
            }
            else if (!flag3)
            {
                str = str + "nodebug,";
            }
            if (target != BuildTarget.iOS)
            {
                str = str + "print-skipped,";
            }
            if ((additionalOptions != null) & (additionalOptions.Trim().Length > 0))
            {
                str = str + additionalOptions.Trim() + ",";
            }
            string fileName = Path.GetFileName(output);
            string resultingFile = Path.Combine(assembliesAbsoluteDirectory, fileName);
            if ((crossCompileOptions & CrossCompileOptions.FastICall) != CrossCompileOptions.Dynamic)
            {
                str = str + "ficall,";
            }
            if ((crossCompileOptions & CrossCompileOptions.Static) != CrossCompileOptions.Dynamic)
            {
                str = str + "static,";
            }
            string str5 = str;
            string[] textArray1 = new string[] { str5, "outfile=\"", fileName, "\" \"", input, "\" " };
            str = string.Concat(textArray1);
            Process process = new Process {
                StartInfo = { FileName = crossCompilerAbsolutePath, Arguments = str }
            };
            process.StartInfo.EnvironmentVariables["MONO_PATH"] = assembliesAbsoluteDirectory;
            process.StartInfo.EnvironmentVariables["GAC_PATH"] = assembliesAbsoluteDirectory;
            process.StartInfo.EnvironmentVariables["GC_DONT_GC"] = "yes please";
            if ((crossCompileOptions & CrossCompileOptions.ExplicitNullChecks) != CrossCompileOptions.Dynamic)
            {
                process.StartInfo.EnvironmentVariables["MONO_DEBUG"] = "explicit-null-checks";
            }
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            if (ArtifactsPath != null)
            {
                if (!Directory.Exists(ArtifactsPath))
                {
                    Directory.CreateDirectory(ArtifactsPath);
                }
                File.AppendAllText(ArtifactsPath + "output.txt", process.StartInfo.FileName + "\n");
                File.AppendAllText(ArtifactsPath + "output.txt", process.StartInfo.Arguments + "\n");
                File.AppendAllText(ArtifactsPath + "output.txt", assembliesAbsoluteDirectory + "\n");
                File.AppendAllText(ArtifactsPath + "output.txt", resultingFile + "\n");
                File.AppendAllText(ArtifactsPath + "output.txt", input + "\n");
                File.AppendAllText(ArtifactsPath + "houtput.txt", fileName + "\n\n");
                File.Copy(assembliesAbsoluteDirectory + @"\" + input, ArtifactsPath + @"\" + input, true);
            }
            process.StartInfo.WorkingDirectory = assembliesAbsoluteDirectory;
            MonoProcessUtility.RunMonoProcess(process, "AOT cross compiler", resultingFile);
            File.Move(resultingFile, output);
            if ((crossCompileOptions & CrossCompileOptions.Static) == CrossCompileOptions.Dynamic)
            {
                string path = Path.Combine(assembliesAbsoluteDirectory, fileName + ".def");
                if (File.Exists(path))
                {
                    File.Move(path, output + ".def");
                }
            }
        }

        public static void CrossCompileAOTDirectory(BuildTarget buildTarget, CrossCompileOptions crossCompileOptions, string sourceAssembliesFolder, string targetCrossCompiledASMFolder, string additionalOptions)
        {
            CrossCompileAOTDirectory(buildTarget, crossCompileOptions, sourceAssembliesFolder, targetCrossCompiledASMFolder, string.Empty, additionalOptions);
        }

        public static void CrossCompileAOTDirectory(BuildTarget buildTarget, CrossCompileOptions crossCompileOptions, string sourceAssembliesFolder, string targetCrossCompiledASMFolder, string pathExtension, string additionalOptions)
        {
            string buildToolsDirectory = BuildPipeline.GetBuildToolsDirectory(buildTarget);
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                buildToolsDirectory = Path.Combine(Path.Combine(buildToolsDirectory, pathExtension), "mono-xcompiler");
            }
            else
            {
                buildToolsDirectory = Path.Combine(Path.Combine(buildToolsDirectory, pathExtension), "mono-xcompiler.exe");
            }
            sourceAssembliesFolder = Path.Combine(Directory.GetCurrentDirectory(), sourceAssembliesFolder);
            targetCrossCompiledASMFolder = Path.Combine(Directory.GetCurrentDirectory(), targetCrossCompiledASMFolder);
            foreach (string str2 in Directory.GetFiles(sourceAssembliesFolder))
            {
                if (Path.GetExtension(str2) == ".dll")
                {
                    string fileName = Path.GetFileName(str2);
                    string output = Path.Combine(targetCrossCompiledASMFolder, fileName + ".s");
                    if (EditorUtility.DisplayCancelableProgressBar("Building Player", "AOT cross compile " + fileName, 0.95f))
                    {
                        throw new OperationCanceledException();
                    }
                    CrossCompileAOT(buildTarget, buildToolsDirectory, sourceAssembliesFolder, crossCompileOptions, fileName, output, additionalOptions);
                }
            }
        }

        public static bool CrossCompileAOTDirectoryParallel(BuildTarget buildTarget, CrossCompileOptions crossCompileOptions, string sourceAssembliesFolder, string targetCrossCompiledASMFolder, string additionalOptions)
        {
            return CrossCompileAOTDirectoryParallel(buildTarget, crossCompileOptions, sourceAssembliesFolder, targetCrossCompiledASMFolder, string.Empty, additionalOptions);
        }

        public static bool CrossCompileAOTDirectoryParallel(string crossCompilerPath, BuildTarget buildTarget, CrossCompileOptions crossCompileOptions, string sourceAssembliesFolder, string targetCrossCompiledASMFolder, string additionalOptions)
        {
            sourceAssembliesFolder = Path.Combine(Directory.GetCurrentDirectory(), sourceAssembliesFolder);
            targetCrossCompiledASMFolder = Path.Combine(Directory.GetCurrentDirectory(), targetCrossCompiledASMFolder);
            int workerThreads = 1;
            int completionPortThreads = 1;
            ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);
            List<JobCompileAOT> list = new List<JobCompileAOT>();
            List<ManualResetEvent> events = new List<ManualResetEvent>();
            bool flag = true;
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = path => Path.GetExtension(path) == ".dll";
            }
            List<string> list3 = new List<string>(Directory.GetFiles(sourceAssembliesFolder).Where<string>(<>f__am$cache1));
            int count = list3.Count;
            int filesFinished = 0;
            DisplayAOTProgressBar(count, filesFinished);
            long timeout = Math.Min(0x1b7740, ((count + 3) * 0x3e8) * 30);
            foreach (string str in list3)
            {
                string fileName = Path.GetFileName(str);
                string output = Path.Combine(targetCrossCompiledASMFolder, fileName + ".s");
                JobCompileAOT item = new JobCompileAOT(buildTarget, crossCompilerPath, sourceAssembliesFolder, crossCompileOptions, fileName, output, additionalOptions);
                list.Add(item);
                events.Add(item.m_doneEvent);
                ThreadPool.QueueUserWorkItem(new WaitCallback(item.ThreadPoolCallback));
                if (events.Count >= Environment.ProcessorCount)
                {
                    flag = WaitForBuildOfFile(events, ref timeout);
                    DisplayAOTProgressBar(count, filesFinished);
                    filesFinished++;
                    if (!flag)
                    {
                        break;
                    }
                }
            }
            while (events.Count > 0)
            {
                flag = WaitForBuildOfFile(events, ref timeout);
                DisplayAOTProgressBar(count, filesFinished);
                filesFinished++;
                if (!flag)
                {
                    break;
                }
            }
            foreach (JobCompileAOT eaot2 in list)
            {
                if (eaot2.m_Exception != null)
                {
                    object[] args = new object[] { eaot2.m_input, eaot2.m_Exception };
                    Debug.LogErrorFormat("Cross compilation job {0} failed.\n{1}", args);
                    flag = false;
                }
            }
            return flag;
        }

        public static bool CrossCompileAOTDirectoryParallel(BuildTarget buildTarget, CrossCompileOptions crossCompileOptions, string sourceAssembliesFolder, string targetCrossCompiledASMFolder, string pathExtension, string additionalOptions)
        {
            string buildToolsDirectory = BuildPipeline.GetBuildToolsDirectory(buildTarget);
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                buildToolsDirectory = Path.Combine(Path.Combine(buildToolsDirectory, pathExtension), "mono-xcompiler");
            }
            else
            {
                buildToolsDirectory = Path.Combine(Path.Combine(buildToolsDirectory, pathExtension), "mono-xcompiler.exe");
            }
            return CrossCompileAOTDirectoryParallel(buildToolsDirectory, buildTarget, crossCompileOptions, sourceAssembliesFolder, targetCrossCompiledASMFolder, additionalOptions);
        }

        public static void DisplayAOTProgressBar(int totalFiles, int filesFinished)
        {
            string info = string.Format("AOT cross compile ({0}/{1})", (filesFinished + 1).ToString(), totalFiles.ToString());
            EditorUtility.DisplayProgressBar("Building Player", info, 0.95f);
        }

        private static bool IsDebugableAssembly(string fname)
        {
            fname = Path.GetFileName(fname);
            return fname.StartsWith("Assembly", StringComparison.OrdinalIgnoreCase);
        }

        private static bool WaitForBuildOfFile(List<ManualResetEvent> events, ref long timeout)
        {
            long num = DateTime.Now.Ticks / 0x2710L;
            int index = WaitHandle.WaitAny(events.ToArray(), (int) timeout);
            long num3 = DateTime.Now.Ticks / 0x2710L;
            if (index == 0x102)
            {
                return false;
            }
            events.RemoveAt(index);
            timeout -= num3 - num;
            if (timeout < 0L)
            {
                timeout = 0L;
            }
            return true;
        }

        private class JobCompileAOT
        {
            public string m_additionalOptions;
            private string m_assembliesAbsoluteDirectory;
            private CrossCompileOptions m_crossCompileOptions;
            private string m_crossCompilerAbsolutePath;
            public ManualResetEvent m_doneEvent = new ManualResetEvent(false);
            public Exception m_Exception;
            public string m_input;
            public string m_output;
            private BuildTarget m_target;

            public JobCompileAOT(BuildTarget target, string crossCompilerAbsolutePath, string assembliesAbsoluteDirectory, CrossCompileOptions crossCompileOptions, string input, string output, string additionalOptions)
            {
                this.m_target = target;
                this.m_crossCompilerAbsolutePath = crossCompilerAbsolutePath;
                this.m_assembliesAbsoluteDirectory = assembliesAbsoluteDirectory;
                this.m_crossCompileOptions = crossCompileOptions;
                this.m_input = input;
                this.m_output = output;
                this.m_additionalOptions = additionalOptions;
            }

            public void ThreadPoolCallback(object threadContext)
            {
                try
                {
                    MonoCrossCompile.CrossCompileAOT(this.m_target, this.m_crossCompilerAbsolutePath, this.m_assembliesAbsoluteDirectory, this.m_crossCompileOptions, this.m_input, this.m_output, this.m_additionalOptions);
                }
                catch (Exception exception)
                {
                    this.m_Exception = exception;
                }
                this.m_doneEvent.Set();
            }
        }
    }
}

