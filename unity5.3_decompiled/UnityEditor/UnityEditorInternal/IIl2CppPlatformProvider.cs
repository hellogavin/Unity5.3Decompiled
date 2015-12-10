namespace UnityEditorInternal
{
    using System;
    using UnityEditor;

    internal interface IIl2CppPlatformProvider
    {
        INativeCompiler CreateNativeCompiler();

        bool compactMode { get; }

        bool developmentMode { get; }

        bool emitNullChecks { get; }

        bool enableArrayBoundsCheck { get; }

        bool enableStackTraces { get; }

        string il2CppFolder { get; }

        string[] includePaths { get; }

        string[] libraryPaths { get; }

        bool loadSymbols { get; }

        string moduleStrippingInformationFolder { get; }

        string nativeLibraryFileName { get; }

        bool supportsEngineStripping { get; }

        BuildTarget target { get; }
    }
}

