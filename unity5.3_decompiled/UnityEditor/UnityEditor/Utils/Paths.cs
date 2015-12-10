namespace UnityEditor.Utils
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;

    internal static class Paths
    {
        public static bool AreEqual(string pathA, string pathB, bool ignoreCase)
        {
            if ((pathA == string.Empty) && (pathB == string.Empty))
            {
                return true;
            }
            if (string.IsNullOrEmpty(pathA) || string.IsNullOrEmpty(pathB))
            {
                return false;
            }
            return (string.Compare(Path.GetFullPath(pathA), Path.GetFullPath(pathB), !ignoreCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) == 0);
        }

        public static string Combine(params string[] components)
        {
            if (components.Length < 1)
            {
                throw new ArgumentException("At least one component must be provided!");
            }
            string str = components[0];
            for (int i = 1; i < components.Length; i++)
            {
                str = Path.Combine(str, components[i]);
            }
            return str;
        }

        public static string CreateTempDirectory()
        {
            string tempFileName = Path.GetTempFileName();
            File.Delete(tempFileName);
            Directory.CreateDirectory(tempFileName);
            return tempFileName;
        }

        public static string GetFileOrFolderName(string path)
        {
            if (File.Exists(path))
            {
                return Path.GetFileName(path);
            }
            if (!Directory.Exists(path))
            {
                throw new ArgumentException("Target '" + path + "' does not exist.");
            }
            string[] strArray = Split(path);
            return strArray[strArray.Length - 1];
        }

        public static string NormalizePath(this string path)
        {
            if (Path.DirectorySeparatorChar == '\\')
            {
                return path.Replace('/', Path.DirectorySeparatorChar);
            }
            return path.Replace('\\', Path.DirectorySeparatorChar);
        }

        public static string[] Split(string path)
        {
            char[] separator = new char[] { Path.DirectorySeparatorChar };
            List<string> list = new List<string>(path.Split(separator));
            int index = 0;
            while (index < list.Count)
            {
                list[index] = list[index].Trim();
                if (list[index].Equals(string.Empty))
                {
                    list.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }
            return list.ToArray();
        }
    }
}

