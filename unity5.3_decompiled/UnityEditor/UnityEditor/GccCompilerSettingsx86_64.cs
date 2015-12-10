namespace UnityEditor
{
    using System;

    internal class GccCompilerSettingsx86_64 : ICompilerSettings
    {
        public string CompilerPath
        {
            get
            {
                return "/usr/bin/g++";
            }
        }

        public string[] LibPaths
        {
            get
            {
                return new string[0];
            }
        }

        public string LinkerPath
        {
            get
            {
                return this.CompilerPath;
            }
        }

        public string MachineSpecification
        {
            get
            {
                return "-m64";
            }
        }
    }
}

