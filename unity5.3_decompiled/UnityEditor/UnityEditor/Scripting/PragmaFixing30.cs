namespace UnityEditor.Scripting
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    internal class PragmaFixing30
    {
        private static bool CheckOrFixPragmas(string fileName, bool onlyCheck)
        {
            string str = File.ReadAllText(fileName);
            StringBuilder sb = new StringBuilder(str);
            LooseComments(sb);
            Match match = PragmaMatch(sb, "strict");
            if (!match.Success)
            {
                return false;
            }
            bool success = PragmaMatch(sb, "downcast").Success;
            bool hasImplicit = PragmaMatch(sb, "implicit").Success;
            if (success && hasImplicit)
            {
                return false;
            }
            if (!onlyCheck)
            {
                DoFixPragmasInFile(fileName, str, match.Index + match.Length, success, hasImplicit);
            }
            return true;
        }

        private static string[] CollectBadFiles()
        {
            List<string> list = new List<string>();
            IEnumerator<string> enumerator = SearchRecursive(Path.Combine(Directory.GetCurrentDirectory(), "Assets"), "*.js").GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    string current = enumerator.Current;
                    try
                    {
                        if (FileNeedsPragmaFixing(current))
                        {
                            list.Add(current);
                        }
                        continue;
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError("Failed to fix pragmas in file '" + current + "'.\n" + exception.Message);
                        continue;
                    }
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            return list.ToArray();
        }

        private static void DoFixPragmasInFile(string fileName, string oldText, int fixPos, bool hasDowncast, bool hasImplicit)
        {
            string str = string.Empty;
            string str2 = !HasWinLineEndings(oldText) ? "\n" : "\r\n";
            if (!hasImplicit)
            {
                str = str + str2 + "#pragma implicit";
            }
            if (!hasDowncast)
            {
                str = str + str2 + "#pragma downcast";
            }
            File.WriteAllText(fileName, oldText.Insert(fixPos, str));
        }

        private static bool FileNeedsPragmaFixing(string fileName)
        {
            return CheckOrFixPragmas(fileName, true);
        }

        public static void FixFiles(string[] filesToFix)
        {
            foreach (string str in filesToFix)
            {
                try
                {
                    FixPragmasInFile(str);
                }
                catch (Exception exception)
                {
                    Debug.LogError("Failed to fix pragmas in file '" + str + "'.\n" + exception.Message);
                }
            }
        }

        private static void FixJavaScriptPragmas()
        {
            string[] paths = CollectBadFiles();
            if (paths.Length != 0)
            {
                if (!InternalEditorUtility.inBatchMode)
                {
                    PragmaFixingWindow.ShowWindow(paths);
                }
                else
                {
                    FixFiles(paths);
                }
            }
        }

        private static void FixPragmasInFile(string fileName)
        {
            CheckOrFixPragmas(fileName, false);
        }

        private static bool HasWinLineEndings(string text)
        {
            return (text.IndexOf("\r\n") != -1);
        }

        private static void LooseComments(StringBuilder sb)
        {
            Regex regex = new Regex("//");
            IEnumerator enumerator = regex.Matches(sb.ToString()).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Match current = (Match) enumerator.Current;
                    int index = current.Index;
                    while (((index < sb.Length) && (sb[index] != '\n')) && (sb[index] != '\r'))
                    {
                        sb[index++] = ' ';
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
        }

        private static Match PragmaMatch(StringBuilder sb, string pragma)
        {
            return new Regex(@"#\s*pragma\s*" + pragma).Match(sb.ToString());
        }

        [DebuggerHidden]
        private static IEnumerable<string> SearchRecursive(string dir, string mask)
        {
            return new <SearchRecursive>c__Iterator8 { dir = dir, mask = mask, <$>dir = dir, <$>mask = mask, $PC = -2 };
        }

        [CompilerGenerated]
        private sealed class <SearchRecursive>c__Iterator8 : IDisposable, IEnumerator, IEnumerable, IEnumerable<string>, IEnumerator<string>
        {
            internal string $current;
            internal int $PC;
            internal string <$>dir;
            internal string <$>mask;
            internal string[] <$s_1844>__0;
            internal int <$s_1845>__1;
            internal IEnumerator<string> <$s_1846>__3;
            internal string[] <$s_1847>__5;
            internal int <$s_1848>__6;
            internal string <d>__2;
            internal string <f>__4;
            internal string <f>__7;
            internal string dir;
            internal string mask;

            [DebuggerHidden]
            public void Dispose()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 1:
                        try
                        {
                        }
                        finally
                        {
                            if (this.<$s_1846>__3 == null)
                            {
                            }
                            this.<$s_1846>__3.Dispose();
                        }
                        break;
                }
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                bool flag = false;
                switch (num)
                {
                    case 0:
                        this.<$s_1844>__0 = Directory.GetDirectories(this.dir);
                        this.<$s_1845>__1 = 0;
                        goto Label_00EE;

                    case 1:
                        break;

                    case 2:
                        goto Label_014F;

                    default:
                        goto Label_0177;
                }
            Label_0076:
                try
                {
                    while (this.<$s_1846>__3.MoveNext())
                    {
                        this.<f>__4 = this.<$s_1846>__3.Current;
                        this.$current = this.<f>__4;
                        this.$PC = 1;
                        flag = true;
                        goto Label_0179;
                    }
                }
                finally
                {
                    if (!flag)
                    {
                    }
                    if (this.<$s_1846>__3 == null)
                    {
                    }
                    this.<$s_1846>__3.Dispose();
                }
                this.<$s_1845>__1++;
            Label_00EE:
                if (this.<$s_1845>__1 < this.<$s_1844>__0.Length)
                {
                    this.<d>__2 = this.<$s_1844>__0[this.<$s_1845>__1];
                    this.<$s_1846>__3 = PragmaFixing30.SearchRecursive(this.<d>__2, this.mask).GetEnumerator();
                    num = 0xfffffffd;
                    goto Label_0076;
                }
                this.<$s_1847>__5 = Directory.GetFiles(this.dir, this.mask);
                this.<$s_1848>__6 = 0;
                while (this.<$s_1848>__6 < this.<$s_1847>__5.Length)
                {
                    this.<f>__7 = this.<$s_1847>__5[this.<$s_1848>__6];
                    this.$current = this.<f>__7;
                    this.$PC = 2;
                    goto Label_0179;
                Label_014F:
                    this.<$s_1848>__6++;
                }
                this.$PC = -1;
            Label_0177:
                return false;
            Label_0179:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<string> IEnumerable<string>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new PragmaFixing30.<SearchRecursive>c__Iterator8 { dir = this.<$>dir, mask = this.<$>mask };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<string>.GetEnumerator();
            }

            string IEnumerator<string>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }
    }
}

