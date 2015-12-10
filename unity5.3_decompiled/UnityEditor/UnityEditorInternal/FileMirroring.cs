namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;

    internal static class FileMirroring
    {
        private static bool AreFilesIdentical(string filePath1, string filePath2)
        {
            using (FileStream stream = File.OpenRead(filePath1))
            {
                using (FileStream stream2 = File.OpenRead(filePath2))
                {
                    int num2;
                    if (stream.Length != stream2.Length)
                    {
                        return false;
                    }
                    byte[] array = new byte[0x10000];
                    byte[] buffer2 = new byte[0x10000];
                    while ((num2 = stream.Read(array, 0, array.Length)) > 0)
                    {
                        stream2.Read(buffer2, 0, buffer2.Length);
                        for (int i = 0; i < num2; i++)
                        {
                            if (array[i] != buffer2[i])
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        public static bool CanSkipCopy(string from, string to)
        {
            if (!File.Exists(to))
            {
                return false;
            }
            return AreFilesIdentical(from, to);
        }

        private static void DeleteFileOrDirectory(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            else if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        private static FileEntryType FileEntryTypeFor(string fileEntry)
        {
            if (File.Exists(fileEntry))
            {
                return FileEntryType.File;
            }
            if (Directory.Exists(fileEntry))
            {
                return FileEntryType.Directory;
            }
            return FileEntryType.NotExisting;
        }

        public static void MirrorFile(string from, string to)
        {
            MirrorFile(from, to, new Func<string, string, bool>(FileMirroring.CanSkipCopy));
        }

        public static void MirrorFile(string from, string to, Func<string, string, bool> comparer)
        {
            if (!comparer(from, to))
            {
                if (!File.Exists(from))
                {
                    DeleteFileOrDirectory(to);
                }
                else
                {
                    string directoryName = Path.GetDirectoryName(to);
                    if (!Directory.Exists(directoryName))
                    {
                        Directory.CreateDirectory(directoryName);
                    }
                    File.Copy(from, to, true);
                }
            }
        }

        public static void MirrorFolder(string from, string to)
        {
            MirrorFolder(from, to, new Func<string, string, bool>(FileMirroring.CanSkipCopy));
        }

        public static void MirrorFolder(string from, string to, Func<string, string, bool> comparer)
        {
            <MirrorFolder>c__AnonStorey6D storeyd;
            storeyd = new <MirrorFolder>c__AnonStorey6D {
                to = to,
                from = from,
                from = Path.GetFullPath(storeyd.from),
                to = Path.GetFullPath(storeyd.to)
            };
            if (!Directory.Exists(storeyd.from))
            {
                if (Directory.Exists(storeyd.to))
                {
                    Directory.Delete(storeyd.to, true);
                }
            }
            else
            {
                if (!Directory.Exists(storeyd.to))
                {
                    Directory.CreateDirectory(storeyd.to);
                }
                IEnumerable<string> first = Directory.GetFileSystemEntries(storeyd.to).Select<string, string>(new Func<string, string>(storeyd.<>m__F0));
                IEnumerable<string> second = Directory.GetFileSystemEntries(storeyd.from).Select<string, string>(new Func<string, string>(storeyd.<>m__F1));
                IEnumerator<string> enumerator = first.Except<string>(second).GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        string current = enumerator.Current;
                        DeleteFileOrDirectory(Path.Combine(storeyd.to, current));
                    }
                }
                finally
                {
                    if (enumerator == null)
                    {
                    }
                    enumerator.Dispose();
                }
                IEnumerator<string> enumerator2 = second.GetEnumerator();
                try
                {
                    while (enumerator2.MoveNext())
                    {
                        string str2 = enumerator2.Current;
                        string fileEntry = Path.Combine(storeyd.from, str2);
                        string str4 = Path.Combine(storeyd.to, str2);
                        FileEntryType type = FileEntryTypeFor(fileEntry);
                        FileEntryType type2 = FileEntryTypeFor(str4);
                        if ((type == FileEntryType.File) && (type2 == FileEntryType.Directory))
                        {
                            DeleteFileOrDirectory(str4);
                        }
                        if (type == FileEntryType.Directory)
                        {
                            if (type2 == FileEntryType.File)
                            {
                                DeleteFileOrDirectory(str4);
                            }
                            if (type2 != FileEntryType.Directory)
                            {
                                Directory.CreateDirectory(str4);
                            }
                            MirrorFolder(fileEntry, str4);
                        }
                        if (type == FileEntryType.File)
                        {
                            MirrorFile(fileEntry, str4, comparer);
                        }
                    }
                }
                finally
                {
                    if (enumerator2 == null)
                    {
                    }
                    enumerator2.Dispose();
                }
            }
        }

        private static string StripPrefix(string s, string prefix)
        {
            return s.Substring(prefix.Length + 1);
        }

        [CompilerGenerated]
        private sealed class <MirrorFolder>c__AnonStorey6D
        {
            internal string from;
            internal string to;

            internal string <>m__F0(string s)
            {
                return FileMirroring.StripPrefix(s, this.to);
            }

            internal string <>m__F1(string s)
            {
                return FileMirroring.StripPrefix(s, this.from);
            }
        }

        private enum FileEntryType
        {
            File,
            Directory,
            NotExisting
        }
    }
}

