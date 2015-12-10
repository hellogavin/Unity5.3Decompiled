namespace UnityEditor.Utils
{
    using System;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    internal class MonoInstallationFinder
    {
        public static string GetFrameWorksFolder()
        {
            string str = FileUtil.NiceWinPath(EditorApplication.applicationPath);
            if ((Application.platform != RuntimePlatform.WindowsEditor) && (Application.platform == RuntimePlatform.OSXEditor))
            {
                return Path.Combine(str, Path.Combine("Contents", "Frameworks"));
            }
            return Path.Combine(Path.GetDirectoryName(str), "Data");
        }

        public static string GetMonoInstallation()
        {
            return GetMonoInstallation("Mono");
        }

        public static string GetMonoInstallation(string monoName)
        {
            return Path.Combine(GetFrameWorksFolder(), monoName);
        }

        public static string GetProfileDirectory(BuildTarget target, string profile)
        {
            return Path.Combine(GetMonoInstallation(), Path.Combine("lib", Path.Combine("mono", profile)));
        }
    }
}

