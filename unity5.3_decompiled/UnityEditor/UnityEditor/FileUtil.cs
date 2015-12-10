namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using UnityEngine;

    public sealed class FileUtil
    {
        [CompilerGenerated]
        private static Func<string, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<string, bool> <>f__am$cache1;

        internal static bool AppendTextAfter(string path, string find, string append)
        {
            bool flag = false;
            path = NiceWinPath(path);
            List<string> list = new List<string>(File.ReadAllLines(path));
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Contains(find))
                {
                    list.Insert(i + 1, append);
                    flag = true;
                    break;
                }
            }
            File.WriteAllLines(path, list.ToArray());
            return flag;
        }

        internal static string CombinePaths(params string[] paths)
        {
            if (paths == null)
            {
                return string.Empty;
            }
            return string.Join(Path.DirectorySeparatorChar.ToString(), paths);
        }

        internal static void CopyDirectory(string source, string target, bool overwrite)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = f => true;
            }
            CopyDirectoryFiltered(source, target, overwrite, <>f__am$cache0, false);
        }

        internal static void CopyDirectoryFiltered(string source, string target, bool overwrite, Func<string, bool> includeCallback, bool recursive)
        {
            if (!Directory.Exists(target))
            {
                Directory.CreateDirectory(target);
                overwrite = false;
            }
            foreach (string str in Directory.GetFiles(source))
            {
                if (includeCallback(str))
                {
                    string fileName = Path.GetFileName(str);
                    string to = Path.Combine(target, fileName);
                    UnityFileCopy(str, to, overwrite);
                }
            }
            if (recursive)
            {
                foreach (string str4 in Directory.GetDirectories(source))
                {
                    if (includeCallback(str4))
                    {
                        string str5 = Path.GetFileName(str4);
                        CopyDirectoryFiltered(Path.Combine(source, str5), Path.Combine(target, str5), overwrite, includeCallback, recursive);
                    }
                }
            }
        }

        internal static void CopyDirectoryFiltered(string source, string target, bool overwrite, string regExExcludeFilter, bool recursive)
        {
            <CopyDirectoryFiltered>c__AnonStorey12 storey = new <CopyDirectoryFiltered>c__AnonStorey12 {
                exclude = null
            };
            try
            {
                if (regExExcludeFilter != null)
                {
                    storey.exclude = new Regex(regExExcludeFilter);
                }
            }
            catch (ArgumentException)
            {
                Debug.Log("CopyDirectoryRecursive: Pattern '" + regExExcludeFilter + "' is not a correct Regular Expression. Not excluding any files.");
                return;
            }
            Func<string, bool> includeCallback = new Func<string, bool>(storey.<>m__15);
            CopyDirectoryFiltered(source, target, overwrite, includeCallback, recursive);
        }

        internal static void CopyDirectoryRecursive(string source, string target)
        {
            CopyDirectoryRecursive(source, target, false, false);
        }

        internal static void CopyDirectoryRecursive(string source, string target, bool overwrite)
        {
            CopyDirectoryRecursive(source, target, overwrite, false);
        }

        internal static void CopyDirectoryRecursive(string source, string target, bool overwrite, bool ignoreMeta)
        {
            CopyDirectoryRecursiveFiltered(source, target, overwrite, !ignoreMeta ? null : @"\.meta$");
        }

        internal static void CopyDirectoryRecursiveFiltered(string source, string target, bool overwrite, string regExExcludeFilter)
        {
            CopyDirectoryFiltered(source, target, overwrite, regExExcludeFilter, true);
        }

        internal static void CopyDirectoryRecursiveForPostprocess(string source, string target, bool overwrite)
        {
            CopyDirectoryRecursiveFiltered(source, target, overwrite, @".*/\.+|\.meta$");
        }

        internal static void CopyDirectoryRecursiveIgnoreMeta(string source, string target)
        {
            CopyDirectoryRecursive(source, target, false, true);
        }

        internal static void CopyFileIfExists(string src, string dst, bool overwrite)
        {
            if (File.Exists(src))
            {
                UnityFileCopy(src, dst, overwrite);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void CopyFileOrDirectory(string from, string to);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void CopyFileOrDirectoryFollowSymlinks(string from, string to);
        internal static void CreateOrCleanDirectory(string dir)
        {
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
            Directory.CreateDirectory(dir);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool DeleteFileOrDirectory(string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string DeleteLastPathNameComponent(string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string GetActualPathName(string path);
        internal static List<string> GetAllFilesRecursive(string path)
        {
            <GetAllFilesRecursive>c__AnonStorey13 storey = new <GetAllFilesRecursive>c__AnonStorey13 {
                files = new List<string>()
            };
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = p => true;
            }
            WalkFilesystemRecursively(path, new Action<string>(storey.<>m__16), <>f__am$cache1);
            return storey.files;
        }

        internal static long GetDirectorySize(string path)
        {
            string[] strArray = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            long num = 0L;
            foreach (string str in strArray)
            {
                FileInfo info = new FileInfo(str);
                num += info.Length;
            }
            return num;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string GetLastPathNameComponent(string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string GetPathExtension(string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string GetPathWithoutExtension(string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetProjectRelativePath(string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetUniqueTempPathInProject();
        internal static void MoveFileIfExists(string src, string dst)
        {
            if (File.Exists(src))
            {
                DeleteFileOrDirectory(dst);
                MoveFileOrDirectory(src, dst);
                File.SetLastWriteTime(dst, DateTime.Now);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void MoveFileOrDirectory(string from, string to);
        internal static string NiceWinPath(string unityPath)
        {
            return ((Application.platform != RuntimePlatform.WindowsEditor) ? unityPath : unityPath.Replace("/", @"\"));
        }

        internal static string RemovePathPrefix(string fullPath, string prefix)
        {
            char[] separator = new char[] { Path.DirectorySeparatorChar };
            string[] strArray = fullPath.Split(separator);
            char[] chArray2 = new char[] { Path.DirectorySeparatorChar };
            string[] strArray2 = prefix.Split(chArray2);
            int index = 0;
            if (strArray[0] == string.Empty)
            {
                index = 1;
            }
            while (((index < strArray.Length) && (index < strArray2.Length)) && (strArray[index] == strArray2[index]))
            {
                index++;
            }
            if (index == strArray.Length)
            {
                return string.Empty;
            }
            return string.Join(Path.DirectorySeparatorChar.ToString(), strArray, index, strArray.Length - index);
        }

        public static void ReplaceDirectory(string src, string dst)
        {
            if (Directory.Exists(dst))
            {
                DeleteFileOrDirectory(dst);
            }
            CopyFileOrDirectory(src, dst);
        }

        public static void ReplaceFile(string src, string dst)
        {
            if (File.Exists(dst))
            {
                DeleteFileOrDirectory(dst);
            }
            CopyFileOrDirectory(src, dst);
        }

        internal static void ReplaceText(string path, params string[] input)
        {
            path = NiceWinPath(path);
            string[] contents = File.ReadAllLines(path);
            for (int i = 0; i < input.Length; i += 2)
            {
                for (int j = 0; j < contents.Length; j++)
                {
                    contents[j] = contents[j].Replace(input[i], input[i + 1]);
                }
            }
            File.WriteAllLines(path, contents);
        }

        internal static bool ReplaceTextRegex(string path, params string[] input)
        {
            bool flag = false;
            path = NiceWinPath(path);
            string[] contents = File.ReadAllLines(path);
            for (int i = 0; i < input.Length; i += 2)
            {
                for (int j = 0; j < contents.Length; j++)
                {
                    string str = contents[j];
                    contents[j] = Regex.Replace(str, input[i], input[i + 1]);
                    if (str != contents[j])
                    {
                        flag = true;
                    }
                }
            }
            File.WriteAllLines(path, contents);
            return flag;
        }

        internal static void UnityDirectoryDelete(string path)
        {
            UnityDirectoryDelete(path, false);
        }

        internal static void UnityDirectoryDelete(string path, bool recursive)
        {
            Directory.Delete(NiceWinPath(path), recursive);
        }

        internal static void UnityDirectoryRemoveReadonlyAttribute(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] directories = Directory.GetDirectories(target_dir);
            foreach (string str in files)
            {
                File.SetAttributes(str, FileAttributes.Normal);
            }
            foreach (string str2 in directories)
            {
                UnityDirectoryRemoveReadonlyAttribute(str2);
            }
        }

        internal static void UnityFileCopy(string from, string to)
        {
            UnityFileCopy(from, to, false);
        }

        internal static void UnityFileCopy(string from, string to, bool overwrite)
        {
            File.Copy(NiceWinPath(from), NiceWinPath(to), overwrite);
        }

        internal static string UnityGetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path.Replace("//", @"\\")).Replace(@"\\", "//");
        }

        internal static string UnityGetFileName(string path)
        {
            return Path.GetFileName(path.Replace("//", @"\\")).Replace(@"\\", "//");
        }

        internal static string UnityGetFileNameWithoutExtension(string path)
        {
            return Path.GetFileNameWithoutExtension(path.Replace("//", @"\\")).Replace(@"\\", "//");
        }

        internal static void WalkFilesystemRecursively(string path, Action<string> fileCallback, Func<string, bool> directoryCallback)
        {
            foreach (string str in Directory.GetFiles(path))
            {
                fileCallback(str);
            }
            foreach (string str2 in Directory.GetDirectories(path))
            {
                if (directoryCallback(str2))
                {
                    WalkFilesystemRecursively(str2, fileCallback, directoryCallback);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <CopyDirectoryFiltered>c__AnonStorey12
        {
            internal Regex exclude;

            internal bool <>m__15(string file)
            {
                return ((this.exclude == null) || !this.exclude.IsMatch(file));
            }
        }

        [CompilerGenerated]
        private sealed class <GetAllFilesRecursive>c__AnonStorey13
        {
            internal List<string> files;

            internal void <>m__16(string p)
            {
                this.files.Add(p);
            }
        }
    }
}

