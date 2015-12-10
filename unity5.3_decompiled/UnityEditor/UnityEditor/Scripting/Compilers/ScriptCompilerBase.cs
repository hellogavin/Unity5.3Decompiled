namespace UnityEditor.Scripting.Compilers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.Scripting;
    using UnityEditor.Utils;
    using UnityEngine;

    internal abstract class ScriptCompilerBase : IDisposable
    {
        protected MonoIsland _island;
        private string _responseFile;
        private Program process;

        protected ScriptCompilerBase(MonoIsland island)
        {
            this._island = island;
        }

        protected void AddCustomResponseFileIfPresent(List<string> arguments, string responseFileName)
        {
            string path = Path.Combine("Assets", responseFileName);
            if (File.Exists(path))
            {
                arguments.Add("@" + path);
            }
        }

        public void BeginCompiling()
        {
            if (this.process != null)
            {
                throw new InvalidOperationException("Compilation has already begun!");
            }
            this.process = this.StartCompiler();
        }

        protected bool CompilationHadFailure()
        {
            return (this.process.ExitCode != 0);
        }

        protected bool CompilingForWSA()
        {
            return (this._island._target == BuildTarget.WSAPlayer);
        }

        protected abstract CompilerOutputParserBase CreateOutputParser();
        public virtual void Dispose()
        {
            if (this.process != null)
            {
                this.process.Dispose();
                this.process = null;
            }
            if (this._responseFile != null)
            {
                File.Delete(this._responseFile);
                this._responseFile = null;
            }
        }

        private void DumpStreamOutputToLog()
        {
            bool flag = this.CompilationHadFailure();
            string[] errorOutput = this.GetErrorOutput();
            if (flag || (errorOutput.Length != 0))
            {
                Console.WriteLine(string.Empty);
                Console.WriteLine("-----Compiler Commandline Arguments:");
                this.process.LogProcessStartInfo();
                string[] standardOutput = this.GetStandardOutput();
                Console.WriteLine(string.Concat(new object[] { "-----CompilerOutput:-stdout--exitcode: ", this.process.ExitCode, "--compilationhadfailure: ", flag, "--outfile: ", this._island._output }));
                foreach (string str in standardOutput)
                {
                    Console.WriteLine(str);
                }
                Console.WriteLine("-----CompilerOutput:-stderr----------");
                foreach (string str2 in errorOutput)
                {
                    Console.WriteLine(str2);
                }
                Console.WriteLine("-----EndCompilerOutput---------------");
            }
        }

        protected string GenerateResponseFile(List<string> arguments)
        {
            this._responseFile = CommandLineFormatter.GenerateResponseFile(arguments);
            return this._responseFile;
        }

        public virtual CompilerMessage[] GetCompilerMessages()
        {
            if (!this.Poll())
            {
                Debug.LogWarning("Compile process is not finished yet. This should not happen.");
            }
            this.DumpStreamOutputToLog();
            return this.CreateOutputParser().Parse(this.GetStreamContainingCompilerMessages(), this.CompilationHadFailure()).ToArray<CompilerMessage>();
        }

        protected string[] GetErrorOutput()
        {
            return this.process.GetErrorOutput();
        }

        protected string[] GetStandardOutput()
        {
            return this.process.GetStandardOutput();
        }

        protected virtual string[] GetStreamContainingCompilerMessages()
        {
            List<string> list = new List<string>();
            list.AddRange(this.GetErrorOutput());
            list.Add(string.Empty);
            list.AddRange(this.GetStandardOutput());
            return list.ToArray();
        }

        public virtual bool Poll()
        {
            return ((this.process == null) || this.process.HasExited);
        }

        protected static string PrepareFileName(string fileName)
        {
            return CommandLineFormatter.PrepareFileName(fileName);
        }

        protected abstract Program StartCompiler();
    }
}

