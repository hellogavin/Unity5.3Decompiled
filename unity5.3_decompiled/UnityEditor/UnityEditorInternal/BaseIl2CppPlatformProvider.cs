namespace UnityEditorInternal
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Unity.DataContract;
    using UnityEditor;
    using UnityEditor.Modules;
    using UnityEngine;

    internal class BaseIl2CppPlatformProvider : IIl2CppPlatformProvider
    {
        [CompilerGenerated]
        private static Func<PackageInfo, bool> <>f__am$cache2;

        public BaseIl2CppPlatformProvider(BuildTarget target, string libraryFolder)
        {
            this.target = target;
            this.libraryFolder = libraryFolder;
        }

        public virtual INativeCompiler CreateNativeCompiler()
        {
            return null;
        }

        private static PackageInfo FindIl2CppPackage()
        {
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = e => e.name == "IL2CPP";
            }
            return ModuleManager.packageManager.unityExtensions.FirstOrDefault<PackageInfo>(<>f__am$cache2);
        }

        protected string GetFileInPackageOrDefault(string path)
        {
            PackageInfo info = FindIl2CppPackage();
            if (info == null)
            {
                return Path.Combine(this.libraryFolder, path);
            }
            string str = Path.Combine(info.basePath, path);
            return (File.Exists(str) ? str : Path.Combine(this.libraryFolder, path));
        }

        protected string GetFolderInPackageOrDefault(string path)
        {
            PackageInfo info = FindIl2CppPackage();
            if (info == null)
            {
                return Path.Combine(this.libraryFolder, path);
            }
            string str = Path.Combine(info.basePath, path);
            return (Directory.Exists(str) ? str : Path.Combine(this.libraryFolder, path));
        }

        public virtual bool compactMode
        {
            get
            {
                return false;
            }
        }

        public virtual bool developmentMode
        {
            get
            {
                return false;
            }
        }

        public virtual bool emitNullChecks
        {
            get
            {
                return true;
            }
        }

        public virtual bool enableArrayBoundsCheck
        {
            get
            {
                return true;
            }
        }

        public virtual bool enableStackTraces
        {
            get
            {
                return true;
            }
        }

        public virtual string il2CppFolder
        {
            get
            {
                PackageInfo info = FindIl2CppPackage();
                if (info == null)
                {
                    return Path.GetFullPath(Path.Combine(EditorApplication.applicationContentsPath, (Application.platform != RuntimePlatform.OSXEditor) ? "il2cpp" : "Frameworks/il2cpp"));
                }
                return info.basePath;
            }
        }

        public virtual string[] includePaths
        {
            get
            {
                return new string[] { this.GetFolderInPackageOrDefault("bdwgc/include"), this.GetFolderInPackageOrDefault("libil2cpp/include") };
            }
        }

        public virtual string libraryFolder { get; private set; }

        public virtual string[] libraryPaths
        {
            get
            {
                return new string[] { this.GetFileInPackageOrDefault("bdwgc/lib/bdwgc." + this.staticLibraryExtension), this.GetFileInPackageOrDefault("libil2cpp/lib/libil2cpp." + this.staticLibraryExtension) };
            }
        }

        public virtual bool loadSymbols
        {
            get
            {
                return false;
            }
        }

        public virtual string moduleStrippingInformationFolder
        {
            get
            {
                return Path.Combine(BuildPipeline.GetPlaybackEngineDirectory(EditorUserBuildSettings.activeBuildTarget, BuildOptions.CompressTextures), "Whitelists");
            }
        }

        public virtual string nativeLibraryFileName
        {
            get
            {
                return null;
            }
        }

        public virtual string staticLibraryExtension
        {
            get
            {
                return "a";
            }
        }

        public virtual bool supportsEngineStripping
        {
            get
            {
                return false;
            }
        }

        public virtual BuildTarget target { get; private set; }
    }
}

