namespace UnityEditor.Scripting.Compilers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEditor.Scripting;
    using UnityEditor.Utils;

    internal abstract class MonoScriptCompilerBase : ScriptCompilerBase
    {
        private readonly bool runUpdater;

        protected MonoScriptCompilerBase(MonoIsland island, bool runUpdater) : base(island)
        {
            this.runUpdater = runUpdater;
        }

        protected string GetProfileDirectory()
        {
            return MonoInstallationFinder.GetProfileDirectory(this._island._target, this._island._classlib_profile);
        }

        protected ManagedProgram StartCompiler(BuildTarget target, string compiler, List<string> arguments)
        {
            return this.StartCompiler(target, compiler, arguments, true);
        }

        protected ManagedProgram StartCompiler(BuildTarget target, string compiler, List<string> arguments, bool setMonoEnvironmentVariables)
        {
            base.AddCustomResponseFileIfPresent(arguments, Path.GetFileNameWithoutExtension(compiler) + ".rsp");
            string responseFile = CommandLineFormatter.GenerateResponseFile(arguments);
            if (this.runUpdater)
            {
                APIUpdaterHelper.UpdateScripts(responseFile, this._island.GetExtensionOfSourceFiles());
            }
            ManagedProgram program = new ManagedProgram(MonoInstallationFinder.GetMonoInstallation(), this._island._classlib_profile, compiler, " @" + responseFile, setMonoEnvironmentVariables);
            program.Start();
            return program;
        }
    }
}

