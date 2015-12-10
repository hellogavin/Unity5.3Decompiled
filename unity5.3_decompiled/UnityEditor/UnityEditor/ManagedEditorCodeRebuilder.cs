namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEditor.Modules;
    using UnityEditor.Scripting.Compilers;

    internal class ManagedEditorCodeRebuilder
    {
        private static readonly char[] kNewlineChars = new char[] { '\r', '\n' };

        private static ProcessStartInfo GetJamStartInfo(bool includeModules)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("jam.pl LiveReloadableEditorAssemblies");
            if (includeModules)
            {
                foreach (string str in ModuleManager.GetJamTargets())
                {
                    builder.Append(" ").Append(str);
                }
            }
            return new ProcessStartInfo { WorkingDirectory = Unsupported.GetBaseUnityDeveloperFolder(), RedirectStandardOutput = true, RedirectStandardError = false, Arguments = builder.ToString(), FileName = "perl" };
        }

        private static string GetOutputStream(ProcessStartInfo startInfo, out int exitCode)
        {
            <GetOutputStream>c__AnonStorey31 storey = new <GetOutputStream>c__AnonStorey31();
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            Process process = new Process {
                StartInfo = startInfo
            };
            storey.sbStandardOut = new StringBuilder();
            storey.sbStandardError = new StringBuilder();
            process.OutputDataReceived += new DataReceivedEventHandler(storey.<>m__48);
            process.ErrorDataReceived += new DataReceivedEventHandler(storey.<>m__49);
            process.Start();
            if (startInfo.RedirectStandardError)
            {
                process.BeginErrorReadLine();
            }
            else
            {
                process.BeginOutputReadLine();
            }
            process.WaitForExit();
            string str = !startInfo.RedirectStandardError ? storey.sbStandardOut.ToString() : storey.sbStandardError.ToString();
            exitCode = process.ExitCode;
            return str;
        }

        private static CompilerMessage[] ParseResults(string text)
        {
            Console.Write(text);
            string[] errorOutput = text.Split(kNewlineChars, StringSplitOptions.RemoveEmptyEntries);
            string baseUnityDeveloperFolder = Unsupported.GetBaseUnityDeveloperFolder();
            List<CompilerMessage> list = new MonoCSharpCompilerOutputParser().Parse(errorOutput, false).ToList<CompilerMessage>();
            for (int i = 0; i < list.Count; i++)
            {
                CompilerMessage message = list[i];
                message.file = Path.Combine(baseUnityDeveloperFolder, message.file);
                list[i] = message;
            }
            return list.ToArray();
        }

        private static bool Run(out CompilerMessage[] messages, bool includeModules)
        {
            int num;
            messages = ParseResults(GetOutputStream(GetJamStartInfo(includeModules), out num));
            return (num == 0);
        }

        [CompilerGenerated]
        private sealed class <GetOutputStream>c__AnonStorey31
        {
            internal StringBuilder sbStandardError;
            internal StringBuilder sbStandardOut;

            internal void <>m__48(object sender, DataReceivedEventArgs data)
            {
                this.sbStandardOut.AppendLine(data.Data);
            }

            internal void <>m__49(object sender, DataReceivedEventArgs data)
            {
                this.sbStandardError.AppendLine(data.Data);
            }
        }
    }
}

