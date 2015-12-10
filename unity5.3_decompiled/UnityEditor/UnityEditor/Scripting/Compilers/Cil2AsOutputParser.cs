namespace UnityEditor.Scripting.Compilers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;

    internal class Cil2AsOutputParser : UnityScriptCompilerOutputParser
    {
        private static CompilerMessage CompilerErrorFor(StringBuilder currentErrorBuffer)
        {
            return new CompilerMessage { type = CompilerMessageType.Error, message = currentErrorBuffer.ToString() };
        }

        [DebuggerHidden]
        public override IEnumerable<CompilerMessage> Parse(string[] errorOutput, string[] standardOutput, bool compilationHadFailure)
        {
            return new <Parse>c__Iterator9 { errorOutput = errorOutput, <$>errorOutput = errorOutput, $PC = -2 };
        }

        [CompilerGenerated]
        private sealed class <Parse>c__Iterator9 : IDisposable, IEnumerator, IEnumerable, IEnumerable<CompilerMessage>, IEnumerator<CompilerMessage>
        {
            internal CompilerMessage $current;
            internal int $PC;
            internal string[] <$>errorOutput;
            internal string[] <$s_1867>__2;
            internal int <$s_1868>__3;
            internal StringBuilder <currentErrorBuffer>__1;
            internal bool <parsingError>__0;
            internal string <str>__4;
            internal string[] errorOutput;

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
                        this.<parsingError>__0 = false;
                        this.<currentErrorBuffer>__1 = new StringBuilder();
                        this.<$s_1867>__2 = this.errorOutput;
                        this.<$s_1868>__3 = 0;
                        goto Label_0103;

                    case 1:
                        this.<currentErrorBuffer>__1.Length = 0;
                        break;

                    case 2:
                        goto Label_013E;

                    default:
                        goto Label_0145;
                }
            Label_00AB:
                this.<currentErrorBuffer>__1.AppendLine(this.<str>__4.Substring("ERROR: ".Length));
                this.<parsingError>__0 = true;
            Label_00F5:
                this.<$s_1868>__3++;
            Label_0103:
                if (this.<$s_1868>__3 < this.<$s_1867>__2.Length)
                {
                    this.<str>__4 = this.<$s_1867>__2[this.<$s_1868>__3];
                    if (!this.<str>__4.StartsWith("ERROR: "))
                    {
                        if (this.<parsingError>__0)
                        {
                            this.<currentErrorBuffer>__1.AppendLine(this.<str>__4);
                        }
                        goto Label_00F5;
                    }
                    if (!this.<parsingError>__0)
                    {
                        goto Label_00AB;
                    }
                    this.$current = Cil2AsOutputParser.CompilerErrorFor(this.<currentErrorBuffer>__1);
                    this.$PC = 1;
                    goto Label_0147;
                }
                if (this.<parsingError>__0)
                {
                    this.$current = Cil2AsOutputParser.CompilerErrorFor(this.<currentErrorBuffer>__1);
                    this.$PC = 2;
                    goto Label_0147;
                }
            Label_013E:
                this.$PC = -1;
            Label_0145:
                return false;
            Label_0147:
                return true;
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
                return new Cil2AsOutputParser.<Parse>c__Iterator9 { errorOutput = this.<$>errorOutput };
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

