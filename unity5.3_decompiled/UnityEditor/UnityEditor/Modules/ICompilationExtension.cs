namespace UnityEditor.Modules
{
    using Mono.Cecil;
    using System;

    internal interface ICompilationExtension
    {
        IAssemblyResolver GetAssemblyResolver(bool buildingForEditor, string assemblyPath, string[] searchDirectories);
        string[] GetCompilerExtraAssemblyPaths(bool isEditor, string assemblyPathName);
        CSharpCompiler GetCsCompiler(bool buildingForEditor, string assemblyName);
    }
}

