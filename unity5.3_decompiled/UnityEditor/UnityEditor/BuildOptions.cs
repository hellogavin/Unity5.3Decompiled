namespace UnityEditor
{
    using System;

    [Flags]
    public enum BuildOptions
    {
        AcceptExternalModificationsToPlayer = 0x20,
        AllowDebugging = 0x200,
        AutoRunPlayer = 4,
        BuildAdditionalStreamedScenes = 0x10,
        BuildScriptsOnly = 0x8000,
        [Obsolete("Texture Compression is now always enabled")]
        CompressTextures = 0,
        ConnectToHost = 0x1000,
        ConnectWithProfiler = 0x100,
        Development = 1,
        EnableHeadlessMode = 0x4000,
        ForceEnableAssertions = 0x20000,
        ForceOptimizeScriptCompilation = 0x80000,
        Il2CPP = 0x10000,
        InstallInBuildFolder = 0x40,
        None = 0,
        ShowBuiltPlayer = 8,
        [Obsolete("Use BuildOptions.Development instead")]
        StripDebugSymbols = 0,
        SymlinkLibraries = 0x400,
        UncompressedAssetBundle = 0x800,
        WebPlayerOfflineDeployment = 0x80
    }
}

