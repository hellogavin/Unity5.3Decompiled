using System;

internal interface ICompilerSettings
{
    string CompilerPath { get; }

    string[] LibPaths { get; }

    string LinkerPath { get; }

    string MachineSpecification { get; }
}

