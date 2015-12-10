namespace UnityEditor.Scripting.Compilers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;

    internal class GendarmeOutputParser : UnityScriptCompilerOutputParser
    {
        private static CompilerMessage CompilerErrorFor(GendarmeRuleData gendarmeRuleData)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(gendarmeRuleData.Problem);
            builder.AppendLine(gendarmeRuleData.Details);
            builder.AppendLine(string.IsNullOrEmpty(gendarmeRuleData.Location) ? string.Format("{0} at line : {1}", gendarmeRuleData.Source, gendarmeRuleData.Line) : gendarmeRuleData.Location);
            string str = builder.ToString();
            return new CompilerMessage { type = CompilerMessageType.Error, message = str, file = gendarmeRuleData.File, line = gendarmeRuleData.Line, column = 1 };
        }

        private static string GetFileNameFrome(string currentLine)
        {
            int startIndex = currentLine.LastIndexOf("* Source:") + "* Source:".Length;
            int index = currentLine.IndexOf("(");
            if ((startIndex != -1) && (index != -1))
            {
                return currentLine.Substring(startIndex, index - startIndex).Trim();
            }
            return string.Empty;
        }

        private static GendarmeRuleData GetGendarmeRuleDataFor(IList<string> output, int index)
        {
            GendarmeRuleData data = new GendarmeRuleData();
            for (int i = index; i < output.Count; i++)
            {
                string currentLine = output[i];
                if (currentLine.StartsWith("Problem:"))
                {
                    data.Problem = currentLine.Substring(currentLine.LastIndexOf("Problem:", StringComparison.Ordinal) + "Problem:".Length);
                }
                else if (currentLine.StartsWith("* Details"))
                {
                    data.Details = currentLine;
                }
                else if (currentLine.StartsWith("* Source"))
                {
                    data.IsAssemblyError = false;
                    data.Source = currentLine;
                    data.Line = GetLineNumberFrom(currentLine);
                    data.File = GetFileNameFrome(currentLine);
                }
                else if (currentLine.StartsWith("* Severity"))
                {
                    data.Severity = currentLine;
                }
                else if (currentLine.StartsWith("* Location"))
                {
                    data.IsAssemblyError = true;
                    data.Location = currentLine;
                }
                else if (currentLine.StartsWith("* Target"))
                {
                    data.Target = currentLine;
                }
                else
                {
                    data.LastIndex = i;
                    return data;
                }
            }
            return data;
        }

        private static int GetLineNumberFrom(string currentLine)
        {
            int startIndex = currentLine.IndexOf("(") + 2;
            int index = currentLine.IndexOf(")");
            if ((startIndex != -1) && (index != -1))
            {
                return int.Parse(currentLine.Substring(startIndex, index - startIndex));
            }
            return 0;
        }

        public override IEnumerable<CompilerMessage> Parse(string[] errorOutput, bool compilationHadFailure)
        {
            throw new ArgumentException("Gendarme Output Parser needs standard out");
        }

        [DebuggerHidden]
        public override IEnumerable<CompilerMessage> Parse(string[] errorOutput, string[] standardOutput, bool compilationHadFailure)
        {
            return new <Parse>c__IteratorA { standardOutput = standardOutput, <$>standardOutput = standardOutput, $PC = -2 };
        }

        [CompilerGenerated]
        private sealed class <Parse>c__IteratorA : IDisposable, IEnumerator, IEnumerable, IEnumerable<CompilerMessage>, IEnumerator<CompilerMessage>
        {
            internal CompilerMessage $current;
            internal int $PC;
            internal string[] <$>standardOutput;
            internal CompilerMessage <compilerErrorFor>__2;
            internal GendarmeRuleData <grd>__1;
            internal int <i>__0;
            internal string[] standardOutput;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<i>__0 = 0;
                        goto Label_00AF;

                    case 1:
                        this.<i>__0 = this.<grd>__1.LastIndex + 1;
                        break;

                    default:
                        goto Label_00C9;
                }
            Label_00A1:
                this.<i>__0++;
            Label_00AF:
                if (this.<i>__0 < this.standardOutput.Length)
                {
                    if (this.standardOutput[this.<i>__0].StartsWith("Problem:"))
                    {
                        this.<grd>__1 = GendarmeOutputParser.GetGendarmeRuleDataFor(this.standardOutput, this.<i>__0);
                        this.<compilerErrorFor>__2 = GendarmeOutputParser.CompilerErrorFor(this.<grd>__1);
                        this.$current = this.<compilerErrorFor>__2;
                        this.$PC = 1;
                        return true;
                    }
                    goto Label_00A1;
                }
                this.$PC = -1;
            Label_00C9:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<CompilerMessage> IEnumerable<CompilerMessage>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new GendarmeOutputParser.<Parse>c__IteratorA { standardOutput = this.<$>standardOutput };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<UnityEditor.Scripting.Compilers.CompilerMessage>.GetEnumerator();
            }

            CompilerMessage IEnumerator<CompilerMessage>.Current
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

