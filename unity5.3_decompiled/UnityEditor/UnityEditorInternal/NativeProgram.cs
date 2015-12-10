namespace UnityEditorInternal
{
    using System;
    using System.Diagnostics;
    using UnityEditor.Utils;
    using UnityEngine;

    internal class NativeProgram : Program
    {
        public NativeProgram(string executable, string arguments)
        {
            ProcessStartInfo info = new ProcessStartInfo {
                Arguments = arguments,
                CreateNoWindow = true,
                FileName = executable,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                WorkingDirectory = Application.dataPath + "/..",
                UseShellExecute = false
            };
            base._process.StartInfo = info;
        }
    }
}

