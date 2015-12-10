namespace UnityEditor.Scripting.Compilers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEditor.Scripting;
    using UnityEditor.Utils;
    using UnityEngineInternal;

    internal class UnityScriptCompiler : MonoScriptCompilerBase
    {
        private static readonly Regex UnityEditorPattern = new Regex(@"UnityEditor\.dll$", RegexOptions.ExplicitCapture);

        public UnityScriptCompiler(MonoIsland island, bool runUpdater) : base(island, runUpdater)
        {
        }

        protected override CompilerOutputParserBase CreateOutputParser()
        {
            return new UnityScriptCompilerOutputParser();
        }

        protected override string[] GetStreamContainingCompilerMessages()
        {
            return base.GetStandardOutput();
        }

        protected override Program StartCompiler()
        {
            List<string> arguments = new List<string> {
                "-debug",
                "-target:library",
                "-i:UnityEngine",
                "-i:System.Collections",
                "-base:UnityEngine.MonoBehaviour",
                "-nowarn:BCW0016",
                "-nowarn:BCW0003",
                "-method:Main",
                "-out:" + this._island._output,
                "-x-type-inference-rule-attribute:" + typeof(TypeInferenceRuleAttribute)
            };
            if (this.StrictBuildTarget())
            {
                arguments.Add("-pragmas:strict,downcast");
            }
            IEnumerator<string> enumerator = this._island._defines.Distinct<string>().GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    arguments.Add("-define:" + enumerator.Current);
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            foreach (string str2 in this._island._references)
            {
                arguments.Add("-r:" + ScriptCompilerBase.PrepareFileName(str2));
            }
            if (Array.Exists<string>(this._island._references, new Predicate<string>(UnityEditorPattern.IsMatch)))
            {
                arguments.Add("-i:UnityEditor");
            }
            else if (!BuildPipeline.IsUnityScriptEvalSupported(this._island._target))
            {
                arguments.Add(string.Format("-disable-eval:eval is not supported on the current build target ({0}).", this._island._target));
            }
            foreach (string str3 in this._island._files)
            {
                arguments.Add(ScriptCompilerBase.PrepareFileName(str3));
            }
            string compiler = Path.Combine(base.GetProfileDirectory(), "us.exe");
            return base.StartCompiler(this._island._target, compiler, arguments);
        }

        private bool StrictBuildTarget()
        {
            return (Array.IndexOf<string>(this._island._defines, "ENABLE_DUCK_TYPING") == -1);
        }
    }
}

