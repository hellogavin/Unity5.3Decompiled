namespace UnityEditor.Scripting
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using UnityEditor.Scripting.Compilers;
    using UnityEditor.Utils;
    using UnityEngine;

    internal class ManagedProgram : Program
    {
        public ManagedProgram(string monodistribution, string profile, string executable, string arguments) : this(monodistribution, profile, executable, arguments, true)
        {
        }

        public ManagedProgram(string monodistribution, string profile, string executable, string arguments, bool setMonoEnvironmentVariables)
        {
            string[] parts = new string[] { monodistribution, "bin", "mono" };
            string str = PathCombine(parts);
            string[] textArray2 = new string[] { monodistribution, "lib", "mono", profile };
            string str2 = PathCombine(textArray2);
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                str = CommandLineFormatter.PrepareFileName(str + ".exe");
            }
            ProcessStartInfo info = new ProcessStartInfo {
                Arguments = CommandLineFormatter.PrepareFileName(executable) + " " + arguments,
                CreateNoWindow = true,
                FileName = str,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                WorkingDirectory = Application.dataPath + "/..",
                UseShellExecute = false
            };
            if (setMonoEnvironmentVariables)
            {
                info.EnvironmentVariables["MONO_PATH"] = str2;
                string[] textArray3 = new string[] { monodistribution, "etc" };
                info.EnvironmentVariables["MONO_CFG_DIR"] = PathCombine(textArray3);
            }
            base._process.StartInfo = info;
        }

        private static string PathCombine(params string[] parts)
        {
            string str = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                str = Path.Combine(str, parts[i]);
            }
            return str;
        }
    }
}

