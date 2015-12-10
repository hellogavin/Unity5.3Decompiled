namespace UnityEditor.Scripting.Compilers
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    internal abstract class CompilerOutputParserBase
    {
        protected CompilerOutputParserBase()
        {
        }

        protected internal static CompilerMessage CreateCompilerMessageFromMatchedRegex(string line, Match m, string erroridentifier)
        {
            CompilerMessage message;
            message.file = m.Groups["filename"].Value;
            message.message = line;
            message.line = int.Parse(m.Groups["line"].Value);
            message.column = int.Parse(m.Groups["column"].Value);
            message.type = !(m.Groups["type"].Value == erroridentifier) ? CompilerMessageType.Warning : CompilerMessageType.Error;
            message.normalizedStatus = new NormalizedCompilerStatus();
            return message;
        }

        protected static CompilerMessage CreateInternalCompilerErrorMessage(string[] compileroutput)
        {
            CompilerMessage message;
            message.file = string.Empty;
            message.message = string.Join("\n", compileroutput);
            message.type = CompilerMessageType.Error;
            message.line = 0;
            message.column = 0;
            message.normalizedStatus = new NormalizedCompilerStatus();
            return message;
        }

        protected abstract string GetErrorIdentifier();
        protected abstract Regex GetOutputRegex();
        protected virtual NormalizedCompilerStatus NormalizedStatusFor(Match match)
        {
            return new NormalizedCompilerStatus();
        }

        public virtual IEnumerable<CompilerMessage> Parse(string[] errorOutput, bool compilationHadFailure)
        {
            return this.Parse(errorOutput, new string[0], compilationHadFailure);
        }

        public virtual IEnumerable<CompilerMessage> Parse(string[] errorOutput, string[] standardOutput, bool compilationHadFailure)
        {
            bool flag = false;
            List<CompilerMessage> list = new List<CompilerMessage>();
            Regex outputRegex = this.GetOutputRegex();
            foreach (string str in errorOutput)
            {
                string input = (str.Length <= 0x3e8) ? str : str.Substring(0, 100);
                Match m = outputRegex.Match(input);
                if (m.Success)
                {
                    CompilerMessage item = CreateCompilerMessageFromMatchedRegex(str, m, this.GetErrorIdentifier());
                    item.normalizedStatus = this.NormalizedStatusFor(m);
                    if (item.type == CompilerMessageType.Error)
                    {
                        flag = true;
                    }
                    list.Add(item);
                }
            }
            if (compilationHadFailure && !flag)
            {
                list.Add(CreateInternalCompilerErrorMessage(errorOutput));
            }
            return list;
        }

        protected static NormalizedCompilerStatus TryNormalizeCompilerStatus(Match match, string memberNotFoundError, Regex missingMemberRegex)
        {
            string str = match.Groups["id"].Value;
            NormalizedCompilerStatus status = new NormalizedCompilerStatus();
            if (str == memberNotFoundError)
            {
                status.code = NormalizedCompilerStatusCode.MemberNotFound;
                Match match2 = missingMemberRegex.Match(match.Groups["message"].Value);
                status.details = match2.Groups["type_name"].Value + "%" + match2.Groups["member_name"].Value;
            }
            return status;
        }
    }
}

